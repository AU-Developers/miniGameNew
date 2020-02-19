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

        [SerializeField] GameObject menuPanel;

        [SerializeField] Text text;

        [SerializeField] GameObject panel;

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

        private void Start()
        {
            _barPosition.y = movingBar.anchoredPosition.y;
            _whiteSize = white.sizeDelta;
            _blackSize = black.sizeDelta;
            _redSize = black.sizeDelta;

            blackSize = (GameController.Instance.playerData.goodChanceRange / 100) * _whiteSize.x;
            redSize = (GameController.Instance.playerData.perfectChanceRange / 100) * _whiteSize.x;

            audioSource = GetComponent<AudioSource>();
            animator = panel.GetComponent<Animator>();
        }

        void Update()
        {
            barPosition = (whiteSize/2) * (GameController.Instance.GaugePoint / 100);

            if (GameController.Instance.Results)
                menuPanel.SetActive(true);
            else
                menuPanel.SetActive(false);



            if (GameController.Instance.SpeedMultiplier == 0 && GameController.Instance.PlaySoundOnce)
            {
                int index = 0;
                if (GameController.Instance.playerData.stoppingPoint <= 10)
                {
                    index = 2;
                    
                    text.text = "Hit";
                }
                else if (GameController.Instance.playerData.stoppingPoint <= 20)
                {
                    index = 1;
                    text.text = "Good";
                }
                else
                {
                    index = 0;
                    text.text = "Miss";
                }

                animator.SetInteger("state", index+1);
                audioSource.PlayOneShot(soundClips[index]);
                GameController.Instance.PlaySoundOnce = false;
            }
            else if (GameController.Instance.SpeedMultiplier > 0)
            {
                animator.SetInteger("state", 0);
            }
        }
    }
}
