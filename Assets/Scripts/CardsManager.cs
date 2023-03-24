using System.Collections;
using UnityEngine;

namespace DungeonDraws
{
    public class CardsManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject _cardCanvas;

        private void Awake()
        {
            StartCoroutine(ShowCards());
        }

        public void HideCards()
        {
            _cardCanvas.SetActive(false);
            StartCoroutine(ShowCards());
        }

        private IEnumerator ShowCards()
        {
            yield return new WaitForSeconds(10f);
            _cardCanvas.SetActive(true);
        }
    }
}
