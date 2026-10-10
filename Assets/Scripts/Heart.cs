using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

//dum
public class Heart : MonoBehaviour
{
    List<SpriteRenderer> heartPixels = new List<SpriteRenderer>();
    [SerializeField] Timer timer;
    Rigidbody2D rb;
    [SerializeField] float initialOpacity = 0;
    [SerializeField] Light2D light;
    [SerializeField] int numTargets = 3; 

    [SerializeField] Color initialColor = Color.violetRed;
    [SerializeField] Color winColor = Color.goldenRod;
    [SerializeField] float InitialInnerRadius = 3;
    [SerializeField] float InitialOuterRadius = 10;
    [SerializeField] float WinInnerRadius = 10;
    [SerializeField] float WinOuterRadius = 20;
    bool hasWon = false;
    [SerializeField] Vector2 winDestination = Vector2.zero;
    float joy = 0;  //Normalized value representing stage progress
    public float elapsedTime = 0;
    float winTime = 0;

    void Awake()
    {        
        rb = gameObject.GetComponent<Rigidbody2D>();
        gameObject.GetComponentsInChildren<SpriteRenderer>(heartPixels);
        light = gameObject.GetComponent<Light2D>();
        rb.simulated = false;
        heartPixels.ForEach(sprite => sprite.color = initialColor);
        SetOpacity(0);
        light.intensity = 0;
        light.pointLightInnerRadius = InitialInnerRadius;
        light.pointLightOuterRadius = InitialOuterRadius;
        elapsedTime = 0;
    }

    void Start()
    {
        timer.enabled = false;
    }

    void FixedUpdate()
    {
        elapsedTime += Time.fixedDeltaTime;
    } 

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (joy >= 1 && !hasWon)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.linearVelocity = Vector2.zero;
            light.color = winColor;
            light.pointLightInnerRadius = WinInnerRadius;
            light.pointLightOuterRadius = WinOuterRadius;
            heartPixels.ForEach(sprite => sprite.color = winColor);
            winTime = elapsedTime;
            timer.enabled = true;
            timer.SetTimeText(winTime);
            hasWon = true;
        }
    }

    public float AddJoy(string tag = "")
    {
        joy += 1f/numTargets;
        SetOpacity(joy);
        if (joy >= 1)
        {
            light.intensity = 1;
            rb.simulated = true;
        }
        return joy;
    }

    void SetOpacity(float opacity)
    {
        foreach(SpriteRenderer sprite in heartPixels)
        {
            Color newColor = sprite.color;
            newColor.a = opacity;
            sprite.color = newColor;
        }
    }
}
