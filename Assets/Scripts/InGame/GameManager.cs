using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    private GameObject player;

    void Awake()
    {
        if (playerPrefab != null)
        {
            player = Instantiate(playerPrefab);
        }
    }

    void Start()
    {
        GameObject spawnPoint = GameObject.FindGameObjectWithTag("Spawn");
        if (spawnPoint != null && player != null)
        {
            player.transform.position = spawnPoint.transform.position + new Vector3(0f, 0.5f, 0f);
        }
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

}
