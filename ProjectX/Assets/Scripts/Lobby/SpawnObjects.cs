using UnityEngine;
using Mirror;

public class BallSpawner : MonoBehaviour
{
    public GameObject ballPrefab;
    public Vector3 spawnPosition = new Vector3(0, 2, 0);

    void Start()
    {
        if (NetworkServer.active)
        {
            SpawnBall();
        }
    }

    public void SpawnBall()
    {
        GameObject ball = Instantiate(ballPrefab, spawnPosition, Quaternion.identity);
        NetworkServer.Spawn(ball); // 👈 Spawns the ball over the network
    }
}
