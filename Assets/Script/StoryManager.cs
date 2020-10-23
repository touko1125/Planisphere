using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StoryClass;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StoryManager : MonoBehaviour
{
    private Story story;

    [SerializeField]
    private List<int> storyNumbers;

    [SerializeField]
    private Enum.StoryType storyType;

    [SerializeField]
    private int storyUnlockNumber;

    private bool isPlayingStory;

    private Image backGround;

    private Image rightIcon;

    private Image leftIcon;

    private Image coverImage;

    private GameObject storyObj;

    private TextMeshProUGUI messageText;

    private bool isSetStory;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        SetObjects();
        CheckStoryPlay();
    }

    public void SetObjects()
    {
        if (isSetStory) return;

        if (!CSVReader.Instance.isSetCsvDate) return;

        story = new Story(storyNumbers.ToArray(),storyType,storyUnlockNumber);

        storyObj = gameObject.transform.Find("StoryObj").gameObject;

        backGround = gameObject.transform.Find("StoryObj").Find("Back").gameObject.GetComponent<Image>();

        rightIcon = gameObject.transform.Find("StoryObj").Find("IconRight").Find("Person").gameObject.GetComponent<Image>();

        leftIcon = gameObject.transform.Find("StoryObj").Find("IconLeft").Find("Person").gameObject.GetComponent<Image>();

        coverImage = gameObject.transform.Find("StoryObj").Find("cover").GetComponent<Image>();

        messageText = gameObject.transform.Find("StoryObj").Find("Story").Find("storyText").gameObject.GetComponent<TextMeshProUGUI>();

        isSetStory = true;
    }

    public void CheckStoryPlay()
    {
        if (!isSetStory) return;

        if (isPlayingStory) return;

        switch (story.story_Type)
        {
            //ホーム画面で再生　ある程度ステージが進んだら解放される
            case Enum.StoryType.story:
                if (!isStoryUnlocked()) break;
                StartCoroutine(PlayStory());
                isPlayingStory = true;
                break;
            //チュートリアルがないとこにこれは存在しない
            case Enum.StoryType.tutorial:
                if (getCollectionStageNum(SceneManager.GetActiveScene().name) <= GameManager.Instance.clearStageNum) break;
                StartCoroutine(PlayStory());
                isPlayingStory = true;
                break;
        }
    }

    public int getCollectionStageNum(string collectionStr)
    {
        return (int)(Enum.Stage)System.Enum.Parse(typeof(Enum.Stage), collectionStr);
    }

    public IEnumerator PlayStory()
    {
        GameManager.Instance.isPauseGame = true;

        storyObj.SetActive(true);

        if (isPlayingStory) yield break;

        Sprite firstLeftIconSprite = null;
        Sprite firstRightIconSprite = null;

        //一番初めの左右それぞれの画像を探す
        for (int i = 0; i < story.story_Chart.Count; i++)
        {
            if (story.story_Chart[i].message_PersonPos == Enum.LR.right)
            {
                firstRightIconSprite = story.story_Chart[i].message_PersonSprite;
                break;
            }
        }
        for (int i = 0; i < story.story_Chart.Count; i++)
        {
            if (story.story_Chart[i].message_PersonPos == Enum.LR.left)
            {
                firstLeftIconSprite = story.story_Chart[i].message_PersonSprite;
                break;
            }
        }

        for (int i = 0; i < story.story_Chart.Count;i++)
        {
            while (InputManager.Instance.tapinfo.is_tap) yield return null;

            backGround.sprite = story.story_Chart[i].message_BackGroudSprite;

            messageText.text = story.story_Chart[i].message_String;

            switch (story.story_Chart[i].message_PersonPos)
            {
                case Enum.LR.left:
                    //話者のアイコン変更
                    leftIcon.sprite = i == 0 ? firstLeftIconSprite : story.story_Chart[i].message_PersonSprite;

                    //はじめじゃなきゃ変更なし
                    rightIcon.sprite = i == 0 ? firstRightIconSprite : rightIcon.sprite;

                    //相手は暗く
                    coverImage.transform.SetParent(rightIcon.transform);
                    coverImage.rectTransform.anchoredPosition = new Vector2(0, 6);
                    break;
                case Enum.LR.right:
                    //話者のアイコン変更
                    rightIcon.sprite = i == 0 ? firstRightIconSprite : story.story_Chart[i].message_PersonSprite;

                    //はじめじゃなきゃ変更なし
                    leftIcon.sprite = i == 0 ? firstLeftIconSprite : leftIcon.sprite;

                    //相手は暗く
                    coverImage.transform.SetParent(leftIcon.transform);
                    coverImage.rectTransform.anchoredPosition = new Vector2(0, 6);
                    break;
            }

            yield return new WaitUntil(() => InputManager.Instance.tapinfo.is_tap);
        }

        storyObj.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        GameManager.Instance.isPauseGame = false;
    }

    public bool isStoryUnlocked()
    {
        return story.unlock_StoryStageNum == GameManager.Instance.clearStageNum;
    }
}
