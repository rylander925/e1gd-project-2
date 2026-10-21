using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class VolumeSetter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] CinemachineVolumeSettings volume;
    [SerializeField] Color startingColorFilter;
    [SerializeField] float startingSaturation;
    ColorAdjustments colorAdjustments;
    void Start()
    {
        if (volume.Profile.TryGet<ColorAdjustments>(out colorAdjustments))
        {
            colorAdjustments.colorFilter.value = startingColorFilter;
            colorAdjustments.saturation.value = startingSaturation;
        }
    }
}
