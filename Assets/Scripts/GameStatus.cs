using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatus : MonoBehaviour
{
    public GameObject block;

    // Start is called before the first frame update
    void Start()
    {

    }
    void Awake()
    {
        StartBlockCoroutine();

    }


    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator CreateNewBlock()
    {
        yield return new WaitForSeconds(2f);
        Instantiate(block, new Vector3(0f, 6f, 0f), Quaternion.identity);
        
    }
    public void StartBlockCoroutine()
    {
        StartCoroutine("CreateNewBlock");
    }
}
