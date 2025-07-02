using UnityEngine;
using Unity.Netcode;

public class MoveCameraMulti : NetworkBehaviour {

    public Transform player;

    void Update() {
        if (!IsOwner) return;
        else
        {
            transform.position = player.transform.position;
        }
    }
}