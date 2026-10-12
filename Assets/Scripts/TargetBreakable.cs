using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TargetBreakable : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Heart score;
    Light2D light;
    Color defaultColor = Color.softBlue;
    Color brokenColor = Color.goldenRod;
    SpriteRenderer sprite;
    float defaultOpacity = 0.6f;
    float brokenOpacity = 0.2f;

    void Awake()
    {
        score = FindAnyObjectByType<Heart>();
        light = gameObject.GetComponent<Light2D>();
        sprite = gameObject.GetComponent<SpriteRenderer>();
        defaultColor.a = defaultOpacity;
    }

    void Start()
    {
        light.color = defaultColor;
        sprite.color = defaultColor;
    }

    void OnJointBreak2D(Joint2D joint)
    {
        Debug.Log("Broke");
        score.AddJoy("TargetBreakable");
        light.color = brokenColor;
        Color temp = defaultColor;
        temp.a = brokenOpacity;
        sprite.color = temp;
    }
}
