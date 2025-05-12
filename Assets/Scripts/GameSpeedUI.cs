using UnityEngine;

public class GameSpeedUI : MonoBehaviour
{
   public void setPause()
   {
    Time.timeScale = 0f;
    Debug.Log("Paused");
   } 
   public void setNormal()
   {
    Time.timeScale = 1f;
    Debug.Log("Normal speed");
   }
   public void setFast()
   {
    Time.timeScale = 2f;
    Debug.Log("Fast speed");
   }
}
