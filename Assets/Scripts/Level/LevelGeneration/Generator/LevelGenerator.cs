using System;
using DungeonDraws.Scripts.Systems.LevelGeneration.Domain;
using DungeonDraws.Scripts.Systems.LevelGeneration.Plotters;
using DungeonDraws.Scripts.Systems.LevelGeneration.Pickers;
using DungeonDraws.Scripts.Utils.Logging;

namespace DungeonDraws.Scripts.Systems.LevelGeneration
{
    public class LevelGenerator
    {
        private int _corridorLengthMin;
        private int _corridorLengthMax;
        private int _corridorWidthMin;
        private int _corridorWidthMax;

        private int _roomSizeMin;
        private int _roomSizeMax;
        private int _roomsNumberMin;
        private int _roomsNumberMax;
        private int _seed;
        private int _mapRows;
        private int _mapColumns;
        private readonly int _maxAttempts;

        private IXLogger _logger;
        private IBoardPlotter _plotter;
        private Board _board;
        private int _mapMargin;
        private bool _mapCropEnabled;

        private int GetMinRoomSize()
        {
            return _roomsNumberMin;
        }

        protected int GetSeed()
        {
            return _seed;
        }

        protected IXLogger GetLogger()
        {
            return _logger;
        }

        private void SetBoard(Board board)
        {
            _board = board;
        }

        private void ClearBoard()
        {
            _board = null;
        }

        private bool IsBoardCleared()
        {
            return _board == null;
        }

        public void SetCorridorLengthRange(int v1, int v2)
        {
            _corridorLengthMin = v1;
            _corridorLengthMax = v2;
            ClearBoard();
        }

        public void SetCorridorWidthRange(int v1, int v2)
        {
            _corridorWidthMin = v1;
            _corridorWidthMax = v2;
            ClearBoard();
        }

        public void SetRoomSizeRange(int v1, int v2)
        {
            _roomSizeMin = v1;
            _roomSizeMax = v2;
            ClearBoard();
        }

        public void SetRoomsNumberRange(int v1, int v2)
        {
            _roomsNumberMin = v1;
            _roomsNumberMax = v2;
            ClearBoard();
        }

        public void SetSeed(int v)
        {
            _seed = v;
            ClearBoard();
        }

        public void SetMapSize(int rows, int columns)
        {
            _mapRows = rows;
            _mapColumns = columns;
            ClearBoard();
        }

        public void SetLogger(IXLogger logger)
        {
            _logger = logger;
            ClearBoard();
        }

        public LevelGenerator(int maxAttempts)
        {
            _logger = new NullLogger();

            _corridorWidthMin = 3;
            _corridorWidthMax = 3;
            _mapMargin = 0;
            _mapCropEnabled = false;
            _maxAttempts = maxAttempts;
            ClearBoard();
        }

