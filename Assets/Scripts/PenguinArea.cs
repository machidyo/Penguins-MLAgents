using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MLAgents;
using UnityEngine;
using TMPro;

public class PenguinArea : Area
{
    [Tooltip("The agent inside the area")] 
    public PenguinAgent penguinAgent;

    [Tooltip("The baby penguin inside the area")]
    public GameObject penguinBaby;

    [Tooltip("The TextMeshPro text that shows the cumulative reward of the agent")]
    public TextMeshPro cumulativeRewardText;

    [Tooltip("Prefab of a live fish")] public Fish fishPrefab;

    private PenguinAcademy penguinAcademy;
    private List<GameObject> fishList;

    void Start()
    {
        penguinAcademy = FindObjectOfType<PenguinAcademy>();
        ResetArea();
    }

    void Update()
    {
        cumulativeRewardText.text = penguinAgent.GetCumulativeReward().ToString("0.00");
    }

    public override void ResetArea()
    {
        RemoveAllFish();
        PlacePenguin();
        PlaceBaby();
        SpawnFish(4, penguinAcademy.FishSpeed);
    }

    public void RemoveSpecificFish(GameObject fishObject)
    {
        fishList.Remove(fishObject);
        Destroy(fishObject);
    }

    public int FishRemaining => fishList.Count;

    public static Vector3 ChooseRandomPosition(Vector3 center, float minAngle, float maxAngle, float minRadius,
        float maxRadius)
    {
        var radius = minRadius;
        var angle = minAngle;

        if (maxRadius > minRadius)
        {
            radius = Random.Range(minRadius, maxRadius);
        }

        if (maxAngle > minAngle)
        {
            angle = Random.Range(minAngle, maxAngle);
        }

        return center + Quaternion.Euler(0f, angle, 0f) * Vector3.forward * radius;
    }

    private void RemoveAllFish()
    {
        if (fishList != null)
        {
            foreach (var f in fishList.Where(f => f != null))
            {
                Destroy(f);
            }
        }

        fishList = new List<GameObject>();
    }

    private void PlacePenguin()
    {
        var rigidbody = penguinAgent.GetComponent<Rigidbody>();
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        penguinAgent.transform.position =
            ChooseRandomPosition(transform.position, 0f, 360f, 0f, 9f)
            + Vector3.up * 0.5f;
        penguinAgent.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
    }

    private void PlaceBaby()
    {
        var rigidbody = penguinBaby.GetComponent<Rigidbody>();
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        penguinAgent.transform.position =
            ChooseRandomPosition(transform.position, -45f, 45f, 4f, 9f)
            + Vector3.up * 0.5f;
        penguinAgent.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 180f), 0f);
    }

    private void SpawnFish(int count, float fishSpeed)
    {
        for (var i = 0; i < count; i++)
        {
            var fishObject = Instantiate(fishPrefab.gameObject, transform, true);
            fishObject.transform.position =
                ChooseRandomPosition(transform.position, 100f, 260f, 2f, 13f)
                + Vector3.up * 0.5f;
            fishObject.transform.rotation =
                Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
            fishList.Add(fishObject);
            fishObject.GetComponent<Fish>().FishSpeed = fishSpeed;
        }
    }
}