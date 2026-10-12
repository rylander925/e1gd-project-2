using UnityEngine;

public class GroundLineRenderer : MonoBehaviour
{
    [SerializeField] PlatformerController player;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] int numSegments = 2;

    float maxLineLength;
    
    Vector3[] segments;
    void Awake()
    {
        player = GetComponentInParent<PlatformerController>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = numSegments;
        segments = new Vector3[numSegments];
        maxLineLength = player.maxGroundingDistance;
    }

    void OnEnable()
    {
        lineRenderer.enabled = true;
    }

    void OnDisable()
    {
        lineRenderer.enabled = false;
    }

    //Calculate & draw line
    void Update()
    {
        Vector3 segmentDirection = Vector3.down;
        float lineLength = player.IsOnGround(player.maxGroundingDistance).distance;
        Vector3 startPos = player.transform.position + player.GetRayOffset() * 1.5f * segmentDirection;

        for (int i = 0; i < numSegments; i++)
        {
            segments[i] = startPos + (float)i / (numSegments - 1) * segmentDirection * lineLength;
        }
        lineRenderer.SetPositions(segments);
    }
}
