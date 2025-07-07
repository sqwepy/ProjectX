using UnityEngine;
using Mirror;

public class BallSpawner : MonoBehaviour
{
    public GameObject ObjectPrefab;
    public Vector3 spawnPosition = new Vector3(0, 2, 0);
    public int Objectcount = 10;

    void Start()
    {
        if (NetworkServer.active)
        {
            for (int i = 0; i < Objectcount; i++)
            {
                Vector3 spawnPosition_i = new Vector3(0, i, 0);
                Vector3 spawnPosition_new = spawnPosition + spawnPosition_i;
                SpawnObject(spawnPosition_new);
            }
        }
    }

    public void SpawnObject(Vector3 spawnPosition)
    {
        GameObject Object = Instantiate(ObjectPrefab, spawnPosition, Quaternion.identity);
        NetworkServer.Spawn(Object); // 👈 Spawns the ball over the network
    }
}
