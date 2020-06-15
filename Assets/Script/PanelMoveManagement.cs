using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UniRx;
using TapClass;

public class PanelMoveManagement : MonoBehaviour
{

    //タブごとに保管
    private Vector2[] previous_TapPosition=new Vector2[2];
    private Vector2[] current_TapPosition = new Vector2[2];

    private bool isTaped;

    //最後に触ってたタブ
    private GameObject lastTappedTab;

    //パネルが回転するTweenをここにいれる
    private Sequence panelRotateSequence;

    private Tween tween;

    public GameObject Beem;

    private int i;

    // Start is called before the first frame update
    void Start()
    {
        panelRotateSequence = DOTween.Sequence();
    }

    // Update is called once per frame
    void Update()
    {
        //if (InputManager.Instance.tapinfo.Value.is_tap)
        //{
        //    //Tabをタップし始めた位置を取得
        //    if (getTapObj()!=null)
        //    {
        //        //タブをタップしているかどうか
        //        if (getTapObj().tag != "Tab") return;

        //        //タップしているかどうか
        //        //if (InputManager.Instance.tapinfo.Value.tap_touchPhase == TouchPhase.Ended) return;

        //        //current_TapPosition[int.Parse(getTapObj().name)] = InputManager.Instance.tapinfo.Value.tap_position;

        //        //Tabを回し始める
        //        RotateTab();

        //        lastTappedTab = getTapObj();

        //        //押していたのを示す
        //        isTaped = true;
        //    }
        //}
        //else
        //{
        //    if (isTaped)
        //    {
        //        Debug.Log("1");

        //        //ビームの描画
        //        Beem.GetComponent<BeamComponent>().StartShotBeam(lastTappedTab);

        //        lastTappedTab = null;

        //        isTaped = false;
        //    }

        //    ResetTabPos();
        //}
    }

    public void ResetTabPos()
    {
        //リセット
        for (int i = 0; i < previous_TapPosition.Length; i++)
        {
            previous_TapPosition[i] = Vector2.zero;
        }
        for (int i = 0; i < current_TapPosition.Length; i++)
        {
            current_TapPosition[i] = Vector2.zero;
        }
    }

    public void RotateTab(Tap tap)
    {
        //父 回転用
        GameObject tabParent = tap.tap_Obj.transform.parent.gameObject;

        //曾祖父 中心位置取得用
        Vector2 parentCenterPanel = tabParent.transform.parent.transform.parent.position;

        Debug.Log(parentCenterPanel);

        //中心から初期タップ位置への差分
        Vector2 center_startTapPos_Vector = tap.start_tapPosition - parentCenterPanel;

        //中心からちょっと移動したとこへの差分
        Vector2 center_endTapPos_Vector = tap.end_tapPosition - parentCenterPanel;

        Debug.Log(center_startTapPos_Vector);

        Debug.Log(center_endTapPos_Vector);

        //初期タップ位置の中心角度取得
        float angle1 = Mathf.Atan2(center_startTapPos_Vector.x,center_startTapPos_Vector.y) * Mathf.Rad2Deg;
        
        //最終タップ位置の中心角度取得
        float angle2 = Mathf.Atan2(center_endTapPos_Vector.x,center_endTapPos_Vector.y) * Mathf.Rad2Deg;

        //微小角度差分
        float angle = angle2 - angle1;

        Debug.Log("angle1=" + angle1);

        Debug.Log("angle2=" + angle2);

        Debug.Log("angle=" + angle);

        //角度分だけ回し続ける
        panelRotateSequence
            .Append(
                tabParent.transform.DOLocalRotate(
                new Vector3(tabParent.transform.localEulerAngles.x,
                            tabParent.transform.localEulerAngles.y,
                            tabParent.transform.localEulerAngles.z + angle)
                ,0.001f*(tap.end_tapPosition-tap.start_tapPosition).magnitude
                , RotateMode.FastBeyond360));

        previous_TapPosition[int.Parse(tap.tap_Obj.name)] = current_TapPosition[int.Parse(tap.tap_Obj.name)];

        //再生中でなければ
        if (AudioManager.Instance.AudioSources[0].isPlaying) return;

        //動きなし
        if ((tap.end_tapPosition - tap.start_tapPosition).magnitude == 0) return;

        //効果音再生
        AudioManager.Instance.PlayAudio(AudioManager.Instance.AudioClips[0], AudioManager.Instance.AudioSources[0], Const.volume_SE, false);
    }
}