        private Board AsBoardSingle()
        {
            CheckConstraints();
            if (!IsBoardCleared()) return _board;
            _board = new Board(_mapRows - _mapMargin * 2, _mapColumns - _mapMargin * 2);

            CustomSeededPickerStrategy seedStrategy = new CustomSeededPickerStrategy(_seed);
            seedStrategy.setLogger(_logger);

            IntInRangePicker roomNumberPicker = new IntInRangePicker(_roomsNumberMin, _roomsNumberMax, seedStrategy);
            IntInRangePicker roomSizePicker = new IntInRangePicker(_roomSizeMin, _roomSizeMax, seedStrategy);
            IntInRangePicker corrLengthPicker =
                new IntInRangePicker(_corridorLengthMin, _corridorLengthMax, seedStrategy);
            IntInRangePicker corrWidthPicker = new IntInRangePicker(_corridorWidthMin, _corridorWidthMax, seedStrategy);
            CardinalPointPicker cardPointPicker = new CardinalPointPicker(seedStrategy);
            CellInRangePicker cellRangePicker = new CellInRangePicker(seedStrategy);

            int roomNumber = roomNumberPicker.draw();
            if (roomNumber < 1)
            {
                _logger.warning("Room number should be at least 1. Instead is: " + roomNumber);
                return _board;
            }

            Grid grid = new Grid(roomSizePicker.draw(), roomSizePicker.draw());
            Cell topLeftVertexMin = new Cell(0, 0);
            Cell topLeftVertexMax =
                new Cell(_board.rows() - 1, _board.cols() - 1).minusSize(grid.rows(), grid.columns());
            Cell topLeftCell = cellRangePicker.drawBetween(topLeftVertexMin, topLeftVertexMax);
            Room lastRoom = new Room(topLeftCell, grid);
            if (!_board.fitsIn(lastRoom))
            {
                _logger.error("First room not fit in. This should never happen");
                return _board;
            }

            _logger.info("OK: " + lastRoom);
            _board.addRoom(lastRoom);

            CardinalPoint lastDirection = CardinalPoint.NORD;
            int roomCreationAttempt = 1;
            for (int i = 1; i < roomNumber; i++)
            {
                //If room and corridor has not been dropped, pick random direction
                lastDirection = roomCreationAttempt == 1 ? cardPointPicker.draw() :
                    //else force to next direction
                    cardPointPicker.nextClockwise(lastDirection);

                //Try to create a Corridor on a direction if there is enough space
                int cardinalPointAttempt = 1;
                Corridor lastCorridor;
                do
                {
                    lastCorridor = GenerateCorridor(lastDirection, lastRoom, corrLengthPicker, corrWidthPicker,
                        cellRangePicker);
                    if (!_board.fitsIn(lastCorridor))
                    {
                        _logger.info("NO FITS: " + lastCorridor + " " + lastDirection);
                        lastCorridor = null;
                        cardinalPointAttempt++;
                        lastDirection = cardPointPicker.nextClockwise(lastDirection);
                    }
                } while (cardinalPointAttempt <= 4 && lastCorridor == null);

                //If no corridor has been created, then terminate the algorithm
                if (lastCorridor == null)
                {
                    _logger.warning("PROCEDURAL GENERATION INTERRUPTED: No more chance for a Corridor to fit in");
                    break;
                }

                _logger.info("OK: " + lastCorridor + " " + lastDirection);
                _board.addCorridor(lastCorridor);

                //Try to create a room. If there is enough space retry (until 4 times) restarting from a new corridor
                Room newRoom = GenerateRoom(lastDirection, lastCorridor, roomSizePicker, cellRangePicker);
                if (!_board.fitsIn(newRoom))
                {
                    _board.removeLast(); //remove last item added to the board (a corridor in this case)
                    if (roomCreationAttempt <= 4)
                    {
                        _logger.info("NO FITS: " + newRoom + " Retry: " + roomCreationAttempt);
                        roomCreationAttempt++;
                        i--;
                    }
                    else
                    {
                        _logger.warning("PROCEDURAL GENERATION INTERRUPTED: No more chance for a Room to fit in");
                        break;
                    }
                }
                else
                {
                    _logger.info("OK: " + newRoom + " Retry: " + roomCreationAttempt);
                    lastRoom = newRoom;
                    roomCreationAttempt = 1;
                    _board.addRoom(lastRoom);
                }
            }

            if (_mapMargin > 0)
            {
                _board = _board.resize(_mapMargin);
            }

            if (_mapCropEnabled)
            {
                _board = _board.crop(_mapMargin);
            }

            return _board;
        }

        private Board AsBoard()
        {
            if (!IsBoardCleared()) return AsBoardSingle();

            Board bestBoard = null;
            int count = 1;
            while (count < _maxAttempts)
            {
                _logger.info("Board generation attempt: " + count+"/"+_maxAttempts);

                Board board = AsBoardSingle();
                if (bestBoard == null) bestBoard = board;
                else if (board.roomSize() > bestBoard.roomSize()) bestBoard = board;

                if (bestBoard.roomSize() >= GetMinRoomSize()) break;

                int seed = _seed;
                if (seed >= 0) seed++;
                else seed--;
                SetSeed(seed);

                count++;
            }

            _logger.info("Board generation completed at attempt: " + count);
            SetBoard(bestBoard);
            return bestBoard;
        }

        private void CheckConstraints()
        {
            if (_roomsNumberMax < _roomsNumberMin) throw new FormatException("Invalid Room Number: Max < Min");
            if (_roomSizeMax < _roomSizeMin) throw new FormatException("Invalid Room Size: Max < Min");
            if (_corridorLengthMax < _corridorLengthMin)
                throw new FormatException("Invalid Corridor Length: Max < Min");
            if (_corridorWidthMax < _corridorWidthMin) throw new FormatException("Invalid Corridor Width: Max < Min");
            if (_corridorWidthMax > _roomSizeMin)
                throw new FormatException("Invalid Corridor Width Max > Room Size Min");
        }

        public void SetMapCropEnabled(bool enabled)
        {
            _mapCropEnabled = enabled;
            ClearBoard();
        }

