using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.LeftArrow)) {
            transform.Translate(-1,0,0);
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow)) {
            transform.Translate(-1,0,0);
        }
    }
}