using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minigame
{
    public class View : MonoBehaviour
    {
        [SerializeField] RectTransform movingBar;

        private Vector2 _barPosition;
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

        private void Start()
        {
            _barPosition.y = movingBar.anchoredPosition.y;
        }

        void Update()
        {
            barPosition = 953 * (GameController.Instance.GaugePoint / 100);
        }
    }
}
