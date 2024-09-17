using System.Collections.Generic;
using UnityEngine;

public class OnBoardingManager : MonoBehaviour
{
    public struct Goal
    {
        public GoalTemplate current;
        public List<GameObject> gameObjects;
        public bool isCompleted;

        public Goal(GoalTemplate goal, List<GameObject> gameObjects)
        {
            this.current = goal;
            this.isCompleted = false;
            this.gameObjects = gameObjects;
        }

        public void Activate()
        {
            foreach (GameObject gameObject in this.gameObjects)
            {
                gameObject.SetActive(true);
            }
        }

        public void Deactivate()
        {
            foreach (GameObject gameObject in this.gameObjects)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public enum GoalTemplate
    {
        Start,
        ScanSurface
    }

    [Tooltip("The starting button is the beginning of the guide.")]
    [SerializeField]
    GameObject m_startButton;

    [Tooltip("Step of the unboarding")]
    [SerializeField]
    List<Goal> m_goals;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Launch()
    {

    }
}
