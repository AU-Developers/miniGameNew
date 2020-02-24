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
        public int ScoreType { get; private set; } = -1;

        public float SpeedMultiplier{ get { return _speedMultiplier;} }

        /// <summary>
        /// Starting point of red object
        /// </summary>
        public float StartingPointOfPerfectChanceRange { get; private set; } = 0;
        /// <summary>
        /// Starting point of black object
        /// </summary>
        public float StartingPointOfGoodChanceRange { get; private set; } = 0;
        /// <summary>
        /// if the starting points are over 100 the exceeding number will be the offset
        /// </summary>
        float perfectExceedingOffset = 0, goodExceedingOffset = 0;
        /// <summary>
        /// 
        /// </summary>
        public bool RankUps { get; set; } = false;
        public bool PlaySoundOnce { get; set; } = false;
        public bool Resets { get; set; } = false;
        public int Attempts { get; set; } = 5;
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

        private void Update()
        {
            if (Input.GetKeyDown(input) && !PlaySoundOnce)
            {
                startedDecelerating = true;
            }
        }

        private void FixedUpdate()
        {
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
            {

                if (GaugePoint <= StartingPointOfPerfectChanceRange + _LevelData.perfectChanceRange
                    && GaugePoint > StartingPointOfPerfectChanceRange)
                {
                    ScoreType = 2;
                    RankUps = true;
                    Ranks++;
                }
                else if (GaugePoint <= StartingPointOfGoodChanceRange + _LevelData.goodChanceRange
                    && GaugePoint > StartingPointOfGoodChanceRange)
                {
                    ScoreType = 1;
                    RankUps = true;
                    Ranks++;
                }
                else
                {
                    ScoreType = 0;
                }
                StartCoroutine(RankUp());
            }

            if (GaugePoint > 100)
            {
                GaugePoint = 100;
                movingState = -1;
                if (!startedDecelerating)
                {
                    if (HP > 0)
                        HP -= _LevelData.penalty;
                    else
                        HP = 0;
                }
            }
            else if (GaugePoint < 0)
            {
                GaugePoint = 0;
                movingState = 1;
                if (!startedDecelerating)
                {
                    if (HP > 0)
                        HP -= _LevelData.penalty;
                    else
                        HP = 0;
                }
            }
        }

        private void FindingStartingPoint()
        {
            StartingPointOfPerfectChanceRange = _LevelData.position - _LevelData.perfectChanceRange;
            StartingPointOfGoodChanceRange = _LevelData.position - _LevelData.goodChanceRange;

            if (StartingPointOfPerfectChanceRange + _LevelData.perfectChanceRange > 100)
                perfectExceedingOffset = (StartingPointOfPerfectChanceRange + (_LevelData.perfectChanceRange * 2)) - 100;
            if (StartingPointOfGoodChanceRange + _LevelData.goodChanceRange > 100)
                goodExceedingOffset = (StartingPointOfGoodChanceRange + (_LevelData.goodChanceRange * 2)) - 100;

            if (StartingPointOfPerfectChanceRange < 0)
                StartingPointOfPerfectChanceRange = 0;
            if (StartingPointOfGoodChanceRange < 0)
                StartingPointOfGoodChanceRange = 0;

            StartingPointOfPerfectChanceRange -= perfectExceedingOffset;
            StartingPointOfGoodChanceRange -= goodExceedingOffset;
        }

        /// <summary>
        /// Reseting values to play again
        /// </summary>
        private IEnumerator RankUp()
        {
            PlaySoundOnce = true;
            if (Ranks < 4)
                _levelData = Resources.Load<LevelData>("ScriptableObjects/" + Ranks);
            else
                PlayGame = false;

            if (Attempts <= 0)
                PlayGame = false;
            if (HP <= 0)
                PlayGame = false;



            _LevelData.position = Random.Range(0, 101);
            Resets = true;
            movingState = 1;
            time = _LevelData.time;
            _decelerationRate = 0;
            GaugePoint = 0;
            startedDecelerating = false;
            RankUps = false;
            Attempts--;
            FindingStartingPoint();


            yield return new WaitForFixedUpdate();
            Resets = false;
            _speedMultiplier = _LevelData.speed;
            PlaySoundOnce = false;
        }

        #region Functions for Buttons

        public void Play()
        {
            Ranks = 0;
            Attempts = 5;
            HP = 100;
            StartCoroutine(RankUp());
            PlayGame = true;
        }

        public void Exit()
        {
            PlayGame = false;
        }

        #endregion
    }
}
