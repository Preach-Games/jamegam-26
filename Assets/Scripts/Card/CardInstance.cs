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
        private CardsManager _manager;

        public void Init(CardsManager manager, CardInfo info)
        {
            _manager = manager;
            _info = info;
            _titleText.text = info.Name;
            _descriptionText.text = info.Description;
            _image.sprite = null;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _manager.ShowTooltip(_info);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _manager.HideTooltip();
        }
    }
}
