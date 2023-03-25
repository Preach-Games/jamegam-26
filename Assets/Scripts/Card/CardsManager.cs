using DungeonDraws.SO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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

        private void Awake()
        {
            _deck = _cards.ToList();
            StartCoroutine(ShowCards());
        }

        public void HideCards()
        {
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
                card.GetComponent<CardInstance>().Init(_deck[index]);
                _deck.RemoveAt(index);

                if (!_deck.Any())
                {
                    _deck = _cards.ToList();
                }
            }
            _cardCanvas.SetActive(true);
        }
    }
}
