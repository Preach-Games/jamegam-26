using DungeonDraws.Game;
using DungeonDraws.SO;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace DungeonDraws.Card
{
    public class CardsManager : MonoBehaviour
    {
        public static CardsManager Instance { private set; get; }

        [SerializeField]
        private GameObject _cardCanvas;

        [SerializeField]
        private Transform _cardContainer;

        [SerializeField]
        private CardInfo[] _cards;

        [SerializeField]
        private GameInfo _info;

        private List<CardInfo> _deck;

        // Tooltip
        [SerializeField]
        private GameObject _tooltip;
        private TMP_Text _tooltipText;

        private CardInfo _target;
        private float _cardTimer;

        private void Awake()
        {
            Instance = this;
            _deck = _cards.ToList();
            _tooltipText = _tooltip.GetComponentInChildren<TMP_Text>();
            _cardTimer = _info.TimeBeforeCardDisplay;
        }

        private void Update()
        {
            if (_cardTimer > 0f && !GameManager.Instance.IsPaused)
            {
                _cardTimer -= Time.deltaTime;
                if (_cardTimer <= 0f)
                {
                    ShowCards();
                }
            }

            if (_target != null)
            {
                _tooltip.transform.position = Mouse.current.position.ReadValue();
            }
        }

        public void ResetDay()
        {
            HideCards();
            _deck = _cards.ToList();
        }

        public void HideCards()
        {
            HideTooltip();
            for (int i = 0; i < _cardContainer.childCount; i++)
            {
                Destroy(_cardContainer.GetChild(i).gameObject);
            }
            _cardCanvas.SetActive(false);
            _cardTimer = _info.TimeBeforeCardDisplay;
            GameManager.Instance.IsPaused = false;
        }

        private void ShowCards()
        {
            for (int i = 0; i < _info.CardCount; i++)
            {
                var card = Instantiate(_info.CardPrefab, _cardContainer);
                card.GetComponent<Button>().onClick.AddListener(new(HideCards));
                var index = Random.Range(0, _deck.Count);
                card.GetComponent<CardInstance>().Init(_deck[index]);
                _deck.RemoveAt(index);

                if (!_deck.Any())
                {
                    _deck = _cards.ToList();
                }
            }
            _cardCanvas.SetActive(true);
            GameManager.Instance.IsPaused = true;
        }

        public void ShowTooltip(CardInfo card)
        {
            _target = card;
            _tooltip.SetActive(true);
            _tooltip.transform.position = Mouse.current.position.ReadValue();
            _tooltipText.text = string.Join("\n", card.Modifiers.OrderBy(x => x.IsBonus).Select(x => x.ToString()));
            Cursor.visible = false;
        }

        public void HideTooltip()
        {
            _target = null;
            _tooltip.SetActive(false);
            Cursor.visible = true;
        }
    }
}
