using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "GameAction", menuName = "Checkpoints/Generic GameAction")]

public class GameAction : ScriptableObject
{
    public UnityAction raise;
    public void RaiseAction(){
        raise?.Invoke();
        //Debug.Log("Action Raised - Updating Score?");
       }
    
}
