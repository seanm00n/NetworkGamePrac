using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 30.0f;
    public float fireRange = 300.0f;
    public float damage = 10.0f;

    private Transform tr;
    private Vector3 spawnPoint;

    private void Start()
    {
        tr = GetComponent<Transform>();
        spawnPoint = tr.position;
    }
    private void Update()
    {
        tr.Translate(Vector3.forward * Time.deltaTime * speed);
        if((spawnPoint - tr.position).sqrMagnitude > fireRange)
        {
            StartCoroutine(this.DestroyBullet());
        }
    }

    IEnumerator DestroyBullet()
    {
        Destroy(gameObject);
        yield return null;
    }
}
