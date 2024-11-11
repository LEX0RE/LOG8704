using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Gestures;

[Serializable]
public enum HandDetection
{
	Both,
	Left,
	Right,
}

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
    public HandDetection handToDetect = HandDetection.Both;

	[NonSerialized]
    public bool wasDetected;
	
	[NonSerialized]
    public bool performedTriggered;
	
	[NonSerialized]
    public float holdStartTime;
}

public class HandTrackingManager : MonoBehaviour
{
    private XRHandTrackingEvents _leftTrackingEvents;
    private XRHandTrackingEvents _rightTrackingEvents;

    [SerializeField]
    [Tooltip("The interval at which a pose detection is performed.")]
    float _poseDetectionInterval = 0.1f;

	[SerializeField]
	[Tooltip("The list of all poses to be detected.")]
	PoseDetectionList _poseDetections;

    float _timeLastCheck;

    private void OnEnable()
    {
		GameObject leftTrackingGameObject = Instantiate(new GameObject("Left Hand Tracking"), gameObject.transform);
		GameObject rightTrackingGameObject = Instantiate(new GameObject("Right Hand Tracking"), gameObject.transform);

		this._leftTrackingEvents = leftTrackingGameObject.AddComponent<XRHandTrackingEvents>();
		this._rightTrackingEvents = rightTrackingGameObject.AddComponent<XRHandTrackingEvents>();

		this._leftTrackingEvents.handedness = Handedness.Left;
		this._rightTrackingEvents.handedness = Handedness.Right;

        this._leftTrackingEvents.jointsUpdated.AddListener(OnJointsUpdated);
        this._rightTrackingEvents.jointsUpdated.AddListener(OnJointsUpdated);
    }

    private void OnDisable()
    {
		if (this._leftTrackingEvents != null)
        	this._leftTrackingEvents.jointsUpdated.RemoveListener(OnJointsUpdated);

		if (this._rightTrackingEvents != null)
        	this._rightTrackingEvents.jointsUpdated.RemoveListener(OnJointsUpdated);
    }

    private void OnJointsUpdated(XRHandJointsUpdatedEventArgs eventArgs)
    {
		Debug.Log(eventArgs.hand.handedness);
        if (!isActiveAndEnabled || Time.timeSinceLevelLoad < this._timeLastCheck + this._poseDetectionInterval)
            return;

		foreach(PoseDetection poseDetection in this._poseDetections.list)
		{
			bool isDetectedHand = poseDetection.handToDetect == HandDetection.Both ||
					 			(poseDetection.handToDetect == HandDetection.Left && eventArgs.hand.handedness == Handedness.Left) || 
					 			(poseDetection.handToDetect == HandDetection.Right && eventArgs.hand.handedness == Handedness.Right);

			if ((poseDetection.posePerformedEvent == null && poseDetection.poseEndedEvent == null) || !isDetectedHand)
				return;

			bool detected = this._leftTrackingEvents.handIsTracked && poseDetection.handPose != null && poseDetection.handPose.CheckConditions(eventArgs);

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

			if (!poseDetection.performedTriggered && detected)
			{
				float holdTimer = Time.timeSinceLevelLoad - poseDetection.holdStartTime;

				if (holdTimer > poseDetection.minimumHoldTime)
				{
					poseDetection.posePerformedEvent?.Invoke();
					poseDetection.performedTriggered = true;
				}
			}
		}

        this._timeLastCheck = Time.timeSinceLevelLoad;
    }
}
