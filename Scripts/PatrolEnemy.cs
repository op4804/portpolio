using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemy : Enemy
{
    // 시작 지점
    [Header("Patrol set")]
    private float startPosition;
    private float patrolDirection;
    [SerializeField] private float patrolWidth;
    [SerializeField] private float patrolStep;
    [SerializeField] private float patrolStepTime;


    // Start is called before the first frame update
    void Start()
    {
        startPosition = this.transform.position.x;
        patrolDirection = -1;
        StartCoroutine(Patrol());

    }

    // Update is called once per frame
     protected override void Update()
    {
        base.Update();
    }
    IEnumerator Patrol()
    {
        while (true)
        {
            if (patrolDirection == -1 && this.transform.position.x < startPosition - patrolWidth / 2)
            {
                patrolDirection = 1;
            }
            else if(patrolDirection == 1 && this.transform.position.x > startPosition + patrolWidth / 2)
            {
                patrolDirection = -1;
            }
            this.transform.Translate(new Vector3(patrolStep * patrolDirection, 0, 0));
            yield return new WaitForSeconds(patrolStepTime);
        }

    }

}
