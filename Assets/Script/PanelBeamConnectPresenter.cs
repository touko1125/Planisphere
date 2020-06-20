using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TapClass;
using UniRx;

public static class SkipLatestValue
{
    public static IObservable<T> SkipLatesetValueOnSubscribe<T>(this IReactiveProperty<T> source)
    {
        return source.HasValue ? source.Skip(1) : source;   //初期値を飛ばす
    }
}

public class PanelBeamConnectPresenter : MonoBehaviour
{
    [SerializeField]
    private PanelMoveManagement panelMovement;
    [SerializeField]
    private BeamComponent beamComponent;

    private GameObject tapObj;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ObserveInput();
        ObserveProgressMovePanel();
    }

    public void ObserveProgressMovePanel()
    {
        if (!panelMovement.isMoved) return;

        tapObj = panelMovement.previousTab;

        if (getTapInfo().is_tap) return;

        if (beamComponent.isDrawLine) return;

        BeamSet();

        panelMovement.isMoved = false;
    }

    public void BeamSet()
    {
        //タブ位置からビーム発射位置にする調整
        Vector3 direction = tapObj.transform.parent.transform.parent.transform.parent.position - tapObj.transform.position;
        Vector3 rayOrigin = new Vector3(tapObj.transform.position.x + (direction/7).x, tapObj.transform.position.y + (direction/7).y, -1);

        //コルーチンの設定(最初はタブから対角線)
        beamComponent.shot_beam_coroutine=beamComponent.ShotBeam
            (rayOrigin
            ,tapObj.transform.parent.transform.parent.transform.parent.position);
        StartCoroutine(beamComponent.shot_beam_coroutine);
    }

    public void ObserveInput()
    {
        if (getTapInfo() == null) return;

        if (getTapInfo().is_tap == false) return;

        if (getTapInfo().tap_Obj == null) return;

        if (getTapInfo().tap_Obj.tag != "Tab") return;

        panelMovement.RotateTab(getTapInfo());
    }

    public Tap getTapInfo()
    {
        return InputManager.Instance.tapinfo;
    }
}
