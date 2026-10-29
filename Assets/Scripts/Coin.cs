using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Coin : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        PlatformerController player = collision.gameObject.GetComponent<PlatformerController>();
        if (player != null)
        {
            player.points++;
            player.coinsCollected.Add(gameObject);
            Destroy(gameObject);   
        }
    }

}
