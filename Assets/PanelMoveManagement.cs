using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PanelMoveManagement : MonoBehaviour
{

    //タブごとに保管
    private Vector2[] previous_TapPosition=new Vector2[2];
    private Vector2[] current_TapPosition = new Vector2[2];

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.inputManager.tapinfo.is_tap)
        {
            //Tabをタップし始めた位置と終了位置を取得
            if (InputManager.inputManager.tapinfo.tap_Obj!=null&&InputManager.inputManager.tapinfo.tap_Obj.tag == "Tab")
            {
                //タップしているかどうか
                if(InputManager.inputManager.tapinfo.tap_touchPhase == TouchPhase.Ended) return;

                current_TapPosition[int.Parse(InputManager.inputManager.tapinfo.tap_Obj.name)] = InputManager.inputManager.tapinfo.tap_position;

                //Tabを回し始める
                RotateTab();
            }
        }
    }

    public void RotateTab()
    {
        Vector2 differencePos = current_TapPosition[int.Parse(InputManager.inputManager.tapinfo.tap_Obj.name)] - previous_TapPosition[int.Parse(InputManager.inputManager.tapinfo.tap_Obj.name)];

        Debug.Log(differencePos);

        float angle = Mathf.Atan2(differencePos.x, differencePos.y) * Mathf.Rad2Deg;

        GameObject tabParent = InputManager.inputManager.tapinfo.tap_Obj.transform.parent.gameObject;

        tabParent.transform.DOLocalRotateQuaternion(Quaternion.Euler
            (tabParent.transform.localRotation.x,tabParent.transform.localRotation.y,tabParent.transform.localRotation.z+(angle/10f))
            ,0.01f);

        previous_TapPosition[int.Parse(InputManager.inputManager.tapinfo.tap_Obj.name)] = current_TapPosition[int.Parse(InputManager.inputManager.tapinfo.tap_Obj.name)];
    }
}
