using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class BodyPartCarousel : MonoBehaviour
{
    public Transform carouselRoater;
    public List<Transform> pieces;
    public Transform SelectionRect;
    public int examineIndex = 0;

    public int lastIndex = 5;


    private float oldDeltay = 0;

    private float scrollVal;
    public List<float> distances;

    private float maxDistance = 0;

    public GameObject newValbar;
    public GameObject currentValbar;

    public StatBlock currentStats;

    public StatBlock enemyStats;
    public float barSizeToUnitConversionRation = 0.548f;

    public TextMeshPro newBarText;
    public TextMeshPro curBarText;
    public TextMeshPro StatName;

    // Use this for initialization
    void Start ()
    {
        Initialise();
    }

    public void Initialise()
    {
        distances = pieces.Select(e => Vector3.Distance(e.position, SelectionRect.position)).ToList();
        float minDis = 99999;
        for (int i = 0; i < 5; i++)
        {
            if (distances[i] < minDis)
            {
                examineIndex = i;
                minDis = distances[i];
            }

            if (distances[i] > maxDistance)
            {
                maxDistance = distances[i];
            }
        }

        lastIndex = examineIndex;
        ResetBar(newValbar);
        ResetBar(currentValbar);
        newBarText.text = "";
        curBarText.text = "";
        UpdateComparisonText();
        currentStats.gameObject.GetComponent<PlayerMind>().paused = true;
        Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        float scrollAxis = Input.GetAxis("Mouse ScrollWheel");

        if (scrollAxis != 0)
        {
            float amount = Input.mouseScrollDelta.y - oldDeltay;
            scrollVal += amount * 10;

            distances = pieces.Select(e => Vector3.Distance(e.position, SelectionRect.position)).ToList();
            float minDis = 99999;
            for (int i = 0; i < 5; i++)
            {
                if (distances[i] < minDis)
                {
                    examineIndex = i;
                    minDis = distances[i];
                }

                if (distances[i] > maxDistance)
                {
                    maxDistance = distances[i];
                }
            }

            if (lastIndex != examineIndex)
            {
                lastIndex = examineIndex;
                ResetBar(newValbar);
                ResetBar(currentValbar);
                newBarText.text = "";
                curBarText.text = "";
                UpdateComparisonText();
            }
        }

        for (int i = 0; i < 5; i++)
        {
            float targetScale = 3;
            if (i == examineIndex)
            {
                targetScale = 5;
            }

            float scale = Mathf.Lerp(pieces[i].localScale.x, targetScale, 1 - distances[i] / maxDistance);
            pieces[i].localScale = new Vector3(scale, scale, pieces[i].localScale.z);
        }

        float angle = Mathf.LerpAngle(carouselRoater.rotation.eulerAngles.z, scrollVal, 0.0166f * 20);
        carouselRoater.rotation = Quaternion.Euler(0, 0, angle);
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            var compName = "";
            switch (examineIndex)
            {
                case 0: // body
                    currentStats.Defense = enemyStats.Defense;
                    compName = "Body";
                    StealEnemySprites(compName);
                    break;
                case 1: // arm
                    currentStats.LDamage = enemyStats.LDamage;
                    compName = "ArmRight";
                    StealEnemySprites(compName);
                    break;
                case 2: // foot
                    currentStats.RSpeed = enemyStats.RSpeed;
                    compName = "FootRight";
                    StealEnemySprites(compName);
                    break;
                case 3: // foot
                    currentStats.LSpeed = enemyStats.LSpeed;
                    compName = "FootLeft";
                    StealEnemySprites(compName);
                    break;
                case 4: //arm
                    currentStats.RDamage = enemyStats.RDamage;
                    compName = "ArmLeft";
                    StealEnemySprites(compName);
                    break;
            }
            currentStats.gameObject.GetComponent<PlayerMind>().paused = false;
            currentStats.gameObject.GetComponent<PlayerMind>().delayOneFrame = true;
            gameObject.SetActive(false);
            Time.timeScale = 1;
        }

}

    private void StealEnemySprites(string compName)
    {
        GameObject bodyPart1 = null;
        GameObject bodyPart2 = null;
        Transform[] children = currentStats.transform.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child.name == compName)
            {
                bodyPart1 = child.gameObject;
                break;
            }
        }

        children = enemyStats.transform.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child.name == compName)
            {
                bodyPart2 = child.gameObject;
                break;
            }
        }

        bodyPart1.gameObject.GetComponent<SpriteRenderer>().sprite =
            bodyPart2.gameObject.GetComponent<SpriteRenderer>().sprite;
        bodyPart2.gameObject.GetComponent<SpriteRenderer>().sprite = null;
    }

    private void ResetBar(GameObject bar)
    {
        bar.GetComponent<SpriteRenderer>().size = new Vector2(2, bar.GetComponent<SpriteRenderer>().size.y);
        ((RectTransform)bar.transform).anchoredPosition = new Vector2(50.1328f, ((RectTransform)bar.transform).anchoredPosition.y);
    }

    private void UpdateComparisonText()
    {
        string partName = "";
        float maxStat = 0;
        float newStat = 0;
        float curStat = 0;
        switch (examineIndex)
        {
            case 0: // body
                newStat = enemyStats.Defense;
                curStat = currentStats.Defense;
                StatName.text = "Defence";
                partName = "Body";
                break;
            case 1: // arm
                newStat = enemyStats.LDamage;
                curStat = currentStats.LDamage;
                StatName.text = "Damage";
                partName = "L Arm";
                break;
            case 2: // foot
                newStat = enemyStats.RSpeed;
                curStat = currentStats.RSpeed;
                StatName.text = "Speed";
                partName = "R Foot";
                break;
            case 3: // foot
                newStat = enemyStats.LSpeed;
                curStat = currentStats.LSpeed;
                StatName.text = "Speed";
                partName = "L Foot";
                break;
            case 4: //arm
                newStat = enemyStats.RDamage;
                curStat = currentStats.RDamage;
                StatName.text = "Damage";
                partName = "R Arm";
                break;
        }
        maxStat = newStat>curStat?newStat:curStat;

        SetBarLenght(newStat, maxStat, newValbar);
        SetBarLenght(curStat, maxStat, currentValbar);
        newBarText.text = enemyStats.Name + " " + partName + " - " + newStat;
        curBarText.text = "Current - " + curStat;
    }

    private void SetBarLenght(float newStat, float maxStat, GameObject bar)
    {
        var newValPercentage = (newStat / maxStat) * 2;
        bar.GetComponent<SpriteRenderer>().size = new Vector2(newValPercentage, bar.GetComponent<SpriteRenderer>().size.y);
        ((RectTransform)bar.transform).anchoredPosition = new Vector3(((RectTransform)bar.transform).anchoredPosition.x - ((2-newValPercentage) * barSizeToUnitConversionRation), ((RectTransform)bar.transform).anchoredPosition.y);
    }
}
