using DungeonDraws.SO;
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
        private Image _image;

        private CardInfo _info;

        public void Init(CardInfo info)
        {
            _info = info;
            _titleText.text = info.Name;
            _descriptionText.text = info.Description;
            _image.sprite = null;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            CardsManager.Instance.ShowTooltip(_info);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            CardsManager.Instance.HideTooltip();
        }
    }
}
