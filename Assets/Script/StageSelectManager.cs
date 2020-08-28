using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;
using TapClass;

public class StageSelectManager : MonoBehaviour
{
    [SerializeField]
    private Image LoadUI;

    [SerializeField]
    private Image[] LoadPlanetImage;

    [SerializeField]
    private List<GameObject> stageSprites;

    private string NextSceneStr;

    private bool isTransitionScene;

    private GameObject canvasObj;

    public Image stagePanel;

    private AsyncOperation async;

    // Start is called before the first frame update
    void Start()
    {
        canvasObj = GameObject.Find("StageSelectCanvas");

        StartCoroutine(FillStageImage());
    }

    // Update is called once per frame
    void Update()
    {
        ObserveInputPanelMove();
    }

    public void ObserveInputPanelMove()
    {
        if (isTransitionScene) return;

        if (getTapInfo() == null) return;

        if (!getTapInfo().is_tap) return;

        RotateStagePanel(getTapInfo());
    }

    public IEnumerator FillStageImage()
    {
        Debug.Log("aa");
        for(int i = 0; i < stageSprites.Count; i++)
        {
            if (i > GameManager.Instance.clearStageNum) yield break;

            int j = i;

            DOTween.To(() => stageSprites[j].transform.Find("Planet").Find("Cover").gameObject.GetComponent<Image>().fillAmount
                      ,(x) => stageSprites[j].transform.Find("Planet").Find("Cover").gameObject.GetComponent<Image>().fillAmount = x
                      ,1.0f, 0.9f);
        }

        Debug.Log("ii");
    }

    public void RotateStagePanel(Tap tapInfo)
    {
        var canvas = canvasObj.GetComponent<Canvas>();

        var canvasRectTransform = canvas.GetComponent<RectTransform>();

        var stagePanelImage = canvasObj.transform.Find("StageImage").gameObject;

        var stagePanelRectPos = stagePanelImage.GetComponent<RectTransform>();

        //UI座標からスクリーン座標の変換
        var stagePanelScreenPos = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, stagePanelRectPos.position);

        var stagePanelWorldPos = Vector2.zero;

        //スクリーン座標からワールド座標に変換
        RectTransformUtility.ScreenPointToLocalPointInRectangle
            (canvasRectTransform, stagePanelScreenPos, canvas.worldCamera, out stagePanelWorldPos);

        //中心から初期タップ位置への差分
        var center_startTapPos_Vector = tapInfo.start_tapPosition - stagePanelWorldPos;

        //中心からちょっと移動したとこへの差分
        var center_endTapPos_Vector = tapInfo.end_tapPosition - stagePanelWorldPos;

        //初期タップ位置の中心角度取得
        var angle1 = Mathf.Atan2(center_startTapPos_Vector.x, center_startTapPos_Vector.y) * Mathf.Rad2Deg;

        //最終タップ位置の中心角度取得
        var angle2 = Mathf.Atan2(center_endTapPos_Vector.x, center_endTapPos_Vector.y) * Mathf.Rad2Deg;

        //微小角度差分
        var angle = angle2 - angle1;

        //角度分だけ回し続ける
        stagePanelImage.transform.DOLocalRotate(
            new Vector3(stagePanelImage.transform.localEulerAngles.x,
                        stagePanelImage.transform.localEulerAngles.y,
                        stagePanelImage.transform.localEulerAngles.z - angle*500)
                        , 0.001f * ((tapInfo.end_tapPosition - tapInfo.start_tapPosition).magnitude)
                        , RotateMode.FastBeyond360);    
    }

    public void PressStage(GameObject buttonObj)
    {
        NextSceneStr = buttonObj.name;

        if (!isClearBeforeStage(NextSceneStr))
        {
            Debug.Log("すすめへんでー!");
            return;
        }

        StartCoroutine(SceneTransition());
    }

    public bool isClearBeforeStage(string SceneStr)
    {
        bool isClearBeforeStage= (int)(Enum.Stage)System.Enum.Parse(typeof(Enum.Stage), SceneStr)<=PlayerPrefs.GetInt(Const.clearStageNumKey,-1)+1;

        return isClearBeforeStage;
    }

    public IEnumerator SceneTransition()
    {
        isTransitionScene = true;

        //画像の表示
        DOTween.ToAlpha(() => LoadUI.color,
                        color => LoadUI.color = color,
                        1,
                        1f);

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < LoadPlanetImage.Length; i++)
        {
            var planetImage = LoadPlanetImage[i];

            //ロード用の惑星も表示
            DOTween.ToAlpha(() => planetImage.color,
                            color => planetImage.color = color,
                            1,
                            1f);
        }

        async = SceneManager.LoadSceneAsync(NextSceneStr);

        GameManager.Instance.isClearGame = false;

        GameManager.Instance.isPauseGame = false;

        while (!async.isDone)
        {
            LoadPlanetImage[0].transform.rotation = Quaternion.Euler(0, 0, 360 * async.progress);
            yield return null;
        }
    }

    public Tap getTapInfo()
    {
        return InputManager.Instance.tapinfo;
    }
}

