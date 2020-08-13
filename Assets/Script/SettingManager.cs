using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;


public class SettingManager : MonoBehaviour
{
    [SerializeField]
    private GameObject settingScreen;

    // Start is called before the first frame update
    void Start()
    {
        //初期化
        gameObject.transform.Find("BGM").Find("BGMSlider").gameObject.GetComponent<Slider>().value = AudioManager.Instance.volumeBGM;
        gameObject.transform.Find("SE").Find("SESlider").gameObject.GetComponent<Slider>().value = AudioManager.Instance.volumeSE;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SettingOpen()
    {
        //時間カウントストップ
        GameManager.Instance.isPauseGame = true;

        //黒背景
        DOTween.ToAlpha(() => settingScreen.GetComponent<Image>().color
                      , (x) => settingScreen.GetComponent<Image>().color = x
                      , 1.0f, 0.2f);

        var activeSettingScreenPos = settingScreen.transform.Find("ActiveSettingPoint");

        gameObject.GetComponent<RectTransform>().DOMove(activeSettingScreenPos.position, 0.5f);
    }

    public void SettingClose()
    {
        //時間カウントストップ
        GameManager.Instance.isPauseGame = false;

        var activeSettingScreenPos = settingScreen.transform.Find("IdleSettingPoint");

        gameObject.GetComponent<RectTransform>().DOMove(activeSettingScreenPos.position, 0.5f);

        //黒背景
        DOTween.ToAlpha(() => settingScreen.GetComponent<Image>().color
                      , (x) => settingScreen.GetComponent<Image>().color = x
                      , 0f, 0.2f);
    }

    public void PlusVolume(int audioSourceNum)
    {
        var newAudioValue = AudioManager.Instance.AudioSources[audioSourceNum].GetComponent<AudioSource>().volume + 0.1f > 1.0f ? 1.0f : AudioManager.Instance.AudioSources[audioSourceNum].GetComponent<AudioSource>().volume + 0.1f;

        AudioManager.Instance.ChangeVolume(newAudioValue,audioSourceNum);

        switch (audioSourceNum)
        {
            case 0:
                gameObject.transform.Find("BGM").Find("BGMSlider").gameObject.GetComponent<Slider>().value = newAudioValue;
                break;
            case 1:
                gameObject.transform.Find("SE").Find("SESlider").gameObject.GetComponent<Slider>().value = newAudioValue;
                break;
        }
    }

    public void MinusVolume(int audioSourceNum)
    {
        var newAudioValue = AudioManager.Instance.AudioSources[audioSourceNum].GetComponent<AudioSource>().volume - 0.1f < 0 ? 0 : AudioManager.Instance.AudioSources[audioSourceNum].GetComponent<AudioSource>().volume - 0.1f;

        AudioManager.Instance.ChangeVolume(newAudioValue, audioSourceNum);

        switch (audioSourceNum)
        {
            case 0:
                gameObject.transform.Find("BGM").Find("BGMSlider").gameObject.GetComponent<Slider>().value = newAudioValue;
                break;
            case 1:
                gameObject.transform.Find("SE").Find("SESlider").gameObject.GetComponent<Slider>().value = newAudioValue;
                break;
        }
    }
}
