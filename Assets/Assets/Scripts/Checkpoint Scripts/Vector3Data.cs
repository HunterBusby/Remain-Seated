
using UnityEngine;
[CreateAssetMenu(fileName = "Vector3Data", menuName = "Checkpoints/Vector3 Data SO")]
public class Vector3Data : ScriptableObject
{
    public Vector3 value;
    public bool doRotation = false;
    public Vector3 valueRotation;
    public bool doDEBUG = false;
    public void UpdateValueVector3Data(Vector3Data newData)
    {
        value = newData.value;
        if (!doRotation)
        {
            return;
        }
        valueRotation = newData.valueRotation;
    }

    public void UpdateValueTransform(Transform transformobj)
    {
        value = transformobj.position;
        if (!doRotation)
        {
            return;
        }
        valueRotation = transformobj.eulerAngles;
    }
    public void UpdateValueV3(Vector3 obj)
    {
        value = obj;
    }

    public void addValueV3(Vector3 obj)
    {
        value += obj;
    }
    public void moveObjecttoValue(GameObject obj){
        obj.transform.position = value;
        if (doDEBUG)
        {
            Debug.Log(obj.name + " OBJ moved to " + obj.transform.position);

        }
        if (!doRotation)
        {
            return;
        }

        obj.transform.rotation = Quaternion.Euler(valueRotation);

    }

    public void moveCharactertoPoint(GameObject obj)
    {   
        //Special method required for character controllers as they override obj position. 
        obj.GetComponent<CharacterController>().enabled = false;
        obj.transform.position = value;
        
        obj.GetComponent<CharacterController>().enabled = true;
        if (doDEBUG)
        {
            Debug.Log(obj.name + " CHARACTER moved to " + obj.transform.position);

        }
        if (!doRotation)
        {
            return;
        }

        obj.transform.rotation = Quaternion.Euler(valueRotation);
    }
    
}
