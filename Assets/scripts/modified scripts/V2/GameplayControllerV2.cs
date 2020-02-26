using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinigameV2
{
    public class GameplayControllerV2 : MonoBehaviour
    {
        [SerializeField] private KeyCode input = KeyCode.Space;
        [SerializeField] private LevelDataV2 _levelData = null;
        public LevelDataV2 LevelData
        {
            get
            {
                return _levelData;
            }
        }


        [Range(.01f, 400)] [SerializeField] private float speedMultiplier = 160;

        /// <summary>
        /// <para>Moving States:</para>
        /// <br>-1 - Moving Left</br>
        /// <br>0 - Not Moving</br>
        /// <br>1 - Moving Right</br>
        /// </summary>
        private int moveState = 1;
        /// <summary>
        /// The point of the moving bar.
        /// </summary>
        public float GaugePoint = -100;

        /// <summary>
        /// The type of score the player got when the gauge is judged.
        /// <para>Score Types:</para>
        /// <br>0 - Bad/Missed</br>
        /// <br>1 - Good</br>
        /// <br>2 - Perfect</br>
        /// </summary>
        public int ScoreType { get; private set; } = -1;
        /// <summary>
        /// The player's score in the minigame.
        /// </summary>
        public int Score { get; private set; } = 0;
        private bool checkedScore = false;

        public int Attempts { get; private set; } = 5;


        private float decelerationRate = 0;
        [SerializeField] [Range(0.01f, 2)] private float timeToDecelerate;
        private bool startedDecelerating = false;

        
        private static GameplayControllerV2 _instance;
        /// <summary>
        /// Can be used to refer to this monobehaviour script anywhere within the MinigameV2 namespace.
        /// </summary>
        public static GameplayControllerV2 Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = (GameplayControllerV2)FindObjectOfType(typeof(GameplayControllerV2));
                }
                return _instance;
            }
        }



        #region Monobehavior Methods
        private void Awake()
        {
            _instance = this;
            Initialize();
        }
        private void Start()
        {

        }
        private void FixedUpdate()
        {
            if (Attempts > 0)
                GaugeMechanics();
            else
            {

            }
        }
        private void Update()
        {
            if (Input.GetKeyDown(input))
            {
                startedDecelerating = true;
            }
        }

        #endregion

        private void HardReset()
        {
            Score = 0;
            Initialize();
        }
        /// <summary>
        /// Reset values back to original (except the score), and adjust to current level data.
        /// </summary>
        private void Initialize()
        {
            timeToDecelerate = LevelData.time;
            speedMultiplier = LevelData.speed;
            startedDecelerating = false;
            checkedScore = false;
            GaugePoint = -100;
            moveState = 1;
            ScoreType = -1;
            decelerationRate = speedMultiplier / timeToDecelerate;

            float perfectStartPoint = -LevelData.perfectChanceRange + LevelData.perfectOffset;
            float perfectEndPoint = LevelData.perfectChanceRange + LevelData.perfectOffset;
            float goodStartPoint = -LevelData.goodChanceRange + LevelData.goodOffset;
            float goodEndPoint = LevelData.goodChanceRange + LevelData.goodOffset;
            float overOffset = 0;

            // Check if starting point of perfect range exceeds -100 with its offset.
            if (perfectStartPoint < -100)
            {
                overOffset = -100 - perfectStartPoint;
            }
            // Check if end point of perfect range exceeds 100 with its offset.
            else if (perfectEndPoint > 100)
            {
                overOffset = 100 - perfectEndPoint;
            }
            // If it exceeded either end of the spectrum, adjust the points accordingly.
            if (overOffset != 0)
            {
                LevelData.perfectOffset += overOffset;
                overOffset = 0;
            }

            // Check if starting point of good range exceeds -100 with its offset.
            if (goodStartPoint < -100)
            {
                overOffset = -100 - goodStartPoint;
            }
            // Check if end point of good range exceeds 100 with its offset.
            else if (goodEndPoint > 100)
            {
                overOffset = 100 - goodEndPoint;
            }
            // If it exceeded either end of the spectrum, adjust the points accordingly.
            if (overOffset != 0)
            {
                LevelData.goodOffset += overOffset;
            }
        }
        /// <summary>
        /// Gauge point functions, including checking where it landed and moving it.
        /// </summary>
        private void GaugeMechanics()
        {
            // Happens when the player presses the input key. By default, the input key is spacebar.
            if (startedDecelerating)
            {
                speedMultiplier -= decelerationRate * Time.fixedDeltaTime; // Kills the speed overtime.
                if (speedMultiplier <= 0)
                {
                    speedMultiplier = 0;
                    startedDecelerating = false;
                }
            }

            // Move the gauge point while it's still able to.
            if (speedMultiplier > 0)
            {
                GaugePoint += speedMultiplier * Time.fixedDeltaTime * moveState * Time.timeScale;
                if (GaugePoint > 100)
                {
                    GaugePoint = 100;
                    moveState = -1;
                }
                else if (GaugePoint < -100)
                {
                    GaugePoint = -100;
                    moveState = 1;
                }
            }
            else // Judgement of gauge point placement
            {
                // Checks score
                if (!checkedScore)
                {
                    float perfectStartPoint = -LevelData.perfectChanceRange + LevelData.perfectOffset;
                    float perfectEndPoint = LevelData.perfectChanceRange + LevelData.perfectOffset;
                    float goodStartPoint = -LevelData.goodChanceRange + LevelData.goodOffset;
                    float goodEndPoint = LevelData.goodChanceRange + LevelData.goodOffset;

                    // Checks if inside perfect range
                    if (GaugePoint >= perfectStartPoint && GaugePoint <= perfectEndPoint)
                    {
                        ScoreType = 2;
                        Score += 10;
                        print("Perfect!");
                    }
                    // Otherwise, checks if inside good range
                    else if (GaugePoint >= goodStartPoint && GaugePoint <= goodEndPoint)
                    {
                        ScoreType = 1;
                        Score += 5;
                        print("Good!");
                    }
                    // If it's not inside either good or perfect range, then it is judged as bad/missed.
                    else
                    {
                        ScoreType = 0;
                        print("Bad!");
                    }
                    checkedScore = true;
                    Attempts--;
                    Invoke("Initialize", Time.fixedDeltaTime / Time.timeScale);
                }
            }
        }
    }
}
