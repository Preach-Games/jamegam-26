using DungeonDraws.SO;
using System.Collections;
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
        private GameInfo _info;

        private void Awake()
        {
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
            }
            _cardCanvas.SetActive(true);
        }
    }
}
