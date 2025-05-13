
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms.GameCenter;
using UnityEngine.UIElements;
using UnityEngine.WSA;

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
    public BoxCollider2D RunwayCollider;


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
        MouseListeners();
        // Rotate moveDirection toward targetDirection
        if (targetDirection != Vector2.zero)
        {

            UpdateTargetDirection(targetPosition);
            RotateSprite();
            float radPerTurn= turnSpeed * Mathf.Deg2Rad * Time.deltaTime;
            Vector2 currentPosition = transform.position;
            moveDirection = RotateTowards(moveDirection, targetDirection, radPerTurn, ref currentPosition, targetPosition, Time.deltaTime);
            transform.position = currentPosition;

        }
        if (targetPosition != Vector2.zero)
        {
            // Check if the plane is close to the target position
            if (Vector2.Distance(transform.position, targetPosition) < 0.3f)
            {
                lineDrawer.ClearLine();
                targetPosition = Vector2.zero;
            }else{
                var path = PredictPath(20f);
                lineDrawer.DrawPath(path);
            }
        }
        // Move forward in current moveDirection
        // Rotate the sprite to match moveDirection
    } 


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Runway"))
        {
            // Plane is on the runway, stop it
            speed = 0f;
            targetDirection = Vector2.zero;
            targetPosition = Vector2.zero;
            lineDrawer.ClearLine();
        }
    }

    void MouseListeners()
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
                UnityEngine.Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }
        }
    }
    Vector2 MovetoDirection(Vector2 direction, float dt)
    {
        // Move the plane in the specified direction
        return direction * speed * dt;
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
    
    private Vector2 RotateTowards(Vector2 currentAngle, Vector2 targetAngle, float maxRadians, ref Vector2 planePos, Vector2 targetPos, float dt)
    {
        float angleToTarget = Vector2.SignedAngle(currentAngle, targetAngle);
        float timeToTurn = Mathf.Abs(angleToTarget) / turnSpeed;
        float dinstanceToNeededToTurn = speed * timeToTurn;

        float distanceToTarget = Vector2.Distance(planePos, targetPos);
        // Clamp angle
        float clampedAngle = Mathf.Clamp(angleToTarget, -maxRadians * Mathf.Rad2Deg, maxRadians * Mathf.Rad2Deg);
        Vector2 clampedDirection = Quaternion.Euler(0, 0, clampedAngle) * currentAngle;

        //If we can not reach the target angel before due to speed we keep going straight until we are far enough to be able to make the turn
        if (distanceToTarget < dinstanceToNeededToTurn)
        {
            planePos += MovetoDirection(currentAngle, dt);
            return currentAngle; 
        }
        planePos += MovetoDirection(clampedDirection, dt);
        // Apply rotation
        return clampedDirection; 
    }
    private void RotateSprite()
    {
        // Rotate the sprite to match moveDirection
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
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
        UnityEngine.Cursor.SetCursor(planeType.cursorSprite, hotspot, CursorMode.Auto);
        UnityEngine.Cursor.visible = true;
    }
    
    List<Vector3> PredictPath(float secondsAhead, float dt = 0.01f)
    {
        Vector2  dirCurrent = moveDirection;
        Vector2  dirTarget  = targetDirection;
        Vector2  planePos = transform.position;
        Vector2  targetPos = targetPosition;

        float    maxRadPerStep = turnSpeed * Mathf.Deg2Rad * dt;

        List<Vector3> path = new() { planePos };
        float time = 0f;
        while (true)
        {

            dirTarget = (targetPos - (Vector2)planePos).normalized;
            // samma sväng‑algoritm som i Update, fast på kopierade variabler
            dirCurrent = RotateTowards(dirCurrent, dirTarget, maxRadPerStep, ref planePos, targetPos, dt);
            if(Vector2.Distance(planePos, targetPos) < 0.3f){
                break;
            }
            if (time > secondsAhead) break;
            path.Add(planePos);
            time += dt;
        }
        return path;
}

}
