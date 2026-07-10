using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class NamedUnityEvent
{
    public string eventName;
    public UnityEvent unityEvent;
}

public class AnimationEventRelay : MonoBehaviour
{
    [Header("Named Events")]
    public List<NamedUnityEvent> namedEvents = new List<NamedUnityEvent>();

    public void TriggerEventByName(string eventName)
    {
        for (int i = 0; i < namedEvents.Count; i++)
        {
            if (namedEvents[i].eventName == eventName)
            {
                namedEvents[i].unityEvent.Invoke();
                return;
            }
        }

        Debug.LogWarning("No animation relay event found with name: " + eventName, this);
    }
}