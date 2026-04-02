using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "SimpleBoolData", menuName = "Checkpoints/Simple Bool Data")]
public class SimpleBoolData : ScriptableObject
{
    public bool value;
    
    public UnityEvent OnValueChanged, TrueEvent, FalseEvent;
    
    //change value
    public void MakeTrue()
    {
        OnValueChanged.Invoke();
        TrueEvent.Invoke();
        value = true;
    }

    public void MakeFalse()
    {
        OnValueChanged.Invoke();
        FalseEvent.Invoke();
        value = false;
    }
}
