namespace DungeonDraws.Scripts.Systems.LevelGeneration.Pickers
{
    public interface IPickerStrategy {
        int drawBetween(int min, int max);
    }
}