using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlaneSelect : MonoBehaviour
{
    void OnMouseDown()
    {
        
        if(EventSystem.current.IsPointerOverGameObject()) return;
        if(PlaneUI.Instance.IsShowing(gameObject)){
            PlaneUI.Instance.Hide();
            return;
        }
        Debug.Log("Plane Clicked " + name);
        PlaneUI.Instance.ShowFor(gameObject);
    }
}
