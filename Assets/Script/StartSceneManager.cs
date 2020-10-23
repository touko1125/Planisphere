using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using TMPro;

public class StartSceneManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI titleText;

    [SerializeField]
    private Image[] LoadPlanetImage = new Image[3];

    [SerializeField]
    private Image frontBackDestination;

    private AsyncOperation async;

    private bool isSceneTransition;

    private Tween titleTextTween;

    // Start is called before the first frame update
    void Start()
    {
        //ちかちか
        titleTextTween = titleText.GetComponent<TextMeshProUGUI>().DOFade(0.0f, 1.0f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo).Play();
    }

    // Update is called once per frame
    void Update()
    {
        //入力監視
        ObserveInput();
    }

    public void ObserveInput()
    {
        if (!InputManager.Instance.tapinfo.is_tap) return;

        if (isSceneTransition) return;

        StartCoroutine(SceneTransition());
    }

    public IEnumerator SceneTransition()
    {
        isSceneTransition = true;

        //ロード画面上に
        LoadPlanetImage[3].rectTransform.DOAnchorPos(frontBackDestination.rectTransform.anchoredPosition, 1.5f);

        //ちかちか解除
        titleTextTween.Kill();

        //タイトルの文字フェードアウト
        DOTween.ToAlpha(() => titleText.GetComponent<TextMeshProUGUI>().color,
                           color => titleText.GetComponent<TextMeshProUGUI>().color = color,
                           0,
                           0.5f);

        //ロード用の惑星も表示
        for (int i = 0; i < LoadPlanetImage.Length-1; i++)
        {
            var planetImage = LoadPlanetImage[i];

            DOTween.ToAlpha(() => planetImage.color,
                            color => planetImage.color = color,
                            1,
                            1f);
        }

        //演出待ち
        yield return new WaitForSeconds(1.0f);

        //ホーム画面遷移
        async = SceneManager.LoadSceneAsync(Const.homeStageKey);

        while (!async.isDone)
        {
            LoadPlanetImage[0].transform.rotation = Quaternion.Euler(0, 0, Const.degAngle360 * async.progress);
            yield return null;
        }
    }
}
