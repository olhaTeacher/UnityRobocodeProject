using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class menuButtons : MonoBehaviour
{
    public int type = 0; 
    private Button button;

    private void Start()
    {
        if (type != 10 && type != 11)
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnButtonClick);
        }
    }
    void OnButtonClick()
    {
        if (type == 1) SceneManager.LoadScene("Saves");
        else if (type == 2) SceneManager.LoadScene("Settings");
        else if (type == 3) SceneManager.LoadScene("About");
        else if (type == 4) Application.Quit();
        else if (type == 6) SceneManager.LoadScene("Menu");
    }
}