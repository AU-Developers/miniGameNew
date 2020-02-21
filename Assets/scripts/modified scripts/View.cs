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
                    black.anchoredPosition = _blackSize;
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
                    red.anchoredPosition = _redSize;
                }
            }
        }

        private void ResetPositions()
        {
            _barPosition.y = movingBar.anchoredPosition.y;
            _redSize.y = red.anchoredPosition.y;
            _blackSize.y = black.anchoredPosition.y;

            _whiteSize = white.sizeDelta;

            RedSize = whiteSize * ((GameController.Instance.StartingPointOfPerfectChanceRange + (GameController.Instance._LevelData.perfectChanceRange / 2)) / 100);
            BlackSize = whiteSize * ((GameController.Instance.StartingPointOfGoodChanceRange + (GameController.Instance._LevelData.goodChanceRange / 2)) / 100);

            _redSize = black.sizeDelta;
            _blackSize = black.sizeDelta;

            blackSize = (GameController.Instance._LevelData.goodChanceRange / 100) * _whiteSize.x;
            redSize = (GameController.Instance._LevelData.perfectChanceRange / 100) * _whiteSize.x;
        }

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            animator = panel.GetComponent<Animator>();
        }

        void Update()
        {
            barPosition = whiteSize * (GameController.Instance.GaugePoint / 100);

            hpText.text = "HP:" + GameController.Instance.HP;
            chancesText.text = "Tries: " + GameController.Instance.Chances;

            if (GameController.Instance.PlayGame)
            {
                btnPlay.SetActive(false);
                btnExit.SetActive(true);
            }
            else
            {
                btnPlay.SetActive(true);
                btnExit.SetActive(false);
            }
                

            if (GameController.Instance.Resets)
            {
                ResetPositions();
                GameController.Instance.Resets = false;
            }

            if (GameController.Instance.SpeedMultiplier == 0 && GameController.Instance.PlaySoundOnce)
            {
                int index = 0;

                if (GameController.Instance.GaugePoint <= GameController.Instance.StartingPointOfPerfectChanceRange + GameController.Instance._LevelData.perfectChanceRange 
                    && GameController.Instance.GaugePoint > GameController.Instance.StartingPointOfPerfectChanceRange)
                {
                    index = 2;
                    GameController.Instance.RankUps = true;
                }
                else if (GameController.Instance.GaugePoint <= GameController.Instance.StartingPointOfGoodChanceRange + GameController.Instance._LevelData.goodChanceRange 
                    && GameController.Instance.GaugePoint > GameController.Instance.StartingPointOfGoodChanceRange)
                {
                    index = 1;
                    GameController.Instance.RankUps = true;
                }
                else
                {
                    index = 0;
                }

                animator.SetInteger("state", index+1);
                audioSource.PlayOneShot(soundClips[index]);
            }
            else
            {
                animator.SetInteger("state", 0);
            }

            if(GameController.Instance.Ranks > 3)
                text.text = "Success";
            else if(GameController.Instance.Ranks < 1)
                text.text = "";
            else
                text.text = "Failed";
        }
    }
}
