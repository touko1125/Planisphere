using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StoryClass;
using UnityEngine.UI;
using TMPro;

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

    private TextMeshProUGUI messageText;

    // Start is called before the first frame update
    void Start()
    {
        SetObjects();
    }

    // Update is called once per frame
    void Update()
    {
        CheckStoryPlay();
    }

    public void SetObjects()
    {
        story = new Story(storyNumbers.ToArray(),storyType,storyUnlockNumber);

        backGround = gameObject.transform.Find("Back").gameObject.GetComponent<Image>();

        rightIcon = gameObject.transform.Find("IconRight").Find("Person").gameObject.GetComponent<Image>();

        leftIcon = gameObject.transform.Find("IconLeft").Find("Person").gameObject.GetComponent<Image>();

        coverImage = gameObject.transform.Find("Cover").GetComponent<Image>();

        messageText = gameObject.transform.Find("Story").Find("storyText").gameObject.GetComponent<TextMeshProUGUI>();
    }

    public void CheckStoryPlay()
    {
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
                StartCoroutine(PlayStory());
                isPlayingStory = true;
                break;
        }
    }

    public IEnumerator PlayStory()
    {
        GameManager.Instance.isPauseGame = true;

        if (isPlayingStory) yield break;

        Sprite firstLeftIconSprite=null;
        Sprite firstRightIconSprite=null;

        //一番初めの左右の画像を探す
        for (int i = 0; i < story.story_Chart.Count; i++)
        {
            if (story.story_Chart[i].message_PersonPos == Enum.LR.left)
            {
                firstLeftIconSprite = story.story_Chart[i].message_PersonSprite;
                break;
            } 
        }

        for (int i = 0; i < story.story_Chart.Count; i++)
        {
            if (story.story_Chart[i].message_PersonPos == Enum.LR.right)
            {
                firstRightIconSprite = story.story_Chart[i].message_PersonSprite;
                break;
            }
        }

        Debug.Log(firstLeftIconSprite);

        Debug.Log(firstRightIconSprite);

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

        GameManager.Instance.isPauseGame = false;

        gameObject.SetActive(false);
    }

    public bool isStoryUnlocked()
    {
        return story.unlock_StoryStageNum <= GameManager.Instance.clearStageNum;
    }
}
