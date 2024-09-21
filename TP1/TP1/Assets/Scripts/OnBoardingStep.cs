using UnityEngine;

public class OnBoardingStep : MonoBehaviour
{
    private OnBoardingManager manager;
    private bool isInit = false;

    protected void Start()
    {
        this.Init();
    }

    public void StartStep()
    {
        this.Init();
        gameObject.SetActive(true);

    }

    public void EndStep()
    {
        this.Init();
        gameObject.SetActive(false);
        this.manager.NextStep();
    }

    private void Init()
    {
        if (!this.isInit)
        {
            gameObject.SetActive(false);
            this.manager = FindObjectsByType<OnBoardingManager>(FindObjectsSortMode.None)?[0];
            this.isInit = true;
        }
    }
}
