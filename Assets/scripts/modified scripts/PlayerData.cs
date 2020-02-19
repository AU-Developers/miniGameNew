using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minigame
{
    [CreateAssetMenu(menuName = "Player Data", fileName = "New Player Data.asset")]
    public class PlayerData : ScriptableObject
    {
        [Range(0, 100)] public float winChanceRange = 20;
        [Range(1, 100)] public float goodChanceRange = 30, perfectChanceRange = 10;
        [SerializeField] public float stoppingPoint;
    }
}
