using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carrot : MonoBehaviour
{
    // GR: Configuration variables
    [SerializeField] GameObject[] carrotStages;

    // GR: State variables
    int currentStage = -1;
    float growSize = 1f;
    float riseHeight = 0.185f;
    bool grow = false;
    bool rise = false;
    float timeBetweenStages = 5f;
    float currentTime = 0f;    
    float originalHeight = 0f;
    GameObject[] carrotStageInstance;

    private void Start()
    {
        carrotStageInstance = new GameObject[carrotStages.Length];
    }

    public void CreateCarrot(Vector3 position)
    {
        currentStage++;
        growSize = Random.Range(0.5f * growSize, growSize);
        Color plantColor = new Color(1f, Random.Range(0.5f, 1f), Random.Range(0f, 1f));
        Quaternion randomPlantRotation = new Quaternion();
        randomPlantRotation.eulerAngles = new Vector3(Random.Range(0f, 16f), Random.Range(-180f, 180f), Random.Range(0f, 16f));
        for (int i = 0; i < carrotStages.Length; i++)
        {
            Quaternion plantRotation = carrotStages[i].transform.rotation;
            plantRotation.eulerAngles += randomPlantRotation.eulerAngles;
            carrotStageInstance[i] = Instantiate(carrotStages[i], new Vector3(position.x, 0f, position.z), plantRotation);
            carrotStageInstance[i].GetComponent<MeshRenderer>().enabled = false;
            carrotStageInstance[i].GetComponent<MeshRenderer>().material.color = plantColor;
        }
        carrotStageInstance[currentStage].GetComponent<MeshRenderer>().enabled = true;
        originalHeight = carrotStageInstance[currentStage].transform.position.y;
    }

    void Update()
    {
        if (currentStage < 0) return;

        currentTime += Time.deltaTime;
        if (currentTime < timeBetweenStages)
        {
            if (grow)
            {
                float y = Mathf.Lerp(carrotStageInstance[currentStage].transform.localScale.y, growSize, Time.deltaTime * 1f);
                carrotStageInstance[currentStage].transform.localScale = new Vector3(y, y, y);
            }
            if (rise)
            {
                float y = carrotStageInstance[currentStage].transform.position.y + (Time.deltaTime * 0.03f);
                carrotStageInstance[currentStage].transform.position = new Vector3(carrotStageInstance[currentStage].transform.position.x, y, carrotStageInstance[currentStage].transform.position.z);
            }
        }
        else
        {
            if (currentStage == 0)
            {
                grow = true;
                currentStage++;
                carrotStageInstance[currentStage].transform.localScale /= 10f;
            }
            else if (currentStage == 1)
            {
                grow = true;
                currentStage++;
                carrotStageInstance[currentStage].transform.localScale /= 5f;
            }                
            else if (currentStage == 2)
            {
                grow = false;
                rise = true;
                currentStage++;
                carrotStageInstance[currentStage].transform.position = new Vector3(carrotStageInstance[currentStage].transform.position.x, carrotStageInstance[currentStage].transform.position.y - riseHeight, carrotStageInstance[currentStage].transform.position.z);
                carrotStageInstance[currentStage].transform.localScale = carrotStageInstance[currentStage-1].transform.localScale;
            }
            else if (currentStage == 3)
            {
                return;
            }

            for (int i = 0; i < carrotStages.Length; i++)
            {
                carrotStageInstance[i].GetComponent<MeshRenderer>().enabled = false;
            }
            carrotStageInstance[currentStage].GetComponent<MeshRenderer>().enabled = true;
            currentTime = 0f;
        }
    }
}
