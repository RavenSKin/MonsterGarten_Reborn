using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using UnityEngine.Rendering;
using System.Linq;



public class DialogManager : MonoBehaviour
{
    public AudioStructure _audio;
    [Header("CharactersEditor")]
    [Tooltip("Sert à gérer l'highlight  : renseigner les images ")]
    [SerializeField] List<Image> characterImages;
    [SerializeField] Character_Component _characterSpeaking;
    [Tooltip(" Renseigner les ScriptableObjects")]
    [SerializeField] List<Character_Component> _characters;
    [SerializeField] TextEffects _effects;
    [SerializeField] List<Color> fadedColor;



    [Header("Dialogue UI")]

    [SerializeField] private GameObject DialogPanel;
    [SerializeField] private TextMeshProUGUI DialogText;
    [SerializeField] private Image SpeechBubble;


    [Header("Choices UI")]

    [SerializeField] private GameObject[] ArrowChoices;
    [SerializeField] private GameObject[] choices;
    [SerializeField] private TextMeshProUGUI[] choicesText;

    [Header("Dialogue Ended")]
    public UnityEvent OnDialogueEnd;

    [Serializable]
    public struct AudioStructure
    {
        public AudioSource DialogueCue;
        public AudioSource Music;
        public AudioSource SFX;
        public AudioClip[] _clips;
        public bool SFX_ToPlay;
        [HideInInspector] public AudioClip SFX_ClipToPlay;
        [HideInInspector] public AudioClip Music_ClipToPlay;
    }

    #region Liste des tags existant
    private const string PLACEMENT = "Placement";
    private const string SPEAKER_TAG = "Speaker";
    private const string PORTRAIT = "Portrait";
    private const string Emotion = "Image";
    private const string MUSIQUE = "Musique";
    private const string SFX = "SFX";
    private const string TextEffect = "Effect";
    #endregion

    #region UtileAuTexte
    string lettercollapsed;
    List<char> _reformedWord;
    char[] LastWordChar;
    int FirstVertCount;
    int LastVertCount;
    string ShowTextToDisplay;
    string[] valueTag;
    string[] preparedText;
    int firstVert;

    private bool IsTextComplete;
    public bool isShakingText;
    public int characterPerCycle { get { return speed <= 2f ? characterMultiplier : speed <= 2.5 ? characterMultiplier * 2 : characterMultiplier * 3; } }
    float baseSpeed = 1;
    float SpeedMultiplier = 1;
    int characterMultiplier = 1;
    [SerializeField] float speed { get { return baseSpeed * SpeedMultiplier; } set { SpeedMultiplier = value; } }
    [SerializeField] private Story CurrentStory;
    private static DialogManager instance;
    public bool dialogIsPlaying { get; set; }
    #endregion