        public void SetMapMargin(int mapMargin)
        {
            _mapMargin = mapMargin;
            ClearBoard();
        }

        public int[,] AsMatrix()
        {
            return AsBoard().asTilesMatrix(_plotter);
        }

        public void SetPlotter(IBoardPlotter plotter)
        {
            _plotter = plotter;
        }

        private Room GenerateRoom(CardinalPoint lastCorridorDirection, Corridor lastCorr,
            IntInRangePicker roomSizePicker, CellInRangePicker cellInRangePicker)
        {
            int roomRows = roomSizePicker.draw();
            int roomCols = roomSizePicker.draw();
            Grid grid = new Grid(roomRows, roomCols);
            Cell topLeftCell = null;
            if (lastCorridorDirection == CardinalPoint.NORD)
            {
                Cell topLeftVertexMax = lastCorr.topLeftVertex().minusSize(roomRows, 0);
                Cell topLeftVertexMin = topLeftVertexMax.minusCell(0, roomCols - lastCorr.width());
                //Excluding cells to avoid Inward and Outward Corner Walls Overlapping
                Cell excludeOne = topLeftVertexMin.plusCell(0, 1);
                Cell excludeTwo = topLeftVertexMax.minusCell(0, 1);
                topLeftCell =
                    cellInRangePicker.drawBetweenWithExclusion(topLeftVertexMin, topLeftVertexMax, excludeOne,
                        excludeTwo);
                _logger.info("Min: " + topLeftVertexMin + " Max: " + topLeftVertexMax + " Selected: " + topLeftCell + " Exclusions: " + excludeOne + " - " + excludeTwo);
            }
            else if (lastCorridorDirection == CardinalPoint.EST)
            {
                Cell topLeftVertexMax = lastCorr.topRightVertex();
                Cell topLeftVertexMin = topLeftVertexMax.minusCell(roomRows - lastCorr.height(), 0);
                //Excluding cells to avoid Inward and Outward Corner Walls Overlapping
                Cell excludeOne = topLeftVertexMin.plusCell(1, 0);
                Cell excludeTwo = topLeftVertexMax.minusCell(1, 0);
                topLeftCell =
                    cellInRangePicker.drawBetweenWithExclusion(topLeftVertexMin, topLeftVertexMax, excludeOne,
                        excludeTwo);
                _logger.info("Min: " + topLeftVertexMin + " Max: " + topLeftVertexMax + " Selected: " + topLeftCell + " Exclusions: " + excludeOne + " - " + excludeTwo);
            }
            else if (lastCorridorDirection == CardinalPoint.SUD)
            {
                Cell topLeftVertexMax = lastCorr.bottomLeftVertex();
                Cell topLeftVertexMin = topLeftVertexMax.minusCell(0, roomCols - lastCorr.width());
                //Excluding cells to avoid Inward and Outward Corner Walls Overlapping
                Cell excludeOne = topLeftVertexMin.plusCell(0, 1);
                Cell excludeTwo = topLeftVertexMax.minusCell(0, 1);
                topLeftCell =
                    cellInRangePicker.drawBetweenWithExclusion(topLeftVertexMin, topLeftVertexMax, excludeOne,
                        excludeTwo);
                _logger.info("Min: " + topLeftVertexMin + " Max: " + topLeftVertexMax + " Selected: " + topLeftCell + " Exclusions: " + excludeOne + " - " + excludeTwo);
            }
            else if (lastCorridorDirection == CardinalPoint.WEST)
            {
                Cell topLeftVertexMax = lastCorr.topLeftVertex().minusSize(0, roomCols);
                Cell topLeftVertexMin = topLeftVertexMax.minusCell(roomRows - lastCorr.height(), 0);
                //Excluding cells to avoid Inward and Outward Corner Walls Overlapping
                Cell excludeOne = topLeftVertexMin.plusCell(1, 0);
                Cell excludeTwo = topLeftVertexMax.minusCell(1, 0);
                topLeftCell =
                    cellInRangePicker.drawBetweenWithExclusion(topLeftVertexMin, topLeftVertexMax, excludeOne,
                        excludeTwo);
                _logger.info("Min: " + topLeftVertexMin + " Max: " + topLeftVertexMax + " Selected: " + topLeftCell + " Exclusions: " + excludeOne + " - " + excludeTwo);
            }

            return new Room(topLeftCell, grid);
        }

