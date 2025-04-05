using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class doors : MonoBehaviour
{
    GameObject[] doorZones;

    List<string[]> dials = new List<string[]>
    {
        new string[] { "<d=2; d=r;> Costumer <d=1; d=r;> Thanks!", "[endD]" },
        new string[] { "<d=2; d=r;> Costumer <d=1; d=r;> mhm..", "[endD]" },
        new string[] { "<d=2; d=r;> Costumer <d=1; d=r;> Yeah! ", "Thanks a lot", "[endD]" },
        new string[] { "<d=2; d=r;> Costumer <d=1; d=r;>  Oh, awesome! Thanks!", "[endD]" },
        new string[] { "<d=2; d=r;> Costumer <d=1; d=r;>  Cheers!", "[endD]" },
        new string[] { "<d=2; d=r;> Costumer <d=1; d=r;>  Oh wow! That was fast! Thanks!", "[endD]" },
        new string[] { "<d=2; d=r;> Costumer <d=1; d=r;>  Finally! Thanks a million!", "[endD]" },
        new string[] { "<d=2; d=r;> Costumer <d=1; d=r;>  Got it. Thanks!", "[endD]" },
        new string[] { "<d=2; d=r;> Costumer <d=1; d=r;>  Thank you. Have a nice day", "[endD]" },
        new string[] { "<d=2; d=r;> Costumer <d=1; d=r;>  Everything looks good. Appreciate it", "[endD]" },
        new string[] { "<d=2; d=r;> Costumer <d=1; d=r;>  Received. Thank you", "[endD]" },
        new string[] { "<d=2; d=r;> Costumer <d=1; d=r;>  Thanks for the delivery", "[endD]" },
        new string[] { "<d=2; d=r;> Costumer <d=1; d=r;>  You just made my whole week!", "[endD]" },
        new string[] { "<d=2; d=r;> Costumer <d=1; d=r;>  Sweet! Just in time for world domination!", "[endD]" },
        new string[] { "<d=2; d=r;> Costumer <d=1; d=r;>  Behold! The package of destiny!", "[endD]" },
        new string[] { "<d=2; d=r;> Costumer <d=1; d=r;>  Mission accomplished, courier!", "[endD]" },
        new string[] { "<d=2; d=r;> Costumer <d=1; d=r;>  Hallelujah! It’s here!", "[endD]" },
        new string[] { "<d=2; d=r;> Costumer <d=1; d=r;>  Great, let’s see if it’s actually not broken this time", "[endD]" },
        new string[] { "<d=2; d=r;> Costumer <d=1; d=r;>  I should’ve delivered it myself, would’ve been faster", "[endD]" },
        new string[] { "<d=2; d=r;> Costumer <d=1; d=r;>  Took so long I almost forgot I ordered this", "[endD]" },
        new string[] { "<d=2; d=r;> Costumer <d=1; d=r;>  Finally! Took you long enough!", "[endD]" },
        new string[] { "<d=2; d=r;> Costumer <d=1; d=r;>  I hope the package is in one piece after that journey", "[endD]" },
        new string[] { "<d=2; d=r;> Costumer <d=1; d=r;>  Thanks! Take care!", "[endD]" },
        new string[] { "<d=2; d=r;> Costumer <d=1; d=r;>  Oh wow, you finally made it. Should I throw a party?", "[endD]" },
        new string[] { "<d=2; d=r;> Costumer <d=1; d=r;>  Hope you didn’t kick this package around for fun", "[endD]" },
        new string[] { "<d=2; d=r;> Costumer <d=1; d=r;>  Let me guess, this thing’s already broken, right?", "<d=2; d=r;> You <d=1; d=r;> no..", "<d=2; d=r;> Costumer <d=1; d=r;> Why did it take so long then?", "[endD]" },
        new string[] { "<d=2; d=r;> Costumer <d=1; d=r;>  Did you deliver this by crawling on your hands and knees?", "[endD]" },
        new string[] { "<d=2; d=r;> Costumer <d=1; d=r;>  Is it really that hard to be on time for once?", "[endD]" },
        new string[] { "<d=2; d=r;> Costumer <d=1; d=r;>  I hope this isn’t as messed up as your timing", "[endD]" },
        new string[] { "<d=2; d=r;> Costumer <d=1; d=r;>  Did you have to fight a dragon on the way or what?", "[endD]" },
        new string[] { "<d=2; d=r;> Costumer <d=1; d=r;>  ...", "[endD]" },
    };
    void Update()
    {
        bool aktive = _Variables.GetAktive(gameObject);
        GameObject plr = _Variables.GetGameObjectVariable(gameObject, "plr");
        if (aktive == true)
        {
            plrMain plrMain = plr.GetComponent<plrMain>();
            inventory inventory = plr.GetComponent<inventory>();
            customDialoges dial = plr.GetComponent<customDialoges>();
            offInter(plrMain);

            if (inventory.hasItem(2) && plrMain.doorZone == Mathf.FloorToInt(_Variables.GetFloatVariableApp("boxIndex")-1) )
            {
                dial.LoadDialogue(gameObject, dials[Random.Range(0, dials.Count - 1)], true);
                inventory.dropItem(inventory.findItem(2));
                inventory.choicedItem = 0;
                plrMain.money += Random.Range(10, 30);
                _Variables.SetVariableApp("boxIndex", 0); 
            }
        }
    }
    void offInter(plrMain scr)
    {
        _Variables.SetVariable(gameObject, "aktive", false);
        scr.inter = null;
    }
}
