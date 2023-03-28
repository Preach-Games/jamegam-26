﻿using UnityEngine;

namespace DungeonDraws.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/GameInfo", fileName = "GameInfo")]
    public class GameInfo : ScriptableObject
    {
        [Header("Deck")]
        public CardInfo[] BaseDeck;
        public CardInfo[] Shop;
        public int ShopCost;

        [Header("Card")]
        public int CardCount;
        public float TimeBeforeCardDisplay;
        public GameObject CardPrefab;

        [Header("Global")]
        public int BaseGold;
        public int DailyIncome;
        public float DayDuration;

        [Header("Spawn")]
        public float TimeBetweenSpawn;
        public CharacterInfo[] Enemies;
    }
}