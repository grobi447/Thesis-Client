using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum CanonType
{
    Left,
    Right,
    Up,
    Down
}

public class Canon : Trap
{
    [SerializeField] public CanonType canonType;
    private Vector3 initialPosition;
    private MapLoader mapLoader;
    [SerializeField] public float fireRate = 1f;
    [SerializeField] public float projectileSpeed = 100f;
    [SerializeField] public Bullet bulletPrefab;
    [SerializeField] public Transform pivotPoint;
    [SerializeField] public GameObject canonBody;
    [SerializeField] public GameObject launchPoint;
    [SerializeField] float maxAimAngle;
    [SerializeField] float minAimAngle;
    [SerializeField] float idleSpeed = 50f;
    public bool reloading = false;
    public bool targetingPlayer = false;

    void Awake()
    {
        initialPosition = transform.position;
    }
    public void ResetPosition()
    {
        transform.position = initialPosition;
        pivotPoint.transform.localEulerAngles = Vector3.zero;
        canonBody.transform.position = initialPosition;
    }
    void Start()
    {
        if (!inGame)
        {
            canonType = gridManager.currentCanonDirection;
        }
        else
        {
            mapLoader = FindObjectOfType<MapLoader>();
        }
        UpdateCanon();
    }

    void Update()
    {
        if(inGame && targetingPlayer)
        {
            //aim at player
        }
        else if (!inGame && targetingPlayer)
        {
            AimAtMouse();
        }
        else
        {
            Fire();
        }
    }

    private bool HasBlockNeighbor(Vector3 direction)
    {
        if (inGame && mapLoader != null)
        {
            return mapLoader.HasTileAtPosition(transform.position + direction);
        }
        else if (!inGame && gridManager != null)
        {
            return gridManager.HasBlockNeighbor(this, direction);
        }
        return false;
    }

    public void UpdateCanon()
    {
        if (canonType == CanonType.Up)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            bool hasLeft = HasBlockNeighbor(Vector3.left);
            bool hasRight = HasBlockNeighbor(Vector3.right);
            bool hasDown = HasBlockNeighbor(Vector3.down);
            
            if ((hasLeft && hasRight) || hasDown)
                canonBody.transform.position = initialPosition;
            else if (hasLeft)
                canonBody.transform.position = initialPosition - new Vector3(0.23f, 0, 0);
            else if (hasRight)
                canonBody.transform.position = initialPosition + new Vector3(0.23f, 0, 0);
            else
                canonBody.transform.position = initialPosition;
        }
        else if (canonType == CanonType.Down)
        {
            transform.rotation = Quaternion.Euler(0, 0, -180);
            bool hasLeft = HasBlockNeighbor(Vector3.left);
            bool hasRight = HasBlockNeighbor(Vector3.right);
            bool hasUp = HasBlockNeighbor(Vector3.up);
            
            if ((hasLeft && hasRight) || hasUp)
                canonBody.transform.position = initialPosition;
            else if (hasLeft)
                canonBody.transform.position = initialPosition - new Vector3(0.23f, 0, 0);
            else if (hasRight)
                canonBody.transform.position = initialPosition + new Vector3(0.23f, 0, 0);
            else
                canonBody.transform.position = initialPosition;
        }
        else if (canonType == CanonType.Left)
        {
            transform.rotation = Quaternion.Euler(0, 0, 90);
            bool hasDown = HasBlockNeighbor(Vector3.down);
            bool hasUp = HasBlockNeighbor(Vector3.up);
            bool hasRight = HasBlockNeighbor(Vector3.right);
            
            if ((hasDown && hasUp) || hasRight)
                canonBody.transform.position = initialPosition;
            else if (hasDown)
                canonBody.transform.position = initialPosition - new Vector3(0, 0.23f, 0);
            else if (hasUp)
                canonBody.transform.position = initialPosition + new Vector3(0, 0.23f, 0);
            else
                canonBody.transform.position = initialPosition;
        }
        else if (canonType == CanonType.Right)
        {
            transform.rotation = Quaternion.Euler(0, 0, -90);
            bool hasDown = HasBlockNeighbor(Vector3.down);
            bool hasUp = HasBlockNeighbor(Vector3.up);
            bool hasLeft = HasBlockNeighbor(Vector3.left);
            
            if ((hasDown && hasUp) || hasLeft)
                canonBody.transform.position = initialPosition;
            else if (hasDown)
                canonBody.transform.position = initialPosition - new Vector3(0, 0.23f, 0);
            else if (hasUp)
                canonBody.transform.position = initialPosition + new Vector3(0, 0.23f, 0);
            else
                canonBody.transform.position = initialPosition;
        }
        
