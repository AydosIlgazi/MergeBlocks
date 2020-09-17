﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Wall : MonoBehaviour
{
    public enum WallType
    {
        LeftWall,
        RightWall
    }
    WallType wallType;
    void Start()
    {
        if (transform.position.x < 0)
        {
            wallType = WallType.LeftWall;
        }
        else
        {
            wallType = WallType.RightWall;
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.GetComponent<Block>().isBlockReleased)
        {
            if (wallType == WallType.LeftWall)
            {
                Vector3 forceVec = new Vector3(Random.Range(4f, 7f), Random.Range(3f, 5f));
                collision.collider.GetComponent<Rigidbody>().AddRelativeForce(forceVec , ForceMode.Impulse);

            }
            else
            {
                Vector3 forceVec = new Vector3(Random.Range(-4f, 7f), Random.Range(3f, 5f));
                collision.collider.GetComponent<Rigidbody>().AddRelativeForce(forceVec , ForceMode.Impulse);

            }
        }

    }
}
