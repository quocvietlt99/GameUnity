using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TrajectoryPreview : MonoBehaviour
{
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private int maxBounces = 2;
    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private float lineWidth = 0.08f;

    private LineRenderer lr;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        SetupLineRenderer();
    }

    private void SetupLineRenderer()
    {
        lr.useWorldSpace = true;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.positionCount = 0;
        lr.enabled = true;

        if (lr.material == null)
        {
            lr.material = new Material(Shader.Find("Sprites/Default"));
        }

        lr.startColor = Color.white;
        lr.endColor = Color.white;
        lr.sortingOrder = 50;
    }

    public void Draw(Vector2 origin, Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.001f)
            return;

        List<Vector3> points = new List<Vector3>();
        points.Add(origin);

        Vector2 currentPos = origin;
        Vector2 currentDir = direction.normalized;
        float remain = maxDistance;

        for (int i = 0; i <= maxBounces; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(currentPos, currentDir, remain, hitMask);

            if (hit.collider == null)
            {
                points.Add(currentPos + currentDir * remain);
                break;
            }

            points.Add(hit.point);

            if (hit.collider.CompareTag("Wall"))
            {
                remain -= hit.distance;
                currentDir = Vector2.Reflect(currentDir, hit.normal);
                currentPos = hit.point + currentDir * 0.02f;
            }
            else
            {
                break;
            }
        }

        lr.positionCount = points.Count;
        lr.SetPositions(points.ToArray());
    }

    public void Hide()
    {
        lr.positionCount = 0;
    }
}