        CalculateMinMaxRotation();
    }

    public void Fire()
    {
        if (!reloading)
        {
            Bullet bullet = Instantiate(bulletPrefab, launchPoint.transform.position, launchPoint.transform.rotation);
            bullet.Init(projectileSpeed, launchPoint.transform.up);
            reloading = true;
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(fireRate);
        reloading = false;
    }

    private void CalculateMinMaxRotation()
    {
        if (canonType == CanonType.Left)
        {
            maxAimAngle = !HasBlockNeighbor(Vector3.down) ? 90f : 0f;
            minAimAngle = !HasBlockNeighbor(Vector3.up) ? -90f : 0f;
        }
        else if (canonType == CanonType.Right)
        {
            maxAimAngle = !HasBlockNeighbor(Vector3.up) ? 90f : 0f;
            minAimAngle = !HasBlockNeighbor(Vector3.down) ? -90f : 0f;
        }
        else if (canonType == CanonType.Up)
        {
            maxAimAngle = !HasBlockNeighbor(Vector3.left) ? 90f : 0f;
            minAimAngle = !HasBlockNeighbor(Vector3.right) ? -90f : 0f;
        }
        else if (canonType == CanonType.Down)
        {
            maxAimAngle = !HasBlockNeighbor(Vector3.right) ? 90f : 0f;
            minAimAngle = !HasBlockNeighbor(Vector3.left) ? -90f : 0f;
        }
    }



    private void AimAtMouse()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;
        Vector3 direction = mouseWorld - pivotPoint.position;
        float targetWorldAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        float baseAngle = 0f;
        if (canonType == CanonType.Up) baseAngle = 90f;
        else if (canonType == CanonType.Down) baseAngle = -90f;
        else if (canonType == CanonType.Left) baseAngle = 180f;
        else if (canonType == CanonType.Right) baseAngle = 0f;

        float targetLocalAngle = Mathf.DeltaAngle(0f, targetWorldAngle - baseAngle);

        if (float.IsNaN(targetLocalAngle) || float.IsInfinity(targetLocalAngle))
            targetLocalAngle = 0f;

        if (!IsLineBlocked(pivotPoint.position, mouseWorld) && targetLocalAngle >= minAimAngle && targetLocalAngle <= maxAimAngle)
        {
            pivotPoint.transform.localEulerAngles = new Vector3(0f, 0f, targetLocalAngle);
            Fire();
        }
        else
        {
            IdleRotation();
        }
    }

    private void IdleRotation()
    {
        float range = maxAimAngle - minAimAngle;
        float idleAngle = Mathf.PingPong(Time.time * idleSpeed, range) + minAimAngle;
        if (float.IsNaN(idleAngle) || float.IsInfinity(idleAngle))
            idleAngle = 0f;
        pivotPoint.transform.localEulerAngles = new Vector3(0f, 0f, idleAngle);
    }

    private bool IsLineBlocked(Vector3 start, Vector3 end)
    {
        Vector2 start2D = new Vector2(start.x, start.y);
        Vector2 end2D = new Vector2(end.x, end.y);
        Vector2 direction = end2D - start2D;
        RaycastHit2D[] hitInfos = Physics2D.RaycastAll(start2D, direction, Vector2.Distance(start2D, end2D));

        foreach (var hitInfo in hitInfos)
        {
            if (hitInfo.collider != null && (hitInfo.collider.CompareTag("Block") || (hitInfo.collider.CompareTag("Trap") && hitInfo.collider.gameObject != this.gameObject)))
            {
                return true;
            }
        }
        return false;
    }
}