    void Awake()
    {

        if (instance != null)
        {
            Debug.LogWarning("More than one DialogManager");
        }
        instance = this;
        choicesText = new TextMeshProUGUI[choices.Length];
        int indexA = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[indexA] = choice.GetComponentInChildren<TextMeshProUGUI>();
            indexA++;
        }
        foreach (Image _img in characterImages)
        {
            fadedColor.Add(_img.color);
        }

    }

    public static DialogManager GetInstance()
    {
        return instance;
    }

    private void Update()
    {
        _effects.CanShake = isShakingText;

        if (DialogText.maxVisibleCharacters == DialogText.textInfo.characterCount)
        {
            IsTextComplete = true;

        }
        else
        {
            IsTextComplete = false;

        }
        if (!dialogIsPlaying)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space) && IsTextComplete)
        {
            //StartCoroutine(WaitForAudioExecution(_audio.DialogueCue, _audio.DialogueCue.clip, 0f));
            ContinueStory();
            if (_audio.SFX_ToPlay)
            {
                if (_audio.Music.clip != _audio.Music_ClipToPlay)
                {
                    //StartCoroutine(WaitForAudioExecution(_audio.Music, _audio.Music_ClipToPlay, 0f));
                }
                else
                {
                    StartCoroutine(WaitForAudioExecution(_audio.SFX, _audio.SFX_ClipToPlay, 0f));
                }


                _audio.SFX_ToPlay = false;
            }
            else
            {
                //if (_audio.Music.clip != _audio.Music_ClipToPlay)
                //{
                //    StartCoroutine(WaitForAudioExecution(_audio.Music, _audio.Music_ClipToPlay, 0f));


                //}

            }
        }
    }

    public void EnterDialogMode(TextAsset inkJson)
    {
        CurrentStory = new Story(inkJson.text);
        dialogIsPlaying = true;
        DialogPanel.SetActive(true);
        ContinueStory();
        //_audio.Music.clip = _audio.Music_ClipToPlay;
        //_audio.Music.Play();
        if (_audio.SFX_ToPlay)
        {

            StartCoroutine(WaitForAudioExecution(_audio.SFX, _audio.SFX_ClipToPlay, 0f));

            _audio.SFX_ToPlay = false;
        }

    }

    private IEnumerator ExitDialogMode()
    {
        yield return new WaitForSeconds(0f);
        dialogIsPlaying = false;
        DialogPanel.SetActive(false);
        DialogText.text = "";
        OnDialogueEnd.Invoke();
    }

    private void ContinueStory()
    {
        isShakingText = false;
        if (CurrentStory.canContinue)
        {
            PrepareText(CurrentStory.Continue());

            //DisplayChoice();
            HandleTags(CurrentStory.currentTags);
        }
        else
        {
            StartCoroutine(ExitDialogMode());
        }
    }

    private void DisplayChoice()
    {

        List<Choice> currentChoices = CurrentStory.currentChoices;
        //Debug.Log(currentChoices.Count);
        if (currentChoices.Count > choices.Length)
        {
            Debug.LogWarning("Too much choices that can handle the choice panel, current number of choices = " + currentChoices);
        }
        else
        {

            int index = 0;
            foreach (Choice choice in currentChoices)
            {
                choices[index].gameObject.SetActive(true);
                choicesText[index].text = choice.text;
                //Debug.Log("index = " + index);
                index++;

            }
            for (int i = index; i < choices.Length; i++)
            {
                choices[i].gameObject.SetActive(false);

            }
            for (int i = index; i < ArrowChoices.Length; i++)
            {
                ArrowChoices[i].gameObject.SetActive(false);
            }
            StartCoroutine(SelectFirstChoice());
        }

    }

    void HandleTags(List<string> CurrentTags)
    {
        foreach (string tags in CurrentTags)
        {
            currentduration = 0f;
            string[] splitTags = tags.Split(":");
            if (splitTags.Length != 2)
            {
                Debug.LogError("Tag could not be parsed" + tags);
            }
            string tagKey = splitTags[0].Trim();
            string tagValue = splitTags[1].Trim();


            switch (tagKey)
            {

                case SPEAKER_TAG:
                    if (tagValue == "None")
                    {
                        Debug.Log("Ici : le narrateur/joueur");
                    }

                    else
                    {
                        #region Highlight management
                        string[] splitWord = tagValue.Split(";");
                        string CharacterTag = splitWord[0];
                        foreach (char _tag in tagValue)
                        {
                            //Debug.Log(_tag);
                        }
                        Debug.Log(tagValue.Count());
                        HighlightCharacter(splitWord);
                        #endregion

                        var charaSetting = new SetCharactersToUI();
                        charaSetting.GetAccuratedCharacter(CharacterTag);
                        charaSetting.CharacterSetting(_characters);
                        _characterSpeaking = charaSetting.SetCharacterSetting();
                        Set_ui_Information();
                    }

                    break;
                case PLACEMENT:
                    string[] splitIt = tagValue.Split(";");
                    if (splitIt[0] == "Switch")
                    {
                        int _headInt = int.Parse(splitIt[1]);
                        int _followInt = int.Parse(splitIt[2]);
                        _effects.gameObject.GetComponent<ImageEffects>().SwitchPosition(characterImages[_headInt - 1], characterImages[_followInt - 1]);

                    }

                    break;

                case Emotion:

                    if (tagValue == "None")
                        Debug.Log("Let image");
                    else
                    {


                    }
                    break;
                case MUSIQUE:
                    foreach (AudioClip _clip in _audio._clips)
                    {
                        if (tagValue == _clip.name)
                        {
                            _audio.Music_ClipToPlay = _clip;
                        }
                    }
                    break;
                case SFX:
                    _audio.SFX_ToPlay = true;

                    foreach (AudioClip _clip in _audio._clips)
                    {
                        if (tagValue == _clip.name)
                        {
                            _audio.SFX_ClipToPlay = _clip;
                        }
                    }
                    break;
                case TextEffect:
                    string[] BaseText = tagValue.Split(";");
                    string kindOfEffect = BaseText[0];
                    if (BaseText[0] == "Shake")
                    {
                        string wordToShake = BaseText[1];
                        string[] splitTagValue = wordToShake.Split(" ");
                        valueTag = splitTagValue;
                        for (int i = 0; i < valueTag.Length; i++) Debug.Log(valueTag[i]);
                        int lastWord = splitTagValue.Length - 1;
                        SplitText(splitTagValue);
                    }


                    break;
                // ajouter un case pour chaques AudioSource
                default:
                    Debug.LogWarning("Tag comes in but is not in the handled list, name : " + tags);
                    break;
            }

        }
    }
    void Set_ui_Information()
    {
        SpeechBubble.sprite = _characterSpeaking.spr_DialogBox;
        // ajouter le sprite qui s'allume 
    }

    void HighlightCharacter(string[] keytag)
    {

        int HighlightNumber = int.Parse(keytag[1]);

        Image Speaker = characterImages[HighlightNumber - 1];
        for (int w = 0; w < characterImages.Count; w++)
        {
            if (characterImages[w] != Speaker) characterImages[w].color = fadedColor[w];
            else characterImages[w].color = new Vector4(255f, 255f, 255f, 255f);
        }

    }
    public void MakeChoices(int choiceIndex)
    {
        CurrentStory.ChooseChoiceIndex(choiceIndex);
    }

    private void PrepareText(string textToDisplay)
    {
        ShowTextToDisplay = textToDisplay;
        DialogText.color = DialogText.color;
        DialogText.maxVisibleCharacters = 0;
        DialogText.text = textToDisplay;

        DialogText.ForceMeshUpdate();
        Debug.Log("Preparation du texte");
        StartCoroutine(Typewriter());


    }
    public float duration = 0.5f;
    float currentduration;
    public float magnitude = 0.1f;




    void ReformateLastWord()
    {
        LastWordChar = preparedText[preparedText.Length - 1].ToCharArray();
        _reformedWord = LastWordChar.ToList();
        _reformedWord.RemoveAt(_reformedWord.Count - 1);
        for (int i = 0; i < _reformedWord.Count; i++)
        {
            lettercollapsed = new string(_reformedWord.ToArray());
        }
        //Debug.Log("Mon mot est donc : " + lettercollapsed);
        preparedText[preparedText.Length - 1] = lettercollapsed;
    }

    void SplitText(string[] argument)
    {
        FirstVertCount = 0;
        LastVertCount = 0;
        preparedText = DialogText.text.Split(" ");
        ReformateLastWord();
        for (int i = 0; i < preparedText.Length; i++)
        {

            if (argument[0] != preparedText[i])
            {
                FirstVertCount += preparedText[i].Length * 4;

            }
            else
            {
                firstVert = FirstVertCount;
                _effects.firstVertice = firstVert;

            }
            if (argument[argument.Length - 1] == preparedText[i])
            {
                for (int j = 0; j <= i; j++)
                {
                    LastVertCount += preparedText[j].Length * 4;
                    //Debug.Log(preparedText[j]);
                    _effects.lastVertice = LastVertCount;
                }

            }

        }
        isShakingText = true;
        // ShakeText();

    }
    IEnumerator DelayTextAnim()
    {



        float elapsed = 0.0f;
        currentduration = duration;
        while (elapsed < currentduration)
        {

            elapsed += Time.deltaTime;

            yield return null;
        }
        //isShakingText = false;
        for (int w = _effects.firstVertice; w < _effects.lastVertice; w++)
        {
            DialogText.mesh.vertices[w] = _effects._startVertices[w];
            //DialogText.ForceMeshUpdate();
        }






    }
    IEnumerator WaitForAudioExecution(AudioSource _source, AudioClip _clip, float duration)
    {
        _source.clip = _clip;
        yield return new WaitForSeconds(duration);
        _source.Play();
        Debug.Log("Audio fini" + _source.name);
        // StartCoroutine(WaitForImageExecution(Image , Sprite , duration);
    }
    // 
    IEnumerator WaitForImageExecution(Image _imageToModify, Sprite _spriteToLoad, float duration)
    {
        Debug.Log(_spriteToLoad.name);
        yield return new WaitForSeconds(duration);

        _imageToModify.sprite = _spriteToLoad;
    }
    private IEnumerator Typewriter()
    {

        while (DialogText.maxVisibleCharacters < DialogText.textInfo.characterCount)
        {

            DialogText.maxVisibleCharacters += characterPerCycle;

            _effects._startVertices = DialogText.mesh.vertices;
            if (isShakingText)
            {
                _effects.MoveLetters();
            }

            yield return new WaitForSeconds(0.01f / speed);
        }
    }

    private IEnumerator SelectFirstChoice()
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(ArrowChoices[0].gameObject);
    }
}
