using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class DialogManager : MonoBehaviour
{
    [SerializeField] private Story CurrentStory;
    private bool dialogIsPlaying;
    private static DialogManager instance;
    public TextMeshProUGUI DialogText;
    public int characterPerCycle;
    public float speed;
    public UnityEvent OnDialogueEnd;
    private bool IsTextComplete;

    private IEnumerator ExitDialogMode()
    {
        yield return new WaitForSeconds(0f);
        dialogIsPlaying = false;
        // DialogPanel.SetActive(false);
        DialogText.text = "";
        OnDialogueEnd.Invoke();
    }
    void Awake()
    {

        if (instance != null)
        {
            Debug.LogWarning("More than one DialogManager");
        }
        instance = this;
    }
    public static DialogManager GetInstance()
    {
        return instance;
    }
    private void ContinueStory()
    {
        if (CurrentStory.canContinue)
        {
            PrepareText(CurrentStory.Continue());

        }
        else
        {
            StartCoroutine(ExitDialogMode());
        }
    }
    private void PrepareText(string textToDisplay)
    {

        DialogText.color = DialogText.color;
        DialogText.maxVisibleCharacters = 0;
        DialogText.text = textToDisplay;

        DialogText.ForceMeshUpdate();
        StartCoroutine(Typewriter());


    }
    public void EnterDialogMode(TextAsset inkJson)
    {
        CurrentStory = new Story(inkJson.text);
        dialogIsPlaying = true;
        //DialogPanel.SetActive(true);
        ContinueStory();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (DialogText.maxVisibleCharacters == DialogText.textInfo.characterCount)
        {
            IsTextComplete = true;
        }
        else
        {
            IsTextComplete = false;
        }
        if (Input.GetKeyDown(KeyCode.Space) && IsTextComplete)
        {
            ContinueStory();

        }
    }
    private IEnumerator Typewriter()
    {

        while (DialogText.maxVisibleCharacters < DialogText.textInfo.characterCount)
        {

            DialogText.maxVisibleCharacters += characterPerCycle;

            yield return new WaitForSeconds(0.01f / speed);
        }
    }
}
