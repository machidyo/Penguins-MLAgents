using System;
using System.Collections;
using System.Collections.Generic;
using MLAgents;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

public class PenguinAgent : Agent
{
    [Tooltip("How fast the agent moves forward")]
    public float MoveSpeed = 5f;

    [Tooltip("How fast the agent turns")] public float TurnSpeed = 180f;

    [Tooltip("Prefab of heart that appears when the baby is fed")]
    public GameObject heartPrefab;

    [Tooltip("Prefab of regurgitated fish that appears when the baby is fed")]
    public GameObject regurgitatedFishPrefab;

    private PenguinArea penguinArea;
    private PenguinAcademy penguinAcademy;
    private new Rigidbody rigidbody;
    private GameObject baby;

    private bool isFull;
    private float feedRadius = 0f;

    private void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, baby.transform.position) < feedRadius)
        {
            RegurgitateFish();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("fish"))
        {
            EatFish(collision.gameObject);
        }
        else if (collision.transform.CompareTag("baby"))
        {
            RegurgitateFish();
        }
    }

    public override void InitializeAgent()
    {
        base.InitializeAgent();
        penguinArea = GetComponentInParent<PenguinArea>();
        penguinAcademy = FindObjectOfType<PenguinAcademy>();
        baby = penguinArea.penguinBaby;
        rigidbody = GetComponent<Rigidbody>();
    }

    public override void AgentAction(float[] vectorAction)
    {
        var forwardAmount = vectorAction[0];
        rigidbody.MovePosition(transform.position +
                               transform.forward * (forwardAmount * MoveSpeed * Time.fixedDeltaTime));

        var turnAmount = 0f;
        switch (vectorAction[1])
        {
            case 0f:
                turnAmount = 0f;
                break;
            case 1f:
                turnAmount = -1f;
                break;
            case 2f:
                turnAmount = 1f;
                break;
        }

        transform.Rotate(transform.up * (turnAmount * TurnSpeed * Time.fixedDeltaTime));

        AddReward(-1f / agentParameters.maxStep);
    }

    public override float[] Heuristic()
    {
        var forwardAction = 0f;
        var turnAction = 0f;
        if (Input.GetKey(KeyCode.W))
        {
            forwardAction = 1f;
        }

        if (Input.GetKey(KeyCode.A))
        {
            turnAction = 1f;
        }

        if (Input.GetKey(KeyCode.D))
        {
            turnAction = 2f;
        }

        return new [] {forwardAction, turnAction};
    }

    public override void AgentReset()
    {
        isFull = false;
        penguinArea.ResetArea();
        feedRadius = penguinAcademy.FeedRadius;
    }

    public override void CollectObservations()
    {
        // eaten a fish
        AddVectorObs(isFull);
        // distance to the baby
        AddVectorObs(Vector3.Distance(baby.transform.position, transform.position));
        // distance to baby(vector3 = 3values)
        AddVectorObs((baby.transform.position - transform.position).normalized);
        // penguin is facing
        AddVectorObs(transform.forward);
    }

    private void EatFish(GameObject fishObject)
    {
        if(isFull) return;

        isFull = true;
        penguinArea.RemoveSpecificFish(fishObject);
        AddReward(1f);
    }

    private void RegurgitateFish()
    {
        if(!isFull) return;
        isFull = false;

        var regurgitatedFish = Instantiate(regurgitatedFishPrefab);
        regurgitatedFish.transform.parent = transform.parent;
        regurgitatedFish.transform.position = baby.transform.position;
        Destroy(regurgitatedFish, 4f);

        var heart = Instantiate(heartPrefab);
        heart.transform.parent = transform.parent;
        heart.transform.position = baby.transform.position + Vector3.up;
        Destroy(heart, 4f);
        
        AddReward(1f);

        if (penguinArea.FishRemaining <= 0)
        {
            Done();   
        }
    }
}