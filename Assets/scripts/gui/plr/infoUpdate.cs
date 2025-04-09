using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class infoUpdate : MonoBehaviour
{
    plrMain info;
    inventory invt;
    //gameObject.SetActive
    [Header("health")]
    [SerializeField] Image healthUI;

    [Header("energy")]
    [SerializeField] Image[] energyUI;
    [SerializeField] Sprite[] energyUIIcons;
    int[] energySavesForAnim = { 0, 0, 0, 0 };

    [Header("item")]
    [SerializeField] Sprite noneItemUi;
    [SerializeField] Image choicedItem;

    [Header("freezing")]
    [SerializeField] Image froze;
    [SerializeField] Image freezeBar;
    [SerializeField] Image barFull;
    [SerializeField] Image snowflakeIcon;
    bool freezeUIAnim = false;

    [Header("gps")]
    [SerializeField] Image leftArrow;
    [SerializeField] Image rightArrow;
    [SerializeField] Image posMark;
    public GameObject markedObj = null;

    [Header("coins")]
    [SerializeField] Text coinText;
    [SerializeField] Image coinImage;
    float coinSave = 0;

    [Header("visualIntr")]
    [SerializeField] Image fullDisplay;
    [SerializeField] Image interactImg;
    int interAlpha = 0;

    [Header("lvl")]
    [SerializeField] Text level;

    void Start()
    {
        info = GetComponent<plrMain>();
        invt = GetComponent<inventory>();

        StartCoroutine(displayChange(false, false));
    }
    void Update()
    {
        float healthX = (-0.9f * info.health) + 90;

        healthUI.transform.localScale = new Vector3(info.health / 100, healthUI.transform.localScale.y,1);
        healthUI.transform.localPosition = new Vector2(Mathf.Floor(-healthX), healthUI.transform.localPosition.y);

        energyUpd();
        UpdateGPS();
        UpdateCoins();
        interactDisplay();

        choicedItem.sprite = invt.choicedItem != 0 ? invt.images[invt.choicedItem] : noneItemUi;

        StartCoroutine(freezeVisualize());
        CanvasGroup canvasGroup = froze.GetComponent<CanvasGroup>();
        SetCanvasAlpha(canvasGroup, info.freezed);

        level.text = "lvl:"+info.level.ToString();

        if (_Variables.GetFloatVariableApp("boxIndex") != 0)
        {
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("doorZone"))
            {
                if (Mathf.FloorToInt(_Variables.GetFloatVariableApp("boxIndex") - 1) == _Variables.GetFloatVariable(obj, "num"))
                {
                    markedObj = obj;
                    break;
                }
            }
        } else
        {
            markedObj = null;
        }
    }

    // FREEZE
    IEnumerator freezeVisualize()
    {
        Animator barAnimator = freezeBar.GetComponent<Animator>();
        Animator snowflakeAnimator = snowflakeIcon.GetComponent<Animator>();
        barAnimator.speed = 1.5f;
        snowflakeAnimator.speed = 1.5f;

        if (info.freezed > 0)
        {
            barFull.transform.localScale = new Vector3(info.freezed/100, 1, 1);
            barFull.transform.localPosition = Vector2.zero;
            if (!freezeUIAnim)
            {
                freezeUIAnim = true;
                freezeBar.gameObject.SetActive(freezeUIAnim);
                snowflakeIcon.gameObject.SetActive(freezeUIAnim);
                barAnimator.Play("freezebarREVERSE");
                snowflakeAnimator.Play("snowflakeREVERSE");
            }
        }
        else
        {
            if (freezeUIAnim)
            {
                freezeUIAnim = false;
                StartCoroutine(HideUIAfterAnimation(freezeBar.gameObject, snowflakeIcon.gameObject, barAnimator, snowflakeAnimator));
            }
        }
        yield return new WaitForSeconds(0.1f);
    }

    IEnumerator HideUIAfterAnimation(GameObject freezeBar, GameObject snowflakeIcon, Animator barAnimator, Animator snowflakeAnimator)
    {
        barAnimator.Play("freezebar");
        snowflakeAnimator.Play("snowflake");

        yield return new WaitForSeconds(0.5f);

        if (!freezeUIAnim)
        {
            freezeBar.SetActive(false);
            snowflakeIcon.SetActive(false);
        }
    }
    public static void SetCanvasAlpha(CanvasGroup canvasGroup, float percent)
    {
        percent = Mathf.Clamp(percent, 0f, 100f);
        canvasGroup.alpha = percent / 100f;
        canvasGroup.gameObject.SetActive(percent > 0f);
    }
    // DISPLAY
    public IEnumerator displayChange(bool visible,bool color,float delay = 0f)
    {
        CanvasGroup canvasGroup = fullDisplay.GetComponent<CanvasGroup>();
        fullDisplay.color = color ? new Color(1, 1, 1, 1) : new Color(0, 0, 0, 1);
        if (visible) fullDisplay.gameObject.SetActive(true);

        if (visible) for (int i = 0; i < 255; i++) { SetCanvasAlpha(canvasGroup, i); yield return new WaitForSeconds(delay); }
        else for (int i = 255; i > 0; i--) { SetCanvasAlpha(canvasGroup, i); yield return new WaitForSeconds(delay); }

        if (!visible) fullDisplay.gameObject.SetActive(false);
    }
    public void interactDisplay()
    {
        CanvasGroup canvasGroup = interactImg.GetComponent<CanvasGroup>();
        interAlpha = Mathf.Clamp(interAlpha + (info.inter != null ? 1 : -1), 0, 85);
        SetCanvasAlpha(canvasGroup, interAlpha);
    }

    // ENERGY
    void energyUpd()
    {
        int[] energy = DivideNumber(Mathf.FloorToInt(info.stamina));
        for (int i = 0; i < energy.Length; i++)
        {
            if (energySavesForAnim[i] != Mathf.FloorToInt(energy[i] / 5))
            {
                energySavesForAnim[i] = Mathf.FloorToInt(energy[i] / 5);
                StartCoroutine(AnimateSlot(energyUI[i], Mathf.FloorToInt(energy[i] / 5)));
            }
            if (Mathf.FloorToInt(energy[i] / 5) == 0 && energyUI[i].sprite != energyUIIcons[0]) StartCoroutine(AnimateSlot(energyUI[i], 0));
        }
    }
    IEnumerator AnimateSlot(Image slot,int sprite)
    {
        Vector3 originalPosition = new Vector3(slot.transform.localPosition.x,0, slot.transform.localPosition.z);
        Vector3 offsetPosition = new Vector3(originalPosition.x, -5, originalPosition.z);
        slot.transform.localPosition = offsetPosition;
        if (sprite == 5) slot.sprite = energyUIIcons[6];
        yield return new WaitForSeconds(0.05f);
        slot.transform.localPosition = originalPosition;
        if (sprite == 5) yield return new WaitForSeconds(0.2f);
        slot.sprite = energyUIIcons[sprite];
    }
    // COIN SYSTEM
    void UpdateCoins()
    {
        if (coinSave != info.money)
        {
            coinSave = info.money;
            coinText.text = coinSave.ToString();
            StartCoroutine(coinImageAnim());
        }
    }
    IEnumerator coinImageAnim()
    {
        coinImage.transform.localScale = new Vector3(1.7f, 1.7f, 1);
        yield return new WaitForSeconds(0.1f);
        coinImage.transform.localScale = new Vector3(1.6f, 1.6f, 1);
        yield return new WaitForSeconds(0.1f);
        coinImage.transform.localScale = new Vector3(1.5f, 1.5f, 1);
    }

    // OTHER
    static int[] DivideNumber(int input)
    {
        input = Mathf.Clamp(input,0,100);
        int[] result = new int[4];
        for (int i = 0; i < 3; i++)
        {
            result[i] = Mathf.Min(input, 25);
            input -= result[i];
        }
        result[3] = input;
        return result;
    }
    bool IsAnimationPlaying(Animator animator)
    {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f;
    }

    // GPS
    void UpdateGPS()
    {
        if (markedObj)
        {
            Vector3 screenPoint = Camera.main.WorldToViewportPoint(markedObj.transform.position);
            bool isVisible = screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

            leftArrow.gameObject.SetActive(!isVisible && screenPoint.x < 1); rightArrow.gameObject.SetActive(!isVisible && screenPoint.x > 0);
            posMark.gameObject.SetActive(isVisible);
            if (isVisible) posMark.gameObject.transform.position = new Vector2(screenPoint.x * Screen.width, screenPoint.y * Screen.height);
        } else
        {
            leftArrow.gameObject.SetActive(false); rightArrow.gameObject.SetActive(false);
            posMark.gameObject.SetActive(false);
        }
    }
}