using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinigameV2
{
    [CreateAssetMenu(menuName = "Level Data V2", fileName = "New Level Difficulty.asset")]
    public class LevelDataV2 : ScriptableObject
    {
        [Range(0, 400)] public float speed;
        [Range(0, 2)] public float time;
        [Range(-100, 100)] public float goodOffset, perfectOffset;
        public int penalty;
        [Range(1, 100)] public float goodChanceRange = 30, perfectChanceRange = 10;
    }
}