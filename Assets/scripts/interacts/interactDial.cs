using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

using Microsoft.Win32;
using UnityEngine;

public class interactDial : MonoBehaviour
{
    [SerializeField] bool variablesConnect = false;
    public string[] displayText = { "<d=2;> Name <d=1;> Text" , "[endD]"};

    private void Update()
    {
        bool aktive = _Variables.GetAktive(gameObject);
        GameObject plr = _Variables.GetGameObjectVariable(gameObject, "plr");
        if (aktive == true)
        {
            plrMain plrMain = plr.GetComponent<plrMain>();
            customDialoges dial = plr.GetComponent<customDialoges>();
            offInter(plrMain);

            dial = plr.GetComponent<customDialoges>();

            if (variablesConnect) dial.LoadDialogue(gameObject, "text0", true);
            else dial.LoadDialogue(gameObject, displayText, true);
        }
    }

    void offInter(plrMain scr)
    {
        _Variables.SetVariable(gameObject, "aktive", false);
        scr.inter = null;
    }
}