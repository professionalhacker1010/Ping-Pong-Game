using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class CutsceneDatabase : MonoBehaviour
{
    //references to dialogue stuffs
    private DialogueRunner dialogueRunner;
    private CustomVariableStorage CVStore;
    [SerializeField] private GameObject itemInputObject;
    [SerializeField] private TMPro.TMP_InputField itemInput;
    [SerializeField] private DialogueBubble dialogueBubble;

    //current assets
    [SerializeField] private SpriteRenderer currBackground;
    [SerializeField] private SpriteRenderer currSprite;
    [SerializeField] private GameObject speechBubble;
    private RectTransform speechBubbleTransform;

    //storage of all the cutscene assets
    [SerializeField] private List<Sprite> importBackgrounds;
    [SerializeField] private List<Sprite> importSprites;
    [SerializeField] private List<Vector2> importSpeechBubbleLocations;
    [SerializeField] private List<string> speechBubbleLocationNames;
    [SerializeField] private List<Vector3> importSpriteLocations;
    [SerializeField] private List<string> spriteLocationNames;
    private Dictionary<string, Sprite> backgrounds;
    private Dictionary<string, Sprite> sprites;
    private Dictionary<string, Vector2> speechBubbleLocations;
    private Dictionary<string, Vector3> spriteLocations;

    //static variables
    public static string item = "";

    private void Start()
    {
        //get components
        dialogueRunner = FindObjectOfType<Yarn.Unity.DialogueRunner>();
        CVStore = FindObjectOfType<CustomVariableStorage>();
        //itemInput = itemInputObject.GetComponent<TMPro.TMP_InputField>();

        //update variables
        if (item != "") CVStore.SetValue("$item", item);

        //get rect transform for speech bubble??
        speechBubbleTransform = speechBubble.GetComponent<RectTransform>();

        //put all data into dictionaries
        backgrounds = new Dictionary<string, Sprite>();
        for (int i = 0; i < importBackgrounds.Count; i++)
        {
            backgrounds.Add(importBackgrounds[i].name, importBackgrounds[i]);
        }

        sprites = new Dictionary<string, Sprite>();
        for (int i = 0; i < importSprites.Count; i++)
        {
            sprites.Add(importSprites[i].name, importSprites[i]);
        }

        speechBubbleLocations = new Dictionary<string, Vector2>();
        for (int i = 0; i< importSpeechBubbleLocations.Count; i++)
        {
            speechBubbleLocations.Add(speechBubbleLocationNames[i], importSpeechBubbleLocations[i]);
        }

        spriteLocations = new Dictionary<string, Vector3>();
        for (int i = 0; i< importSpriteLocations.Count; i++)
        {
            spriteLocations.Add(spriteLocationNames[i], importSpriteLocations[i]);
        }

        //create commands for yarn
        dialogueRunner.AddCommandHandler("item_prompt", ItemPrompt);
        dialogueRunner.AddCommandHandler("end_scene1", EndScene1);
        dialogueRunner.AddCommandHandler("change_speaker", ChangeSpeaker);
        dialogueRunner.AddCommandHandler("start_speaker", StartSpeaker);
        dialogueRunner.AddCommandHandler("end_speaker", EndSpeaker);
        dialogueRunner.AddCommandHandler("change_emotion", ChangeEmotion);
        dialogueRunner.AddCommandHandler("transition_out", TransitionOut);
        dialogueRunner.AddCommandHandler("transition_in", TransitionIn);

    }

    public void ChangeSpeaker(string[] parameters, System.Action onComplete) //first string = name of speaker
    {
        StartCoroutine(ChangeSpeakerHelper(parameters, onComplete));
    }

    private IEnumerator ChangeSpeakerHelper (string[] parameters, System.Action onComplete)
    {
        yield return dialogueBubble.ChangeSpeaker(parameters[0]);
        onComplete();
    }

    public void StartSpeaker(string[] parameters, System.Action onComplete)
    {
        StartCoroutine(StartSpeakerHelper(parameters, onComplete));
    }

    private IEnumerator StartSpeakerHelper(string[] parameters, System.Action onComplete)
    {
        dialogueBubble.isChangingSpeaker = true;
        dialogueBubble.MoveBubbleToSpeaker(parameters[0]);
        yield return new WaitForSeconds(0f);
        onComplete();
    }

    public void EndSpeaker(string[] parameters, System.Action onComplete)
    {
        StartCoroutine(EndSpeakerHelper(parameters, onComplete));
    }

    private IEnumerator EndSpeakerHelper(string[] parameters, System.Action onComplete)
    {
        yield return dialogueBubble.ExitBubbleHelper();
        onComplete();
    }

    public void ChangeEmotion(string[] parameters) //0 = name of speaker, 1 = trigger     OR     0 = name of speaker, 1 = name of child object, 2 = trigger
    {
        Animator animator;
        if (parameters.Length == 3)
        {
            animator = GameObject.Find(parameters[1]).GetComponent<Animator>();
            animator.SetTrigger(parameters[2]);
        }
        else {
            animator = GameObject.Find(parameters[0]).GetComponentInChildren<Animator>();
            animator.SetTrigger(parameters[1]);
        }
    }
    public void TransitionOut(string[] parameters, System.Action onComplete)
    {
        StartCoroutine(TransitionOutHelper(parameters, onComplete));
    }

    private IEnumerator TransitionOutHelper(string[] parameters, System.Action onComplete)
    {
        yield return TransitionManager.Instance.QuickOutHelper();
        yield return new WaitForSeconds(0.25f);
        onComplete();
    }

    public void TransitionIn(string[] parameters, System.Action onComplete)
    {
        StartCoroutine(TransitionInHelper(parameters, onComplete));
    }

    private IEnumerator TransitionInHelper(string[] parameters, System.Action onComplete)
    {
        yield return new WaitForSeconds(0.25f);

        TransitionManager.Instance.QuickIn();
        yield return new WaitForSeconds(0.25f);
        onComplete();
    }

    //Cutscene 1 specific stuff
    #region
    [YarnCommand("pan_parkPicnic")]
    public void PanParkPicnic()
    {
        speechBubble.SetActive(false);
        StartCoroutine(PanParkPicnicHelper());
    }

    private IEnumerator PanParkPicnicHelper()
    {
        float[] positions = { -10f, -9.8f, -9.6f, -8.75f, -7.5f, -5f, 0f, 5f, 7.5f, 8.75f, 9.6f, 9.8f, 10f};
        Debug.Log("coroutine called");
        for (int i = 0; i < 13; i++)
        {
            currBackground.transform.position = new Vector3(0f, positions[i]);
            yield return new WaitForSeconds(2 / 24f);
        }
        yield return new WaitForSeconds(22 / 24f);
        speechBubble.SetActive(true);
    }

    [YarnCommand("reset_BG_transform")]
    public void ResetBGTransform()
    {
        currBackground.transform.position = new Vector3(0f, 0f);
    }

    public void ItemPrompt(string[] parameters, System.Action onComplete)
    {
        itemInputObject.SetActive(true);
        speechBubble.SetActive(false);
        itemInput.Select();
        StartCoroutine(waitUntilItemEntered(onComplete));
    }

    private IEnumerator waitUntilItemEntered(System.Action onComplete)
    {
        yield return new WaitUntil(()=> Input.GetKeyUp(KeyCode.Return)); //wait until user hits enter
        yield return new WaitForSeconds(0.1f); //need some padding for variable to be assigned correctly
        speechBubble.SetActive(true);
        onComplete();
    }

    public void InputToItem(string input)
    {
        CVStore.SetValue("$item", input);
        item = input;
    }

    public void EndScene1(string[] parameters, System.Action onComplete)
    {
        StartCoroutine(EndScene1Helper(onComplete));
    }

    private IEnumerator EndScene1Helper(System.Action onComplete)
    {
        speechBubble.SetActive(false);
        yield return new WaitUntil(() => KeyCodes.InteractGetUp() || KeyCodes.HitGetUp());
        EndCutscene();
        yield return new WaitForSeconds(1f);
        ChangeBG("transparent");
        onComplete();
    }
    #endregion

    //STUFF IM SCARED TO DELETE
    //change image locations
    #region
    [YarnCommand("transition")]
    public void PlayTransition(string name)
    {
        StartCoroutine(PlayTransitionHelper(name));
    }

    private IEnumerator PlayTransitionHelper(string name)
    {
        TransitionManager.Instance.QuickOut();
        yield return new WaitForSeconds(18 / 24f);
        currBackground.sprite = backgrounds[name];
        yield return new WaitForSeconds(0.5f);
        TransitionManager.Instance.QuickIn();
    }

    [YarnCommand("disable_bubble")]
    public void DisableBubble()
    {
        //speechBubble.SetActive(false);
        dialogueBubble.ExitBubble();
    }

    [YarnCommand("enable_bubble")]
    public void EnableBubble()
    {
        //speechBubble.SetActive(true);
        dialogueBubble.EnterBubble();
    }
    [YarnCommand("change_BG")]
    public void ChangeBG(string name)
    {
        Debug.Log("change BG called");
        currBackground.sprite = backgrounds[name];
    }

    [YarnCommand("change_bubble_loc")]
    public void ChangeBubbleLocation(string name)
    {
        Debug.Log("change bubble loc called");
        speechBubbleTransform.anchoredPosition = speechBubbleLocations[name];
    }

    [YarnCommand("change_sprite")]
    public void ChangeSprite(string name)
    {
        Debug.Log("change sprite called");
        currSprite.sprite = sprites[name];
    }

    [YarnCommand("change_sprite_loc")]
    public void ChangeSpriteLocation(string name)
    {
        currSprite.transform.position = spriteLocations[name];
    }

    [YarnCommand("end_cutscene")] //this is put at the end of all cutscenes except the first one
    public void EndCutscene()
    {
        StartCoroutine(CutsceneManager.Instance.ChangeScene());
    }
    #endregion
}
