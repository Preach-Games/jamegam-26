using DungeonDraws.SO;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DungeonDraws.Card
{
    public class CardInstance : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private TMP_Text _titleText, _descriptionText;

        [SerializeField]
        private float _dissolveSpeed = 1f;
        private float _dissolveAmount = 1f;

        [SerializeField]
        private Material _bgCardMat;

        [SerializeField]
        private Image _cardArt;

        private CardInfo _info;

        public void Init(CardInfo info)
        {
            _info = info;
            _titleText.text = info.Name;
            _descriptionText.text = info.Description;
            // _cardArt.sprite = null; // TODO:
            _bgCardMat = Instantiate(GetComponent<Image>().material);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            CardsManager.Instance.ShowTooltip(_info);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            CardsManager.Instance.HideTooltip();
        }

        public IEnumerator Autodestroy(bool wasSelected = false)
        {
            if (wasSelected) Debug.Log("wasSelected");

            yield return DissolveThenDestroyCard();
        }

        private IEnumerator DissolveThenDestroyCard()
        {
            while (_dissolveAmount > 0)
            {
                _dissolveAmount -= Time.deltaTime * _dissolveSpeed;
                _bgCardMat.SetFloat("_Dissolve", _dissolveAmount);
                GetComponent<Image>().material = _bgCardMat;
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}
