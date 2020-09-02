using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class StageProduction : MonoBehaviour
{
    [SerializeField]
    private Image[] ClearImages;

    [SerializeField]
    private TextMeshProUGUI[] ClearTexts;

    [SerializeField]
    private Image[] GameoverImages;

    [SerializeField]
    private TextMeshProUGUI[] GameoverTexts;

    [SerializeField]
    private GameObject ClockParent;

    [SerializeField]
    private GameObject Panel;

    [SerializeField]
    private Image LoadUI;

    [SerializeField]
    private List<Image> LoadPlanetImages;

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
        RaycastTarget(GameoverImages, false);

        RaycastTarget(ClearImages, false);
    }

    // Update is called once per frame
    void Update()
    {
        //経過時間観測
        CountTime();
    }

    public void RaycastTarget(Image[] objects,bool raycastTarget)
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

        ClockParent.transform.localRotation = Quaternion.Euler(0, 0, 70 - (140 * (currentTime / Const.limitedTime)));
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

        RaycastTarget(GameoverImages, true);

        RaycastTarget(ClearImages, false);

        //ゲームパネルの非表示
        Panel.SetActive(false);

        for (int i = 0; i < GameoverImages.Length; i++)
        {
            var img = GameoverImages[i];

            img.gameObject.SetActive(true);

            //画像の表示
            DOTween.ToAlpha(() => img.color,
                            color => img.color = color,
                            1,
                            0.5f);
        }

        for (int i = 0; i < GameoverTexts.Length; i++)
        {
            var txt = GameoverTexts[i];

            //テキストの表示
            DOTween.ToAlpha(() => txt.color,
                            color => txt.color = color,
                            1,
                            0.5f);
        }
    }

    public IEnumerator ClearProduction(List<GameObject> planetObjects)
    {
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

        yield return new WaitForSeconds(0.3f);

        RaycastTarget(GameoverImages, false);

        RaycastTarget(ClearImages, true);

        //ゲームパネルの非表示
        Panel.SetActive(false);

        for (int i = 0; i < ClearImages.Length; i++)
        {
            var img = ClearImages[i];

            img.gameObject.SetActive(true);

            //画像の表示
            DOTween.ToAlpha(() => img.color,
                            color => img.color = color,
                            1,
                            0.5f);
        }

        for (int i = 0; i < ClearTexts.Length; i++)
        {
            var txt = ClearTexts[i];

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
        buttonText.color = new Color(1, 1, 1);

        DOTween.To(
            ()=>buttonObj.fillAmount,
            (num)=>buttonObj.fillAmount=num,
            1.0f,
            1.2f);

        yield return new WaitForSeconds(1.5f);

        for (int i = 0; i < ClearImages.Length; i++)
        {
            var img = ClearImages[i];

            //画像の非表示
            DOTween.ToAlpha(() => img.color,
                            color => img.color = color,
                            0,
                            0.2f);
        }

        for (int i = 0; i < ClearTexts.Length; i++)
        {
            var txt = ClearTexts[i];

            //テキストの非表示
            DOTween.ToAlpha(() => txt.color,
                            color => txt.color = color,
                            0,
                            0.2f);
        }

        for (int i = 0; i < GameoverImages.Length; i++)
        {
            var img = GameoverImages[i];

            //画像の表示
            DOTween.ToAlpha(() => img.color,
                            color => img.color = color,
                            0,
                            0.2f);
        }

        for (int i = 0; i < GameoverTexts.Length; i++)
        {
            var txt = GameoverTexts[i];

            //テキストの表示
            DOTween.ToAlpha(() => txt.color,
                            color => txt.color = color,
                            0,
                            0.2f);
        }

        //画像の非表示
        DOTween.ToAlpha(() => buttonObj.color,
                        color => buttonObj.color = color,
                        0,
                        0.2f);

        //画像の表示
        DOTween.ToAlpha(() => LoadUI.color,
                        color => LoadUI.color = color,
                        1,
                        1f);

        yield return new WaitForSeconds(0.5f);

        LoadPlanetImages.Add(LoadPlanetImages[0].gameObject.transform.Find("Planet").gameObject.GetComponent<Image>());

        for(int i = 0; i < LoadPlanetImages.Count; i++)
        {
            var planetImage = LoadPlanetImages[i];

            //ロード用の惑星も表示
            DOTween.ToAlpha(() => planetImage.color,
                            color => planetImage.color = color,
                            1,
                            1f);
        }

        //Counttimeのチェックを通ってしまう
        gameEnd = true;

        GameManager.Instance.isClearGame = false;

        GameManager.Instance.isPauseGame = false;

        async = SceneManager.LoadSceneAsync(nextSceneStr);

        while (!async.isDone)
        {
            LoadPlanetImages[0].transform.rotation = Quaternion.Euler(0, 0, 360 * async.progress);
            yield return null;
        }
    }
}
