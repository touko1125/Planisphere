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

        isPop = true;

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

    public void SetPlanetPos(GameObject planetObj)
    {
        var childPlanet = planetObj.transform.Find("planetObj").gameObject;

        for (int i = 0; i < GameManager.Instance.planetPosList[getCollectionStageNum(collectionStr)].Count; i++)
        {
            Debug.Log(getCollectionStageNum(collectionStr) +"番目の"+ i +"番目は"+ GameManager.Instance.planetPosList[getCollectionStageNum(collectionStr)][i]);
        }

        for (int i = 0; i < GameManager.Instance.planetPosList[getCollectionStageNum(collectionStr)].Count; i++)
        {
            var planetObject = Instantiate(childPlanet,Vector3.zero,Quaternion.identity);

            collectionPlanetObjects.Add(planetObject);

            planetObject.transform.parent = planetObj.transform;

            planetObject.transform.localScale=new Vector3(1, 1, 1);

            planetObject.SetActive(true);

            planetObject.GetComponent<RectTransform>().anchoredPosition = getWorldPosToAnchoredPos(GameManager.Instance.planetPosList[getCollectionStageNum(collectionStr)][i])/Const.shrinkPersantage;

            Debug.Log(GameManager.Instance.planetPosList[getCollectionStageNum(collectionStr)][i]);
        }

        //StartCoroutine(ConnectPlanet());
    }

    public int getCollectionStageNum(string collectionStr)
    {
        return (int)(Enum.Stage)System.Enum.Parse(typeof(Enum.Stage), collectionStr);
    }

    public Vector2 getWorldPosToAnchoredPos(Vector2 worldPos)
    { 
        //first you need the RectTransform component of your canvas
        RectTransform CanvasRect = canvasObj.GetComponent<RectTransform>();

        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(worldPos);
        Vector2 WorldPos_ScreenPosition = new Vector2(
            ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
            ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));


        return WorldPos_ScreenPosition;
    }

    public IEnumerator ConnectPlanet()
    {
        lineRendererObjects.Clear();

        List<Vector2> drawConnectLinePosList = new List<Vector2>();

        for (int i = 0; i < GameManager.Instance.planetPosList[getCollectionStageNum(collectionStr)].Count; i++)
        {
            var drawPoint = new Vector3((GameManager.Instance.planetPosList[getCollectionStageNum(collectionStr)][i] / 5).x,(GameManager.Instance.planetPosList[getCollectionStageNum(collectionStr)][i] / 5).y+0.15f);

            drawConnectLinePosList.Add(drawPoint);
        }

        //n個星があったらn-1個線を引く
        for (int i = 0; i < drawConnectLinePosList.Count - 1; i++)
        {
            //線のベクトル取得
            Vector2 lineVector = drawConnectLinePosList[i + 1] - drawConnectLinePosList[i];

            //線のLineRendererの生成
            GameObject lineRendererObj = Instantiate(lineRendererObjPrefab, Vector3.zero, Quaternion.identity);

            lineRendererObj.transform.localPosition = Vector3.zero;

            lineRendererObjects.Add(lineRendererObj);

            LineRenderer lineRenderer = lineRendererObj.GetComponent<LineRenderer>();

            lineRenderer.alignment = LineAlignment.TransformZ;

            lineRenderer.material = lineRendererMaterial;

            lineRenderer.material.color = new Color(1, 1, 1, 0);

            lineRenderer.SetWidth(1f, 1f);

            lineRenderer.textureMode = LineTextureMode.Tile;

            Vector2 lineDifference = lineVector;

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
            //yield return StartCoroutine(FadeLine(line.GetComponent<LineRenderer>()));

            var lineMaterial = lineRendererObjects[i].GetComponent<LineRenderer>().material;

            Debug.Log(lineMaterial.color.a);

            //LIneの表示
            DOTween.ToAlpha(() => lineMaterial.color,
                            color => lineMaterial.color = color,
                            1,
                            waitTime);

            Debug.Log(lineMaterial.color.a);
        }

        yield return new WaitForSeconds(waitTime);

        yield break;
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

        SetPlanetPos(popUpScreen.transform.Find("Planet").gameObject);

        popScreenChildImage.Add(popUpScreen.transform.Find("Explanation").gameObject);

        popScreenChildImage.Add(popUpScreen.transform.Find("Cross").gameObject);

        for (int i = 0; i < popScreenChildImage.Count; i++)
        {
            var screen = popScreenChildImage[i];

            screen.transform.DOScale(Vector3.one, 0f);
        }

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
    }

    public int getStageNum(string stage)
    {
        return (int)(Enum.Stage)System.Enum.Parse(typeof(Enum.Stage), stage);
    }
}
