using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinigameV2
{
    public class ViewV2 : MonoBehaviour
    {
        [SerializeField] RectTransform movingBar = null;
        [SerializeField] RectTransform white = null, black = null, red = null;

        private Vector2 _barPosition = Vector2.zero;
        private Vector2 _whiteSize = Vector2.zero, _blackSize = Vector2.zero, _redSize = Vector2.zero;
        private Vector2 _whitePosition = Vector2.zero, _blackPosition = Vector2.zero, _redPosition = Vector2.zero;

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
            Initialize();
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            barPosition = (WhiteSize/2) * (GameplayControllerV2.Instance.GaugePoint / 100);
        }
        #endregion

        private void Initialize()
        {
            RedSize = WhiteSize * (GameplayControllerV2.Instance.LevelData.perfectChanceRange / 100);
            BlackSize = WhiteSize * (GameplayControllerV2.Instance.LevelData.goodChanceRange / 100);

            RedPosition = (WhiteSize / 2) * (GameplayControllerV2.Instance.LevelData.perfectOffset / 100);
            BlackPosition = (WhiteSize / 2) * (GameplayControllerV2.Instance.LevelData.goodOffset / 100);
        }
    }
}
