using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PanelMoveManagement : MonoBehaviour
{

    //タブごとに保管
    private Vector2[] previous_TapPosition=new Vector2[2];
    private Vector2[] current_TapPosition = new Vector2[2];

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
        if (InputManager.inputManager.tapinfo.is_tap)
        {
            //Tabをタップし始めた位置を取得
            if (InputManager.inputManager.tapinfo.tap_Obj!=null&&InputManager.inputManager.tapinfo.tap_Obj.tag == "Tab")
            {
                //タップしているかどうか
                if(InputManager.inputManager.tapinfo.tap_touchPhase == TouchPhase.Ended) return;

                current_TapPosition[int.Parse(InputManager.inputManager.tapinfo.tap_Obj.name)] = InputManager.inputManager.tapinfo.tap_position;

                //Tabを回し始める
                RotateTab();
            }
        }
        else
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
    }

    public void RotateTab()
    {
        //タップ位置と直前のタップ位置の差分のベクトルを取る
        Vector2 differencePos = current_TapPosition[int.Parse(InputManager.inputManager.tapinfo.tap_Obj.name)] - previous_TapPosition[int.Parse(InputManager.inputManager.tapinfo.tap_Obj.name)];

        //ベクトルの角度取得
        float angle = (Mathf.Atan2(differencePos.x, differencePos.y) * Mathf.Rad2Deg/4);

        GameObject tabParent = InputManager.inputManager.tapinfo.tap_Obj.transform.parent.gameObject;

        //角度分だけ回し続ける
        panelRotateSequence.Append(tabParent.transform.DOLocalRotate(new Vector3(tabParent.transform.localEulerAngles.x,tabParent.transform.localEulerAngles.y,tabParent.transform.localEulerAngles.z-(angle/10f)),0.07f,RotateMode.FastBeyond360));

        previous_TapPosition[int.Parse(InputManager.inputManager.tapinfo.tap_Obj.name)] = current_TapPosition[int.Parse(InputManager.inputManager.tapinfo.tap_Obj.name)];

        //再生中でなければ
        if (AudioManager.audiomanager.AudioSources[0].isPlaying) return;

        //効果音再生
        AudioManager.audiomanager.PlayAudio(AudioManager.audiomanager.AudioClips[0], AudioManager.audiomanager.AudioSources[0], 0.9f, false);

        //ビームの描画
        Beem.GetComponent<BeamComponent>().ShotBeam(InputManager.inputManager.tapinfo.tap_Obj);
    }
}
