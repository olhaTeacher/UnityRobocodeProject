using System;
using Unity.VisualScripting;
using UnityEngine;

public class _Variables : MonoBehaviour
{
    // ----------------------------------------------------------------- OBJECT ----------------------------------------------------------------- 

    // -------------------------------------------------------- GET --------------------------------------------------------
    public static object GetVariable(GameObject obj, string variableName)
    {
        return Variables.Object(obj).Get(variableName) ?? false; // Return false if the variable is not found
    }

    // --------------- TYPE GET ---------------
    public static string GetStringVariable(GameObject obj, string variableName) // --- string ---
    {
        object var = Variables.Object(obj).Get(variableName);
        return var?.ToString() ?? string.Empty; // Return an empty string if the variable is null
    }
    public static float GetFloatVariable(GameObject obj, string variableName)  // --- float ---
    {
        object var = Variables.Object(obj).Get(variableName);
        return var != null ? Convert.ToSingle(var) : 0f; // Return 0 if the variable is null
    }
    public static bool GetBoolVariable(GameObject obj, string variableName)  // --- bool ---
    {
        object var = Variables.Object(obj).Get(variableName);
        return var switch
        {
            bool boolValue => boolValue,                        // Return the boolean value if it's already a bool
            string strValue => bool.TryParse(strValue, out bool parsed) && parsed, // Convert "true"/"false" strings to boolean
            int intValue => intValue != 0,                      // Treat any non-zero number as true
            _ => false                                           // Default to false for other types
        };
    }
    public static GameObject GetGameObjectVariable(GameObject obj, string variableName)  // --- game object ---
    {
        return Variables.Object(obj).Get(variableName) as GameObject;
    }

    // --------------- GET AKTIVE ---------------

    // Get the "aktive" variable, used for interaction
    public static bool GetAktive(GameObject obj) // --- bool ---
    {
        return GetBoolVariable(obj, "aktive");
    }

    // -------------------------------------------------------- SET --------------------------------------------------------

    public static void SetVariable(GameObject obj, string variableName, object value)
    {
        Variables.Object(obj).Set(variableName, value);
    }

    // -------------------------------------------------------- TEST --------------------------------------------------------

    public static bool HasVriablesCommponent(GameObject obj)  // --- bool ---
    {
        return obj.TryGetComponent<Variables>(out Variables vars);
    }

    public static bool HasVariable(GameObject obj, string variableName)  // --- bool ---
    {
        bool var = Variables.Object(obj).IsDefined(variableName);
        return var;
    }

    // ----------------------------------------------------------------- APPLICATION ----------------------------------------------------------------- 

    // -------------------------------------------------------- GET --------------------------------------------------------
    public static object GetVariableApp(string variableName)
    {
        if (!HasVariableApp(variableName)) return null;
        return Variables.Application.Get(variableName) ?? false; // Return false if the variable is not found
    }

    // --------------- TYPE GET ---------------
    public static string GetStringVariableApp(string variableName) // --- string ---
    {
        if (!HasVariableApp(variableName)) return "";
        object var = Variables.Application.Get(variableName);
        return var?.ToString() ?? string.Empty; // Return an empty string if the variable is null
    }
    public static float GetFloatVariableApp(string variableName)  // --- float ---
    {
        if (!HasVariableApp(variableName)) return 0f;
        object var = Variables.Application.Get(variableName);
        return var != null ? Convert.ToSingle(var) : 0f; // Return 0 if the variable is null
    }
    public static bool GetBoolVariableApp(string variableName)  // --- bool ---
    {
        if (!HasVariableApp(variableName)) return false;
        object var = Variables.Application.Get(variableName);
        return var switch
        {
            bool boolValue => boolValue,                        // Return the boolean value if it's already a bool
            string strValue => bool.TryParse(strValue, out bool parsed) && parsed, // Convert "true"/"false" strings to boolean
            int intValue => intValue != 0,                      // Treat any non-zero number as true
            _ => false                                           // Default to false for other types
        };
    }
    public static GameObject GetGameObjectVariableApp(string variableName)  // --- game object ---
    {
        if (!HasVariableApp(variableName)) return null;
        return Variables.Application.Get(variableName) as GameObject;
    }

    // --------------- GET SAVE ---------------
    public static int GetSaveConservation()  // --- float ---
    {
        int var = Mathf.FloorToInt(GetFloatVariableApp("conv"));
        return var;
    }

    // -------------------------------------------------------- SET --------------------------------------------------------

    public static void SetVariableApp(string variableName, object value)
    {
        Variables.Application.Set(variableName, value);
    }

    // -------------------------------------------------------- TEST --------------------------------------------------------
    public static bool HasVariableApp(string variableName)  // --- bool ---
    {
        bool var = Variables.Application.IsDefined(variableName);
        return var;
    }
}