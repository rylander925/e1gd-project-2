using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TargetBreakable : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Heart score;
    Light2D light;
    Color defaultColor = Color.softBlue;
    Color brokenColor = Color.goldenRod;

    void Start()
    {
        score = FindAnyObjectByType<Heart>();
        light = gameObject.GetComponent<Light2D>();
        light.color = defaultColor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnJointBreak2D(Joint2D joint)
    {
        Debug.Log("Broke");
        score.AddJoy("TargetBreakable");
        light.color = brokenColor;
    }
}
