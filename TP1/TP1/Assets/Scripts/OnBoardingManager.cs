using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class OnBoardingManager : MonoBehaviour
{
    [Tooltip("Step of the unboarding")]
    [SerializeField]
    public List<OnBoardingStep> m_steps;

    public int m_nextStepIndex;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (var step in m_steps)
        {
            if (!step.GetInitState()) step.gameObject.SetActive(true);
        }
        this.m_nextStepIndex = 0;
        this.NextStep();
    }

    public void NextStep()
    {
        if (this.m_steps.Count > this.m_nextStepIndex) this.m_steps[this.m_nextStepIndex++].StartStep();
    }
}
