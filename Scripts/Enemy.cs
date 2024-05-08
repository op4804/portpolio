using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    [SerializeField] float enemyHp = 10;
    [SerializeField] public float enemyDamage = 1f;

    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
        if (enemyHp <= 0) 
        {
            Destroy(gameObject);
        }


    }



    public void Hit(float _damage)
    {
        enemyHp -= _damage;

        // rb.AddForce(Vector2.right * 10f * Time.deltaTime, ForceMode2D.Force);

    }
}
