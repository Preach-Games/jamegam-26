using System;
using DungeonDraws.Game;
using DungeonDraws.Scripts.Systems.LevelGeneration;
using DungeonDraws.Scripts.Systems.LevelGeneration.Plotters;
using DungeonDraws.Scripts.Systems.LevelGeneration.Renderer;
using DungeonDraws.Scripts.Utils.Attributes;
using DungeonDraws.Scripts.Utils.Logging;
using DungeonDraws.Scripts.Utils.Singleton;
using Unity.AI.Navigation;
using UnityEngine;

namespace DungeonDraws.Level
{
    public class LevelManager : Singleton<LevelManager>
    {
        [Header("Required")] [SerializeField] private LevelData _levelData;
        public GameObject _floorPrefab;
        public GameObject _wallPrefab;
        public GameObject _wallSeparatorPrefab;
        public GameObject _cornerInPrefab;
        public GameObject _cornerOutPrefab;
        public GameObject _boardHolder;

        [SerializeField]
        private GameObject _camera;

        [SerializeField]
        private NavMeshSurface _nav;

        [Header("Dev Options")]
        [SerializeField]
        private bool _randomSeed; 
        [SerializeField]
        private bool _devLog;
        [SerializeField]
        private bool _drawGrid = false;
        [SerializeField]
        private bool _drawTiles = false;
        [SerializeField]
        private Loglevel _logLevel;

        private IXLogger _logger;
        private int _seed;

        [Header("Generated Data")] [SerializeField]
        private int[,] _tilesMap;

        private LevelGenerator _generator;
        private LevelRenderer _renderer;

        private Vector3 _lastSpawn;
        private int _lastSpawnX;
        private int _lastSpawnZ;

        private void Awake()
        {
            SetParams();
            GameStatusHandler.Instance.OnLoading += (sender, eventArgs) =>
            {
                var rects = new[]
                {
                    new Rect(0, 0, .5f, .5f),
                    new Rect(.5f, 0, .5f, 0f),
                    new Rect(0, .5f, .5f, .5f),
                    new Rect(.5f, .5f, .5f, .5f)
                };
                for (var i = 0; i < rects.Length; i++)
                {
                    LoadLevel(new(i * 1000f, 0f, 0f));
                    var cam = Instantiate(_camera, new(i * 1000f + 50f, -10f, 50f), _camera.transform.rotation);
                    var camera = cam.GetComponent<Camera>();
                    camera.rect = rects[i];
                }
            };
            _logger.info("Registered event to trigger on game load");
        }

        private void LoadLevel(Vector3 pos)
        {
            _logger.info("Generating new level with seed: " + _levelData._seed);
            GenerateDungeon(pos);
            // TODO: Sort out load complete and placement of dungeon assets etc.
            _nav.BuildNavMesh();

            DungeonDraws.Game.GameStatusHandler.Instance.WorldBuilt(this, new EventArgs());
        }

        private void OnValidate()
        {
            SetParams();
        }

        private void SetParams()
        {
            _logger = _devLog ? new UnityEngineLogger() : new NullLogger();
            _logger.setLogLimit(_logLevel);
            _generator = new LevelGenerator(10);
            _renderer = LevelRenderer.newInstance(this, _boardHolder);
            _seed = _levelData._seed;
        }

        private void GenerateSeed()
        {
            _seed = Time.time.ToString().GetHashCode();
        }

        [Button]
        private void GenerateDungeon(Vector3 pos)
        {
            if (_randomSeed)
            {
                GenerateSeed();
            }

            _boardHolder.transform.rotation = Quaternion.Euler(0, 0, 0);
            _boardHolder.transform.localScale = Vector3.one;
            _generator.SetMapSize(_levelData._mapHeight, _levelData._mapWidth);
            _generator.SetRoomsNumberRange(_levelData._roomsNumberMin, _levelData._roomsNumberMax);
            _generator.SetRoomSizeRange(_levelData._roomSizeMin, _levelData._roomSizeMax);
            _generator.SetCorridorLengthRange(_levelData._corridorLengthMin, _levelData._corridorLengthMax);
            _generator.SetCorridorWidthRange(_levelData._corridorWidthMin, _levelData._corridorWidthMax);
            _generator.SetPlotter(new DetailedTilesPlotter());
            if (_devLog) _generator.SetLogger(new UnityEngineLogger());
            _generator.SetSeed(_seed);

            _tilesMap = _generator.AsMatrix();
            _renderer.convertToMeshes(_tilesMap, pos);
        }

        public void OnDrawGizmos()
        {
            if (_lastSpawn != null)
            {
                float tileWidth = _floorPrefab.GetComponent<MeshRenderer>().bounds.size.x;
                float xPos = _lastSpawnX * tileWidth;
                float zPos = _lastSpawnZ * tileWidth;
                Gizmos.color = Color.cyan;
                /// Why is this negative X to render last spawn??
                Gizmos.DrawWireSphere(new Vector3(-_lastSpawn.x, 0.5f, _lastSpawn.z), 1f);
                Gizmos.color = Color.magenta;
                Gizmos.DrawCube(new Vector3(xPos + tileWidth / 2, 0, zPos + tileWidth / 2),
                    new Vector3(tileWidth, 1, tileWidth));
            }
            if (_floorPrefab && _drawGrid)
            {
                float tileWidth = _floorPrefab.GetComponent<MeshRenderer>().bounds.size.x;
                float mapWidth = _levelData._mapWidth * tileWidth;
                float mapHeight = _levelData._mapHeight * tileWidth;
                Gizmos.color = Color.red;
                GridGizmo(mapWidth, mapHeight, _levelData._mapWidth,
                    _levelData._mapHeight,
                    new Vector3(transform.position.x, 0, transform.position.z));

                if (_tilesMap != null && _drawTiles)
                {
                    DrawTilesMap();
                }
            }
        }

