using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HeartCollectible : MonoBehaviour
{
    [SerializeField] Color color;
    [SerializeField] CinemachineVolumeSettings volume;

    ColorAdjustments colorAdjustments;
    SpriteRenderer sprite;

    void Awake()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
        sprite.color = color;

        if (volume == null) volume = FindAnyObjectByType<CinemachineVolumeSettings>();
        volume.Profile.TryGet<ColorAdjustments>(out colorAdjustments);


    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        PlatformerController player = collision.gameObject.GetComponent<PlatformerController>();
        if (player != null)
        {
            player.coinsCollected++;
            ApplyAdditiveFilter(color);
            Destroy(gameObject);   
        }
    }

    void ApplyAdditiveFilter(Color c)
    {
        if (colorAdjustments != null)
        { 
            Color oldColor = colorAdjustments.colorFilter.value;
            oldColor = new Color(Mathf.Min(c.r * c.a + oldColor.r, 1), Mathf.Min(c.g * c.a + oldColor.g, 1), Mathf.Min(c.b * c.a + oldColor.b , 1), 1);
            colorAdjustments.saturation.value = (Mathf.Sqrt(oldColor.r * oldColor.r + oldColor.g * oldColor.g + oldColor.b * oldColor.b) - 3)/3 * 100;
            Debug.Log(oldColor + "\nSaturation: " + colorAdjustments.saturation.value);
            colorAdjustments.colorFilter.value = oldColor;
        }
    }

}
