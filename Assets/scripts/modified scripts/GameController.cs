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

        public float SpeedMultiplier{ get { return _speedMultiplier;} }

        public bool Results { get; private set; } = false;
        public bool PlaySoundOnce { get; set; } = false;

        /// <summary>
        /// Moving States
        /// <para>-1 - Moving Left</para>
        /// <para>0 - Not Moving</para>
        /// <para>1 - Moving Right</para>
        /// </summary>
        [SerializeField] private int movingState = 0;

        float maxSpeedMultiplier;

        bool startedDecelerating;

        [SerializeField] private float _decelerationRate = 0;

        #region MonoBehaviour Methods
        private void Awake()
        {
            _instance = this;
            if (!_playerData)
                Debug.Log("No Player Data found.");

            RandomGenerationOfStoppingPoint();

            maxSpeedMultiplier = _speedMultiplier;
        }
        private void FixedUpdate()
        {
            if (Input.GetKeyDown(input) && !PlaySoundOnce)
            {
                startedDecelerating = true;
                PlaySoundOnce = true;
            }
                

            MoveGauge();
        }


        #endregion

        private void MoveGauge()
        {
            if (!startedDecelerating)
            {
                if (movingState == 1)
                    GaugePoint += Time.fixedDeltaTime * _speedMultiplier;
                else if (movingState == -1)
                    GaugePoint -= Time.fixedDeltaTime * _speedMultiplier;
            }
            else
            {
                float distance = 0;
                if (_decelerationRate == 0 && _speedMultiplier > 0)
                {
                    if (movingState == 1) // Moving right
                    {
                        if (GaugePoint >= -playerData.stoppingPoint) // Overlapped negative stopping point
                        {
                            if (GaugePoint >= playerData.stoppingPoint) // Overlapped positive stopping point
                            {
                                distance = (100 - GaugePoint) + (100 - playerData.stoppingPoint);
                            }
                            else // did not overlap positive stopping point
                            {
                                distance = playerData.stoppingPoint - GaugePoint;
                            }
                        }
                        else // did not overlap negative stopping point
                        {
                            distance = GaugePoint - -playerData.stoppingPoint;
                        }
                    }
                    else if (movingState == -1)
                    {
                        if (GaugePoint <= playerData.stoppingPoint) // Overlapped positive stopping point
                        {
                            if (GaugePoint <= -playerData.stoppingPoint) // Overlapped negative stopping point
                            {
                                distance = (-100 - GaugePoint) + (-100 - -playerData.stoppingPoint);
                            }
                            else // did not overlap negative stopping point
                            {
                                distance = -playerData.stoppingPoint - GaugePoint;
                            }
                        }
                        else // did not overlap positive
                        {
                            distance = GaugePoint - playerData.stoppingPoint;
                        }
                    }

                    distance = Mathf.Abs(distance);

                    _decelerationRate = (_speedMultiplier * _speedMultiplier)/(distance * 2);
                }

                if (_speedMultiplier > 0)
                    _speedMultiplier -= _decelerationRate * Time.fixedDeltaTime;
                else if (_speedMultiplier < 0)
                    _speedMultiplier = 0;
                else
                    Results = true;

                if (movingState == 1)
                    GaugePoint += Time.fixedDeltaTime * _speedMultiplier;
                else if (movingState == -1)
                    GaugePoint -= Time.fixedDeltaTime * _speedMultiplier;
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

        /// <summary>
        /// Random Value of Stopping Point
        /// </summary>
        private void RandomGenerationOfStoppingPoint()
        {
            if (Random.value >= .5f)
                movingState = 1;
            else
                movingState = -1;

            float randomValue = Random.value;

            if (randomValue <= playerData.perfectChanceRange / 100)
                playerData.stoppingPoint = Random.Range(0, 10);
            else if (randomValue <= playerData.goodChanceRange / 100)
                playerData.stoppingPoint = Random.Range(11, 20);
            else
                playerData.stoppingPoint = Random.Range(21, 100);
        }

        #region Functions for Buttons

        /// <summary>
        /// Reseting values to play again
        /// </summary>
        public void PlayAgain()
        {
            RandomGenerationOfStoppingPoint();
            _decelerationRate = 0;
            GaugePoint = 0;
            _speedMultiplier = maxSpeedMultiplier;
            startedDecelerating = false;
            Results = false;
        }

        #endregion
    }
}
