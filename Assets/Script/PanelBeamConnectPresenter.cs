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
        if (GameManager.Instance.isClearGame) return;

        if (!panelMovement.isMoved) return;

        tapObj = panelMovement.previousTab;

        beamComponent.ThinandDestroyLine(int.Parse(tapObj.name));

        if (getTapInfo().is_tap) return;

        if (beamComponent.isDrawLine) return;

        Debug.Log("ddddd");

        BeamSet(panelMovement.TabNum);

        panelMovement.isMoved = false;
    }

    public void BeamSet(int TabNum)
    {
        //タブ位置からビーム発射位置にする調整
        Vector3 direction = tapObj.transform.parent.transform.parent.transform.parent.position - tapObj.transform.position;

        Vector3 rayOrigin = Vector3.zero;

        //タブによってビーム発射位置調整の切り替え
        switch (TabNum)
        {
            case 0:
                rayOrigin = new Vector3(tapObj.transform.position.x + (direction / 17).x, tapObj.transform.position.y + (direction / 17).y, -1);
                break;
            case 1:
                rayOrigin = new Vector3(tapObj.transform.position.x + (direction / 13).x, tapObj.transform.position.y + (direction / 13).y, -1);
                break;
        }

        //コルーチンの設定(最初はタブから対角線)
        beamComponent.shot_beam_coroutine=beamComponent.ShotBeam(rayOrigin,direction*Const.radius,TabNum);
        StartCoroutine(beamComponent.shot_beam_coroutine);
    }

    public void ObserveInput()
    {
        if (GameManager.Instance.isClearGame) return;

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
