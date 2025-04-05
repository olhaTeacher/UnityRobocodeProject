using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class teleporter : MonoBehaviour
{
    public bool sceneTP = false;
    public string scene;
    public Vector2 pos = Vector2.zero;
    private void Update()
    {
        // interact params
        bool aktive = _Variables.GetAktive(gameObject);
        GameObject plr = _Variables.GetGameObjectVariable(gameObject,"plr");

        if ( aktive )
        {
            _Variables.SetVariable(gameObject, "aktive", false);

            plr.GetComponent<infoUpdate>().displayChange(false, false);
            StartCoroutine(interf(plr));
        }
    }
    IEnumerator interf(GameObject plr)
    {
        plrMain main = plr.GetComponent<plrMain>(); // geting player scripts
        infoUpdate info = plr.GetComponent<infoUpdate>();

        main.canMove = false; // stop player movement
        StartCoroutine(info.displayChange(true, false)); // display dark screen
        yield return new WaitForSeconds(1f);

        if (sceneTP) plr.transform.position = pos; // teleport
        else {
            int conservation = _Variables.GetSaveConservation();

            _Variables.SetVariableApp("pX", pos.x); // save player position
            _Variables.SetVariableApp("pY", pos.y);
            plr.GetComponent<_Save>().GameSave(); // save player stats

            SceneManager.LoadScene(scene);
        }

        yield return new WaitForSeconds(0f);
        StartCoroutine(info.displayChange(false, false)); // hide dark screen
        main.canMove = true; // allow player movement
    }
}