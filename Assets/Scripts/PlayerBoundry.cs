using UnityEngine;

public class PlayerBoundry : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision entered");
        PlatformerController player = collision.gameObject.GetComponent<PlatformerController>();
        if (player != null)
        {
            player.Respawn();
        }
    }
}
