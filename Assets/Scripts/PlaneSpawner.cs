using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlaneSpawner : MonoBehaviour
{
    public GameObject planePrefab;
    public float spawnInterval = 5f;
    public float spawnDistance = 10f; // How far offscreen to spawn
    private float timer;

    public List<PlaneType> availablePlanes;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnPlane();
        }
    }

    void SpawnPlane()
    {
        Vector2 spawnPos;
        Vector2 direction;

        GetSpawnPositionAndDirection(out spawnPos, out direction);
        Debug.Log($"Spawning plane at {spawnPos} heading {direction}");

        GameObject plane = Instantiate(planePrefab, spawnPos, Quaternion.identity);
        PlaneController controller = plane.GetComponent<PlaneController>();
        if (plane == null)
        {
            Debug.LogError("Failed to instantiate plane — prefab may be null.");
            return;
        }

        if (controller == null)
        {
            Debug.LogError("Spawned plane has no PlaneController script attached!");
        }
            controller.SetDirection(direction);
            PlaneType planeType = availablePlanes[Random.Range(0, availablePlanes.Count)];
            controller.SetPlaneType(planeType);
            // Set the planes sprite to the planeType sprite
            SpriteRenderer spriteRenderer = plane.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = planeType.planeSprite;
            }
            else
            {
                Debug.LogError("Plane prefab does not have a SpriteRenderer component!");
            }
        }
    void GetSpawnPositionAndDirection(out Vector2 pos, out Vector2 dir)
    {
        Camera cam = Camera.main;
        float camHeight = 2f * cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;
        float buffer = 1f;
        int edge = Random.Range(0, 4); // 0=top, 1=right, 2=bottom, 3=left
        switch (edge)
        {
            case 0: // Top → go down
                pos = new Vector2(Random.Range(-camWidth / 2, camWidth / 2), cam.orthographicSize + buffer);
                dir = Vector2.down;
                break;
            case 1: // Right → go left
                pos = new Vector2(camWidth / 2 + buffer, Random.Range(-camHeight / 2, camHeight / 2));
                dir = Vector2.left;
                break;
            case 2: // Bottom → go up
                pos = new Vector2(Random.Range(-camWidth / 2, camWidth / 2), -cam.orthographicSize - buffer);
                dir = Vector2.up;
                break;
            case 3: // Left → go right
                pos = new Vector2(-camWidth / 2 - buffer, Random.Range(-camHeight / 2, camHeight / 2));
                dir = Vector2.right;
                break;
            default:
                pos = Vector2.zero;
                dir = Vector2.zero;
                break;
            }   
    }
  


}
