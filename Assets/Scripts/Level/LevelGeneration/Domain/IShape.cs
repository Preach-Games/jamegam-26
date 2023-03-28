using DungeonDraws.Scripts.Systems.LevelGeneration.Plotters;
namespace DungeonDraws.Scripts.Systems.LevelGeneration.Domain {
    public interface IShape {
        bool isWithin(Grid mapGrid);
        bool collidesWith(IShape each);
        bool containsCell(Cell _topLeftVertex);
        void plotOn(int[,] map, IBoardPlotter plotter);
    }
}