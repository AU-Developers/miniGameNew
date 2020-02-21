using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minigame
{
    [CreateAssetMenu(menuName = "Level Data", fileName = "Level Difficulty.asset")]
    public class LevelData : ScriptableObject
    {
        [Range(0, 400)] public float speed;
        [Range(0, 2)] public float time;
        public float point;
        public int penalty;
        [Range(1, 100)] public float goodChanceRange = 30, perfectChanceRange = 10;
    }
}
