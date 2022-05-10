using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnermyManager : MonoBehaviour
{
    [SerializeField] GameObject basicEnemy;
    GameObject spawnPoint;
    // Start is called before the first frame update
    void Awake()
    {
        spawnPoint = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Instantiate(basicEnemy, spawnPoint.transform.position, Quaternion.identity, transform);
        }
    }
}
