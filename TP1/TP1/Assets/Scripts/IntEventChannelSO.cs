using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "IntEventChannelSO", menuName = "Scriptable Objects/IntEventChannelSO")]
public class IntEventChannelSO : ScriptableObject
{
    public UnityAction<int> RaisedEvent;

    public void RaiseEvent(int input)
    {
        RaisedEvent?.Invoke(input);
    }
}
