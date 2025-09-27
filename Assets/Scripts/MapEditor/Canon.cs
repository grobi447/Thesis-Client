using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


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
    [SerializeField] public bool targetPlayer = false;
    private Vector3 initialPosition;
    [SerializeField] public float fireRate = 1f;
    [SerializeField] public float projectileSpeed = 100f;
    [SerializeField] public Bullet bulletPrefab;
    [SerializeField] public GameObject launchPoint;
    public bool reloading = false;
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
        Fire();
    }

    public void UpdateCanon()
    {
        if (canonType == CanonType.Left)
        {
            transform.rotation = Quaternion.Euler(0, 0, 90);
            transform.position = initialPosition - new Vector3(0, 0.25f, 0);
        }
        else if (canonType == CanonType.Right)
        {
            transform.rotation = Quaternion.Euler(0, 0, -90);
            transform.position = initialPosition - new Vector3(0, 0.25f, 0);
        }
        else if (canonType == CanonType.Up)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            transform.position = initialPosition;
        }
        else if (canonType == CanonType.Down)
        {
            transform.rotation = Quaternion.Euler(0, 0, 180);
            transform.position = initialPosition;
        }
    }

    public void Fire()
    {
        if (!reloading)
        {

            Bullet bullet = Instantiate(bulletPrefab, launchPoint.transform.position, transform.rotation);
            bullet.Init(projectileSpeed,transform.up);
            reloading = true;
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(fireRate);
        reloading = false;
    }
}
