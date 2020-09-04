using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class StageProduction : MonoBehaviour
{
    //中身取得用
    [SerializeField]
    private GameObject canvas;

    //ゲームパネル
    [SerializeField]
    private GameObject Plate;

    private List<Image> clearCanvasImages=new List<Image>();

    private List<TextMeshProUGUI> clearCanvasTexts=new List<TextMeshProUGUI>();

    private List<Image> gameoverCanvasImages=new List<Image>();

    private List<TextMeshProUGUI> gameoverCanvasTexts=new List<TextMeshProUGUI>();

    private Image transitionCanvas;

    private List<Image> transitionCanvasImages=new List<Image>();

    private GameObject clockParent;

    //[SerializeField]
    //private Image[] ClearImages;

    //[SerializeField]
    //private TextMeshProUGUI[] ClearTexts;

    //[SerializeField]
    //private Image[] GameoverImages;

    //[SerializeField]
    //private TextMeshProUGUI[] GameoverTexts;

    //[SerializeField]
    //private GameObject ClockParent;

    //[SerializeField]
    //private GameObject Panel;

    //[SerializeField]
    //private Image LoadUI;

    //[SerializeField]
    //private List<Image> LoadPlanetImages;

    //次ステージ用
    private string nextSceneStr;

    //経過時間
    private float currentTime;

    //制御
    private bool gameEnd;

    private AsyncOperation async;

    // Start is called before the first frame update
    void Start()
    {
        SetObjects();

        RaycastTarget(gameoverCanvasImages, false);

        RaycastTarget(clearCanvasImages, false);
    }

    // Update is called once per frame
    void Update()
    {
        //経過時間観測
        CountTime();
    }

    public void SetObjects()
    {
        //クリア画面の画像設定
        clearCanvasImages.Add(canvas.transform.Find("ClearUI").gameObject.GetComponent<Image>());
        clearCanvasImages.Add(clearCanvasImages[0].transform.Find("NextButtom").gameObject.GetComponent<Image>());
        clearCanvasImages.Add(clearCanvasImages[0].transform.Find("RetryButtom").gameObject.GetComponent<Image>());
        clearCanvasImages.Add(clearCanvasImages[0].transform.Find("TitleButtom").gameObject.GetComponent<Image>());

        clearCanvasTexts.Add(clearCanvasImages[0].transform.Find("STAGECLEAR").gameObject.GetComponent<TextMeshProUGUI>());
        clearCanvasTexts.Add(clearCanvasImages[0].transform.Find("NextButtom").transform.Find("NEXT").gameObject.GetComponent<TextMeshProUGUI>());
        clearCanvasTexts.Add(clearCanvasImages[0].transform.Find("RetryButtom").transform.Find("RETRY").gameObject.GetComponent<TextMeshProUGUI>());
        clearCanvasTexts.Add(clearCanvasImages[0].transform.Find("TitleButtom").transform.Find("TITLE").gameObject.GetComponent<TextMeshProUGUI>());

        //ゲームオーバー画面の画像設定
        gameoverCanvasImages.Add(canvas.transform.Find("GameOverUI").gameObject.GetComponent<Image>());
        gameoverCanvasImages.Add(gameoverCanvasImages[0].transform.Find("TitleButtom").gameObject.GetComponent<Image>());
        gameoverCanvasImages.Add(gameoverCanvasImages[0].transform.Find("RetryButtom").gameObject.GetComponent<Image>());

        gameoverCanvasTexts.Add(gameoverCanvasImages[0].transform.Find("TIMEUP").gameObject.GetComponent<TextMeshProUGUI>());
        gameoverCanvasTexts.Add(gameoverCanvasImages[0].transform.Find("RetryButtom").transform.Find("RETRY").gameObject.GetComponent<TextMeshProUGUI>());
        gameoverCanvasTexts.Add(gameoverCanvasImages[0].transform.Find("TitleButtom").transform.Find("TITLE").gameObject.GetComponent<TextMeshProUGUI>());

        //遷移画面の画像設定
        transitionCanvas = canvas.transform.Find("TransitionUI").gameObject.GetComponent<Image>();
        foreach(Transform childTrans in transitionCanvas.transform)
        {
            transitionCanvasImages.Add(childTrans.gameObject.GetComponent<Image>());
        }
        clockParent = canvas.transform.Find("DownUI").transform.Find("ClockParent").gameObject;
    }

    public void RaycastTarget(List<Image> objects,bool raycastTarget)
    {
        foreach(Image image in objects)
        {
            image.raycastTarget = raycastTarget;
        }
    }

    public void CountTime()
    {
        if (GameManager.Instance.isClearGame) return;

        if (GameManager.Instance.isPauseGame) return;

        if (gameEnd) return;

        //制限時間をすぎたら
        if(currentTime > Const.limitedTime)
        {
            StartCoroutine(GameOverProduction());

            GameManager.Instance.isClearGame = true;
        }

        currentTime += Time.deltaTime;

        clockParent.transform.localRotation = Quaternion.Euler(0, 0, 70 - (140 * (currentTime / Const.limitedTime)));
    }

    public IEnumerator GameOverProduction()
    {
        var planetManager = new PlanetManager();

        //星たちを真顔に
        for (int i = 0; i < planetManager.planetList.Count; i++)
        {
            planetManager.planetList[i].GetComponent<PlanetComponent>().ChangeFace(Enum.PlanetFace.nomal);
        }

        //真顔みるタイム
        yield return new WaitForSeconds(0.3f);

        RaycastTarget(gameoverCanvasImages, true);

        RaycastTarget(clearCanvasImages, false);

        //ゲームパネルの非表示
        Plate.SetActive(false);

        for (int i = 0; i < gameoverCanvasImages.Count; i++)
        {
            var img = gameoverCanvasImages[i];

            img.gameObject.SetActive(true);

            //画像の表示
            DOTween.ToAlpha(() => img.color,
                            color => img.color = color,
                            1,
                            0.5f);
        }

        for (int i = 0; i < gameoverCanvasTexts.Count; i++)
        {
            var txt = gameoverCanvasTexts[i];

            //テキストの表示
            DOTween.ToAlpha(() => txt.color,
                            color => txt.color = color,
                            1,
                            0.5f);
        }
    }

    public IEnumerator ClearProduction(List<GameObject> planetObjects)
    {
        //星たちを笑顔に
        for(int i = 0; i < planetObjects.Count; i++)
        {
            planetObjects[i].GetComponent<PlanetComponent>().ChangeFace(Enum.PlanetFace.smile);
        }

        //今クリアしたステージが未クリアだったらクリア情報を更新
        if ((int)(Enum.Stage)System.Enum.Parse(typeof(Enum.Stage), SceneManager.GetActiveScene().name) > GameManager.Instance.clearStageNum)
        {
            GameManager.Instance.clearStageNum = (int)(Enum.Stage)System.Enum.Parse(typeof(Enum.Stage), SceneManager.GetActiveScene().name);

            GameManager.Instance.SaveClearStageNum();
        }

        //笑顔見るタイム
        yield return new WaitForSeconds(0.3f);

        RaycastTarget(gameoverCanvasImages, false);

        RaycastTarget(clearCanvasImages, true);

        //ゲームパネルの非表示
        Plate.SetActive(false);

        for (int i = 0; i < clearCanvasImages.Count; i++)
        {
            var img = clearCanvasImages[i];

            img.gameObject.SetActive(true);

            //画像の表示
            DOTween.ToAlpha(() => img.color,
                            color => img.color = color,
                            1,
                            0.5f);
        }

        for (int i = 0; i < clearCanvasTexts.Count; i++)
        {
            var txt = clearCanvasTexts[i];

            //テキストの表示
            DOTween.ToAlpha(() => txt.color,
                            color => txt.color = color,
                            1,
                            0.5f);
        }

    }

    public void PressTitle(Image buttonObj)
    {
        nextSceneStr = "Home";

        //効果音再生
        AudioManager.Instance.PlayAudio(AudioManager.Instance.AudioClips[3], AudioManager.Instance.AudioSources[1], AudioManager.Instance.volumeSE, false);

        //シーン遷移
        StartCoroutine(SceneTransition(buttonObj.gameObject.transform.GetChild(0).gameObject.GetComponent<Image>(),
            buttonObj.gameObject.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>()));
    }

    public void PressNext(Image buttonObj)
    {
        //現在のシーン(Enum.Stage型)
        var currentSceneEnum = (Enum.Stage)System.Enum.Parse(typeof(Enum.Stage),SceneManager.GetActiveScene().name);

        //次のシーン(string型)
        nextSceneStr = ((Enum.Stage)System.Enum.ToObject(typeof(Enum.Stage),(int)currentSceneEnum+1)).ToString();

        //効果音再生
        AudioManager.Instance.PlayAudio(AudioManager.Instance.AudioClips[3], AudioManager.Instance.AudioSources[1], AudioManager.Instance.volumeSE, false);

        //シーン遷移
        StartCoroutine(SceneTransition(buttonObj.gameObject.transform.GetChild(0).gameObject.GetComponent<Image>(),
            buttonObj.gameObject.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>()));
    }

    public void PressRetry(Image buttonObj)
    {
        //現在のシーン
        nextSceneStr = SceneManager.GetActiveScene().name;

        //効果音再生
        AudioManager.Instance.PlayAudio(AudioManager.Instance.AudioClips[3], AudioManager.Instance.AudioSources[1], AudioManager.Instance.volumeSE, false);

        //シーン遷移
        StartCoroutine(SceneTransition(buttonObj.gameObject.transform.GetChild(0).gameObject.GetComponent<Image>(),
           buttonObj.gameObject.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>()));
    }

    public IEnumerator SceneTransition(Image buttonObj, TextMeshProUGUI buttonText)
    {
        //Counttimeのチェックを通ってしまう
        gameEnd = true;

        buttonText.color = new Color(1, 1, 1);

        DOTween.To(
            ()=>buttonObj.fillAmount,
            (num)=>buttonObj.fillAmount=num,
            1.0f,
            1.2f);

        yield return new WaitForSeconds(1.5f);

        for (int i = 0; i < clearCanvasImages.Count; i++)
        {
            var img = clearCanvasImages[i];

            //画像の非表示
            DOTween.ToAlpha(() => img.color,
                            color => img.color = color,
                            0,
                            0.2f);
        }

        for (int i = 0; i < clearCanvasTexts.Count; i++)
        {
            var txt = clearCanvasTexts[i];

            //テキストの非表示
            DOTween.ToAlpha(() => txt.color,
                            color => txt.color = color,
                            0,
                            0.2f);
        }

        for (int i = 0; i < gameoverCanvasImages.Count; i++)
        {
            var img = gameoverCanvasImages[i];

            //画像の表示
            DOTween.ToAlpha(() => img.color,
                            color => img.color = color,
                            0,
                            0.2f);
        }

        for (int i = 0; i < gameoverCanvasTexts.Count; i++)
        {
            var txt = gameoverCanvasTexts[i];

            //テキストの表示
            DOTween.ToAlpha(() => txt.color,
                            color => txt.color = color,
                            0,
                            0.2f);
        }

        //ボタン画像の非表示
        DOTween.ToAlpha(() => buttonObj.color,
                        color => buttonObj.color = color,
                        0,
                        0.2f);

        //遷移画像の表示
        DOTween.ToAlpha(() => transitionCanvas.color,
                        color => transitionCanvas.color = color,
                        1,
                        1f);

        yield return new WaitForSeconds(0.5f);

        for(int i = 0; i < transitionCanvasImages.Count; i++)
        {
            var planetImage = transitionCanvasImages[i];

            //ロード用の惑星も表示
            DOTween.ToAlpha(() => planetImage.color,
                            color => planetImage.color = color,
                            1,
                            1f);
        }

        GameManager.Instance.isClearGame = false;

        GameManager.Instance.isPauseGame = false;

        async = SceneManager.LoadSceneAsync(nextSceneStr);

        while (!async.isDone)
        {
            transitionCanvasImages[0].transform.rotation = Quaternion.Euler(0, 0, 360 * async.progress);
            yield return null;
        }
    }
}
