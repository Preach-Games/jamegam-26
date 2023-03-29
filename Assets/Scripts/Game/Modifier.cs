using DungeonDraws.Character;
using DungeonDraws.Spawn;
using System;
using DungeonDraws.SO;
using UnityEngine;

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
                ModifierType.GOLD => $"Gold Earned: {PTS()}",
                ModifierType.GOLD_IN_5_DAYS => $"Gold Earned in 5 Days: {PTS()}",
                ModifierType.RATS => $"Rats Gained: {PTS()}",
                ModifierType.DJINN => $"Djinns Gained: {PTS()}",
                ModifierType.MEDUSA => $"Gorgons Gained: {PTS()}",
                ModifierType.HERO_HEALTH => $"All Heros Health: {PTS()}%",
                ModifierType.MONSTER_HEALTH => $"All Monsters Health: {PTS()}%",
                ModifierType.MONSTER_ATTACK => $"All Monster Attack: {PTS()}%",
                _ => throw new NotImplementedException()
            };
        }

        public void Do()
        {
            switch (Type)
            {
                case ModifierType.MONSTER_ATTACK:
                    SpawnManager.Instance.TakePercentAttack(PercentChange, Faction.OVERLORD);
                    break;

                case ModifierType.INCOME:
                    GameManager.Instance.AddExpensesPercent(PercentChange);
                    break;

                case ModifierType.GOLD:
                    GameManager.Instance.AddExpenses(PercentChange, 0);
                    break;

                case ModifierType.GOLD_IN_5_DAYS:
                    GameManager.Instance.AddExpenses(PercentChange, 5);
                    break;

                case ModifierType.HERO_HEALTH:
                    SpawnManager.Instance.TakePercentDamage(PercentChange, Faction.HERO);
                    break;

                case ModifierType.MONSTER_HEALTH:
                    SpawnManager.Instance.TakePercentDamage(PercentChange, Faction.OVERLORD);
                    break;

                case ModifierType.HERO_SPAWN_RATE:
                    SpawnManager.Instance.ChangeSpawnRate(PercentChange);
                    break;

                case ModifierType.RATS:
                    for (int i = 0; i < PercentChange; i++)
                    {
                        SpawnManager.Instance.Spawn(Race.RAT, SpawnMethod.Random);
                    }
                    break;

                case ModifierType.DJINN:
                    for (int i = 0; i < PercentChange; i++)
                    {
                        SpawnManager.Instance.Spawn(Race.DJINN, SpawnMethod.Random);
                    }
                    break;

                case ModifierType.MEDUSA:
                    for (int i = 0; i < PercentChange; i++)
                    {
                        SpawnManager.Instance.Spawn(Race.MEDUSA, SpawnMethod.Random);
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        public override string ToString()
        {
            return $"<color={(IsBonus ? "#00cc00" : "red")}>{TypeToString()}</color>";
        }
    }
}
