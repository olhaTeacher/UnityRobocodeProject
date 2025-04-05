using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class saves : MonoBehaviour
{
    [Header("info")]
    [SerializeField] Text title;
    [SerializeField] Text stats;
    [SerializeField] int saveNum = 0;

    [Header("buttons")]
    [SerializeField] GameObject start;
    [SerializeField] Text startText;
    [SerializeField] GameObject reset;
    [SerializeField] Text resetText;
    void Start()
    {
        LoadInfo();
        LoadClickEvent();
    }

    void LoadInfo()
    {
        title.text = "Save " + saveNum;
        Debug.Log(_Save.LoadInt("line1_" + saveNum));
        if (_Save.LoadInt("line1_" + saveNum) > 0) 
        {
            stats.text = "lvl: " +
            _Save.LoadInt("level" + saveNum) +
            " | Money: " +
            _Save.LoadFloat("money" + saveNum) +
            "/40.000";

            startText.text = "Load";

            resetText.color = Color.white;
            reset.GetComponent<Button>().interactable = true;
        } else
        {
            stats.text = "lvl: - | Money: -/40.000";

            startText.text = "Start";

            resetText.color = Color.grey;
            reset.GetComponent<Button>().interactable = false;
        }
    }
    void LoadClickEvent()
    {
        start.GetComponent<Button>().onClick.AddListener(delegate
        {
            _Variables.SetVariableApp("conv", saveNum);
            if (_Save.LoadString("scene" + saveNum) == string.Empty) _Save.ResetConv();
            if (_Save.LoadFloat("health" + saveNum) <= 0) _Save.ResetConv();
            SceneManager.LoadScene(
                _Save.LoadString("scene" + saveNum));
        });
        reset.GetComponent<Button>().onClick.AddListener(delegate
        {
            _Variables.SetVariableApp("conv", saveNum);
            _Save.ResetConv();
            LoadInfo();
        });
    }
}