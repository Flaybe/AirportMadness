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
    public void ClearLine()
    {
        lr.positionCount = 0;
    }
}
