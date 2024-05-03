using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    PlayerStateList pState;
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
