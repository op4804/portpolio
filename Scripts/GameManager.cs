using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    // when game start, instantiate player object on startpoint.
    [SerializeField] GameObject startPoint;
    [SerializeField] GameObject player;


    private void Awake()
    {
        Instantiate(player, startPoint.transform.position, Quaternion.identity);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
