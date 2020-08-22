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

    private GameObject previousTapTab;

    private GameObject tapTab;

    private float latestAngle;

    private bool controlCorrect;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ObserveInput();
        ObserveProgressMovePanel();
        ObserveCorrectRotate();
    }

    public void ObserveProgressMovePanel()
    {
        if (GameManager.Instance.isClearGame) return;

        if (GameManager.Instance.isPauseGame) return;

        if (!panelMovement.isMoved) return;

        tapObj = panelMovement.previousTab;

        if (getTapInfo().is_tap) return;

        if (beamComponent.isDrawLine) return;

        if (panelMovement.isCorrecting) return;

        CorrectionRotate(-tapObj.transform.parent.localEulerAngles.z, tapObj);

        if (tapObj.tag == "CoverTab") return;

        //線のリセット
        beamComponent.ThinandDestroyLine(int.Parse(tapObj.name));

        //星の状態のリセット
        beamComponent.RestPlanetState(int.Parse(tapObj.name));

        controlCorrect = true;
    }

    public void ObserveCorrectRotate()
    {
        if (!controlCorrect) return;

        if (panelMovement.isCorrecting) return;

        controlCorrect = false;

        if (tapObj.tag == "CoverTab") return;

        BeamSet(latestAngle, panelMovement.TabNum);
    }

    public void CorrectionRotate(float angle,GameObject tapObj)
    {
        //15の倍数に補正
        float nearlistAngle = Math.Abs(angle - ((int)(angle / 15)) * 15) < (15 / 2) ? ((int)(angle / 15)) * 15 : ((int)(angle / 15) - 1) * 15;

        latestAngle = nearlistAngle;

        IEnumerator correctPanelRotate = panelMovement.CorrectPanelRoatate(nearlistAngle, tapObj);

        StartCoroutine(correctPanelRotate);
    }

    public void BeamSet(float angle,int TabNum)
    {
        //タブ位置からビーム発射位置にする調整
        Vector3 direction = new Vector3(0,0,-1) - tapObj.transform.position;

        Vector3 rayOrigin = Vector3.zero;

        //タブによってビーム発射位置調整の切り替え
        switch (TabNum)
        {
            case 0:
                rayOrigin = new Vector3(panelMovement.previousTab.transform.position.x + (direction / 17).x, panelMovement.previousTab.transform.position.y + (direction / 17).y, -1);
                break;
            case 1:
                rayOrigin = new Vector3(panelMovement.previousTab.transform.position.x + (direction / 13).x, panelMovement.previousTab.transform.position.y + (direction / 13).y, -1);
                break;
        }

        Debug.Log("BeamShot!!");

        //コルーチンの設定(最初はタブから対角線)
        beamComponent.shot_beam_coroutine=beamComponent.ShotBeam(rayOrigin,direction*Const.radius,TabNum);
        StartCoroutine(beamComponent.shot_beam_coroutine);
    }

    public void ObserveInput()
    {
        if (GameManager.Instance.isClearGame) return;

        if (GameManager.Instance.isPauseGame) return;

        if (beamComponent.isDrawLine) return;

        if (getTapInfo() == null) return;

        if (getTapInfo().is_tap == false)
        {
            tapTab = null;
            previousTapTab = null;
            return;
        }

        if (getTapInfo().tap_Obj == null) return;

        if (getTapInfo().tap_Obj.tag != "Tab" && getTapInfo().tap_Obj.tag != "CoverTab")
        {
            if (previousTapTab == null) return;
            panelMovement.RotateTab(getTapInfo(), previousTapTab);
            return;
        }

        tapTab = getTapInfo().tap_Obj;

        if (tapTab.tag == "Tab" || getTapInfo().tap_Obj.tag == "CoverTab")
        {
            if (previousTapTab != null)
            {
                if (tapTab != previousTapTab)
                {
                    panelMovement.RotateTab(getTapInfo(), previousTapTab);
                    return;
                }
            }
            previousTapTab = tapTab;
            panelMovement.RotateTab(getTapInfo(), getTapInfo().tap_Obj);
        }
    }

    public Tap getTapInfo()
    {
        return InputManager.Instance.tapinfo;
    }
}
