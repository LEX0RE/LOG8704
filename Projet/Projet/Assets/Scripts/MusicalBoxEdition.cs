// Inspired By https://discussions.unity.com/t/object-to-follow-hand-joint/341376 from Follow Object

using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;

public class MusicalBoxEdition : MonoBehaviour
{
	[SerializeField]
	GameObject musicalBoxPrefab;

	[SerializeField]
	GameObject m_XROrigin;

	[SerializeField]
	private MusicManager musicManager;

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
					this.m_HandSubsystem = loader.GetLoadedSubsystem<XRHandSubsystem>();
					if (!CheckHandSubsystem())
						return;

					this.m_HandSubsystem.Start();
				}
			}
		}
	}

    void Update()
	{
		if (CheckHandSubsystem() && isNoteInEditing) 
		{
			if ((m_HandSubsystem.TryUpdateHands(XRHandSubsystem.UpdateType.Dynamic) & XRHandSubsystem.UpdateSuccessFlags.RightHandRootPose) != 0)
			{
				XRHandJoint handJoint = m_HandSubsystem.rightHand.GetJoint(XRHandJointID.MiddleMetacarpal);
				if (handJoint.trackingState != XRHandJointTrackingState.None && handJoint.TryGetPose(out Pose pose))
				{
					Vector3 handJointPosition = m_XROrigin.transform.InverseTransformPoint(pose.position);
					this.noteInEdition.transform.position = m_XROrigin.transform.TransformPoint(handJointPosition); 
					this.noteInEdition.transform.rotation = pose.rotation;
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

	public void CreateNote()
	{
		if (!this.isNoteInEditing) {
			this.isNoteInEditing = true;

			GameObject musicalBox = Instantiate(musicalBoxPrefab);

			this.noteInEdition = musicalBox;
			this.musicManager.RegisterNote(this.noteInEdition.GetComponent<NoteComponent>());
		}
	}

	public void DestroyNote()
	{
		if (this.isNoteInEditing) {
			this.isNoteInEditing = false;

			this.musicManager.UnregisterNote(this.noteInEdition.GetComponent<NoteComponent>());

			Destroy(this.noteInEdition);
			this.noteInEdition = null;
		}
	}
}
