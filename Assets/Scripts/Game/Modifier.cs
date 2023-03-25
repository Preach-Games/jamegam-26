using System;

namespace DungeonDraws.Game
{
    [Serializable]
    public class Modifier
    {
        public ModifierType Type;
        public int PercentChange;
        public bool IsBonus;

        private string PTS()
        {
            return $"{(PercentChange > 0 ? "+" : string.Empty)}{PercentChange}";
        }

        private string TypeToString()
        {
            return Type switch
            {
                ModifierType.HERO_SPAWN_RATE => $"Hero Spawn Rate: {PTS()}%",
                ModifierType.INCOME => $"Gold Earned: {PTS()}%",
                ModifierType.RATS => $"Rats Gained: {PTS()}",
                ModifierType.SANITY => $"Dungeon Sanity: {PTS()}%",
                _ => throw new NotImplementedException()
            };
        }

        public override string ToString()
        {
            return $"<color={(IsBonus ? "#00cc00" : "red")}>{TypeToString()}</color>";
        }
    }
}
