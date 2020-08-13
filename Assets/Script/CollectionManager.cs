using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using TapClass;


public class CollectionManager : MonoBehaviour
{
    private string collectionStr;

    [SerializeField]
    private List<GameObject> collectionImageList = new List<GameObject>();

    [SerializeField]
    private List<Sprite> collectionSpriteList = new List<Sprite>();

    [SerializeField]
    private GameObject popUpScreen;

    [SerializeField]
    private GameObject CollectionParent;

    [SerializeField]
    private GameObject canvasObj;

    private bool isPop;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FillCollectionImage());
    }

    // Update is called once per frame
    void Update()
    {
        ObserveInput();
    }

    public void ObserveInput()
    {
        if (isPop) return;

        if (getTapInfo() == null) return;

        if (!getTapInfo().is_tap) return;

        ScrollScreen(getTapInfo());
    }

    public void ScrollScreen(Tap tapInfo)
    {
        var deltaScrollVector = tapInfo.end_tapPosition - tapInfo.start_tapPosition;

        var previousVector = CollectionParent.GetComponent<RectTransform>().localPosition;

        var deltaScrollYPos = previousVector.y + deltaScrollVector.y * 300f<-350?-350: previousVector.y + deltaScrollVector.y * 300f > 1140 ? 1140 : previousVector.y + deltaScrollVector.y * 300f;

        CollectionParent.GetComponent<RectTransform>().localPosition = new Vector3(previousVector.x,deltaScrollYPos, previousVector.z);
    }

    public Tap getTapInfo()
    {
        return InputManager.Instance.tapinfo;
    }

    public void PressCross()
    {
        if (!isPop) return;

        StartCoroutine(PopDownCollectionScreen());

        isPop = false;
    }

    public void PressCollection(GameObject buttonObj)
    {
        collectionStr = buttonObj.name;

        if (isPop) return;

        if (!isClearBeforeStage(collectionStr)) return;

        StartCoroutine(PopUpCollectionScreen());
    }

    public bool isClearBeforeStage(string SceneStr)
    {
        bool isClearBeforeStage = getStageNum(SceneStr) <= PlayerPrefs.GetInt(Const.clearStageNumKey, -1);

        return isClearBeforeStage;
    }

    public IEnumerator FillCollectionImage()
    {
        
        for (int i = 0; i < collectionImageList.Count; i++)
        {
            if (i > GameManager.Instance.clearStageNum) yield break;

            int j = i;

            DOTween.To(() => collectionImageList[j].transform.Find("Cover").gameObject.GetComponent<Image>().fillAmount
                      , (x) => collectionImageList[j].transform.Find("Cover").gameObject.GetComponent<Image>().fillAmount = x
                      , 1.0f, 0.9f);
        }

        Debug.Log("ii");
    }

    public IEnumerator PopDownCollectionScreen()
    {
        List<GameObject> popScreenChildImage = new List<GameObject>();

        popScreenChildImage.Add(popUpScreen.transform.Find("Back").gameObject);

        popScreenChildImage.Add(popUpScreen.transform.Find("Forward").gameObject);

        //恐ろしい感じ

        popScreenChildImage.Add(popUpScreen.transform.Find("Name").gameObject);

        popScreenChildImage.Add(popUpScreen.transform.Find("Planet").gameObject);

        popScreenChildImage.Add(popUpScreen.transform.Find("Explanation").gameObject);

        popScreenChildImage.Add(popUpScreen.transform.Find("Thumbtack").gameObject);

        popScreenChildImage.Add(popUpScreen.transform.Find("Thumbtack1").gameObject);

        popScreenChildImage.Add(popUpScreen.transform.Find("Cross").gameObject);

        popScreenChildImage[2].GetComponent<TextMeshProUGUI>().text = CSVReader.Instance.getCollectionTitle(getStageNum(collectionStr));

        popScreenChildImage[4].GetComponent<TextMeshProUGUI>().text = CSVReader.Instance.getCollectionExplanation(getStageNum(collectionStr));

        DOTween.ToAlpha(() => popScreenChildImage[3].GetComponent<Image>().color
                           , color => popScreenChildImage[3].GetComponent<Image>().color = color
                           , 0
                           , 0.2f);

        DOTween.ToAlpha(() => popScreenChildImage[2].GetComponent<TextMeshProUGUI>().color
                           , color => popScreenChildImage[2].GetComponent<TextMeshProUGUI>().color = color
                           , 0
                           , 0.2f);

        DOTween.ToAlpha(() => popScreenChildImage[4].GetComponent<TextMeshProUGUI>().color
                           , color => popScreenChildImage[4].GetComponent<TextMeshProUGUI>().color = color
                           , 0
                           , 0.2f);

        DOTween.ToAlpha(() => popScreenChildImage[5].GetComponent<Image>().color
                          , color => popScreenChildImage[5].GetComponent<Image>().color = color
                          , 0
                          , 0.2f);

        DOTween.ToAlpha(() => popScreenChildImage[6].GetComponent<Image>().color
                          , color => popScreenChildImage[6].GetComponent<Image>().color = color
                          , 0
                          , 0.2f);

        DOTween.ToAlpha(() => popScreenChildImage[7].GetComponent<Image>().color
                          , color => popScreenChildImage[7].GetComponent<Image>().color = color
                          , 0
                          , 0.2f);

        yield return new WaitForSeconds(0.3f);

        for (int i = 0; i < popScreenChildImage.Count; i++)
        {
            var screen = popScreenChildImage[i];

            screen.transform.DOScale(Vector3.zero, 0.4f);
        }

        yield return new WaitForSeconds(0.3f);

        DOTween.ToAlpha(() => popUpScreen.GetComponent<Image>().color,
                        color => popUpScreen.GetComponent<Image>().color = color,
                        0,
                        0.4f);

        isPop = false;
    }

    public IEnumerator PopUpCollectionScreen()
    {
        isPop = true;

        DOTween.ToAlpha(() => popUpScreen.GetComponent<Image>().color,
                color => popUpScreen.GetComponent<Image>().color = color,
                1,
                0.4f);

        yield return new WaitForSeconds(0.5f);

        List<GameObject> popScreenChildImage=new List<GameObject>();

        popScreenChildImage.Add(popUpScreen.transform.Find("Back").gameObject);

        popScreenChildImage.Add(popUpScreen.transform.Find("Forward").gameObject);

        popScreenChildImage[0].GetComponent<Image>().sprite = collectionSpriteList[getStageNum(collectionStr)];

        for (int i = 0; i < popScreenChildImage.Count; i++)
        {
            var screen = popScreenChildImage[i];

            screen.transform.DOScale(Vector3.one, 0.4f);
        }

        yield return new WaitForSeconds(0.3f);

        //恐ろしい感じ

        popScreenChildImage.Add(popUpScreen.transform.Find("Name").gameObject);

        popScreenChildImage.Add(popUpScreen.transform.Find("Planet").gameObject);

        popScreenChildImage.Add(popUpScreen.transform.Find("Explanation").gameObject);

        popScreenChildImage.Add(popUpScreen.transform.Find("Thumbtack").gameObject);

        popScreenChildImage.Add(popUpScreen.transform.Find("Thumbtack1").gameObject);

        popScreenChildImage.Add(popUpScreen.transform.Find("Cross").gameObject);

        popScreenChildImage[2].GetComponent<TextMeshProUGUI>().text = CSVReader.Instance.getCollectionTitle(getStageNum(collectionStr));

        popScreenChildImage[4].GetComponent<TextMeshProUGUI>().text = CSVReader.Instance.getCollectionExplanation(getStageNum(collectionStr));

        DOTween.ToAlpha(() => popScreenChildImage[3].GetComponent<Image>().color
                           , color => popScreenChildImage[3].GetComponent<Image>().color = color
                           , 1
                           , 0.2f);

        DOTween.ToAlpha(() => popScreenChildImage[2].GetComponent<TextMeshProUGUI>().color
                           , color => popScreenChildImage[2].GetComponent<TextMeshProUGUI>().color = color
                           , 1
                           , 0.2f);

        DOTween.ToAlpha(() => popScreenChildImage[4].GetComponent<TextMeshProUGUI>().color
                           , color => popScreenChildImage[4].GetComponent<TextMeshProUGUI>().color = color
                           , 1
                           , 0.2f);

        DOTween.ToAlpha(() => popScreenChildImage[5].GetComponent<Image>().color
                          , color => popScreenChildImage[5].GetComponent<Image>().color = color
                          , 1
                          , 0.2f);

        DOTween.ToAlpha(() => popScreenChildImage[6].GetComponent<Image>().color
                          , color => popScreenChildImage[6].GetComponent<Image>().color = color
                          , 1
                          , 0.2f);

        DOTween.ToAlpha(() => popScreenChildImage[7].GetComponent<Image>().color
                          , color => popScreenChildImage[7].GetComponent<Image>().color = color
                          , 1
                          , 0.2f);
    }

    public int getStageNum(string stage)
    {
        return (int)(GameManager.Stage)System.Enum.Parse(typeof(GameManager.Stage), stage);
    }
}
