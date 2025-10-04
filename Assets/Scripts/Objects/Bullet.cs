using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Bullet : MonoBehaviour
{
    private float speed;
    Vector3 shootingDirection;

    public void Init(float speed, Vector3 shootingDirection)
    {
        this.speed = speed;
        this.shootingDirection = shootingDirection.normalized;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Block") || collision.gameObject.CompareTag("Trap") && !collision.name.Contains("Canon"))
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        transform.position += shootingDirection * speed * Time.deltaTime;
        if (transform.position.x < 0 || transform.position.x > 31 || transform.position.y < 0 || transform.position.y > 17)
        {
            Destroy(gameObject);
        }
    }
}
