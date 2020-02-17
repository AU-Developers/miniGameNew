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
        [Range(.01f, 200)] [SerializeField] private float _speedMultiplier = 160;
        [SerializeField] public float GaugePoint { get; private set; } = 0;

        [Range(0, 100)] [SerializeField] private int _distanceBetweenTwoValues = 20;
        [Range(0, 1000)] [SerializeField] private float timeBetWeenValues;

        /// <summary>
        /// Moving States
        /// <para>-1 - Moving Left</para>
        /// <para>0 - Not Moving</para>
        /// <para>1 - Moving Right</para>
        /// </summary>
        public int movingState = 0;
        int destinationValue;

        bool captureGaugePoint;
        public int calculatedDistance;
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
            if (Input.GetKeyDown(KeyCode.Space))
                GenerateDestinationValue();

            //if (captureGaugePoint)
               // StoppingGauge();
            
            MoveGauge();
        }


        #endregion

        private void MoveGauge()
        {
            if (!captureGaugePoint)
            {
                if (movingState == 1)
                    GaugePoint += Time.deltaTime * _speedMultiplier;
                else if (movingState == -1)
                    GaugePoint -= Time.deltaTime * _speedMultiplier;
            }
            else
            {
                if (_decelerating > 0)
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
                        _speedMultiplier -= Time.deltaTime * timeBetWeenValues;
                    else
                        _speedMultiplier = 0;
                }
            }*/
        }

        private void GenerateDestinationValue()
        {

            float randomValue = Random.value;
            float chanceOfRed = 0.1f;
            float chanceOfWhite = .45f;
            float chanceOfBlack = .45f;

            captureGaugePoint = true;

            float sumOfAll = chanceOfBlack + chanceOfWhite + chanceOfRed;

            float a = chanceOfRed / sumOfAll;
            float b = (chanceOfWhite / sumOfAll) + a;
            float c = (chanceOfBlack / sumOfAll) + a + b;

            if (randomValue < a)
                destinationValue = Random.Range(-10, 10);
            else if (randomValue < b && movingState == 1 || randomValue < c && movingState == 1)
                destinationValue = Random.Range(11, 100);
            else if (randomValue < c && movingState == -1 || randomValue < b && movingState == -1)
                destinationValue = Random.Range(-100, -11);

            print("Random Number " + destinationValue + " Random Value " + randomValue);

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

            _decelerating = _speedMultiplier * (calculatedDistance / GaugePoint);
        }
    }
}
