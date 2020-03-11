using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MinigameV2
{
    public class ViewV2 : MonoBehaviour
    {
        [SerializeField] private RectTransform movingBar = null;
        [SerializeField] private RectTransform white = null, black = null, red = null;

        private Vector2 _barPosition = Vector2.zero;
        private Vector2 _whiteSize = Vector2.zero, _blackSize = Vector2.zero, _redSize = Vector2.zero;
        private Vector2 _whitePosition = Vector2.zero, _blackPosition = Vector2.zero, _redPosition = Vector2.zero;

        [SerializeField] private GameObject StartingPanel = null, PlayingPanel = null, EndPanel = null;
        [SerializeField] private TextMeshProUGUI HP = null, Attempts = null, Score = null, FinalScore = null;

        [SerializeField] private Animator barAnimator;
        [SerializeField] private List<AudioClip> soundClips = new List<AudioClip>();
        AudioSource sound;

        [SerializeField] public SpriteRenderer penaltyAlpha;

        #region Variable Properties
        private float barPosition
        {
            get
            {
                return _barPosition.x;
            }
            set
            {
                if (value != _barPosition.x)
                {
                    _barPosition.x = value;
                    movingBar.anchoredPosition = _barPosition;
                }
            }
        }
        private float WhiteSize
        {
            get
            {
                return _whiteSize.x;
            }
            set
            {
                if (value != _whiteSize.x)
                {
                    _whiteSize.x = value;
                    white.sizeDelta = _whiteSize;
                }
            }
        }
        private float BlackSize
        {
            get
            {
                return _blackSize.x;
            }
            set
            {
                if (value != _blackSize.x)
                {
                    _blackSize.x = value;
                    black.sizeDelta = _blackSize;
                }
            }
        }
        private float RedSize
        {
            get
            {
                return _redSize.x;
            }
            set
            {
                if (value != _redSize.x)
                {
                    _redSize.x = value;
                    red.sizeDelta = _redSize;
                }
            }
        }

        private float BlackPosition
        {
            get
            {
                return _blackPosition.x;
            }
            set
            {
                if (value != _blackPosition.x)
                {
                    float y = black.anchoredPosition.y;
                    _blackPosition.y = y;
                    _blackPosition.x = value;
                    black.anchoredPosition = _blackPosition;
                }
            }
        }

        private float RedPosition
        {
            get
            {
                return _redPosition.x;
            }
            set
            {
                if (value != _redPosition.x)
                {
                    _redPosition.y = red.anchoredPosition.y;
                    _redPosition.x = value;
                    red.anchoredPosition = _redPosition;
                }
            }
        }
        #endregion

        #region Monobehaviour Methods
        private void Awake()
        {
            _barPosition.y = movingBar.anchoredPosition.y;
            _redSize.y = red.anchoredPosition.y;
            _blackSize.y = black.anchoredPosition.y;

            _whiteSize = white.sizeDelta;
            _redSize = red.sizeDelta;
            _blackSize = black.sizeDelta;
        }

        private void Start()
        {
            sound = GetComponent<AudioSource>();
            Initialize();

        }

        private void FixedUpdate()
        {
            barPosition = (WhiteSize/2) * (GameplayControllerV2.Instance.GaugePoint / 100);
            StartingPanel.SetActive(GameplayControllerV2.Instance.GameState == 0);
            Initialize();
            if (GameplayControllerV2.Instance.GameState == 1 && !GameplayControllerV2.Instance.GameMoveRequest[1])
            {
                PlayingPanel.SetActive(true);
                UpdateTexts();
                if (GameplayControllerV2.Instance.JudgingScore)
                {
                    barAnimator.GetComponent<Animator>().SetInteger("state", GameplayControllerV2.Instance.ScoreType + 1);
                    sound.PlayOneShot(soundClips[GameplayControllerV2.Instance.ScoreType]);
                    print("Played Sound!");
                }
                else
                {
                    barAnimator.GetComponent<Animator>().SetInteger("state", 0);
                }
            }
            else
                PlayingPanel.SetActive(false);
            EndPanel.SetActive(GameplayControllerV2.Instance.GameState == 2);

            Color _alpha = penaltyAlpha.color;
            _alpha.a = GameplayControllerV2.Instance.alphaValue;
            penaltyAlpha.color = _alpha;

        }
        #endregion

        private void Initialize()
        {
            if (GameplayControllerV2.Instance.levelSize < GameplayControllerV2.Instance.Level)
                return;

            RedSize = WhiteSize * (GameplayControllerV2.Instance.LevelData.perfectChanceRange / 100);
            BlackSize = WhiteSize * (GameplayControllerV2.Instance.LevelData.goodChanceRange / 100);

            RedPosition = (WhiteSize / 2) * (GameplayControllerV2.Instance.LevelData.perfectOffset / 100);
            BlackPosition = (WhiteSize / 2) * (GameplayControllerV2.Instance.LevelData.goodOffset / 100);
        }

        private void UpdateTexts()
        {
            HP.text = string.Format("HP: {0}", GameplayControllerV2.Instance.HP);
            Attempts.text = string.Format("Attempts: {0}",GameplayControllerV2.Instance.Attempts);
            Score.text = string.Format("Score: {0}", GameplayControllerV2.Instance.Score);
            FinalScore.text = string.Format("Final Score:\n{0}", GameplayControllerV2.Instance.Score);
        }
    }
}
