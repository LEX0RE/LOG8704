using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class InputController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Vector3EventChannelSO SpawnEventChannel;
    public XRRayInteractor RayInteractor;


    bool m_AttemptSpawn;
    bool m_EverHadSelection;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (m_AttemptSpawn)
        {
            m_AttemptSpawn = false;
            var isPointerOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(-1);
            if (!isPointerOverUI && RayInteractor.TryGetCurrentARRaycastHit(out var arRaycastHit))
            {
                if (!(arRaycastHit.trackable is ARPlane arPlane))
                    return;

                SpawnEventChannel.RaiseEvent(arRaycastHit.pose.position);
            }
            return;
        }

        var selectState = RayInteractor.logicalSelectState;
        if (selectState.wasPerformedThisFrame)
        {
            m_EverHadSelection = RayInteractor.hasSelection;
        }
        else if (selectState.active)
            m_EverHadSelection |= RayInteractor.hasSelection;

        m_AttemptSpawn = false;

        if (selectState.wasCompletedThisFrame)
        {
            m_AttemptSpawn = !RayInteractor.hasSelection && !m_EverHadSelection;
        }

    }

}