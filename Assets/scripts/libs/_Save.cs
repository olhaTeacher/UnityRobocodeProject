using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class _Save : MonoBehaviour
{
    bool[] tests =
    {
        false, // reloaded
        false, // reseted
    };
    // -------------------------------------------------------- EASY SAVING --------------------------------------------------------
    void OnEnable()
    {
        if (!_Variables.GetBoolVariableApp("loaded")) { LoadConv(); Debug.Log("save: " + _Variables.GetSaveConservation() + " was loaded"); }
        else GameLoad(); Debug.Log("player loaded");
        _Variables.SetVariableApp("loaded", true);

        //PlayerPrefs.DeleteKey("FirstLaunch"); // delete first launch key
        if (!PlayerPrefs.HasKey("FirstLaunch"))
        {
            PlayerPrefs.SetInt("FirstLaunch", 1);
            for (int i = 0; i < 3; i++)
            {
                _Variables.SetVariableApp("conv", i);
                ResetConv();
            }
        }
    }

    // --------------- DEFAULT ---------------
    public static void Save(string name, object value, char type = 'n') // save your variable, with "auto type getting"
    {
        if ((value is string && type == 'n') || type == 's') PlayerPrefs.SetString(name, value.ToString());
        else if ((value is float && type == 'n') || type == 'f') PlayerPrefs.SetFloat(name, (float)value);
        else if ((value is int && type == 'n') || type == 'i') PlayerPrefs.SetInt(name, (int)value);
        else if ((value is bool && type == 'n') || type == 'b') PlayerPrefs.SetInt(name, (bool)value ? 1 : 0);
    }

    public static object Load(string name, Type type) // returning your variable
    {
        if (type == typeof(string)) return PlayerPrefs.GetString(name, string.Empty);
        if (type == typeof(int)) return PlayerPrefs.GetInt(name, int.MinValue);
        if (type == typeof(float)) return PlayerPrefs.GetFloat(name, float.MinValue);
        return null;
    }
    public static float LoadFloat(string name) { return PlayerPrefs.GetFloat(name, float.NaN); }
    public static int LoadInt(string name) { return PlayerPrefs.GetInt(name, int.MinValue); }
    public static string LoadString(string name) { return PlayerPrefs.GetString(name, string.Empty); }
    public static bool LoadBool(string name) { return PlayerPrefs.GetInt(name, int.MinValue) > 0; }

    // --------------- GAME SAVES ---------------
    public void GameSave()
    {
        int conservation = _Variables.GetSaveConservation();

        GameObject player = gameObject;

        plrMain playerScript = player.GetComponent<plrMain>();
        inventory playerInventory = player.GetComponent<inventory>();

        _Variables.SetVariableApp("level", playerScript.level);
        _Variables.SetVariableApp("money", playerScript.money);
        _Variables.SetVariableApp("freezed", playerScript.freezed);
        _Variables.SetVariableApp("health", playerScript.health);
        _Variables.SetVariableApp("stamina", playerScript.stamina);

        for (int i = 0; i < playerInventory.items.Length; i++) { _Variables.SetVariableApp("slot" + i, playerInventory.items[i]); }
        Save("slotChoiced", playerInventory.choicedItem);
    }
    void GameLoad()
    {
        int conservation = _Variables.GetSaveConservation();

        GameObject player = gameObject;

        plrMain playerScript = player.GetComponent<plrMain>();
        inventory playerInventory = player.GetComponent<inventory>();
        SpriteRenderer playerSprite = player.GetComponent<SpriteRenderer>();

        playerScript.level = Mathf.FloorToInt(_Variables.GetFloatVariableApp("level"));
        playerScript.money = _Variables.GetFloatVariableApp("money");
        playerScript.freezed = _Variables.GetFloatVariableApp("freezed");
        playerScript.health = _Variables.GetFloatVariableApp("health");
        playerScript.stamina = _Variables.GetFloatVariableApp("stamina");

        Vector2 position = new Vector3(
            _Variables.GetFloatVariableApp("pX"),
            _Variables.GetFloatVariableApp("pY"),
            1);
        player.transform.position = position;

        for (int i = 0; i < playerInventory.items.Length; i++) { playerInventory.items[i] = Mathf.FloorToInt(_Variables.GetFloatVariableApp("slot" + i)); }
        playerInventory.choicedItem = Mathf.FloorToInt(_Variables.GetFloatVariableApp("slotChoiced"));
    }

    public static void GameReset()
    {
        int conservation = _Variables.GetSaveConservation();

        _Variables.SetVariableApp("level", 1);
        _Variables.SetVariableApp("money", 0);
        _Variables.SetVariableApp("freezed", 0);
        _Variables.SetVariableApp("health", 100);
        _Variables.SetVariableApp("stamina", 100);

        for (int i = 0; i < 20; i++) { _Variables.SetVariableApp("slot" + i, 0); }
        Save("slotChoiced", 0);
    }

    // --------------- CONVERSATION SAVES ---------------
    public void SaveConv() // saving all player variables
    {
        int conservation = _Variables.GetSaveConservation();

        Debug.Log("saving "+conservation);

        GameObject player = gameObject;//GameObject.FindWithTag("Player");

        plrMain playerScript = GetComponent<plrMain>();
        inventory playerInventory = GetComponent<inventory>();
        SpriteRenderer playerSprite = GetComponent<SpriteRenderer>();

        Save("health" + conservation, playerScript.health, 'f');
        Save("stamina" + conservation, playerScript.stamina, 'f');
        Save("freezed" + conservation, playerScript.freezed, 'f');
        Save("money" + conservation, playerScript.money, 'f');
        Save("level" + conservation, playerScript.level);
        Debug.Log("player saved");

        Save("pX" + conservation, player.transform.position.x, 'f');
        Save("pY" + conservation, player.transform.position.y, 'f');
        Save("rot" + conservation, playerSprite.flipX);
        Save("scene" + conservation, SceneManager.GetActiveScene().name);
        Debug.Log("position saved");

        for (int i = 0; i < playerInventory.items.Length; i++) { Save("slot" + i + "_" + conservation, playerInventory.items[i]);  }
        Save("slotChoiced" + conservation, playerInventory.choicedItem);
        Debug.Log("inventory saved");
    }
    void LoadConv() // loading all player variables
    {
        int conservation = _Variables.GetSaveConservation();

        GameObject player = gameObject;

        plrMain playerScript = GetComponent<plrMain>();
        inventory playerInventory = GetComponent<inventory>();
        SpriteRenderer playerSprite = GetComponent<SpriteRenderer>();

        if (LoadFloat("health") <= 0) ReLoad();
        Save("line1_"+conservation, 1);

        playerScript.health = LoadFloat("health" + conservation);
        playerScript.stamina = LoadFloat("stamina" + conservation);
        playerScript.freezed = LoadFloat("freezed" + conservation);
        playerScript.money = LoadFloat("money" + conservation);
        playerScript.level = LoadInt("level" + conservation);
        Debug.Log("player loaded");

        Vector2 position = new Vector3(
            LoadFloat("pX" + conservation),
            LoadFloat("pY" + conservation),
            1);
        player.transform.position = position;
        playerSprite.flipX = LoadBool("rot" + conservation);
        Debug.Log("position loaded");

        for (int i = 0; i < playerInventory.items.Length; i++) { playerInventory.items[i] = LoadInt("slot" + i + "_" + conservation); }
        playerInventory.choicedItem = LoadInt("slotChoiced" + conservation);
        Debug.Log("inventory loaded");
    }
    public static void ResetConv() // reseting all player variables
    {
        int conservation = _Variables.GetSaveConservation();

        Save("level" + conservation, 1);
        Save("money" + conservation, 0f);
        Save("freezed" + conservation, 0f);
        Save("health" + conservation, 100f);
        Save("stamina" + conservation, 100f);

        Save("pose" + conservation, 0);
        Save("pX" + conservation, 0f);
        Save("pY" + conservation, 0f);
        Save("rot" + conservation, false);
        Save("scene" + conservation, "Home");

        for (int i = 0; i < 20; i++) Save("slot" + i + "_" + conservation, 0);
        Save("slotChoiced" + conservation, 0);

        for (int i = 0; i < 5; i++) Save("line" + i + "_" + conservation, 0);
    }

    IEnumerator ReLoad()
    {
        int conservation = _Variables.GetSaveConservation();
        yield return new WaitForSeconds(1);

        if (!tests[0]) tests[0] = true; 
        else { ResetConv(); tests[1] = true; Debug.Log("save: " + conservation + " was reseted"); }

        LoadConv();
    }
}