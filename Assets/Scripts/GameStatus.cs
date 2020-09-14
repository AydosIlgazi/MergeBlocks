using System.Collections;
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
    // Start is called before the first frame update
    void Start()
    {
        levelSlider.minValue = 0;
        levelSlider.maxValue = currentLevel * 10;
        levelSlider.value = 0;
        levelText.text = "Level " + currentLevel + '/' + maxLevel;
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
        Instantiate(block, new Vector3(0f, 5f, 0f), Quaternion.identity);
        
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
}
