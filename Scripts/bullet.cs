using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    PlayerStateList pState;

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
        if (transform.rotation.y < 0)
        {
            Debug.Log("Right");
            transform.Translate(transform.right * bulletSpeed * Time.deltaTime);
        }
        else
        {
            Debug.Log("Left");
            transform.Translate(transform.right * -1 * bulletSpeed * Time.deltaTime);
        }
    }

    void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
