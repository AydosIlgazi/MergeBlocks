﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameStatus : MonoBehaviour
{
    public GameObject block;
    public int gameScore = 0;
    public int currentLevel = 1;
    public int maxLevel = 10;
    public Slider levelSlider;
    public TextMeshProUGUI levelText;
    public GameObject comboText;
    public bool blockOnHold;
    private bool blockCreaterAvailable;
    public float lastCollisionTime = 0f;
    private int collisionMultiplier = 1;

    void Start()
    {
        blockCreaterAvailable = true;
        levelSlider.minValue = 0;
        levelSlider.maxValue = currentLevel * 10;
        levelSlider.value = 0;
        levelText.text = "Level " + currentLevel + '/' + maxLevel;
    }

    void Awake()
    {
        //StartBlockCoroutine();

    }

    void Update()
    {
        if (!blockOnHold)
        {
            if (Input.GetMouseButton(0) && blockCreaterAvailable)
            {
                blockCreaterAvailable = false;
                blockOnHold = true;
                StartCoroutine(CreateNewBlockHold());
            }
        }

    }

    IEnumerator CreateNewBlockClick()
    {
        yield return new WaitForSeconds(2f);
        Instantiate(block, new Vector3(0f, 5f, 0f), Quaternion.identity);
        
    }

    IEnumerator CreateNewBlockHold()
    {
        yield return null;
        Instantiate(block, new Vector3(Input.mousePosition.x, 5f, 0f), Quaternion.identity);
        yield return new WaitForSeconds(1f);
        blockCreaterAvailable = true;
    }

    public void StartBlockCoroutine()
    {
        StartCoroutine("CreateNewBlock");
    }

    public void IncreseGameScore(int score)
    {
        if(score + gameScore < levelSlider.maxValue)
        {
            gameScore += score;
            levelSlider.value = gameScore;
        }
        else
        {
            var surplusScore = score + gameScore - levelSlider.maxValue;
            currentLevel++;
            levelText.text = "Level " + currentLevel + '/' + maxLevel;
            levelSlider.maxValue = currentLevel * 10;
            levelSlider.value = surplusScore;
        }
        
    }

    public void CollisionMultiplier(float collisionTime)
    {
        if(collisionTime - lastCollisionTime < 3f)
        {
            collisionMultiplier++;
            GameObject comboTextObj= Instantiate(comboText, transform);
            comboTextObj.GetComponent<TextMeshPro>().text = collisionMultiplier.ToString() + 'x';
        }
        else
        {
            collisionMultiplier = 1;
        }
        Debug.Log(Time.time);
        lastCollisionTime = collisionTime;
    }
}
