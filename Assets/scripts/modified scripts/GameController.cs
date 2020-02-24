using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minigame
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private KeyCode input = KeyCode.Space;
        [SerializeField] private LevelData _levelData = null;
        public LevelData LevelData
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
        public bool RankUps { get; private set; } = false;
        public bool PlaySoundOnce { get; private set; } = false;
        public bool Resets { get; private set; } = false;
        public int Attempts { get; private set; } = 5;
        public int Ranks { get; private set; } = 0;
        public int HP { get; private set; } = 100;
        public bool PlayGame { get; private set; } = false;
        public int Score { get; private set; }

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
                //Landed on Perfect
                if (GaugePoint <= StartingPointOfPerfectChanceRange + LevelData.perfectChanceRange
                    && GaugePoint > StartingPointOfPerfectChanceRange)
                {
                    ScoreType = 2;
                    RankUps = true;
                    Ranks++;
                    Score += 10;
                }
                //Landed on Good
                else if (GaugePoint <= StartingPointOfGoodChanceRange + LevelData.goodChanceRange
                    && GaugePoint > StartingPointOfGoodChanceRange)
                {
                    ScoreType = 1;
                    RankUps = true;
                    Ranks++;
                    Score += 5;
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
                        HP -= LevelData.penalty;
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
                        HP -= LevelData.penalty;
                    else
                        HP = 0;
                }
            }
        }

        private void FindingStartingPoint()
        {
            StartingPointOfPerfectChanceRange = LevelData.position - LevelData.perfectChanceRange;
            StartingPointOfGoodChanceRange = LevelData.position - LevelData.goodChanceRange;

            if (StartingPointOfPerfectChanceRange + LevelData.perfectChanceRange > 100)
                perfectExceedingOffset = (StartingPointOfPerfectChanceRange + (LevelData.perfectChanceRange * 2)) - 100;
            if (StartingPointOfGoodChanceRange + LevelData.goodChanceRange > 100)
                goodExceedingOffset = (StartingPointOfGoodChanceRange + (LevelData.goodChanceRange * 2)) - 100;

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


            print("Before: " + LevelData.position);
            LevelData.position = Random.Range(0, 101);
            print("After Random: " + LevelData.position);
            Resets = true;
            movingState = 1;
            time = LevelData.time;
            _decelerationRate = 0;
            GaugePoint = 0; 
            startedDecelerating = false;
            RankUps = false;
            Attempts--;
            FindingStartingPoint();


            yield return new WaitForFixedUpdate();
            Resets = false;
            _speedMultiplier = LevelData.speed;
            PlaySoundOnce = false;

            if (!PlayGame)
            {
                Score = 0;
            }
        }

        #region Functions for Buttons

        public void Play()
        {
            Score = 0;
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
