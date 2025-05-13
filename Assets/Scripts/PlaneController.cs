
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms.GameCenter;

public class PlaneController : MonoBehaviour
{

    private Vector2 moveDirection;

    public Vector2 targetDirection;
    public PlaneType planeType;
    public float speed;
    public float turnSpeed;
    public string callSign; 

    public LineDrawer lineDrawer;
    public Vector2 targetPosition;

    private bool waitingForWaypoint = false;
    public void SetPlaneType(PlaneType type)
    {
        planeType = type;
        if (planeType == null)
        {
            Debug.LogError("PlaneType is null!");
            return;
        }
        callSign = GenerateRandomPlaneID(planeType);
        speed = planeType.speed;
        turnSpeed = planeType.turnSpeed;

    }
    public void SetDirection(Vector2 dir)
    {
        targetDirection = dir.normalized;
        moveDirection = dir.normalized;

        // Rotate to face direction
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0,0,angle - 90f);
    }

    void Update()
    {
        if (waitingForWaypoint)
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f; // ensure 2D
            lineDrawer.DrawLine(transform.position, mouseWorld);
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject()) return;

                Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                clickPos.z = 0; // make sure it's 2D
                targetPosition = clickPos;

                targetDirection = (Vector2)(clickPos - transform.position).normalized;
                waitingForWaypoint = false;
                // Reset cursor to default
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }
        }
        UpdateTargetDirection(targetPosition);
        // Rotate moveDirection toward targetDirection
        if (targetDirection != Vector2.zero)
        {
            float maxRadians = turnSpeed * Mathf.Deg2Rad * Time.deltaTime;
            moveDirection = RotateTowards(moveDirection, targetDirection, maxRadians);

        }
        if (targetPosition != Vector2.zero)
        {
            // Check if the plane is close to the target position
            if (Vector2.Distance(transform.position, targetPosition) < 0.3f)
            {
                lineDrawer.ClearLine();
                targetPosition = Vector2.zero;
            }else{
                lineDrawer.DrawLine(transform.position, targetPosition);
            }
        }
        // Move forward in current moveDirection
        transform.position += (Vector3)(moveDirection * speed * Time.deltaTime);

        // Rotate the sprite to match moveDirection
        if (moveDirection != Vector2.zero)
        {
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f); // assumes sprite faces up
        }
    } 

    void MovetoDirection(Vector2 direction)
    {
        // Move the plane in the specified direction
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    void UpdateTargetDirection(Vector2 target)
    {
        if (target == Vector2.zero)
        {
            return;
        }
        // Update the target direction
        targetDirection = (target - (Vector2)transform.position).normalized;
    }
    
    void LateUpdate()
    {
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
        if (viewPos.x < -0.1f || viewPos.x > 1.1f || viewPos.y < -0.1f || viewPos.y > 1.1f)
        {
            Destroy(gameObject);
        }
    }

    private string GenerateRandomPlaneID(PlaneType planeType)
    {
       if (planeType == null)
       {
           Debug.LogError("PlaneType is null. Cannot generate ID.");
           return "ERR-000";
       }
    
       string name = planeType.planeName.PadRight(3, '-')[..3].ToUpper() + Random.Range(100, 999).ToString();
       return name;
    } 
    private Vector2 RotateTowards(Vector2 current, Vector2 target, float maxRadians)
    {
        float angle = Vector2.SignedAngle(current, target);

        // Clamp angle
        float clampedAngle = Mathf.Clamp(angle, -maxRadians * Mathf.Rad2Deg, maxRadians * Mathf.Rad2Deg);

        // Apply rotation
        return Quaternion.Euler(0, 0, clampedAngle) * current;
    }



    public void SpeedUp()
    {
        
        speed += 1f;
        if (speed > planeType.maxSpeed)
        {
            speed = planeType.maxSpeed;
        }
    }
    public void SlowDown()
    {
        speed -= 1f;
        if (speed < planeType.minSpeed)
        {
            speed = planeType.minSpeed;
        }
    }
    
    public void AddWaypoint()
    {
        //listens for next click and will plane will change direction to that point
        waitingForWaypoint = true;
        // change cursor to crosshair
        Vector2 hotspot = new Vector2(15, 15);
        Cursor.SetCursor(planeType.cursorSprite, hotspot, CursorMode.Auto);
        Cursor.visible = true;
    }
}
