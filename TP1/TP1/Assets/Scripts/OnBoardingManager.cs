using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class OnBoardingManager : MonoBehaviour
{
    [Tooltip("Step of the unboarding")]
    [SerializeField]
    List<OnBoardingStep> m_steps;

    private static List<OnBoardingStep> steps;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (this.m_steps.Count > 0)
        {
            OnBoardingManager.steps = this.m_steps;
            OnBoardingStep step = OnBoardingManager.steps[0];
            Debug.Log(step);
            step.StartStep();
        }
    }

    public static void NextStep()
    {
        if (OnBoardingManager.steps.Count > 0)
        {
            OnBoardingManager.steps.RemoveAt(0);
            OnBoardingManager.steps.First().StartStep();
        }
    }
}
