using DungeonDraws.SO;
using System.Collections;
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

        private void Awake()
        {
            _deck = _cards.ToList();
            _tooltipText = _tooltip.GetComponentInChildren<TMP_Text>();
            StartCoroutine(ShowCards());
        }

        private void Update()
        {
            if (_target != null)
            {
                _tooltip.transform.position = Mouse.current.position.ReadValue();
            }
        }

        public void HideCards()
        {
            HideTooltip();
            for (int i = 0; i < _cardContainer.childCount; i++)
            {
                Destroy(_cardContainer.GetChild(i).gameObject);
            }
            _cardCanvas.SetActive(false);
            StartCoroutine(ShowCards());
        }

        private IEnumerator ShowCards()
        {
            yield return new WaitForSeconds(_info.TimeBeforeCardDisplay);
            for (int i = 0; i < _info.CardCount; i++)
            {
                var card = Instantiate(_info.CardPrefab, _cardContainer);
                card.GetComponent<Button>().onClick.AddListener(new(HideCards));
                var index = Random.Range(0, _deck.Count);
                card.GetComponent<CardInstance>().Init(this, _deck[index]);
                _deck.RemoveAt(index);

                if (!_deck.Any())
                {
                    _deck = _cards.ToList();
                }
            }
            _cardCanvas.SetActive(true);
        }

        public void ShowTooltip(CardInfo card)
        {
            _target = card;
            _tooltip.SetActive(true);
            _tooltip.transform.position = Mouse.current.position.ReadValue();
            _tooltipText.text = string.Join("\n", card.Modifiers.OrderBy(x => x.IsBonus).Select(x => x.ToString()));
        }

        public void HideTooltip()
        {
            _target = null;
            _tooltip.SetActive(false);
        }
    }
}
