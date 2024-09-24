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
            var isPointerOverUI = CheckUIInteraction(); 
            if (!isPointerOverUI && RayInteractor.TryGetCurrentARRaycastHit(out var arRaycastHit))
            {
                if (!(arRaycastHit.trackable is ARPlane arPlane))
                    return;

                SpawnEventChannel.RaiseEvent(arRaycastHit.pose.position);
            }
            
            return;
        }

        if (RayInteractor.interactablesSelected.Count > 0)
        {
            foreach (var obj in RayInteractor.interactablesSelected)
            {
                Vector3 screenPosition = Camera.main.WorldToScreenPoint(obj.transform.position);
                Vector2 screenCoordinte = new Vector2(screenPosition.x, screenPosition.y);
                
                //TODO Call depth function here
            }
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
    
    

    private bool CheckUIInteraction()
    {
        var eventUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(-1);
        
        var onBoardingManager = GameObject.FindGameObjectWithTag("TutorialMenu").GetComponent<OnBoardingManager>();
        var isOnboarding = onBoardingManager.m_nextStepIndex < onBoardingManager.m_steps.Count;

        var isSelectionMenuActive = GameObject.FindGameObjectWithTag("SelectionMenu")
            .GetComponent<PokemonSelectionMenuManager>().m_IsOptionMenuActive;
        
        return eventUI || isOnboarding || isSelectionMenuActive;
    }
}