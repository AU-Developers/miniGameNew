using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minigame
{
    [CreateAssetMenu(menuName = "Player Data", fileName = "New Player Data.asset")]
    public class PlayerData : ScriptableObject
    {
        [Range(1, 100)] public float goodChance = 30, perfectChance = 10;
        
    }
}
