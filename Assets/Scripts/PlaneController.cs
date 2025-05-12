
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlaneController : MonoBehaviour
{

    private Vector2 moveDirection;

    public Vector2 targetDirection;
    public PlaneType planeType;
    public float speed;
    public float turnSpeed;
    public string callSign; 

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
        if (waitingForWaypoint && Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clickPos.z = 0; // make sure it's 2D

            targetDirection = (Vector2)(clickPos - transform.position).normalized;
            waitingForWaypoint = false;
        }
        // Rotate moveDirection toward targetDirection
        if (targetDirection != Vector2.zero)
        {
            float maxRadians = turnSpeed * Mathf.Deg2Rad * Time.deltaTime;
            moveDirection = RotateTowards(moveDirection, targetDirection, maxRadians);

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
    }
}
