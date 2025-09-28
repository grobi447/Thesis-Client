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

    void Start()
    {
        canonType = gridManager.currentCanonDirection;
        UpdateCanon();
    }

    void Update()
    {
        AimAtMouse();
    }

    public void UpdateCanon()
    {
        if (canonType == CanonType.Up)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            if (gridManager.GetTileAtPosition(transform.position + Vector3.left) != null && gridManager.GetTileAtPosition(transform.position + Vector3.left).SpriteData != null && gridManager.GetTileAtPosition(transform.position + Vector3.right) != null && gridManager.GetTileAtPosition(transform.position + Vector3.right).SpriteData != null) canonBody.transform.position = initialPosition;
            else if (gridManager.GetTileAtPosition(transform.position + Vector3.left) != null && gridManager.GetTileAtPosition(transform.position + Vector3.left).SpriteData != null && gridManager.GetTileAtPosition(transform.position + Vector3.down) != null && gridManager.GetTileAtPosition(transform.position + Vector3.down).SpriteData != null ) pivotPoint.transform.position = initialPosition - new Vector3(0.24f, 0, 0);
            else if (gridManager.GetTileAtPosition(transform.position + Vector3.right) != null && gridManager.GetTileAtPosition(transform.position + Vector3.right).SpriteData != null && gridManager.GetTileAtPosition(transform.position + Vector3.down) != null && gridManager.GetTileAtPosition(transform.position + Vector3.down).SpriteData != null ) pivotPoint.transform.position = initialPosition + new Vector3(0.25f, 0, 0);
            else canonBody.transform.position = initialPosition;
        }
        else if (canonType == CanonType.Down)
        {
            transform.rotation = Quaternion.Euler(0, 0, -180);
            if(gridManager.GetTileAtPosition(transform.position + Vector3.left) != null && gridManager.GetTileAtPosition(transform.position + Vector3.left).SpriteData != null && gridManager.GetTileAtPosition(transform.position + Vector3.right) != null && gridManager.GetTileAtPosition(transform.position + Vector3.right).SpriteData != null) canonBody.transform.position = initialPosition;
            else if (gridManager.GetTileAtPosition(transform.position + Vector3.left) != null && gridManager.GetTileAtPosition(transform.position + Vector3.left).SpriteData != null && gridManager.GetTileAtPosition(transform.position + Vector3.up) != null && gridManager.GetTileAtPosition(transform.position + Vector3.up).SpriteData != null ) pivotPoint.transform.position = initialPosition - new Vector3(0.24f, 0, 0);
            else if (gridManager.GetTileAtPosition(transform.position + Vector3.right) != null && gridManager.GetTileAtPosition(transform.position + Vector3.right).SpriteData != null && gridManager.GetTileAtPosition(transform.position + Vector3.up) != null && gridManager.GetTileAtPosition(transform.position + Vector3.up).SpriteData != null ) pivotPoint.transform.position = initialPosition + new Vector3(0.25f, 0, 0);
            else canonBody.transform.position = initialPosition;
        }
        else if (canonType == CanonType.Left)
        {
            transform.rotation = Quaternion.Euler(0, 0, 90);
            if(gridManager.GetTileAtPosition(transform.position + Vector3.down) != null && gridManager.GetTileAtPosition(transform.position + Vector3.down).SpriteData != null && gridManager.GetTileAtPosition(transform.position + Vector3.up) != null && gridManager.GetTileAtPosition(transform.position + Vector3.up).SpriteData != null) canonBody.transform.position = initialPosition;
            else if (gridManager.GetTileAtPosition(transform.position + Vector3.down) != null && gridManager.GetTileAtPosition(transform.position + Vector3.down).SpriteData != null && gridManager.GetTileAtPosition(transform.position + Vector3.right) != null && gridManager.GetTileAtPosition(transform.position + Vector3.right).SpriteData != null ) pivotPoint.transform.position = initialPosition - new Vector3(0, 0.25f, 0);
            else if (gridManager.GetTileAtPosition(transform.position + Vector3.up) != null && gridManager.GetTileAtPosition(transform.position + Vector3.up).SpriteData != null && gridManager.GetTileAtPosition(transform.position + Vector3.right) != null && gridManager.GetTileAtPosition(transform.position + Vector3.right).SpriteData != null ) pivotPoint.transform.position = initialPosition + new Vector3(0, 0.24f, 0);
            else canonBody.transform.position = initialPosition;
        }
        else if (canonType == CanonType.Right)
        {
            transform.rotation = Quaternion.Euler(0, 0, -90);
            if(gridManager.GetTileAtPosition(transform.position + Vector3.down) != null && gridManager.GetTileAtPosition(transform.position + Vector3.down).SpriteData != null && gridManager.GetTileAtPosition(transform.position + Vector3.up) != null && gridManager.GetTileAtPosition(transform.position + Vector3.up).SpriteData != null) canonBody.transform.position = initialPosition;
            else if (gridManager.GetTileAtPosition(transform.position + Vector3.down) != null && gridManager.GetTileAtPosition(transform.position + Vector3.down).SpriteData != null && gridManager.GetTileAtPosition(transform.position + Vector3.left) != null && gridManager.GetTileAtPosition(transform.position + Vector3.left).SpriteData != null ) pivotPoint.transform.position = initialPosition - new Vector3(0, 0.25f, 0);
            else if (gridManager.GetTileAtPosition(transform.position + Vector3.up) != null && gridManager.GetTileAtPosition(transform.position + Vector3.up).SpriteData != null && gridManager.GetTileAtPosition(transform.position + Vector3.left) != null && gridManager.GetTileAtPosition(transform.position + Vector3.left).SpriteData != null ) pivotPoint.transform.position = initialPosition + new Vector3(0, 0.24f, 0);
            else canonBody.transform.position = initialPosition;
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
            maxAimAngle = (gridManager.GetTileAtPosition(transform.position + Vector3.up) == null || gridManager.GetTileAtPosition(transform.position + Vector3.up).SpriteData == null) ? 90f : 0f;
            minAimAngle = (gridManager.GetTileAtPosition(transform.position + Vector3.down) == null || gridManager.GetTileAtPosition(transform.position + Vector3.down).SpriteData == null) ? -90f : 0f;
        }
        else if (canonType == CanonType.Right)
        {
            maxAimAngle = (gridManager.GetTileAtPosition(transform.position + Vector3.up) == null || gridManager.GetTileAtPosition(transform.position + Vector3.up).SpriteData == null) ? 90f : 0f;
            minAimAngle = (gridManager.GetTileAtPosition(transform.position + Vector3.down) == null || gridManager.GetTileAtPosition(transform.position + Vector3.down).SpriteData == null) ? -90f : 0f;
        }
        else if (canonType == CanonType.Up)
        {
            maxAimAngle = (gridManager.GetTileAtPosition(transform.position + Vector3.left) == null || gridManager.GetTileAtPosition(transform.position + Vector3.left).SpriteData == null) ? 90f : 0f;
            minAimAngle = (gridManager.GetTileAtPosition(transform.position + Vector3.right) == null || gridManager.GetTileAtPosition(transform.position + Vector3.right).SpriteData == null) ? -90f : 0f;
        }
        else if (canonType == CanonType.Down)
        {
            maxAimAngle = (gridManager.GetTileAtPosition(transform.position + Vector3.left) == null || gridManager.GetTileAtPosition(transform.position + Vector3.left).SpriteData == null) ? 90f : 0f;
            minAimAngle = (gridManager.GetTileAtPosition(transform.position + Vector3.right) == null || gridManager.GetTileAtPosition(transform.position + Vector3.right).SpriteData == null) ? -90f : 0f;
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

        if (targetLocalAngle >= minAimAngle && targetLocalAngle <= maxAimAngle)
        {
            pivotPoint.transform.localEulerAngles = new Vector3(0f, 0f, targetLocalAngle);
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
        pivotPoint.transform.localEulerAngles = new Vector3(0f, 0f, idleAngle);
    }
}
