using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    [SerializeField] float EnemyHp = 10;

    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (EnemyHp <= 0) 
        {
            Destroy(gameObject);
        }


    }



    public void Hit(float _damage)
    {
        EnemyHp -= _damage;

        // rb.AddForce(Vector2.right * 10f * Time.deltaTime, ForceMode2D.Force);

    }
}
