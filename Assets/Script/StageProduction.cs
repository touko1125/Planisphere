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
    private GameObject Panel;

    [SerializeField]
    private Image LoadUI;

    [SerializeField]
    private Image[] LoadPlanetImage;

    private string NextSceneStr;

    private AsyncOperation async;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator ClearProduction(List<GameObject> planetObjects)
    {
        for(int i = 0; i < planetObjects.Count; i++)
        {
            planetObjects[i].GetComponent<PlanetComponent>().ChangeFace(GameManager.PlanetFace.smile);
        }

        yield return new WaitForSeconds(0.5f);

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
        NextSceneStr = "StageSelect";

        StartCoroutine(SceneTransition(buttonObj.gameObject.transform.GetChild(0).gameObject.GetComponent<Image>(),
            buttonObj.gameObject.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>()));
    }

    public void PressNext(Image buttonObj)
    {
        var currentSceneStr = (GameManager.Stage)System.Enum.Parse(typeof(GameManager.Stage),SceneManager.GetActiveScene().name);

        NextSceneStr = ((GameManager.Stage)System.Enum.ToObject(typeof(GameManager.Stage),(int)currentSceneStr+1)).ToString();

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

        PlayerPrefs.SetInt(Const.clearStageNumKey
     , (int)(GameManager.Stage)System.Enum.Parse(typeof(GameManager.Stage), SceneManager.GetActiveScene().name));

        PlayerPrefs.Save();

        GameManager.Instance.isClearGame = false;

        while (!async.isDone)
        {
            LoadPlanetImage[0].transform.rotation = Quaternion.Euler(0, 0, 360 * async.progress);
            yield return null;
        }
    }
}
