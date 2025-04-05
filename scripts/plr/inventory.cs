using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class inventory : MonoBehaviour
{
    [Header("elements")]
    public string[] names;
    public Sprite[] images;
    public string[] info;
    [SerializeField] Image[] slots = {};
    [SerializeField] Image img;

    [Header("settings")]
    public int[] items = { };
    public int maxItems = 10, choicedItem = 0;
    //int[] equipedItems = { };

    [Header("UIElements")]
    [SerializeField] bool inInventroy = true;
    [SerializeField] int choicePos = 0;
    [SerializeField] Image mainPanel;
    bool arrowDirect = false;

    [Header("ItemInfoUI")]
    [SerializeField] Image itemImage;
    [SerializeField] Text itemName;
    [SerializeField] Text itemInfo;

    private void Start()
    {
        InvokeRepeating("arrowDirectUpd", 0f, 0.3f);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            inInventroy = !inInventroy;
            StartCoroutine(FadePanel(mainPanel.GetComponent<CanvasGroup>(), inInventroy));
        }
        if (inInventroy)
        {
            if (Input.GetKeyDown(KeyCode.W)) choicePos--;
            else if (Input.GetKeyDown(KeyCode.S)) choicePos++;
            else if (Input.GetKeyDown(KeyCode.D)) choicePos += 5;
            else if (Input.GetKeyDown(KeyCode.A)) choicePos -= 5;
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                choicedItem = items[choicePos - 1]; 
                inInventroy = false; 
                StartCoroutine(FadePanel(mainPanel.GetComponent<CanvasGroup>(), inInventroy));
            }

            choicePos = Mathf.Clamp(choicePos, 1, aktiveItems());
            img.gameObject.SetActive(aktiveItems() > 0);
            arrowUpd();
            slotsUpd();
            if (aktiveItems() > 0) itemInfoUpd();
        }
    }
    int aktiveItems()
    {
        int count = 0;

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != 0) count++;
            else break;
        }

        return count;
    }

    void moveItem(int slot, int newSlot)
    {
        if (slot < 0 || slot >= items.Length || newSlot < 0 || newSlot >= items.Length) return;

        items[newSlot] = slot;
        items[slot] = 0;
    }

    // private
    private void arrowUpd()
    {
        int toFive = choicePos > 5 ? choicePos - 5 : choicePos;
        int x = (choicePos > 5 ? 70 : -60) + 48 + (arrowDirect ? 4 : 0);
        int y = 40 - (22 * math.clamp((toFive - 1), 0, 4));
        Vector2 newPos = new Vector2(x, y);

        img.transform.localPosition = Vector3.Lerp(img.transform.localPosition, newPos, Time.deltaTime * 8);
    }
    private void itemInfoUpd()
    {
        int num = items[Mathf.Clamp(choicePos - 1, 0, items.Length)];
        Sprite img = (Sprite)getElements(num, "img");
        string[] text = (string[])getElements(num, "text");
        itemImage.sprite = img;
        itemName.text = text[0];
        itemInfo.text = text[1];
    }
    private void arrowDirectUpd() { arrowDirect = !arrowDirect; }
    private void slotsUpd()
    {
        if (items == null || slots == null || items.Length == 0 || slots.Length == 0)
        {
            foreach (Image slot in slots)
            {
                if (slot != null)
                {
                    slot.gameObject.SetActive(false);
                }
            }
            return;
        }
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < items.Length && items[i] != 0)
            {
                slots[i].gameObject.SetActive(true);
                CanvasGroup canvasGroup = slots[i].GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = slots[i].gameObject.AddComponent<CanvasGroup>();
                }

                StartCoroutine(FadeInSlot(canvasGroup, true));

                Text name = (Text)Variables.Object(slots[i]).Get("n");
                Image img = (Image)Variables.Object(slots[i]).Get("i");

                if (name != null && img != null && items[i] < names.Length && items[i] < images.Length)
                {
                    name.text = names[items[i]];
                    img.sprite = images[items[i]];
                }
            }
            else
            {

                CanvasGroup canvasGroup = slots[i].GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = slots[i].gameObject.AddComponent<CanvasGroup>();
                }
                StartCoroutine(FadeInSlot(canvasGroup, false));

            }
        }
    }
    private IEnumerator FadeInSlot(CanvasGroup canvasGroup, bool type)
    {
        float alpha = type ? 0f : 1f;
        while (alpha < (type ? 1f : 0f))
        {
            alpha += (Time.deltaTime * 2) * (type ? 1 : -1);
            canvasGroup.alpha = alpha;
            yield return null;
        }
        canvasGroup.alpha = type ? 1f : 0f;
        if (!type && alpha == (type ? 0f : 1f)) canvasGroup.gameObject.SetActive(false);
    }
    private IEnumerator FadePanel(CanvasGroup canvasGroup, bool fadeIn)
    {
        if (canvasGroup == null)
        {
            canvasGroup = mainPanel.gameObject.AddComponent<CanvasGroup>();
        }
        float targetAlpha = fadeIn ? 1f : 0f;
        float currentAlpha = canvasGroup.alpha;
        if (fadeIn) mainPanel.gameObject.SetActive(true);
        while (!Mathf.Approximately(currentAlpha, targetAlpha))
        {
            currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, Time.deltaTime * 2f);
            canvasGroup.alpha = currentAlpha;
            yield return null;
        }
        if (!fadeIn) mainPanel.gameObject.SetActive(false);
    }
    object getElements(int nummer, string type)
    {
        if (nummer >= 0)
        {
            string[] text = { names[nummer], info[nummer] };

            if (type == "text") return text;
            else if (type == "img") return images[nummer];
            else return null;
        }
        else return null;
    }

    // public

    public void resetInvetory()
    {
        for (int i = 0; i < maxItems; i++)
        {
            items[i] = 0;
        }
    }
    public void addItem(int nummer)
    {
        if (nummer != 0)
        {
            if (aktiveItems() < maxItems)
            {
                items[aktiveItems()] = nummer;
                Debug.Log("item: "+names[nummer] + " was added in slot: " + items.Length + 1);
            }
        }
    }
    public void dropItem(int slot)
    {
        items[slot] = 0;
        for (int i = slot; i < maxItems - 1; i++) 
        {
            if (items[i] == 0 && items[i + 1] != 0) moveItem(i + 1, i);
        }
        choicePos = 0;
    }
    public bool hasItem(int necessaryItem)
    {
        if (aktiveItems() > 0)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == necessaryItem) return true;
            }
        }
        return false;
    }
    public int findItem(int necessaryItem)
    {
        if (aktiveItems() > 0)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == necessaryItem) return i;
            }
        }
        return -1;
    }
}