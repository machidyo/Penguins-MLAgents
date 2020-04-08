using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityScript.Macros;
using Random = UnityEngine.Random;

public class Fish : MonoBehaviour
{
    [Tooltip("The swim spped")] public float FishSpeed;

    private float randomizeSpeed = 0f;
    private float nextActionTime = -1f;
    private Vector3 targetPosition;

    private void FixedUpdate()
    {
        if (FishSpeed > 0f)
        {
            Swim();
        }
    }

    private void Swim()
    {
        if (Time.fixedTime >= nextActionTime)
        {
            randomizeSpeed = FishSpeed * Random.Range(0.5f, 1.5f);
            targetPosition = PenguinArea.ChooseRandomPosition(transform.parent.position, 100f, 260f, 2f, 13f);
            transform.rotation = Quaternion.LookRotation(targetPosition - transform.position, Vector3.up);
            var timeToGetThere = Vector3.Distance(transform.position, targetPosition) / randomizeSpeed;
            nextActionTime = Time.fixedTime + timeToGetThere;
        }
        else
        {
            var moveVector = randomizeSpeed * transform.forward * Time.fixedDeltaTime;
            if (moveVector.magnitude <= Vector3.Distance(transform.position, targetPosition))
            {
                transform.position += moveVector;
            }
            else
            {
                transform.position = targetPosition;
                nextActionTime = Time.fixedTime;
            }
        }
    }
}
