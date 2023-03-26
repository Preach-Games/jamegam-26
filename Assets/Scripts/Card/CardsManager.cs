using DungeonDraws.Game;
using DungeonDraws.SO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
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
        private GameInfo _info;

        // Post processing
        [SerializeField]
        private Volume _globalVolume;
        [SerializeField]
        private float _vignetteIntensity;

        private List<CardInfo> _cards;
        private List<CardInfo> _deck;

        // Tooltip
        [SerializeField]
        private GameObject _tooltip;
        private TMP_Text _tooltipText;

        private CardInfo _target;
        private float _cardTimer;
        private CardInstance _choosenCard;

        private void Awake()
        {
            Instance = this;
            _cards = _info.BaseDeck.ToList();
            _deck = new(_cards);
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
                    StartCardSelection();
                }
            }

            if (_target != null)
            {
                _tooltip.transform.position = Mouse.current.position.ReadValue();
            }
        }

        public void ResetDay()
        {
            EndCardSelection();
            _deck = new(_cards);
        }

        public void AddCard(CardInfo card)
        {
            _deck.Add(card);
            _cards.Add(card);
        }
        
        public void EndCardSelection()
        {
            HideTooltip();
            StartCoroutine(RemoveCards());
            Vignette vignette;
            if (_globalVolume.profile.TryGet<Vignette>(out vignette)) {
                vignette.intensity.value = 0;
            }
        }

        private IEnumerator RemoveCards()
        {
            int childCount = _cardContainer.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var card = _cardContainer.GetChild(i).GetComponent<CardInstance>();
                if (card.GetInstanceID() != _choosenCard.GetInstanceID())
                {
                    Destroy(card.gameObject);
                }
            }
            if (_choosenCard != null)
            {
                yield return _choosenCard.Autodestroy();
            }
            _cardCanvas.SetActive(false);
            _cardTimer = _info.TimeBeforeCardDisplay;
            GameManager.Instance.IsPaused = false;
            _choosenCard = null;
        }

        private void StartCardSelection()
        {
            for (int i = 0; i < _info.CardCount; i++)
            {
                var card = Instantiate(_info.CardPrefab, _cardContainer);
                var cardInstance = card.GetComponent<CardInstance>();
                var index = Random.Range(0, _deck.Count);
                card.GetComponent<Button>().onClick.AddListener(new(() => {
                    foreach (var m in _deck[index].Modifiers)
                    {
                        m.Do();
                    }
                    _choosenCard = cardInstance;
                    EndCardSelection();
                }));
                cardInstance.Init(_deck[index]);
                _deck.RemoveAt(index);

                if (!_deck.Any())
                {
                    _deck = new(_cards);
                }
            }
            _cardCanvas.SetActive(true);
            GameManager.Instance.IsPaused = true;
            
            Vignette vignette;
            if (_globalVolume.profile.TryGet<Vignette>(out vignette)) {
                vignette.intensity.value = _vignetteIntensity;
            }
        }

        public void ShowTooltip(CardInfo card)
        {
            _target = card;
            _tooltip.SetActive(true);
            _tooltip.transform.position = Mouse.current.position.ReadValue();
            _tooltipText.text = string.Join("\n", card.Modifiers.OrderBy(x => x.IsBonus).Select(x => x.ToString()));
            if (string.IsNullOrEmpty(_tooltipText.text))
            {
                _tooltipText.text = "<color=#222>No Effect</color>";
            }
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
