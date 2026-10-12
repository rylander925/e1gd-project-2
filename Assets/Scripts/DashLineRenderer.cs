using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class DashLineRenderer : MonoBehaviour
{
    [SerializeField] PlatformerController player;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] int numSegments = 2;
    [SerializeField] float lineLengthFactor = 0.5f; //Line length = factor * dashForce
    
    float lineLength;
    Vector3[] segments;
    void Awake()
    {
        player = GetComponentInParent<PlatformerController>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = numSegments;
        segments = new Vector3[numSegments];
        lineLength = player.dashForce * lineLengthFactor;
    }

    void OnEnable()
    {
        lineRenderer.enabled = true;
    }

    void OnDisable()
    {
        lineRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector3 segmentDirection = ((Vector2)(mousePosition - player.transform.position)).normalized * (player.invertDash ? -1 : 1);
        Vector3 startPos = player.transform.position + player.GetRayOffset() * 1.5f * segmentDirection;

        for (int i = 0; i < numSegments; i++)
        {
            segments[i] = startPos + (float)i / (numSegments - 1) * segmentDirection * lineLength;
        }
        lineRenderer.SetPositions(segments);
    }
}
