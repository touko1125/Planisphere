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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ObserveInput();
    }

    public void ObserveInput()
    {
        if (getTapInfo() == null) return;

        if (getTapInfo().is_tap == false) return;

        if (getTapInfo().tap_Obj == null) return;

        if (getTapInfo().tap_Obj.tag != "Tab") return;

        Debug.Log("aaaaaaa");

        panelMovement.RotateTab(getTapInfo());
    }

    public Tap getTapInfo()
    {
        return InputManager.Instance.tapinfo;
    }
}
