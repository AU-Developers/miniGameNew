using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Minigame
{
    public class View : MonoBehaviour
    {
        [SerializeField] RectTransform movingBar = null;
        [SerializeField] RectTransform white = null, black = null, red = null;

        [SerializeField] Text text,hpText,chancesText;

        [SerializeField] GameObject panel,btnPlay,btnExit;

        AudioSource audioSource;
        Animator animator;

        [SerializeField] List<AudioClip> soundClips = new List<AudioClip>();

        private Vector2 _barPosition = Vector2.zero;
        private Vector2 _whiteSize = Vector2.zero, _blackSize = Vector2.zero, _redSize = Vector2.zero;
        private Vector2 _whitePosition = Vector2.zero, _blackPosition = Vector2.zero, _redPosition = Vector2.zero;
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
        private float whiteSize
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
        private float blackSize
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
        private float redSize
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

        private void ResetPositions()
        {
            _barPosition.y = movingBar.anchoredPosition.y;
            _redSize.y = red.anchoredPosition.y;
            _blackSize.y = black.anchoredPosition.y;

            _whiteSize = white.sizeDelta;

            RedPosition = whiteSize * ((GameController.Instance.StartingPointOfPerfectChanceRange + (GameController.Instance.LevelData.perfectChanceRange / 2)) / 100);
            BlackPosition = whiteSize * ((GameController.Instance.StartingPointOfGoodChanceRange + (GameController.Instance.LevelData.goodChanceRange / 2)) / 100);

            _redSize = black.sizeDelta;
            _blackSize = black.sizeDelta;

            blackSize = (GameController.Instance.LevelData.goodChanceRange / 100) * _whiteSize.x;
            redSize = (GameController.Instance.LevelData.perfectChanceRange / 100) * _whiteSize.x;
        }

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            animator = panel.GetComponent<Animator>();
        }

        void FixedUpdate()
        {
            barPosition = whiteSize * (GameController.Instance.GaugePoint / 100);

            hpText.text = "HP:" + GameController.Instance.HP;
            chancesText.text = "Tries: " + GameController.Instance.Attempts;

            btnPlay.SetActive(!GameController.Instance.PlayGame);
            btnExit.SetActive(GameController.Instance.PlayGame);
                

            if (GameController.Instance.Resets)
            {
                ResetPositions();
            }

            if (GameController.Instance.SpeedMultiplier == 0 && GameController.Instance.PlaySoundOnce)
            {
                print(GameController.Instance.ScoreType + 1);
                animator.SetInteger("state", GameController.Instance.ScoreType+1);
                audioSource.PlayOneShot(soundClips[GameController.Instance.ScoreType]);
            }
            else
            {
                animator.SetInteger("state", 0);
            }

            if (GameController.Instance.PlayGame)
                text.text = "Score: " + GameController.Instance.Score;
        }
    }
}