        private Corridor GenerateCorridor(CardinalPoint mapDirection, Room lastRoom, IntInRangePicker corrLengthPicker,
            IntInRangePicker corrWidthPicker, CellInRangePicker cellRangePicker)
        {
            int corridorLenght = corrLengthPicker.draw();
            int corridorSection = corrWidthPicker.draw();

            Corridor.Orientation corrOrient = 0;
            Grid grid = null;
            Cell topLeftCell = lastRoom.topLeftVertex();
            if (mapDirection == CardinalPoint.NORD)
            {
                grid = new Grid(corridorLenght, corridorSection);
                corrOrient = Corridor.Orientation.vertical;
                Cell topLeftVertexMin = lastRoom.topLeftVertex().minusSize(corridorLenght, 0);
                Cell topLeftVertexMax = topLeftVertexMin.plusCell(0, lastRoom.width() - corridorSection);
                //Excluding cells to avoid Inward and Outward Corner Walls Overlapping
                Cell excludeOne = topLeftVertexMin.plusCell(0, 1);
                Cell excludeTwo = topLeftVertexMax.minusCell(0, 1);
                topLeftCell =
                    cellRangePicker.drawBetweenWithExclusion(topLeftVertexMin, topLeftVertexMax, excludeOne,
                        excludeTwo);
                _logger.info("Min: " + topLeftVertexMin + " Max: " + topLeftVertexMax + " Selected: " + topLeftCell +
                " Exclusions: " + excludeOne + " - " + excludeTwo);
            }
            else if (mapDirection == CardinalPoint.EST)
            {
                grid = new Grid(corridorSection, corridorLenght);
                corrOrient = Corridor.Orientation.horizontal;
                Cell topLeftVertexMin = lastRoom.topRightVertex();
                Cell topLeftVertexMax = topLeftVertexMin.plusCell(lastRoom.height() - corridorSection, 0);
                //Excluding cells to avoid Inward and Outward Corner Walls Overlapping
                Cell excludeOne = topLeftVertexMin.plusCell(1, 0);
                Cell excludeTwo = topLeftVertexMax.minusCell(1, 0);
                topLeftCell =
                    cellRangePicker.drawBetweenWithExclusion(topLeftVertexMin, topLeftVertexMax, excludeOne,
                        excludeTwo);
                _logger.info("Min: " + topLeftVertexMin + " Max: " + topLeftVertexMax + " Selected: " + topLeftCell +
                " Exclusions: " + excludeOne + " - " + excludeTwo);
            }
            else if (mapDirection == CardinalPoint.SUD)
            {
                grid = new Grid(corridorLenght, corridorSection);
                corrOrient = Corridor.Orientation.vertical;
                Cell topLeftVertexMin = lastRoom.bottomLeftVertex();
                Cell topLeftVertexMax = topLeftVertexMin.plusCell(0, lastRoom.width() - corridorSection);
                //Excluding cells to avoid Inward and Outward Corner Walls Overlapping
                Cell excludeOne = topLeftVertexMin.plusCell(0, 1);
                Cell excludeTwo = topLeftVertexMax.minusCell(0, 1);
                topLeftCell =
                    cellRangePicker.drawBetweenWithExclusion(topLeftVertexMin, topLeftVertexMax, excludeOne,
                        excludeTwo);
                _logger.info("Min: " + topLeftVertexMin + " Max: " + topLeftVertexMax + " Selected: " + topLeftCell +
                " Exclusions: " + excludeOne + " - " + excludeTwo);
            }
            else if (mapDirection == CardinalPoint.WEST)
            {
                grid = new Grid(corridorSection, corridorLenght);
                corrOrient = Corridor.Orientation.horizontal;
                Cell topLeftVertexMin = lastRoom.topLeftVertex().minusSize(0, corridorLenght);
                Cell topLeftVertexMax = topLeftVertexMin.plusCell(lastRoom.height() - corridorSection, 0);
                //Excluding cells to avoid Inward and Outward Corner Walls Overlapping
                Cell excludeOne = topLeftVertexMin.plusCell(1, 0);
                Cell excludeTwo = topLeftVertexMax.minusCell(1, 0);
                topLeftCell =
                    cellRangePicker.drawBetweenWithExclusion(topLeftVertexMin, topLeftVertexMax, excludeOne,
                        excludeTwo);
                _logger.info("Min: " + topLeftVertexMin + " Max: " + topLeftVertexMax + " Selected: " + topLeftCell +
                " Exclusions: " + excludeOne + " - " + excludeTwo);
            }

            return new Corridor(topLeftCell, grid, corrOrient);
        }
    }
}