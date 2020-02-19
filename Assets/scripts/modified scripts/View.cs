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

        [SerializeField] GameObject MenuPanel;

        [SerializeField] Text text;

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
        }

        void Update()
        {
            barPosition = (whiteSize/2) * (GameController.Instance.GaugePoint / 100);

            if (GameController.Instance.Results)
                MenuPanel.SetActive(true);
            else
                MenuPanel.SetActive(false);

            if (GameController.Instance.playerData.stoppingPoint <= 10)
                text.text = "Hit";
            else
                text.text = "Miss";
        }
    }
}
