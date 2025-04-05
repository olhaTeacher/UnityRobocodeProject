using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class customDialoges : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] Text dialogueText;
    [SerializeField] Text speakerNameText;
    [SerializeField] Image dialogueBackground;
    [SerializeField] Image speakerImg1, speakerImg2;

    [Header("Settings")]
    float textSpeed = 0.05f; // delay between symbols
    bool autoValues = true; // global value, who set all values automaticly

    [Header("-[for scripting]-")]
    [Header("plrScripts")]
    [SerializeField] plrMain plrMain;

    [Header("main")]
    bool isActive = false;
    bool isDisplayingText = false;
    int position = 0;

    [Header("skip`s")]
    bool canSkip = false;
    bool skiped = false;

    [Header("choices")]
    int choice = 1; // selected answer
    int choiceNum = 1; // choice number
    int choiceM = 0; // max
    int choiced = 0; // choiced answer
    bool choicing = false;
    [SerializeField] Text[] choiceTexts;
    [SerializeField] Image Arrow;
    [SerializeField] Image skipArrow;
    [SerializeField] Sprite[] SpeakerImgs;

    string[] textMassive;
    GameObject dialObject;
    int savePos = 0;

    private bool arrowDirect = false;
    private bool arrowDirect2 = false;

    // -------------------------------------------------------- BASE FUNTIONS --------------------------------------------------------
    private void Start()
    {
        plrMain = GetComponent<plrMain>();
        InvokeRepeating("arrowDirectUpd", 0f, 0.3f);
    }
    private void Update()
    {
        if (isActive && Input.GetKeyDown(KeyCode.E) && !choicing)
        {
            skiped = canSkip;
            if (autoValues) { position = canSkip ? position + 1 : position; skiped = false; canSkip = false; }
        }

        if (choicing)
        {
            if (Input.GetKeyDown(KeyCode.W)) choice = math.clamp(choice - 1, 1, choiceM);
            else if (Input.GetKeyDown(KeyCode.S)) choice = math.clamp(choice + 1, 1, choiceM);
            else if (Input.GetKeyDown(KeyCode.E)) ChoiceResult();
            Vector2 newPos = new Vector2(arrowDirect ? 0.5f : 0f, (choice - 2) * -12);

            Arrow.gameObject.transform.localPosition = Vector3.Lerp(Arrow.gameObject.transform.localPosition,newPos, Time.deltaTime * 8);
        }
        Vector2 newPos2 = new Vector2(0, arrowDirect2 ? -42 : -39);
        skipArrow.gameObject.transform.localPosition = Vector3.Lerp(skipArrow.gameObject.transform.localPosition, newPos2, Time.deltaTime * 8);
        skipArrow.gameObject.SetActive(canSkip);

        if (textMassive != null && position < textMassive.Length)
        {
            if (position > textMassive.Length) { EndDialogue(); }
            if (savePos != position) { savePos = position; DisplayText(textMassive[position]); }
        }

        plrMain.canMove = !isActive;
        UpdateDialogueUI();
    }
    private void arrowDirectUpd() { arrowDirect = !arrowDirect; arrowDirect2 = !arrowDirect2; } // updating arrow direction

    // -------------------------------------------------------- DIALOGUE FUNTIONS --------------------------------------------------------
    void UpdateDialogueUI() // updating dialogue UI
    {
        float speedImg = 10f;
        if (isActive)
        {
            dialogueBackground.gameObject.SetActive(true);
            dialogueBackground.transform.localScale = Vector3.Lerp(dialogueBackground.transform.localScale, new Vector3(5f, 1.5f, 1f), Time.deltaTime * speedImg);
        }
        else
        {
            dialogueBackground.transform.localScale = Vector3.Lerp(dialogueBackground.transform.localScale, Vector3.zero, Time.deltaTime * speedImg * 2);
            if (dialogueBackground.transform.localScale.magnitude <= 0) dialogueBackground.gameObject.SetActive(false);
        }
    }
    void EndDialogue() // end dialogue
    {
        isActive = false;
        dialogueBackground.gameObject.SetActive(false);
        position = 0;
        savePos = 99;
        textMassive = null;
    }
    public void StartDialoge()
    {
        isActive = true;
        canSkip = true;
        choicing = false;
        skiped = true;
    }
    public void LoadDialogue(GameObject obj, object texts, bool reload = false)
    {
        if (reload) { StartDialoge(); ResetAll(); }
        if (autoValues) EndDialogue();

        if (texts is string[]) { textMassive = (string[])texts; dialObject = obj; return; }
        else if (texts is string && _Variables.HasVariable(obj, (string)texts)) 
        {
            List<string> text = (List<string>)_Variables.GetVariable(obj, (string)texts);
            textMassive = text.ToArray();
            dialObject = obj;
        }
    }

    // -------------------------------------------------------- CHOICE FUNTIONS --------------------------------------------------------
    void displayChoice() // display selected choices
    {
        int choiceLength = 0;
        foreach (Text item in choiceTexts) if (item.text != "") { choiceLength++; item.gameObject.SetActive(true); } else item.gameObject.SetActive(false) ; // auto length find

        if (autoValues) { choicing = true; choiceM = choiceLength; }
        Arrow.gameObject.SetActive(true);
    }
    void loadChoice(string[] choice) // load new choice options and display it ||| not using
    {
        ResetChoice();
        if (choice.Length <= choiceTexts.Length)
        {
            for (int i = 0; i < choiceTexts.Length - (choiceTexts.Length - choice.Length); i++)
            {
                if (!string.IsNullOrEmpty(choice[i])) choiceTexts[i].text = choice[i];
            }
        }
        displayChoice();
    }
    void ChoiceResult()
    {
        choiceNum++;
        choiced = choice; 
        if (autoValues) { choicing = false; skiped = true; }
        endChoice();
        LoadDialogue(dialObject, $"text{choiceNum}_{choiced}");
        //Debug.Log($"text{choiceNum}_{choiced}");
    }
    void endChoice() // ending choice
    {
        for (int i = 0; i < choiceTexts.Length; i++)
        {
            choiceTexts[i].text = "";
            choiceTexts[i].gameObject.SetActive(false);
        }
        Arrow.gameObject.SetActive(false);
        choicing = false;
    }
    void ResetChoice() // reseting choice parmeters
    {
        endChoice();
        choice = 1;
        choiced = 0;
        choiceM = 0;
        choiceNum = 0;
    }

    // -------------------------------------------------------- TEXT FUNTIONS --------------------------------------------------------
    IEnumerator textLoad(string text) // text load
    {
        string initialText = dialogueText.text;
        string fullText = text;

        int settingsMode = 0;
        object[] parrams = {
        false, // contionue
        dialogueText, // outputText
    };

        string newParam = "";
        string task = "";

        Text[] texts = {
        dialogueText,
        speakerNameText,
    };

        if (autoValues) { isActive = true; isDisplayingText = true; canSkip = false; }

        for (int i = 0; i < text.Length; i++)
        {
            Text outputText = (Text)parrams[1];
            char symbol = text[i];
            if (settingsMode == 0)
            {
                if (symbol != '<' && symbol != '[')
                {
                    outputText.text += symbol;
                    yield return new WaitForSeconds(textSpeed);
                }
                else
                {
                    settingsMode = symbol == '<' ? 1 : 2;
                }
            }
            else
            {
                if (symbol != '>' && symbol != ']')
                {
                    if (settingsMode == 1) // markup | manager
                    {
                        if (symbol != ';')
                        {
                            newParam += symbol;
                            //Debug.Log(newParam);
                        }
                        else
                        {
                            object[] parsedParam = ParseString(newParam); // [0] - parameter symbol, [1] - new parameter
                            char paramType = (char)parsedParam[0];
                            object paramCase = parsedParam[1];
                            object paramStr = paramCase.ToString();
                            //Debug.Log(paramCase);
                            //Debug.Log(paramStr);
                            //Debug.Log(parsedParam);

                            int index = 0;

                            switch (paramType)
                            {
                                //case 'c': // Continue - [Continue]
                                //    bool continueText = (char)paramCase == 't';
                                //    parrams[0] = continueText;
                                //    break;

                                case 'i': // Speaker image sprite - [Image]
                                    index = Mathf.FloorToInt((float)paramCase);
                                    changeImage(index < 0, SpeakerImgs[Mathf.Abs(index) - 1], true); // i= -2 >>> speaker image 2 is changed to SpeakerImgs[1] ||| - или + это 1-ая или 2 картинка, число это идекс картинки
                                    break;
                                case 'v': // Image visible status - [Visible]
                                    index = Mathf.FloorToInt((float)paramCase);
                                    changeImage(Mathf.Abs(index)>=2, SpeakerImgs[0], index > 0, false); // i= 1 >>> speaker image 1 visible is changed to true ||| - или + это видимость картинки, если число 1 то есто 1-ая картинка если больше то 2-ая
                                    break;
                                case 'd': // Dialogue output - [Dialoge]
                                case 's': // Choice output - [Selection]
                                    if ((string)paramStr == "r") { ResetText(outputText); break; }

                                    index = Mathf.FloorToInt((float)paramCase);
                                    parrams[1] = paramType == 'd'
                                        ? texts[Mathf.Clamp(index - 1, 0, texts.Length - 1)]
                                        : choiceTexts[Mathf.Clamp(index - 1, 0, choiceTexts.Length - 1)];

                                    ((Text)parrams[1]).text = (bool)parrams[0] ? outputText.text : "";
                                    break;

                                //case 'w': // Skip text delay - [Wait]
                                //    textSpeed = (char)paramCase == 't' ? 0 : 0.05f;
                                //    break;
                            }
                            newParam = "";
                        }
                    }
                    else if (settingsMode == 2) task += symbol; // task save

                    //outputText.text = (bool)parrams[0] ? outputText.text : "";
                }
                else
                {
                    switch (task) // task load | manager
                    {
                        // reset
                        case "resT": ResetText(outputText); break;
                        case "resA": ResetAll(); break;
                        case "resC": ResetChoice(); break;
                        // end
                        case "endD": EndDialogue(); break;
                        case "endC": endChoice(); break;
                        // display
                        case "dspC": displayChoice(); break;
                    }


                    settingsMode = 0;
                }
            }
        }

        if (autoValues) { isDisplayingText = false; canSkip = true; }
    }

    void DisplayText(string text) // easy "textLoad" start
    {
        StartCoroutine(textLoad(text));
    }
    void ResetText(Text text) // reseting input text
    {
        text.text = "";
    }

    // -------------------------------------------------------- HELPING FUNTIONS --------------------------------------------------------
    void changeImage(bool side, Sprite sprt, bool visible, bool sprtChange = true) // change speaker image
    {
        Image img = !side ? speakerImg1 : speakerImg2;
        if (sprtChange) { img.GetComponent<Image>().sprite = sprt; img.gameObject.SetActive(visible); }
        else
        {
            float color = visible ? 1 : 0.4f;
            float alpha = visible ? 1 : 0.8f;
            img.color = new Color(color, color, color, alpha);

            img.canvasRenderer.SetAlpha(alpha);
            img.CrossFadeAlpha(alpha, 0.1f, false);
        }
    }
    public void ResetAll() // reseting all setings
    {
        ResetText(dialogueText);
        ResetText(speakerNameText);
        ResetChoice();
        changeImage(false,null,false);
        changeImage(true, null, false);
        position = 0;
    }
    static object[] ParseString(string input) // return a string splited into two parts || input - "a:1" or input - "a:t" >>> output - {'a',1} or {'a','t'}
    {
        input = input.Trim(); // delete spaces
        string[] parts = input.Split('='); // split a string into two parts

        if (parts.Length != 2 || parts[0].Length != 1)
            throw new ArgumentException($"Invalid string format: {input}");

        char first = parts[0][0];

        if (float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float number)) return new object[] { first, number }; // if second is number return float
        else if (parts[1].Length == 1) return new object[] { first, parts[1][0] }; // if second part is character return char
        else throw new ArgumentException($"Invalid second part format: {input}");
    }
}