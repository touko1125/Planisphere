using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;


public class CollectionManager : MonoBehaviour
{
    private string collectionStr;

    [SerializeField]
    private List<GameObject> collectionImageList = new List<GameObject>();

    [SerializeField]
    private List<Sprite> collectionSpriteList = new List<Sprite>();

    [SerializeField]
    private GameObject popUpScreen;

    private bool isPop;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FillCollectionImage());
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public IEnumerator PopUpCollectionScreen()
    {
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

        popScreenChildImage.Add(popUpScreen.transform.Find("Name").gameObject);

        popScreenChildImage.Add(popUpScreen.transform.Find("Planet").gameObject);

        popScreenChildImage.Add(popUpScreen.transform.Find("Explanation").gameObject);

        popScreenChildImage[2].GetComponent<TextMeshProUGUI>().text = CSVReader.Instance.getCollectionTitle(getStageNum(collectionStr));

        popScreenChildImage[4].GetComponent<TextMeshProUGUI>().text = CSVReader.Instance.getCollectionExplanation(getStageNum(collectionStr));

        for (int i = 3; i < popScreenChildImage.Count; i++)
        {
            var component = popScreenChildImage[i];

            DOTween.ToAlpha(() => component.GetComponent<Image>().color
                            , color => component.GetComponent<Image>().color = color
                            , 1
                            , 0.2f);

            DOTween.ToAlpha(() => component.GetComponent<TextMeshProUGUI>().color
                            , color => component.GetComponent<TextMeshProUGUI>().color = color
                            , 1
                            , 0.2f);
        }
    }

    public int getStageNum(string stage)
    {
        return (int)(GameManager.Stage)System.Enum.Parse(typeof(GameManager.Stage), stage);
    }
}
