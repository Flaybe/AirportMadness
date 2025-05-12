using UnityEngine;

[CreateAssetMenu(fileName = "NewPlaneType", menuName = "Plane/Plane Type")]
public class PlaneType : ScriptableObject
{
    public string planeName;
    public string callSign;
    public float speed;

    public float minSpeed;
    public float maxSpeed;
    public float turnSpeed;

    public Sprite planeSprite;

#if UNITY_EDITOR
    private void OnEnable()
    {
        if (string.IsNullOrEmpty(planeName))
        {
            planeName = "DefaultPlane";
            callSign = "DPT";
            speed = 2.0f;
            turnSpeed = 1.0f;

            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
#endif
}
