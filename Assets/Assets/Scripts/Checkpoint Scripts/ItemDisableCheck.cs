using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ItemDisableCheck", menuName = "Checkpoints/Item Disable Check")]

public class ItemDisableCheck : MonoBehaviour
{
    public SimpleBoolData refData;
    // Start is called before the first frame update
    void Start()
    {
        if (refData.value == false)
        {
            this.gameObject.SetActive(false);
        }
    }

}
