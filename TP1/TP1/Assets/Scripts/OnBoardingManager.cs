using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class OnBoardingManager : MonoBehaviour
{
    [Tooltip("Step of the unboarding")]
    [SerializeField]
    List<OnBoardingStep> m_steps;

    private int m_nextStepIndex;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.m_nextStepIndex = 0;
        this.NextStep();
    }

    public void NextStep()
    {
        if (this.m_steps.Count > this.m_nextStepIndex) this.m_steps[this.m_nextStepIndex++].StartStep();
    }
}
