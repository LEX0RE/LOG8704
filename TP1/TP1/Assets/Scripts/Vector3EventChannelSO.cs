using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Vector3EventChannelSO", menuName = "Scriptable Objects/Vector3EventChannelSO")]
public class Vector3EventChannelSO : ScriptableObject
{
    public UnityAction<Vector3> RaisedEvent;

    public void RaiseEvent(Vector3 input)
    {
        RaisedEvent?.Invoke(input);
    }
}
