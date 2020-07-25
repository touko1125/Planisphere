using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class StageSelectManager : MonoBehaviour
{
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

    public void PressStage(GameObject buttonObj)
    {
        NextSceneStr = ((GameManager.Stage)System.Enum.Parse(typeof(GameManager.Stage), buttonObj.name)).ToString();

        StartCoroutine(SceneTransition());
    }

    public IEnumerator SceneTransition()
    {
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

        while (!async.isDone)
        {
            LoadPlanetImage[0].transform.rotation = Quaternion.Euler(0, 0, 360 * async.progress);
            yield return null;
        }
    }
}

