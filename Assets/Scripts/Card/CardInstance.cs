using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DungeonDraws.Card
{
    public class CardInstance : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _titleText, _descriptionText;

        [SerializeField]
        private Image _image;

        public void Init(string title, string description, Sprite image)
        {
            _titleText.text = title;
            _descriptionText.text = description;
            _image.sprite = image;
        }
    }
}
