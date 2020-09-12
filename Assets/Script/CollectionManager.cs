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

    private List<GameObject> popScreenChildImage = new List<GameObject>();

    [SerializeField]
    private GameObject CollectionParent;

    [SerializeField]
    private GameObject canvasObj;

    [SerializeField]
    private GameObject lineRendererObjPrefab;

    [SerializeField]
    private Material lineRendererMaterial;

    private List<GameObject> lineRendererObjects = new List<GameObject>();

    private List<GameObject> collectionPlanetObjects=new List<GameObject>();

    private bool isPop;

    // Start is called before the first frame update
    void Start()
    {
        SetPopScreenObjects();
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

        var deltaScrollYPos = previousVector.y + deltaScrollVector.y * 100f<0?0: previousVector.y + deltaScrollVector.y * 100f > 1460 ? 1460 : previousVector.y + deltaScrollVector.y * 100f;

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

        Debug.Log(collectionStr);

        if (isPop) return;

        if (!isClearBeforeStage(collectionStr)) return;

        isPop = true;

        StartCoroutine(PopUpCollectionScreen());
    }

    public void SetPopScreenObjects()
    {
        popScreenChildImage.Add(popUpScreen.transform.Find("Back").gameObject);

        popScreenChildImage.Add(popUpScreen.transform.Find("Forward").gameObject);

        popScreenChildImage.Add(popUpScreen.transform.Find("Name").gameObject);

        popScreenChildImage.Add(popUpScreen.transform.Find("Explanation").gameObject);

        popScreenChildImage.Add(popUpScreen.transform.Find("Planet").gameObject);

        popScreenChildImage.Add(popUpScreen.transform.Find("Cross").gameObject);

        popScreenChildImage.Add(popUpScreen.transform.Find("Chara").gameObject);
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
    }

    public void SetPlanetPos(GameObject planetObj)
    {
        var childPlanet = planetObj.transform.Find("planetObj").gameObject;

        for (int i = 0; i < GameManager.Instance.planetPosList[getCollectionStageNum(collectionStr)].Count; i++)
        {
            var planetObject = Instantiate(childPlanet,Vector3.zero,Quaternion.identity);

            collectionPlanetObjects.Add(planetObject);

            planetObject.transform.parent = planetObj.transform;

            planetObject.transform.localScale=new Vector3(1, 1, 1);

            planetObject.SetActive(true);

            planetObject.GetComponent<RectTransform>().anchoredPosition = getWorldPosToAnchoredPos(GameManager.Instance.planetPosList[getCollectionStageNum(collectionStr)][i])/Const.shrinkPersantage;
        }

        StartCoroutine(ConnectPlanet());
    }

    public int getCollectionStageNum(string collectionStr)
    {
        return (int)(Enum.Stage)System.Enum.Parse(typeof(Enum.Stage), collectionStr);
    }

    public Vector2 getWorldPosToAnchoredPos(Vector2 worldPos)
    { 
        var canvasRect = canvasObj.GetComponent<RectTransform>();

        var viewportPosition = Camera.main.WorldToViewportPoint(worldPos);
        var worldPos_ScreenPosition = new Vector2(
            ((viewportPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
            ((viewportPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)));


        return worldPos_ScreenPosition;
    }

    public IEnumerator ConnectPlanet()
    {
        lineRendererObjects.Clear();

        var drawConnectLinePosList = new List<Vector2>();

        for (int i = 0; i < GameManager.Instance.planetPosList[getCollectionStageNum(collectionStr)].Count; i++)
        {
            var drawPoint = new Vector3((GameManager.Instance.planetPosList[getCollectionStageNum(collectionStr)][i] / 5).x,(GameManager.Instance.planetPosList[getCollectionStageNum(collectionStr)][i] / 5).y+0.15f);

            drawConnectLinePosList.Add(drawPoint);
        }

        //n個星があったらn-1個線を引く
        for (int i = 0; i < drawConnectLinePosList.Count - 1; i++)
        {
            //線のベクトル取得
            var lineVector = drawConnectLinePosList[i + 1] - drawConnectLinePosList[i];

            //線のLineRendererの生成
            var lineRendererObj = Instantiate(lineRendererObjPrefab, Vector3.zero, Quaternion.identity);

            lineRendererObj.transform.localPosition = Vector3.zero;

            lineRendererObjects.Add(lineRendererObj);

            var lineRenderer = lineRendererObj.GetComponent<LineRenderer>();

            lineRenderer.alignment = LineAlignment.TransformZ;

            lineRenderer.material = lineRendererMaterial;

            lineRenderer.material.color = new Color(1, 1, 1, 0);

            lineRenderer.SetWidth(1f, 1f);

            lineRenderer.textureMode = LineTextureMode.Tile;

            var lineDifference = lineVector;

            while (lineDifference.magnitude > 0.1f)
            {
                lineDifference = lineDifference / 2;
            }

            int differenceNum = (int)(lineVector.magnitude / lineDifference.magnitude);

            for (int n = 0; n < differenceNum; n++)
            {
                lineRenderer.positionCount = n + 1;
                lineRenderer.SetPosition(n, drawConnectLinePosList[i] + (lineDifference * n));
            }
        }

        var waitTime = 0.5f;

        for (int i = 0; i < lineRendererObjects.Count; i++)
        {
            var lineMaterial = lineRendererObjects[i].GetComponent<LineRenderer>().material;

            //LIneの表示
            DOTween.ToAlpha(() => lineMaterial.color,
                            color => lineMaterial.color = color,
                            1,
                            waitTime);
        }

        yield return new WaitForSeconds(waitTime);

        yield break;
    }

    public IEnumerator PopDownCollectionScreen()
    {

        for(int i = 2; i < 4; i++)
        {
            var text = popScreenChildImage[i].GetComponent<TextMeshProUGUI>();

            DOTween.ToAlpha(() => text.color
           , color => text.color = color
           , 0
           , 0.2f);
        }

        for (int i = 4; i < 7; i++)
        {
            var img = popScreenChildImage[i].GetComponent<Image>();

            DOTween.ToAlpha(() => img.color
           , color => img.color = color
           , 0
           , 0.2f);
        }

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

        for(int i = 0; i < collectionPlanetObjects.Count; i++)
        {
            Destroy(collectionPlanetObjects[i]);
        }

        collectionPlanetObjects.Clear();

        isPop = false;
    }

    public IEnumerator PopUpCollectionScreen()
    {
        popScreenChildImage[6].GetComponent<Image>().sprite = Resources.Load<Sprite>("Character/" + collectionStr);

        DOTween.ToAlpha(() => popUpScreen.GetComponent<Image>().color,
                color => popUpScreen.GetComponent<Image>().color = color,
                1,
                0.4f);

        yield return new WaitForSeconds(0.5f);

        popScreenChildImage[0].GetComponent<Image>().sprite = collectionSpriteList[getStageNum(collectionStr)];

        for (int i = 0; i < 2; i++)
        {
            var screen = popScreenChildImage[i];

            screen.transform.DOScale(Vector3.one, 0.4f);
        }

        yield return new WaitForSeconds(0.3f);

        SetPlanetPos(popUpScreen.transform.Find("Planet").gameObject);

        for (int i = 0; i < popScreenChildImage.Count; i++)
        {
            var screen = popScreenChildImage[i];

            screen.transform.DOScale(Vector3.one, 0f);
        }

        popScreenChildImage[2].GetComponent<TextMeshProUGUI>().text = CSVReader.Instance.getCollectionTitle(getStageNum(collectionStr));

        popScreenChildImage[3].GetComponent<TextMeshProUGUI>().text = CSVReader.Instance.getCollectionExplanation(getStageNum(collectionStr));

        for (int i = 2; i < 4; i++)
        {
            var text = popScreenChildImage[i].GetComponent<TextMeshProUGUI>();

            DOTween.ToAlpha(() => text.color
           , color => text.color = color
           , 1
           , 0.2f);
        }

        for (int i = 4; i < 7; i++)
        {
            var img = popScreenChildImage[i].GetComponent<Image>();

            DOTween.ToAlpha(() => img.color
           , color => img.color = color
           , 1
           , 0.2f);
        }
    }

    public int getStageNum(string stage)
    {
        return (int)(Enum.Stage)System.Enum.Parse(typeof(Enum.Stage), stage);
    }
}
