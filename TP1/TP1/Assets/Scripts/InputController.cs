using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class InputController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Vector3EventChannelSO SpawnEventChannel;
    public XRRayInteractor RayInteractor;
    public AROcclusionManager m_occlusionManager;

    bool m_AttemptSpawn;
    bool m_EverHadSelection;

    private XRCpuImage m_image;
    private int m_depthWidth;
    private int m_depthHeight;
    private short[] m_depthArray;
    private Texture2D m_depthTexture;
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
                Vector2 screenCoordinates = new Vector2(screenPosition.x, screenPosition.y);
                float distance = GetDepthFromScreenPosition((int)screenCoordinates.x, (int)screenCoordinates.y);
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
        
        // var onBoardingManager = GameObject.FindGameObjectWithTag("TutorialMenu").GetComponent<OnBoardingManager>();
        // var isOnboarding = onBoardingManager.m_nextStepIndex < onBoardingManager.m_steps.Count;

        var isSelectionMenuActive = GameObject.FindGameObjectWithTag("SelectionMenu")
            .GetComponent<PokemonSelectionMenuManager>().m_IsOptionMenuActive;
        
        return eventUI || isSelectionMenuActive;
    }

    // Code to access Depth API inspired by : https://developers.google.com/ar/develop/unity-arf/depth/developer-guide
    private void GetDepthData()
    {
        if (m_occlusionManager && m_occlusionManager.TryAcquireEnvironmentDepthCpuImage(out m_image)) {
            m_depthWidth = m_image.width;
            m_depthHeight = m_image.height;

            UpdateRawImage(ref m_depthTexture, m_image, TextureFormat.R16);
            var buffer = m_depthTexture.GetRawTextureData();
            m_depthArray = new short[buffer.Length];
            Buffer.BlockCopy(buffer, 0, m_depthArray, 0, buffer.Length);
        }
    }

    private float GetDepthFromScreenPosition(int x, int y)
    {
        GetDepthData();

        if (x >= m_depthWidth || x < 0 || y >= m_depthHeight || y < 0)
        {
            return -1.0f;
        }

        var depthIndex = (y * m_depthWidth) + x;
        var depthInShort = m_depthArray[depthIndex];
        var depthInMeters = depthInShort / 1000.0f;
        return depthInMeters;
    }

    private void UpdateRawImage(ref Texture2D texture, XRCpuImage image, TextureFormat format)
    {
        // Initialize texture if null
        if (texture == null || texture.width != image.width || texture.height != image.height)
        {
            texture = new Texture2D(image.width, image.height, format, false);
        }

        // Prepare conversion parameters
        XRCpuImage.ConversionParams conversionParams = new XRCpuImage.ConversionParams
        {
            inputRect = new RectInt(0, 0, image.width, image.height),
            outputDimensions = new Vector2Int(image.width, image.height),
            outputFormat = format,
            transformation = XRCpuImage.Transformation.None
        };

        // Create a NativeArray for raw image data
        var size = image.GetConvertedDataSize(conversionParams);
        var rawImageData = new NativeArray<byte>(size, Allocator.Temp);

        // Convert the image to raw data
        image.Convert(conversionParams, rawImageData);

        // Load raw data into the texture
        texture.LoadRawTextureData(rawImageData);
        texture.Apply();

        // Clean up
        rawImageData.Dispose();
    }
}