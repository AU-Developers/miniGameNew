using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinigameV2
{
    public class GameplayControllerV2 : MonoBehaviour
    {
        /// <summary>
        /// The input required to play the minigame.
        /// </summary>
        [SerializeField] private KeyCode input = KeyCode.Space;
        /// <summary>
        /// The list of all levels' data.
        /// </summary>
        [SerializeField] private List<LevelDataV2> _levelData = new List<LevelDataV2>();
        /// <summary>
        /// returns the count of all levels
        /// </summary>
        public int levelSize
        {
            get
            {
                return _levelData.Count;
            }
        }
        /// <summary>
        /// Returns the current level data.
        /// </summary>
        public LevelDataV2 LevelData
        {
            get
            {
                return _levelData[Level];
            }
        }

        /// <summary>
        /// The rate of speed of the gauge bar.
        /// </summary>
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
        public bool JudgingScore { get; private set; } = false;
        /// <summary>
        /// The level of difficulty in the minigame.
        /// </summary>
        public int Level { get; private set; } = 0;

        /// <summary>
        /// Player's tolerance for the level's penalty.
        /// </summary>
        public int HP { get; private set; } = 0;
        /// <summary>
        /// The player's attempts to play in the minigame.
        /// </summary>
        public int Attempts { get; private set; } = 5;
        /// <summary>
        /// <para>Game States:</para>
        /// <br>0 - Main menu/Not started playing</br>
        /// <br>1 - Ingame/Playing</br>
        /// <br>2 - Score Screen/Done playing</br>
        /// </summary>
        public int GameState { get; private set; } = 0;
        /// <summary>
        /// Used for moving in between game states, requires the game state as the index, otherwise ignored and reset back to false.
        /// </summary>
        public bool[] GameMoveRequest { get; private set; } = new bool[] { false, false, false };

        /// <summary>
        /// <para>penalty States:</para>
        /// <br>0 - 1st rotation of moving bar,1st warning,adding alpha value</br>
        /// <br>1 - 2nd rotation of moving bar,2nd warning,critical,adding alpha value</br>
        /// <br>2 - 3rd rotation of moving bar,deducting hp from the player,adding alpha value</br>
        /// </summary>
        public int penaltyState { get; set; } = 0;
        /// <summary>
        /// for alpha animation
        /// </summary>
        public float alphaValue = 0;
        ViewV2 viewAcess;
        public bool startAlphaAnimation;

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
            viewAcess = (ViewV2)FindObjectOfType(typeof(ViewV2));
            _instance = this;
            HardReset();
        }
        private void Start()
        {
     
        }
        private void FixedUpdate()
        {
            MinigameUpdate();
        }
        private void Update()
        {
            if (Input.GetKeyDown(input))
            {
                startedDecelerating = true;
            }
            if (startAlphaAnimation)
            {
                print(alphaValue);
                if(viewAcess.penaltyAlpha.color.a < LevelData.alphaPercent[penaltyState])
                    alphaValue += LevelData.alphaPercent[penaltyState]/100 * Time.deltaTime;
                else
                {
                    alphaValue -= LevelData.alphaDecreaseTime / 100 * Time.deltaTime;
                    startAlphaAnimation = false;
                }
            }
        }

        #endregion

        #region Minigame Functions
        private void HardReset()
        {
            Score = 0;
            Attempts = 5;
            HP = 100;
            Level = 0;
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
            JudgingScore = false;
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
                    penaltyState++;
                    startAlphaAnimation = true;
                    if (penaltyState == 3)
                    {
                        penaltyState = 0;
                        if (!startedDecelerating)
                            HP -= LevelData.penalty;
                    }
                    GaugePoint = -100;
                    moveState = 1;
                   
                }
            }
            else // Judgement of gauge point placement
            {
                // Checks score
                if (!JudgingScore)
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
                        Level++;
                    }
                    // Otherwise, checks if inside good range
                    else if (GaugePoint >= goodStartPoint && GaugePoint <= goodEndPoint)
                    {
                        ScoreType = 1;
                        Score += 5;
                        print("Good!");
                        Level++;
                    }
                    // If it's not inside either good or perfect range, then it is judged as bad/missed.
                    else
                    {
                        ScoreType = 0;
                        print("Bad!");
                    }
                    JudgingScore = true;
                    Attempts--;
                    //Invoke("Initialize", Time.fixedDeltaTime / Time.timeScale);
                }
                else
                {
                    Initialize();
                }
            }
        }
        /// <summary>
        /// The minigame's logic.
        /// </summary>
        private void MinigameUpdate()
        {
            // If attempted to request to move from another game state, but the game state is not the right one, reset the move request.
            for (int counter = 0; counter < GameMoveRequest.Length; counter++)
            {
                if (GameState != counter)
                {
                    GameMoveRequest[counter] = false;
                }
            }
            // If move request is valid in the game state, then move the game state to the next one.
            if (GameMoveRequest[GameState])
            {
                GameState++;
                if (GameState >= GameMoveRequest.Length)
                {
                    GameState = 0;
                }
            }

            // All the logic for the main menu.
            if (GameState == 0)
            {
                if (Score != 0)
                {
                    HardReset();
                }
            }
            // All the logic for ingame.
            else if (GameState == 1)
            {
                if (Attempts > 0 && HP > 0)
                {
                    GaugeMechanics();
                }
                else
                {
                    GameMoveRequest[1] = true;
                }
            }
            // All the logic for score screen
            else if (GameState == 2)
            {
                HardReset();
            }
        }
        #endregion

        #region Button Functions
        /// <summary>
        /// Used to start the game from the main menu
        /// </summary>
        public void StartGame()
        {
            GameMoveRequest[0] = true;
        }
        /// <summary>
        /// Used to cancel the game before the chicken dies (before HP gets to 0)
        /// </summary>
        public void CancelGame()
        {
            GameMoveRequest[1] = true;
        }
        /// <summary>
        /// Used to end the minigame (currently only transitions back to main menu)
        /// </summary>
        public void EndGame()
        {
            GameMoveRequest[2] = true;
        }
        #endregion
    }
}