        public static void GridGizmo(float width, float height, int horizontalCellCount, int verticalCellCount,
            Vector3 position)
        {
            for (float x = 0 + position.x; x <= width; x += width / horizontalCellCount)
            {
                Gizmos.DrawLine(new Vector3(x, position.y, position.z), new Vector3(x, 0, height));
            }

            for (float z = 0 + position.z; z <= height; z += height / verticalCellCount)
            {
                Gizmos.DrawLine(new Vector3(position.x, position.y, z), new Vector3(width, 0, z));
            }
        }

        public void DrawTilesMap()
        {
            float floorSpan = _floorPrefab.GetComponent<MeshRenderer>().bounds.size.x;
            for (int row = 0; row < _tilesMap.GetLength(0); row++)
            {
                for (int col = 0; col < _tilesMap.GetLength(1); col++)
                {
                    float xPos = col * floorSpan;
                    float zPos = row * floorSpan;
                    int value = _tilesMap[row, col];
                    DetailedTileType type = (DetailedTileType)value;

                    switch (value)
                    {
                        case (int)DetailedTileType.Floor:
                            Gizmos.color = Color.white;
                            break;
                        case (int)DetailedTileType.Wall_N:
                        case (int)DetailedTileType.Wall_E:
                        case (int)DetailedTileType.Wall_S:
                        case (int)DetailedTileType.Wall_W:
                            Gizmos.color = Color.black;
                            break;
                        case (int)DetailedTileType.Corner_INN_NE:
                        case (int)DetailedTileType.Corner_INN_NW:
                        case (int)DetailedTileType.Corner_INN_SW:
                        case (int)DetailedTileType.Corner_INN_SE:
                            Gizmos.color = Color.green;
                            break;
                        case (int)DetailedTileType.Corner_OUT_NE:
                        case (int)DetailedTileType.Corner_OUT_NW:
                        case (int)DetailedTileType.Corner_OUT_SW:
                        case (int)DetailedTileType.Corner_OUT_SE:
                            Gizmos.color = Color.blue;
                            break;
                    }

                    if (value != (int)DetailedTileType.Empty)
                    {
                        Gizmos.DrawCube(new Vector3(xPos + floorSpan / 2, 0, zPos + floorSpan / 2),
                            new Vector3(floorSpan, 1, floorSpan));
                    }
                }
            }
        }

        public Vector3 GetCameraMapGridLocation()
        {
            if (_tilesMap != null)
            {
                int width = _tilesMap.GetLength((0));
                int height = _tilesMap.GetLength(1);
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        int xPos = (int)Mathf.Round(width / 2);
                        int yPos = (int)Mathf.Round(height / 2);
                        xPos = xPos + x >= width ? x - xPos : xPos + x;
                        yPos = yPos + x >= height ? y - yPos : yPos + y;

                        if ((DetailedTileType)_tilesMap[xPos, yPos] == DetailedTileType.Floor)
                        {
                            Vector2 returnPos = GetTileCoordinates(xPos, yPos);
                            return new Vector3(returnPos.x, 2, returnPos.y);
                        }
                    }
                }
            }

            _logger.error("_tilemap is null");
            return new Vector3(0, 0, 0);
        }

        Vector2 GetTileCoordinates(int x, int z)
        {
            if (_tilesMap != null && _boardHolder != null && _floorPrefab != null)
            {
                float tileSize = _floorPrefab.transform.GetComponent<MeshRenderer>().bounds.size.x;
                float xPos = x * tileSize;
                float zPos = z * tileSize;

                return new Vector2(
                    xPos + tileSize / 2,
                    zPos + tileSize / 2
                );
            }

            _logger.error("_tilemap, _boardHolder and/or _floorPrefab are null");
            return new Vector2(0, 0);
        }

        public Vector2 PickRandomLocation()
        {
            if (_tilesMap != null)
            {
                int maxAttempts = 1000;
                for (int i = 0; i < maxAttempts; i++)
                {
                    int x = UnityEngine.Random.Range(0, _tilesMap.GetLength(0));
                    int z = UnityEngine.Random.Range(0, _tilesMap.GetLength(1));
                    DetailedTileType value = (DetailedTileType)_tilesMap[x, z];
                    if (value == DetailedTileType.Floor)
                    {
                        _lastSpawnX = x;
                        _lastSpawnZ = z;
                        _lastSpawn = GetTileCoordinates(x, z);
                        return _lastSpawn;
                    }
                }
            }

            _logger.error("_tilemap is null");
            return new Vector2(0, 0);
        }

        public Vector2 FurthestWallFrom(Vector2 pos, DetailedTileType[] TileMask)
        {
            return new Vector2(0, 0);
        }

        public Vector4 GetBounds()
        {
            if (_tilesMap != null)
            {
                Vector2 min = GetTileCoordinates(0, 0);
                Vector2 max = GetTileCoordinates(_tilesMap.GetLength(0), _tilesMap.GetLength(1));
                return new Vector4(min.x, min.y, max.x, max.y);
            }

            _logger.error("_tilemap is null");
            return new Vector4(0, 0, 0, 0);
        }

        // TODO: Implement method that returns a map texture of the generated dungeon
        // public Texture2D GetMap(int width, int height)
        // {
        //     return new Texture2D(width, height);
        // }
    }
}