using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    PlayerStateList pState;
    public float distance;
    public LayerMask isLayer;
    public bool dir;

    public float bulletSpeed;
    // Start is called before the first frame update
    void Start()
    {
        pState = GetComponent<PlayerStateList>();
        Invoke("DestroyBullet", 2);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D ray = Physics2D.Raycast(transform.position, transform.right, distance, isLayer);
        if (ray.collider != null)
        {
            if (ray.collider.tag == "Enemy")
            {
                Debug.Log("Bullet Hit!");

                r
            }
            DestroyBullet();
        }
        if (dir)
        {
            transform.Translate(transform.right * bulletSpeed * Time.deltaTime);

        }
        else
        {
            transform.Translate(transform.right * -1 * bulletSpeed * Time.deltaTime);
        }



    }

    void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
