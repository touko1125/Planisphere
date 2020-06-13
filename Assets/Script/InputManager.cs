using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TapClass;
using UniRx;

public class InputManager : SingletonMonoBehaviour<InputManager>
{
    public Tap tapinfo;

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ReceiveInput()
    {
        Vector2 worldTapPos = Camera.main.ScreenToWorldPoint(tapinfo.tap_position);
        RaycastHit2D hit = Physics2D.Raycast(worldTapPos, Vector2.zero);

        //タップしている場所の座標を変換
        var tapObj = hit ? hit.collider.gameObject : null;

        //実機かEditor上かの区別
        if (Application.isEditor)
        {
            //押した瞬間
            if (Input.GetMouseButtonDown(0))
            {
                tapinfo = new Tap(true, Input.mousePosition, TouchPhase.Began, tapObj);
            }
            if (Input.GetMouseButton(0))
            {
                tapinfo = new Tap(true, Input.mousePosition, TouchPhase.Moved, tapObj);
            }
            //離した瞬間
            if (Input.GetMouseButtonUp(0))
            {
                tapinfo = new Tap(false, Input.mousePosition, TouchPhase.Ended, tapObj);
            }
        }
        else
        {
            //タッチされているかのチェック
            if (Input.touchCount > 0)
            {
                //一番最初は終わりの時だけfalseをいれるため
                tapinfo = new Tap((Input.GetTouch(0).phase != TouchPhase.Ended), Input.GetTouch(0).position, Input.GetTouch(0).phase, tapObj);
            }
        }
    }
}
