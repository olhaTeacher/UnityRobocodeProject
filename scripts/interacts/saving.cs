using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class saving : MonoBehaviour
{
    string[] text = {
        "<d=2; d=r;> * <d=1; d=r;> progres saved...",
        "[endD]"
    };
    void Update()
    {
        bool aktive = _Variables.GetAktive(gameObject);
        GameObject plr = _Variables.GetGameObjectVariable(gameObject, "plr");
        
        if (aktive == true)
        {
            plrMain plrMain = plr.GetComponent<plrMain>();
            _Save scr = plr.GetComponent<_Save>();
            customDialoges dial = plr.GetComponent<customDialoges>();
            offInter(plrMain);

            scr.SaveConv();
            dial.LoadDialogue(gameObject, text, true);
        }
    }
    void offInter(plrMain scr)
    {
        _Variables.SetVariable(gameObject, "aktive", false);
        scr.inter = null;
    }
}