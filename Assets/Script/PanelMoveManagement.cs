using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UniRx;
using TapClass;

public class PanelMoveManagement : MonoBehaviour
{
    public bool isMoved;

    public bool isCorrecting;

    public int TabNum;

    public float angle;

    public GameObject previousTab;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RotateTab(Tap tap,GameObject tapTab)
    {
        //父 回転用
        GameObject tabParent = tapTab.transform.parent.gameObject;

        //曾祖父 中心位置取得用
        Vector2 parentCenterPanel = tabParent.transform.parent.transform.parent.position;

        //中心から初期タップ位置への差分
        Vector2 center_startTapPos_Vector = tap.start_tapPosition - parentCenterPanel;

        //中心からちょっと移動したとこへの差分
        Vector2 center_endTapPos_Vector = tap.end_tapPosition - parentCenterPanel;

        //初期タップ位置の中心角度取得
        float angle1 = Mathf.Atan2(center_startTapPos_Vector.x,center_startTapPos_Vector.y) * Mathf.Rad2Deg;
        
        //最終タップ位置の中心角度取得
        float angle2 = Mathf.Atan2(center_endTapPos_Vector.x,center_endTapPos_Vector.y) * Mathf.Rad2Deg;

        //微小角度差分
        angle = angle2 - angle1;

        //角度分だけ回し続ける
        tabParent.transform.DOLocalRotate(
            new Vector3(tabParent.transform.localEulerAngles.x,
                        tabParent.transform.localEulerAngles.y,
                        tabParent.transform.localEulerAngles.z - angle)
                        ,0.001f * ((tap.end_tapPosition - tap.start_tapPosition).magnitude)
                        ,RotateMode.FastBeyond360);

        isMoved = true;

        TabNum = int.Parse(tapTab.gameObject.name);

        previousTab = tapTab;

        //再生中でなければ
        if (AudioManager.Instance.AudioSources[0].isPlaying) return;

        //動きなし
        if ((tap.end_tapPosition - tap.start_tapPosition).magnitude == 0) return;

        //効果音再生
        AudioManager.Instance.PlayAudio(AudioManager.Instance.AudioClips[0], AudioManager.Instance.AudioSources[0],0.1f, false);
    }

    public IEnumerator CorrectPanelRoatate(float angle,GameObject tabObj)
    {
        isCorrecting = true;

        //効果音再生
        AudioManager.Instance.PlayAudio(AudioManager.Instance.AudioClips[1], AudioManager.Instance.AudioSources[0], Const.volume_SE, false);

        //角度分だけ回し続ける
        tabObj.transform.parent.DOLocalRotate(
            new Vector3(tabObj.transform.localEulerAngles.x,
                        tabObj.transform.localEulerAngles.y,
                        tabObj.transform.localEulerAngles.z - angle)
                        , 0.2f
                        , RotateMode.FastBeyond360);

        yield return new WaitForSeconds(0.5f);       

        isMoved = false;

        isCorrecting = false;
    }
}
