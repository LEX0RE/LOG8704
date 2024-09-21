using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class OnBoardingTracker : OnBoardingStep
{
    private ARTrackedImageManager m_TrackedImageManager;

    void OnEnable()
    {
        this.UpdateTracker();
        m_TrackedImageManager.trackablesChanged.AddListener(OnChanged);
    }

    void OnDisable()
    {
        this.UpdateTracker();
        m_TrackedImageManager.trackablesChanged.RemoveListener(OnChanged);
    }

    void OnChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        if (eventArgs.added.Count > 0 || eventArgs.updated.Count > 0 || eventArgs.updated.Count > 0)
        {
            this.EndStep();
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    new void Start()
    {
        base.Start();
        this.UpdateTracker();
    }

    private void UpdateTracker()
    {
        if (this.m_TrackedImageManager == null) this.m_TrackedImageManager = FindObjectsByType<ARTrackedImageManager>(FindObjectsSortMode.None)?[0];
    }
}
