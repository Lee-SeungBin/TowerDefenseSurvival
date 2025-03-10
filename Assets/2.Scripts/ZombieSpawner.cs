using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [SerializeField] private Zombie zombiePrefab;
    [SerializeField] private float spawnInterval;
    [SerializeField] private bool isSpawn;
    [SerializeField] private List<SpawnInfo> spawnInfos;
    
    private float m_ElapsedTime;

    private void Update()
    {
        if (!isSpawn)
        {
            return;
        }
        m_ElapsedTime += Time.deltaTime;

        if (m_ElapsedTime >= spawnInterval)
        {
            m_ElapsedTime = 0f;
            SpawnZombie();
        }
    }

    private void SpawnZombie()
    {
        var spawnInfo = spawnInfos[Random.Range(0, spawnInfos.Count)];
        var zombieInstance = Instantiate(zombiePrefab, spawnInfo.spawnPosition, Quaternion.identity);
        zombieInstance.SetZombie(spawnInfo);
    }
    
    [System.Serializable]
    public struct SpawnInfo
    {
        public Vector2 spawnPosition;
        public LayerMask zombieLayer;
        public int sortingOrder;
    }
}