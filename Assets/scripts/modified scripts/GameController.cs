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

        public float StartingPointOfPerfectChanceRange { get; private set; } = 0;
        public float StartingPointOfGoodChanceRange { get; private set; } = 0;
        float differenceOfPerfect = 0, differenceOfGood = 0;

        public bool RankUps { get; set; } = false;
        public bool PlaySoundOnce { get; set; } = false;
        public bool Resets { get; set; } = false;

        int ranks;

        [Range(0, 2)] [SerializeField] float time;
        /*
         * .5
         * 1
         * 1.5
         * 2
         * 
         */

        /// <summary>
        /// Moving States
        /// <para>-1 - Moving Left</para>
        /// <para>0 - Not Moving</para>
        /// <para>1 - Moving Right</para>
        /// </summary>
        [SerializeField] private int movingState = 0;

        bool startedDecelerating;

        [SerializeField] private float _decelerationRate = 0;

        #region MonoBehaviour Methods
        private void Awake()
        {
            RankUp();

            _instance = this;
        }
        private void FixedUpdate()
        {
            if (Input.GetKeyDown(input) && !PlaySoundOnce)
            {
                startedDecelerating = true;
                PlaySoundOnce = true;
            }

            MoveGauge();

            print(playerData);
        }


        #endregion

        private void MoveGauge()
        {
            if (startedDecelerating)
            {
                if(_decelerationRate == 0 && _speedMultiplier > 0)
                    _decelerationRate = _speedMultiplier / time;
            }

            if (movingState == 1)
                GaugePoint += Time.fixedDeltaTime * _speedMultiplier;
            else if (movingState == -1)
                GaugePoint -= Time.fixedDeltaTime * _speedMultiplier;

            if (_speedMultiplier > 0)
                _speedMultiplier -= _decelerationRate * Time.fixedDeltaTime;
            else if (_speedMultiplier < 0)
                _speedMultiplier = 0;
            else
                RankUp();

            if (GaugePoint > 100)
            {
                GaugePoint = 100;
                movingState = -1;
            }
            else if (GaugePoint < 0)
            {
                GaugePoint = 0;
                movingState = 1;
            }
        }

        private void FindingStartingPoint()
        {
            StartingPointOfPerfectChanceRange = playerData.point - playerData.perfectChanceRange;
            StartingPointOfGoodChanceRange = playerData.point - playerData.goodChanceRange;

            if (StartingPointOfPerfectChanceRange + playerData.perfectChanceRange > 100)
                differenceOfPerfect = (StartingPointOfPerfectChanceRange + (playerData.perfectChanceRange * 2)) - 100;
                //StartingPointOfPerfectChanceRange = 100;
            if (StartingPointOfGoodChanceRange + playerData.goodChanceRange > 100)
                //StartingPointOfGoodChanceRange = 100;
                differenceOfGood = (StartingPointOfGoodChanceRange + (playerData.goodChanceRange * 2)) - 100;

            if (StartingPointOfPerfectChanceRange < 0)
                StartingPointOfPerfectChanceRange = 0;
            if (StartingPointOfGoodChanceRange < 0)
                StartingPointOfGoodChanceRange = 0;

            StartingPointOfPerfectChanceRange -= differenceOfPerfect;
            StartingPointOfGoodChanceRange -= differenceOfGood;
        }

        #region Functions for Buttons

        /// <summary>
        /// Reseting values to play again
        /// </summary>
        public void RankUp()
        {
            if(RankUps)
                ranks++;
            if (ranks < 4)
                _playerData = Resources.Load<PlayerData>("ScriptableObjects/" + ranks);
            else
                return;
            Resets = true;
            movingState = 1;
            time = playerData.time;
            _speedMultiplier = playerData.speed;
            _decelerationRate = 0;
            GaugePoint = 0;
            startedDecelerating = false;
            RankUps = false;
            FindingStartingPoint();
        }

        #endregion
    }
}
