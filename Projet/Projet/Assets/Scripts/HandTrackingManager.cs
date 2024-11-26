using System;
using System.Collections.Generic;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Gestures;
using UnityEngine.XR.Management;
using Volorf.VRNotifications;

[Serializable]
public class PoseDetectionList
{
	[SerializeField]
	[Tooltip("The list of all poses to be detected.")]
	public List<PoseDetection> list = new();
}

[Serializable]
public class PoseDetection
{
	[SerializeField]
	[Tooltip("The pose that need to be detected.")]
	public XRHandPose handPose;

	[SerializeField]
	[Tooltip("The minimum amount of time the hand must be held in the required shape and orientation for the pose to be performed.")]
	public float minimumHoldTime = 0.2f;

	[SerializeField]
	[Tooltip("The event fired when the pose is performed.")]
	public UnityEvent posePerformedEvent;

	[SerializeField]
	[Tooltip("The event fired when the pose is ended.")]
	public UnityEvent poseEndedEvent;

	[SerializeField]
	[Tooltip("The hand that will detect the pose.")]
	public Handedness handToDetect;

	[FormerlySerializedAs("poseName")]
	[SerializeField] 
	[Tooltip("The name of the hand pose")]
	public string message;

	[NonSerialized]
	public bool wasDetected;

	[NonSerialized]
	public bool performedTriggered;

	[NonSerialized]
	public float holdStartTime;
}

public class HandTransform
{
	public Vector3 position;
	public Quaternion rotation;

	public HandTransform(Vector3 position, Quaternion rotation)
	{
		this.position = position;
		this.rotation = rotation;
	}
}

public class HandTrackingManager : MonoBehaviour
{

	[SerializeField]
	[Tooltip("The interval at which a pose detection is performed.")]
	float _poseDetectionInterval = 0.1f;

	[SerializeField]
	[Tooltip("The list of all poses to be detected.")]
	PoseDetectionList _poseDetections;

	private List<HandTracker> _trackers = new List<HandTracker>();

	private GameObject _xrOrigin;
	private XRHandSubsystem m_HandSubsystem;

	public HandTransform GetHandTransform(Handedness handedness, XRHandJointID jointID = XRHandJointID.MiddleMetacarpal)
	{
		if (CheckHandSubsystem() && CheckXROrigin() && handedness != Handedness.Invalid)
		{
			XRHandSubsystem.UpdateSuccessFlags handFlag = handedness == Handedness.Left ?
														  XRHandSubsystem.UpdateSuccessFlags.LeftHandRootPose :
														  XRHandSubsystem.UpdateSuccessFlags.RightHandRootPose;

			if ((m_HandSubsystem.TryUpdateHands(XRHandSubsystem.UpdateType.Dynamic) & handFlag) != 0)
			{
				XRHand hand = handedness == Handedness.Left ? m_HandSubsystem.leftHand : m_HandSubsystem.rightHand;
				XRHandJoint handJoint = hand.GetJoint(jointID);

				if (handJoint.trackingState != XRHandJointTrackingState.None && handJoint.TryGetPose(out Pose pose))
				{
					Vector3 handJointPosition = this._xrOrigin.transform.InverseTransformPoint(pose.position);
					Vector3 nextPosition = this._xrOrigin.transform.TransformPoint(handJointPosition);

					return new HandTransform(nextPosition, pose.rotation);
				}
			}
		}

		return null;
	}

	void Start()
	{
		this._xrOrigin = FindFirstObjectByType<XROrigin>()?.gameObject;
		if (!CheckXROrigin()) return;

		XRLoader loader = XRGeneralSettings.Instance?.Manager?.activeLoader;

		if (loader != null)
		{
			m_HandSubsystem = loader.GetLoadedSubsystem<XRHandSubsystem>();

			if (!CheckHandSubsystem()) return;

			m_HandSubsystem.Start();
		}
	}

	private bool CheckXROrigin()
	{
		if (this._xrOrigin == null)
		{
			Debug.LogError("Could not find XROrigin");
			enabled = false;
			return false;
		}

		return true;
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

	private void OnEnable()
	{
		GameObject leftTrackingGameObject = Instantiate(new GameObject("Left Hand Tracking"), gameObject.transform);
		XRHandTrackingEvents leftTrackingEvents = leftTrackingGameObject.AddComponent<XRHandTrackingEvents>();
		leftTrackingEvents.handedness = Handedness.Left;
		this._trackers.Add(new HandTracker(leftTrackingEvents, this._poseDetectionInterval, this._poseDetections.list));

		GameObject rightTrackingGameObject = Instantiate(new GameObject("Right Hand Tracking"), gameObject.transform);
		XRHandTrackingEvents rightTrackingEvents = rightTrackingGameObject.AddComponent<XRHandTrackingEvents>();
		rightTrackingEvents.handedness = Handedness.Right;
		this._trackers.Add(new HandTracker(rightTrackingEvents, this._poseDetectionInterval, this._poseDetections.list));
	}

	private void OnDisable()
	{
		this._trackers.Clear();
	}
}

class HandTracker
{
	XRHandTrackingEvents _eventsTracking;
	float _timeLastCheck;

	float _poseDetectionInterval = 0.1f;
	List<PoseDetection> _poseDetections = new();

	public HandTracker(XRHandTrackingEvents eventsTracking, float poseDetectionInterval, List<PoseDetection> poseDetections)
	{
		this._poseDetectionInterval = poseDetectionInterval;

		foreach (PoseDetection poseDetection in poseDetections)
		{
			if (poseDetection.handToDetect == eventsTracking.handedness)
			{
				this._poseDetections.Add(poseDetection);
			}
		}

		this._eventsTracking = eventsTracking;
		this._eventsTracking.jointsUpdated.AddListener(OnJointsUpdated);
	}

	~HandTracker()
	{
		if (this._eventsTracking != null)
			this._eventsTracking.jointsUpdated.RemoveListener(OnJointsUpdated);
	}

	private void OnJointsUpdated(XRHandJointsUpdatedEventArgs eventArgs)
	{
		if (Time.timeSinceLevelLoad < this._timeLastCheck + this._poseDetectionInterval)
			return;

		foreach (PoseDetection poseDetection in this._poseDetections)
		{
			if ((poseDetection.posePerformedEvent == null && poseDetection.poseEndedEvent == null) || poseDetection.handPose == null)
			{
				continue;
			}

			bool detected = poseDetection.handPose.CheckConditions(eventArgs);

			if (!poseDetection.wasDetected && detected)
			{
				poseDetection.holdStartTime = Time.timeSinceLevelLoad;
			}
			else if (poseDetection.wasDetected && !detected)
			{
				poseDetection.performedTriggered = false;
				poseDetection.poseEndedEvent?.Invoke();
			}

			poseDetection.wasDetected = detected;

			if (!poseDetection.performedTriggered && detected && Time.timeSinceLevelLoad - poseDetection.holdStartTime > poseDetection.minimumHoldTime)
			{
				poseDetection.posePerformedEvent?.Invoke();
				NotificationManager.Instance.SendMessage(poseDetection.message);
				poseDetection.performedTriggered = true;
			}
		}

		this._timeLastCheck = Time.timeSinceLevelLoad;
	}
}