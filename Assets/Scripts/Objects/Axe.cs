using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AxeMovement
{
    Half,
    Circle
}
public enum AxeDirection
{
    Up,
    Down,
    Left,
    Right
}

public class Axe : Trap
{
    [SerializeField] public AxeMovement axeMovement;
    [SerializeField] public AxeDirection axeDirection;
    public float speed = 5f;
    [SerializeField] CapsuleCollider2D head;
    private float initialRotation;
    private float phase = 0f;

    void Awake()
    {
        head.enabled = false;
    }

    void Start()
    {
        axeMovement = gridManager.currentAxeMovement;
        axeDirection = gridManager.currentAxeDirection;
        SetDirection(axeDirection);
    }

    void Update()
    {
        if (axeMovement == AxeMovement.Circle)
        {
            transform.Rotate(0f, 0f, speed * 90f * Time.deltaTime);
        }
        else
        {
            phase += speed * Time.deltaTime;
            float angle = Mathf.Sin(phase) * 90f;
            transform.eulerAngles = new Vector3(0f, 0f, initialRotation + angle);
        }
    }

    public void SetDirection(AxeDirection axeDirection)
    {  
        switch (axeDirection)
        {
            case AxeDirection.Up:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case AxeDirection.Down:
                transform.rotation = Quaternion.Euler(0, 0, 180);
                break;
            case AxeDirection.Left:
                transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case AxeDirection.Right:
                transform.rotation = Quaternion.Euler(0, 0, 270);
                break;
        }
        initialRotation = transform.eulerAngles.z;
    }
}
