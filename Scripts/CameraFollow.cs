using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float followSpeed = 0.1f;

    [SerializeField] private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (CharacterManager.Instance)
        {
            transform.position = Vector3.Lerp(transform.position, CharacterManager.Instance.transform.position + offset, followSpeed);

        }
    }
}
