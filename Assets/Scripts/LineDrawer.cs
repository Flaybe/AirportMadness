using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    private LineRenderer lr;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 0;
    }

    public void DrawLine(Vector3 start, Vector3 end)
    {
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }

    public void DrawPath(List<Vector3> path)
    {
        lr.positionCount = path.Count;
        lr.SetPositions(path.ToArray());
    }
    public void ClearLine()
    {
        lr.positionCount = 0;
    }
}
