using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minigame
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private PlayerData _playerData = null;
        public PlayerData playerData
        {
            get
            {
                return _playerData;
            }
        }

        private static GameController _instance;

        public static GameController Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = (GameController)FindObjectOfType(typeof(GameController));
                }
                return _instance;   
            }
            set
            {
                _instance = value;
            }
        }

        [Range(0,100)] [SerializeField] private float _playerChance = 60;
        [Range(.01f, 100)] [SerializeField] private float _speedMultiplier = 1;
        [SerializeField] public float GaugePoint { get; private set; } = 0;


        /// <summary>
        /// Moving States
        /// <para>-1 - Moving Left</para>
        /// <para>0 - Not Moving</para>
        /// <para>1 - Moving Right</para>
        /// </summary>
        private int movingState = 0;


        #region MonoBehaviour Methods
        private void Awake()
        {
            _instance = this;
            if (!_playerData)
            {
                Debug.Log("No Player Data found.");
            }
            if (Random.value >= .5f)
                movingState = 1;
            else
                movingState = -1;
        }

        private void Update()
        {
            MoveGauge();
        }


        #endregion

        private void MoveGauge()
        {
            if (movingState == 1)
                GaugePoint += Time.deltaTime* _speedMultiplier;
            else if (movingState == -1)
                GaugePoint -= Time.deltaTime* _speedMultiplier;

            if (GaugePoint > 100)
            {
                GaugePoint = 100;
                movingState = -1;
            }
            else if (GaugePoint< -100)
            {
                GaugePoint = -100;
                movingState = 1;
            }
        }


    }
}
