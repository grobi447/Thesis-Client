using UnityEngine;
using System.Collections.Generic;

public class Saw : Trap
{
    public Vector3 currentDirection = Vector3.right;
    private Vector3 targetPosition;
    private bool isMoving = false;
    public float speed = 5f;
    private Rail currentRail;
    private Vector3 spawnPoint;

    void Awake()
    {
        this.trapType = TrapType.Saw;
    }
    void Update()
    {
        this.transform.rotation = Quaternion.Euler(0, 0, this.transform.rotation.eulerAngles.z + 6f);
        border.transform.rotation = Quaternion.Euler(0, 0, 0);

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetToSpawn();
        }

        if (isMoving)
        {
            MoveTowardsTarget();
        }
        else
        {
            FindNextTarget();
        }
    }
    void Start()
    {
        this.TileRenderer.sortingOrder = 2;
        this.GetComponent<Collider2D>().layerOverridePriority = 1;
    }

    public void StartMoving()
    {
        spawnPoint = transform.position;
        currentRail = gridManager.GetRailAtPosition(transform.position);
        if (currentRail != null)
        {
            FindNextTarget();
        }
    }

    public void ResetToSpawn()
    {
        transform.position = spawnPoint;
        currentRail = gridManager.GetRailAtPosition(spawnPoint);
        isMoving = false;
        currentDirection = Vector3.right;
    }

    private void FindNextTarget()
    {
        if (currentRail == null) return;

        List<Vector3> connected = currentRail.GetConnectedDirections();
        Vector3 opposite = -currentDirection;
        connected.Remove(opposite);

        if (connected.Count == 0)
        {
            Vector3 reverseDir = opposite;
            targetPosition = transform.position + reverseDir;
            if (gridManager.GetRailAtPosition(targetPosition) != null)
            {
                currentDirection = reverseDir;
                isMoving = true;
            }
            else
            {
                isMoving = false;
            }
            return;
        }

        Vector3 nextDir = connected[0];
        targetPosition = transform.position + nextDir;
        if (gridManager.GetRailAtPosition(targetPosition) != null)
        {
            currentDirection = nextDir;
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
    }

    private void MoveTowardsTarget()
    {
        if (gridManager.GetRailAtPosition(targetPosition) == null)
        {
            isMoving = false;
            return;
        }
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            transform.position = targetPosition;
            currentRail = gridManager.GetRailAtPosition(transform.position);
            isMoving = false;
        }
    }
}
