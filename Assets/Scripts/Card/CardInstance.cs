using DungeonDraws.SO;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

namespace DungeonDraws.Card
{
    public class CardInstance : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private TMP_Text _titleText, _descriptionText;

        [SerializeField]
        private float _dissolveSpeed = 1f;
        private float _dissolveAmount = .7f;

        [SerializeField]
        private Material _dissolveMat;

        [SerializeField]
        private Image _cardArt;

        private CardInfo _info;

        public void Init(CardInfo info)
        {
            _info = info;
            _titleText.text = info.Name;
            _descriptionText.text = info.Description;
            _dissolveMat = Instantiate(GetComponent<Image>().material);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            CardsManager.Instance.ShowTooltip(_info);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            CardsManager.Instance.HideTooltip();
        }

        public IEnumerator Autodestroy(bool isSelected = false)
        {
            if (isSelected)
            {
                // TODO: Implements a different effect for the selected card
            }

            yield return DissolveThenDestroyCard();
        }

        private IEnumerator DissolveThenDestroyCard()
        {
            while (_dissolveAmount > 0)
            {
                _titleText.DOFade(0, .3f);
                _descriptionText.DOFade(0, .3f);
                _dissolveAmount -= Time.deltaTime * _dissolveSpeed;
                _dissolveMat.SetFloat("_Dissolve", _dissolveAmount);
                GetComponent<Image>().material = _dissolveMat;
                _cardArt.GetComponent<Image>().material = _dissolveMat;
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}
