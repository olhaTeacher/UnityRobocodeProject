using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class itemGet : MonoBehaviour
{
    [SerializeField] int item = 0;
    [SerializeField] bool sameItemTest = false;
    [SerializeField]
    string[] text =
    {
        "<d=2; d=r;> You <d=1; d=r;> I already have this item",
        "[endD]"
    };
    private void Update()
    {
        bool aktive = _Variables.GetAktive(gameObject);
        GameObject plr = _Variables.GetGameObjectVariable(gameObject, "plr");
        if (aktive == true)
        {
            plrMain plrMain = plr.GetComponent<plrMain>();
            inventory inventory = plr.GetComponent<inventory>();
            customDialoges dial = plr.GetComponent<customDialoges>();
            offInter(plrMain);

            if (sameItemTest && inventory.hasItem(item))
            {
                dial.LoadDialogue(gameObject, text, true); 
                return; 
            }
            inventory.addItem(item);
            inventory.choicedItem = item;
            if (item == 2) _Variables.SetVariableApp("boxIndex", Random.Range(1, 50));
        }
    }

    void offInter(plrMain scr)
    {
        _Variables.SetVariable(gameObject, "aktive", false);
        scr.inter = null;
    }
}
