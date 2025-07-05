using UnityEngine;
using Mirror;

public class BallSpawner : MonoBehaviour
{
    public GameObject ballPrefab;
    public Vector3 spawnPosition = new Vector3(0, 2, 0);
    public int ballcount = 10;

    void Start()
    {
        if (NetworkServer.active)
        {
            for (int i = 0; i < ballcount; i++)
            {
                Vector3 spawnPosition_i = new Vector3(0, i, 0);
                Vector3 spawnPosition_new = spawnPosition + spawnPosition_i;
                SpawnBall(spawnPosition_new);
            }
        }
    }

    public void SpawnBall(Vector3 spawnPosition)
    {
        GameObject ball = Instantiate(ballPrefab, spawnPosition, Quaternion.identity);
        NetworkServer.Spawn(ball); // 👈 Spawns the ball over the network
    }
}
