using System.Collections;
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
        StartCoroutine(WaitSomeSecond());
    }

    private IEnumerator WaitSomeSecond()
    {
        yield return new WaitForSeconds(0.2f);
        this.Init();
        gameObject.SetActive(false);
        this.manager.NextStep();
    }

    public bool GetInitState()
    {
        return this.isInit;
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
