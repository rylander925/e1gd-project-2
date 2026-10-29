using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] public PlatformerController player;
    [SerializeField] public TextMeshProUGUI pointsText;
    [SerializeField] public int totalPoints;

    [SerializeField] AudioMixer mixer;
    [SerializeField] public Slider musicSlider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        pointsText.text = player.points + "/" + totalPoints;
    }

    public void OnMusicChangeVolume()
    {
        float volumeInDb = 20f * Mathf.Log10(Mathf.Max(0.00001f, musicSlider.value));
        mixer.SetFloat("MusicVolume", volumeInDb);
    }
}
