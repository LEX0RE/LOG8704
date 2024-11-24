// Inspired By https://discussions.unity.com/t/object-to-follow-hand-joint/341376 from Follow Object

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class MusicalBoxEdition : MonoBehaviour
{
	[SerializeField]
	private GameObject _musicalBoxPrefab;

	[SerializeField]
	private float _noteEditionHeight = 0.1f;

	[SerializeField]
	private UnityEvent _editionStartedEvent;

	[SerializeField]
	private UnityEvent _editionEndedEvent;

	private MusicManager _musicManager;
	private HandTrackingManager _handTrackingManager;

	private GameObject _noteInEdition;
	private bool _isfollowedLeftHand = true;
	private static bool isNoteInEditing = false;

	public GameObject NoteInEdition
	{
		get { return _noteInEdition; }
	}

	public bool IsFollowedLeftHand
	{
		get { return _isfollowedLeftHand; }
	}

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
			GameObject grabbedNote = CheckForGrabbedNote(isLeftHand);

			isNoteInEditing = true;
			this._isfollowedLeftHand = isLeftHand;

			if (grabbedNote == null)
			{
				grabbedNote = Instantiate(this._musicalBoxPrefab);

				NoteComponent note = grabbedNote.GetComponent<NoteComponent>();

				note.OnSetup();
				this._musicManager.RegisterNote(note);
			}

			this._noteInEdition = grabbedNote;
			this._noteInEdition.GetComponent<XRGrabInteractable>().selectEntered.AddListener(OnEndCreation);
			_editionStartedEvent.Invoke();
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
			_editionEndedEvent.Invoke();
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
				Debug.Log(handTransform);

				this._noteInEdition.transform.position = handTransform.position;
				this._noteInEdition.transform.rotation = handTransform.rotation;
			}
		}
	}

	void OnEndCreation(SelectEnterEventArgs eventArgs)
	{
		if (this._noteInEdition != null)
		{
			this._noteInEdition.GetComponent<XRGrabInteractable>().selectEntered.RemoveListener(OnEndCreation);
			this._noteInEdition = null;
			isNoteInEditing = false;
			_editionEndedEvent.Invoke();
		}
	}

	GameObject CheckForGrabbedNote(bool isLeftHand)
	{
		GameObject noteInOtherHand = null;

		foreach (NoteComponent note in this._musicManager.Notes)
		{
			List<IXRSelectInteractor> interactors = note.GetComponent<XRGrabInteractable>().interactorsSelecting;

			foreach (IXRSelectInteractor interactor in interactors)
			{
				if ((interactor.handedness == InteractorHandedness.Left && isLeftHand) ||
					(interactor.handedness == InteractorHandedness.Right && !isLeftHand))
				{
					return note.gameObject;
				}

				if (noteInOtherHand == null &&
					((interactor.handedness == InteractorHandedness.Left && !isLeftHand) ||
					 (interactor.handedness == InteractorHandedness.Right && isLeftHand)))
				{
					noteInOtherHand = note.gameObject;
				}
			}
		}

		return noteInOtherHand;
	}
}
