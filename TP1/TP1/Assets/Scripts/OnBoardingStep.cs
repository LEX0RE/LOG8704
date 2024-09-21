using System;
using UnityEngine;

public class OnBoardingStep : MonoBehaviour
{
    private OnBoardingManager manager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.SetActive(false);
        this.manager = FindObjectsByType<OnBoardingManager>(FindObjectsSortMode.None)?[0];
    }

    public void StartStep()
    {
        gameObject.SetActive(true);

    }

    public void EndStep()
    {
        gameObject.SetActive(false);
        this.manager.NextStep();
    }
}
