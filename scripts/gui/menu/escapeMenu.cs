using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class escapeMenu : MonoBehaviour
{
    bool vis = false;
    [SerializeField] int type = 0;
    [SerializeField] CanvasGroup canvas;

    [SerializeField] GameObject resum;
    [SerializeField] GameObject menu;
    [SerializeField] GameObject save;
    [SerializeField] GameObject quit;

    void Start()
    {
        LoadClickEvent();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) { vis = !vis; }
        infoUpdate.SetCanvasAlpha(canvas, Mathf.Clamp((canvas.alpha * 100) + (vis ? 1 : -1), 0, 100));
        Time.timeScale = vis ? 0 : 1;
    }
    public void LoadClickEvent()
    {
        resum.GetComponent<Button>().onClick.AddListener(delegate { vis = false; });
        menu.GetComponent<Button>().onClick.AddListener(delegate { Save(); SceneManager.LoadScene("menu"); });
        save.GetComponent<Button>().onClick.AddListener(delegate { Save(); });
        quit.GetComponent<Button>().onClick.AddListener(delegate { Save(); Application.Quit(); });
    }
    void Save()
    {
        _Save Save = GetComponent<_Save>();
        Save.SaveConv();
    }
}