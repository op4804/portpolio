using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    [SerializeField] float EnemyHp = 10;

    // Start is called before the first frame update
    void Start()
    {
        
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
    }
}
