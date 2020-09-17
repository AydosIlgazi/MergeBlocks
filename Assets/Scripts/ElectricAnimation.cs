using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        StartCoroutine(DestroyAnimation());

    }

    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator DestroyAnimation()
    {

        transform.rotation = Quaternion.identity;
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
        yield return new WaitForEndOfFrame();
        foreach (Transform child in transform)
        {
            child.GetComponent<LineRenderer>().enabled = true;
        }
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }
}
