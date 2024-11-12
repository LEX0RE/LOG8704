// Inspired By https://discussions.unity.com/t/object-to-follow-hand-joint/341376 from Follow Object

using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;

public class MusicalBoxEdition : MonoBehaviour
{
	[SerializeField]
	private GameObject musicalBoxPrefab;

	[SerializeField]
	private GameObject m_XROrigin;

	[SerializeField]
	private MusicManager musicManager;

	[SerializeField]
	private float noteEditionHeight = 0.1f;

	private XRHandSubsystem m_HandSubsystem;
	private GameObject noteInEdition;
	private bool isNoteInEditing = false;
	
	void Start()
	{
		XRGeneralSettings xrGeneralSettings = XRGeneralSettings.Instance;
		if (xrGeneralSettings != null)
		{
			XRManagerSettings manager = xrGeneralSettings.Manager;
			if (manager != null)
			{
				XRLoader loader = manager.activeLoader;
				if (loader != null)
				{
					m_HandSubsystem = loader.GetLoadedSubsystem<XRHandSubsystem>();
					if (!CheckHandSubsystem())
						return;

					m_HandSubsystem.Start();
				}
			}
		}
	}

	public void CreateNote()
	{

		if (!isNoteInEditing) {
			Debug.Log("Creating Editing Note");
			isNoteInEditing = true;

			GameObject musicalBox = Instantiate(musicalBoxPrefab);

			noteInEdition = musicalBox;
			musicManager.RegisterNote(noteInEdition.GetComponent<NoteComponent>());
		}
	}

	public void DestroyNote()
	{
		if (isNoteInEditing) {
			Debug.Log("Deleting Editing Note");
			isNoteInEditing = false;

			musicManager.UnregisterNote(noteInEdition.GetComponent<NoteComponent>());

			Destroy(noteInEdition);
			noteInEdition = null;
		}
	}

    void Update()
	{
		Debug.Log("TryUpdateHands: " + m_HandSubsystem.TryUpdateHands(XRHandSubsystem.UpdateType.Dynamic) + "\nisNoteInEditing: " + isNoteInEditing);
		if (CheckHandSubsystem() && isNoteInEditing) 
		{
			if ((m_HandSubsystem.TryUpdateHands(XRHandSubsystem.UpdateType.Dynamic) & XRHandSubsystem.UpdateSuccessFlags.RightHandRootPose) != 0)
			{
				XRHandJoint handJoint = m_HandSubsystem.rightHand.GetJoint(XRHandJointID.MiddleMetacarpal);
				
				if (handJoint.trackingState != XRHandJointTrackingState.None && handJoint.TryGetPose(out Pose pose))
				{
					Vector3 handJointPosition = m_XROrigin.transform.InverseTransformPoint(pose.position);
					Vector3 nextPosition = m_XROrigin.transform.TransformPoint(handJointPosition);
					nextPosition.y += noteEditionHeight; 

					noteInEdition.transform.position = nextPosition;
					noteInEdition.transform.rotation = pose.rotation;
				}
			}
		}
	}

    private bool CheckHandSubsystem()
	{
		if (m_HandSubsystem == null)
		{
			Debug.LogError("Could not find Hand Subsystem");

			enabled = false;
			return false;
		}

		return true;
	}
}
