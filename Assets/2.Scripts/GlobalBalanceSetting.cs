using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public static partial class Database
    {
        public static GlobalBalanceSetting GlobalBalanceSetting;

        public static void AssignGlobalBalanceSetting(GlobalBalanceSetting data)
        {
            GlobalBalanceSetting = data;
        }
    }

    public enum UnitType
    {
        None = -1,
        Player,
        Tower,
        Zombie
    }
}

[CreateAssetMenu(fileName = "GlobalBalanceSetting", menuName = "Tower Defense Survival/Global Balance Setting")]
public class GlobalBalanceSetting : ScriptableObject
{
    [Header("Player Settings")]
    public float attackSpeed;
    public float attackRange;
    public float projectileSpeed;
    
    [Header("Zombie Settings")]
    public float zombieSpeed;
    public float zombieClimbingForce;
    public float zombiePushForce;
    public float zombieClimbingCooldown;
    public float zombieCollisionTolerance;
    public int zombieSpawnCount;
    
    [Space(10), Header("Common Stat Settings")]
    public List<StatData> statDatas;
    
    [System.Serializable]
    public class StatData
    {
        public Data.UnitType unitType;
        public int health;
        public int damage;
    }
}