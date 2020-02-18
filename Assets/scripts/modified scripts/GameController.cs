using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minigame
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private KeyCode input = KeyCode.Space;
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

        [Range(.01f, 400)] [SerializeField] private float _speedMultiplier = 160;
        public float GaugePoint { get; private set; } = 0;

        [Range(0, 100)] [SerializeField] private int _distanceBetweenTwoValues = 20;
        [SerializeField] private float timeBetweenValues;

        /// <summary>
        /// Moving States
        /// <para>-1 - Moving Left</para>
        /// <para>0 - Not Moving</para>
        /// <para>1 - Moving Right</para>
        /// </summary>
        [SerializeField] private int movingState = 0;
        int destinationValue = 0;

        bool startedDecelerating;
        [SerializeField] private int calculatedDistance;
        [SerializeField] float _decelerating;

        #region MonoBehaviour Methods
        private void Awake()
        {
            _instance = this;
            if (!_playerData)
                Debug.Log("No Player Data found.");
            if (Random.value >= .5f)
                movingState = 1;
            else
                movingState = -1;
        }

        private void Update()
        {
            if (Input.GetKeyDown(input))

            
            MoveGauge();
        }


        #endregion

        private void MoveGauge()
        {
            if (!startedDecelerating)
            {
                if (movingState == 1)
                    GaugePoint += Time.deltaTime * _speedMultiplier;
                else if (movingState == -1)
                    GaugePoint -= Time.deltaTime * _speedMultiplier;
            }
            else
            {
                if (_decelerating > _speedMultiplier)
                    _decelerating = _speedMultiplier;
                else if (_decelerating > 0)
                    _decelerating -= Time.deltaTime * _speedMultiplier;
                else if (_decelerating < 0)
                    _decelerating = 0;


                if (movingState == 1)
                    GaugePoint += Time.deltaTime * _decelerating;
                else if (movingState == -1)
                    GaugePoint -= Time.deltaTime * _decelerating;
            }

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

        

        private void StoppingGauge()
        {
            //if (Mathf.Abs(distance) > _distanceBetweenTwoValues)
            //   _speedMultiplier -= Time.deltaTime * (GaugePoint / calculatedDistance);

            /*if (GaugePoint > destinationValue && movingState == 1
                    || GaugePoint < destinationValue && movingState == -1)
            {
                if (Mathf.Abs(distance) < _distanceBetweenTwoValues)
                {
                    if (_speedMultiplier > 0)
                        _speedMultiplier -= Time.deltaTime * timeBetweenValues;
                    else
                        _speedMultiplier = 0;
                }
            }*/
        }

        private void GenerateDestinationValue()
        {
            float chanceOfRed = playerData.perfectChance / 100;
            float chanceOfBlack = playerData.goodChance / 100;
            

            startedDecelerating = true;

            //var distance = (int)GaugePoint - destinationValue;

            print(GaugePoint);

            if (GaugePoint > destinationValue)
            {
                if (movingState == 1)
                    calculatedDistance = (100 - (int)GaugePoint) + (100 - destinationValue);
                else
                    calculatedDistance = ((-100 - (int)GaugePoint) * (-1)) + ((-100 - destinationValue) * (-1));
            }
            else
            {
                if (movingState == 1)
                    calculatedDistance = ((int)GaugePoint * (-1)) + destinationValue;
                else
                    calculatedDistance = ((100 + (int)GaugePoint)) + destinationValue + 100;
            }

            timeBetweenValues = calculatedDistance / _speedMultiplier;

            _decelerating = _speedMultiplier / timeBetweenValues;
        }
    }

    private void StartDecelerating()
    {

    }
}
