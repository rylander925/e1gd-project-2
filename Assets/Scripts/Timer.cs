using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshPro textbox;
    Rigidbody2D rb;
    [SerializeField] string text = "";
    void Awake()
    {
        textbox = gameObject.GetComponent<TextMeshPro>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.simulated = false;
        textbox.text = text;
    }

    void OnEnable()
    {
        textbox.text = text;
        rb.simulated = true;
    }

    void OnDisable()
    {
        rb.simulated = false;
    }


    public void SetTimeText(float timeSeconds)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds((double)timeSeconds); //! Must be positive
        text = $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}.{timeSpan.Milliseconds / 100}";
        textbox.text = text;
        Debug.Log(timeSeconds + ", " + text);
    }
}
