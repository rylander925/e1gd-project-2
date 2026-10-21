using UnityEngine;
using System.Collections;


public class RespawnPoint : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D collision)
    {
        PlatformerController player = collision.gameObject.GetComponent<PlatformerController>();
        if (player != null)
        {
            player.respawnPoint = this;
        }
    }
}
