using DungeonDraws.Scripts.Systems.LevelGeneration.Domain;

namespace DungeonDraws.Scripts.Systems.LevelGeneration.Plotters {
    public interface IBoardPlotter {
        void applyOnRoom(Room room, int[,] map);
        void applyOnCorridor(Corridor corridor, int[,] map);
    }
}