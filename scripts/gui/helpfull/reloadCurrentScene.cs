using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class reloadCurrentScene : MonoBehaviour
{
    public string sceneName;
    void Update()
    {
        if (Input.GetKey(KeyCode.R) && Input.GetKey(KeyCode.LeftControl))
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}