using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlaneUI : MonoBehaviour
{
    public static PlaneUI Instance;

    public GameObject panel;
    public TMP_Text planeNameText;
    private GameObject currentPlane;

    public Button waypointButton;
    public Button speedUpButton;
    public Button slowDownButton;

    private Camera cam;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
        cam = Camera.main;
    }
    void Update()
    {
        if (currentPlane == null) return;
        if(Input.GetKeyDown(KeyCode.W))
        {
            if (waypointButton != null)
            {
                waypointButton.onClick.Invoke();
            }
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (speedUpButton != null)
            {
                speedUpButton.onClick.Invoke();
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (slowDownButton != null)
            {
                slowDownButton.onClick.Invoke();
            }
        }
    } 

    public void ShowFor(GameObject plane)
    {
     
        currentPlane = plane;
        panel.SetActive(true);
        PlaneController controller= plane.GetComponent<PlaneController>();
        planeNameText.text = controller.callSign;
    }

    public void Hide()
    {
        panel.SetActive(false);
        currentPlane = null;
    }

    void LateUpdate()
    {
        if (currentPlane.IsDestroyed()){
            Hide();
            return;
        }
    }

    /*
        void LateUpdate()
        {
            if (currentPlane == null) return;

            // Convert world position to screen position
            Vector3 screenPos = cam.WorldToScreenPoint(currentPlane.transform.position + new Vector3(0, 0.5f, 0));
            //  get the radius of the collider for the plane
            float radius = currentPlane.GetComponent<CircleCollider2D>().radius;
            // offset the position of the panel by the radius of the collider
            panel.transform.position = screenPos + new Vector3(radius * cam.aspect * Screen.width / cam.orthographicSize, radius * Screen.height / cam.orthographicSize, 0);

        }
    */
    public bool IsShowing(GameObject plane)
    {
        return panel.activeSelf && currentPlane == plane;
    }

    public void SpeedUp()
    {
        if (currentPlane == null) return;
        PlaneController controller = currentPlane.GetComponent<PlaneController>();
        if (controller != null)
        {
            controller.SpeedUp();
        }
    }
    public void SlowDown()
    {
        if (currentPlane == null) return;
        PlaneController controller = currentPlane.GetComponent<PlaneController>();
        if (controller != null)
        {
            controller.SlowDown();
        }
    }

    public void AddWaypoint()
    {
        if (currentPlane == null) return;
        PlaneController controller = currentPlane.GetComponent<PlaneController>();
        if (controller != null)
        {
            controller.AddWaypoint();
            Debug.Log("Waypoint added for " + currentPlane.name);
        }
    }
}
