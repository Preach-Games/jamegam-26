using DungeonDraws.Card;
using DungeonDraws.Game;
using DungeonDraws.SO;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DungeonDraws.Shop
{
    public class ShopManager : MonoBehaviour
    {
        public static ShopManager Instance { private set; get; }

        [SerializeField]
        private GameInfo _info;

        [SerializeField]
        private Button _buyButton;

        [SerializeField]
        private GameObject _shopPanel;

        private List<CardInfo> _shop;

        private void Awake()
        {
            Instance = this;
            _shop = _info.Shop.ToList();
            _buyButton.GetComponentInChildren<TMP_Text>().text = $"Buy ({_info.ShopCost} Gold)";
        }

        public void Show()
        {
            _shopPanel.SetActive(true);
            _buyButton.interactable = GameManager.Instance.Gold > _info.ShopCost && _shop.Any();
        }

        public void Buy()
        {
            GameManager.Instance.Gold -= _info.ShopCost;
            _buyButton.interactable = GameManager.Instance.Gold > _info.ShopCost && _shop.Any();
            var index = Random.Range(0, _shop.Count);
            CardsManager.Instance.AddCard(_shop[index]);
            _shop.RemoveAt(index);
        }
    }
}
