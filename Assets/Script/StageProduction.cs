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
    private Image[] LoadPlanetImage;

    private string NextSceneStr;

    private float currentTime;

    private AsyncOperation async;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CountTime();
    }

    public void CountTime()
    {
        if (GameManager.Instance.isClearGame) return;

        if (GameManager.Instance.isPauseGame) return;

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

        for (int i = 0; i < planetManager.planetList.Count; i++)
        {
            planetManager.planetList[i].GetComponent<PlanetComponent>().ChangeFace(Enum.PlanetFace.nomal);
        }

        yield return new WaitForSeconds(0.5f);

        //ゲームパネルの非表示
        Panel.SetActive(false);

        for (int i = 0; i < ClearImages.Length; i++)
        {
            ClearImages[i].raycastTarget = false;
        }

        for (int i = 0; i < GameoverImages.Length; i++)
        {
            var img = GameoverImages[i];

            img.gameObject.SetActive(true);

            img.raycastTarget = true;

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

        if((int)(Enum.Stage)System.Enum.Parse(typeof(Enum.Stage), SceneManager.GetActiveScene().name) > GameManager.Instance.clearStageNum)
        {
            PlayerPrefs.SetInt(Const.clearStageNumKey
            ,(int)(Enum.Stage)System.Enum.Parse(typeof(Enum.Stage), SceneManager.GetActiveScene().name));

            PlayerPrefs.Save();
        }

        yield return new WaitForSeconds(0.5f);

        //ゲームパネルの非表示
        Panel.SetActive(false);

        for (int i = 0; i < GameoverImages.Length; i++)
        {
            GameoverImages[i].raycastTarget = false;
        }

        for (int i = 0; i < ClearImages.Length; i++)
        {
            var img = ClearImages[i];

            img.gameObject.SetActive(true);

            img.raycastTarget = true;

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
        NextSceneStr = "Collection";

        StartCoroutine(SceneTransition(buttonObj.gameObject.transform.GetChild(0).gameObject.GetComponent<Image>(),
            buttonObj.gameObject.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>()));
    }

    public void PressNext(Image buttonObj)
    {
        var currentSceneStr = (Enum.Stage)System.Enum.Parse(typeof(Enum.Stage),SceneManager.GetActiveScene().name);

        NextSceneStr = ((Enum.Stage)System.Enum.ToObject(typeof(Enum.Stage),(int)currentSceneStr+1)).ToString();

        StartCoroutine(SceneTransition(buttonObj.gameObject.transform.Find("Fill").gameObject.GetComponent<Image>(),
            buttonObj.gameObject.transform.Find("NEXT").gameObject.GetComponent<TextMeshProUGUI>()));
    }

    public void PressRetry(Image buttonObj)
    {
        var currentSceneStr = SceneManager.GetActiveScene().name;

        NextSceneStr = currentSceneStr;

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

        for(int i = 0; i < LoadPlanetImage.Length; i++)
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

        while (!async.isDone)
        {
            LoadPlanetImage[0].transform.rotation = Quaternion.Euler(0, 0, 360 * async.progress);
            yield return null;
        }
    }
}
