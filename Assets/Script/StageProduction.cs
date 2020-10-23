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

    private List<GameObject> clearCanvasObjects=new List<GameObject>();

    private List<GameObject> gameoverCanvasObjects=new List<GameObject>();

    private GameObject transitionCanvas;

    private List<GameObject> transitionCanvasObjects=new List<GameObject>();

    private GameObject clockParent;

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

        RaycastTarget(gameoverCanvasObjects, false);

        RaycastTarget(clearCanvasObjects, false);
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
        clearCanvasObjects.Add(canvas.transform.Find("ClearUI").gameObject);
        clearCanvasObjects.Add(clearCanvasObjects[0].transform.Find("NextButtom").gameObject);
        clearCanvasObjects.Add(clearCanvasObjects[0].transform.Find("RetryButtom").gameObject);
        clearCanvasObjects.Add(clearCanvasObjects[0].transform.Find("TitleButtom").gameObject);
        clearCanvasObjects.Add(clearCanvasObjects[0].transform.Find("STAGECLEAR").gameObject);
        clearCanvasObjects.Add(clearCanvasObjects[0].transform.Find("NextButtom").transform.Find("NEXT").gameObject);
        clearCanvasObjects.Add(clearCanvasObjects[0].transform.Find("RetryButtom").transform.Find("RETRY").gameObject);
        clearCanvasObjects.Add(clearCanvasObjects[0].transform.Find("TitleButtom").transform.Find("TITLE").gameObject);

        //ゲームオーバー画面の画像設定
        gameoverCanvasObjects.Add(canvas.transform.Find("GameOverUI").gameObject);
        gameoverCanvasObjects.Add(gameoverCanvasObjects[0].transform.Find("TitleButtom").gameObject);
        gameoverCanvasObjects.Add(gameoverCanvasObjects[0].transform.Find("RetryButtom").gameObject);
        gameoverCanvasObjects.Add(gameoverCanvasObjects[0].transform.Find("TIMEUP").gameObject);
        gameoverCanvasObjects.Add(gameoverCanvasObjects[0].transform.Find("RetryButtom").transform.Find("RETRY").gameObject);
        gameoverCanvasObjects.Add(gameoverCanvasObjects[0].transform.Find("TitleButtom").transform.Find("TITLE").gameObject);

        //遷移画面の画像設定
        transitionCanvas = canvas.transform.Find("TransitionUI").gameObject;
        foreach(Transform childTrans in transitionCanvas.transform)
        {
            transitionCanvasObjects.Add(childTrans.gameObject);
        }
        clockParent = canvas.transform.Find("DownUI").transform.Find("ClockParent").gameObject;
    }

    public void CountTime()
    {
        if (GameManager.Instance.isClearGame) return;

        if (GameManager.Instance.isPauseGame) return;

        if (gameEnd) return;

        //制限時間をすぎたら
        if (currentTime > Const.limitedTime)
        {
            StartCoroutine(GameOverProduction());

            GameManager.Instance.isClearGame = true;
        }

        currentTime += Time.deltaTime;

        clockParent.transform.localRotation = Quaternion.Euler(0, 0, 70 - (140 * (currentTime / Const.limitedTime)));
    }

    public void RaycastTarget(List<GameObject> objects,bool raycastTarget)
    {
        foreach(GameObject obj in objects)
        {
            if (HasComponent<Image>(obj)) obj.GetComponent<Image>().raycastTarget=raycastTarget;
            if (HasComponent<TextMeshProUGUI>(obj)) obj.GetComponent<TextMeshProUGUI>().raycastTarget=raycastTarget;
        }
    }

    public bool HasComponent<T>(GameObject obj) where T : Component
    {
        return obj.GetComponent<T>() != null;
    }

    public List<GameObject> ObjChangeList(GameObject obj)
    {
        var list = new List<GameObject>();
        list.Add(obj);
        return list;
    }

    public void ChangeImageActiveState(List<GameObject> objects, int state, float time)
    {
        for (int i = 0; i < objects.Count; i++)
        {
            if (HasComponent<Image>(objects[i]))
            {
                var img = objects[i].GetComponent<Image>();

                //画像の表示
                DOTween.ToAlpha(() => img.color,
                                x => img.color = x,
                                state,
                                time);
            }
            if (HasComponent<TextMeshProUGUI>(objects[i]))
            {
                var text = objects[i].GetComponent<TextMeshProUGUI>();

                //画像の表示
                DOTween.ToAlpha(() => text.color,
                                x => text.color = x,
                                state,
                                time);
            }
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

        RaycastTarget(gameoverCanvasObjects, true);

        RaycastTarget(clearCanvasObjects, false);

        //ゲームパネルの非表示
        Plate.SetActive(false);

        ChangeImageActiveState(gameoverCanvasObjects, 1, 0.5f);
    }

    public IEnumerator ClearProduction(List<GameObject> planetObjects)
    {
        //星たちを笑顔に
        for (int i = 0; i < planetObjects.Count; i++)
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

        RaycastTarget(gameoverCanvasObjects, false);

        RaycastTarget(clearCanvasObjects, true);

        //ゲームパネルの非表示
        Plate.SetActive(false);

        ChangeImageActiveState(clearCanvasObjects, 1, 0.5f);
    }

    public IEnumerator SceneTransition(Image buttonCoverImage, TextMeshProUGUI buttonText)
    {
        //Counttimeのチェックを通ってしまう
        gameEnd = true;

        buttonText.color = new Color(1, 1, 1);

        DOTween.To(
            () => buttonCoverImage.fillAmount,
            (num) => buttonCoverImage.fillAmount = num,
            1.0f,
            1.2f);

        yield return new WaitForSeconds(1.5f);

        ChangeImageActiveState(clearCanvasObjects, 0, 0.2f);

        ChangeImageActiveState(gameoverCanvasObjects, 0, 0.2f);

        //ボタン画像の非表示
        ChangeImageActiveState(ObjChangeList(buttonCoverImage.gameObject), 0, 1.2f);

        //遷移画像の表示
        ChangeImageActiveState(ObjChangeList(transitionCanvas), 1, 1.2f);

        yield return new WaitForSeconds(0.5f);

        ChangeImageActiveState(transitionCanvasObjects, 1, 1f);

        GameManager.Instance.isClearGame = false;

        GameManager.Instance.isPauseGame = false;

        async = SceneManager.LoadSceneAsync(nextSceneStr);

        while (!async.isDone)
        {
            transitionCanvasObjects[0].transform.rotation = Quaternion.Euler(0, 0, 360 * async.progress);
            yield return null;
        }
    }
}
