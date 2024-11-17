// Inspired By https://discussions.unity.com/t/object-to-follow-hand-joint/341376 from Follow Object

using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Management;

public class MusicalBoxEdition : MonoBehaviour
{
	[SerializeField]
	private GameObject _musicalBoxPrefab;

	[SerializeField]
	private float _noteEditionHeight = 0.1f;

	private MusicManager _musicManager;
	private HandTrackingManager _handTrackingManager;

	private GameObject _noteInEdition;
	private bool _isfollowedLeftHand = true;
	private static bool isNoteInEditing = false;

	void Start()
	{
		this._handTrackingManager = FindFirstObjectByType<HandTrackingManager>();
		if (this._handTrackingManager == null)
		{
			Debug.LogError("Could not find HandTrackingManager");
			enabled = false;
			return;
		}

		this._musicManager = FindFirstObjectByType<MusicManager>();
		if (this._musicManager == null)
		{
			Debug.LogError("Could not find MusicManager");
			enabled = false;
			return;
		}
	}

	public void CreateNote(bool isLeftHand)
	{
		if (!isNoteInEditing)
		{
			isNoteInEditing = true;
			this._isfollowedLeftHand = isLeftHand;

			GameObject musicalBox = Instantiate(this._musicalBoxPrefab);

			this._noteInEdition = musicalBox;
			this._musicManager.RegisterNote(this._noteInEdition.GetComponent<NoteComponent>());

			this._noteInEdition.GetComponent<XRGrabInteractable>().selectEntered.AddListener(OnGrab);
			Debug.Log("OnGrab listen added");
		}
	}

	public void DestroyNote(bool isLeftHand)
	{
		if (isNoteInEditing && this._isfollowedLeftHand == isLeftHand)
		{
			isNoteInEditing = false;

			this._musicManager.UnregisterNote(this._noteInEdition.GetComponent<NoteComponent>());

			Destroy(this._noteInEdition);
			this._noteInEdition = null;
		}
	}

	void Update()
	{
		if (isNoteInEditing)
		{
			HandTransform handTransform = this._handTrackingManager.GetHandTransform(this._isfollowedLeftHand ? Handedness.Left : Handedness.Right);

			if (handTransform != null)
			{
				handTransform.position.y += this._noteEditionHeight;

				this._noteInEdition.transform.position = handTransform.position;
				this._noteInEdition.transform.rotation = handTransform.rotation;
			}
		}
	}

	void OnGrab(SelectEnterEventArgs eventArgs)
	{
		this._noteInEdition = null;
		isNoteInEditing = false;
	}
}
