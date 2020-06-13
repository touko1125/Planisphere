using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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

    public GameObject Beem;

    // Start is called before the first frame update
    void Start()
    {
        panelRotateSequence = DOTween.Sequence();
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.Instance.tapinfo.is_tap)
        {
            //Tabをタップし始めた位置を取得
            if (getTapObj()!=null)
            {
                //タブをタップしているかどうか
                if (getTapObj().tag != "Tab") return;

                //タップしているかどうか
                if (InputManager.Instance.tapinfo.tap_touchPhase == TouchPhase.Ended) return;

                current_TapPosition[int.Parse(getTapObj().name)] = InputManager.Instance.tapinfo.tap_position;

                //Tabを回し始める
                RotateTab();

                lastTappedTab = getTapObj();

                //押していたのを示す
                isTaped = true;
            }
            else
            {
                if (isTaped)
                {
                    //ビームの描画
                    Beem.GetComponent<BeamComponent>().StartShotBeam(lastTappedTab);

                    lastTappedTab = null;

                    isTaped = false;
                }
            }
        }
        else
        {
            if (isTaped)
            {
                //ビームの描画
                Beem.GetComponent<BeamComponent>().StartShotBeam(lastTappedTab);

                lastTappedTab = null;

                isTaped = false;
            }

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
    }

    public void RotateTab()
    {
        //タップ位置と直前のタップ位置の差分のベクトルを取る
        Vector2 differencePos = current_TapPosition[int.Parse(getTapObj().name)] - previous_TapPosition[int.Parse(getTapObj().name)];

        //ベクトルの角度取得
        float angle = (Mathf.Atan2(differencePos.x, differencePos.y) * Mathf.Rad2Deg/4);

        GameObject tabParent = getTapObj().transform.parent.gameObject;

        //角度分だけ回し続ける
        panelRotateSequence.Append(tabParent.transform.DOLocalRotate(
            new Vector3(tabParent.transform.localEulerAngles.x,
                        tabParent.transform.localEulerAngles.y,
                        tabParent.transform.localEulerAngles.z-(angle/10f))
            ,Const.rotate_TIME
            ,RotateMode.FastBeyond360));

        previous_TapPosition[int.Parse(getTapObj().name)] = current_TapPosition[int.Parse(getTapObj().name)];

        //再生中でなければ
        if (AudioManager.Instance.AudioSources[0].isPlaying) return;

        //効果音再生
        AudioManager.Instance.PlayAudio(AudioManager.Instance.AudioClips[0], AudioManager.Instance.AudioSources[0], Const.volume_SE, false);
    }

    private GameObject getTapObj()
    {
        return InputManager.Instance.tapinfo.tap_Obj;
    }
}
