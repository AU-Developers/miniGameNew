using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minigame
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private KeyCode input = KeyCode.Space;
        [SerializeField] private LevelData _levelData = null;
        public LevelData _LevelData
        {
            get
            {
                return _levelData;
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
        public int Chances { get; set; } = 5;
        public int Ranks { get; set; } = 0;
        public int HP { get; set; } = 100;
        public bool PlayGame { get; set; } = false;

        bool penalty;

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
            _instance = this;
        }

        private void FixedUpdate()
        {
            if (Input.GetKeyDown(input) && !PlaySoundOnce)
            {
                startedDecelerating = true;
                PlaySoundOnce = true;
            }

            if (PlayGame && HP > 0)
                MoveGauge();

            if (HP < 0)
                HP = 0;
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
                if (HP > 0)
                    HP -= _LevelData.penalty;
                else
                    HP = 0;
            }
            else if (GaugePoint < 0)
            {
                GaugePoint = 0;
                movingState = 1;
                if (HP > 0)
                    HP -= _LevelData.penalty;
                else
                    HP = 0;

            }
        }

        private void FindingStartingPoint()
        {
            StartingPointOfPerfectChanceRange = _LevelData.point - _LevelData.perfectChanceRange;
            StartingPointOfGoodChanceRange = _LevelData.point - _LevelData.goodChanceRange;

            if (StartingPointOfPerfectChanceRange + _LevelData.perfectChanceRange > 100)
                differenceOfPerfect = (StartingPointOfPerfectChanceRange + (_LevelData.perfectChanceRange * 2)) - 100;
            if (StartingPointOfGoodChanceRange + _LevelData.goodChanceRange > 100)
                differenceOfGood = (StartingPointOfGoodChanceRange + (_LevelData.goodChanceRange * 2)) - 100;

            if (StartingPointOfPerfectChanceRange < 0)
                StartingPointOfPerfectChanceRange = 0;
            if (StartingPointOfGoodChanceRange < 0)
                StartingPointOfGoodChanceRange = 0;

            StartingPointOfPerfectChanceRange -= differenceOfPerfect;
            StartingPointOfGoodChanceRange -= differenceOfGood;
        }

        /// <summary>
        /// Reseting values to play again
        /// </summary>
        private void RankUp()
        {
            if (RankUps)
                Ranks++;
            if (Ranks < 4)
                _levelData = Resources.Load<LevelData>("ScriptableObjects/" + Ranks);
            else
                PlayGame = false;
            if (Chances <= 0)
                PlayGame = false;
            if (HP <= 0)
                PlayGame = false;

            _LevelData.point = Random.Range(0, 101);
            Resets = true;
            movingState = 1;
            time = _LevelData.time;
            _speedMultiplier = _LevelData.speed;
            _decelerationRate = 0;
            GaugePoint = 0;
            startedDecelerating = false;
            RankUps = false;
            Chances--;
            PlaySoundOnce = false;
            FindingStartingPoint();
        }

        #region Functions for Buttons

        public void Play()
        {
            Ranks = 0;
            Chances = 5;
            HP = 100;
            RankUp();
            PlayGame = true;
        }

        public void Exit()
        {
            PlayGame = false;
        }

        #endregion
    }
}
