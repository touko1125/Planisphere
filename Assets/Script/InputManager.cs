using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TapClass;
using UniRx;

public class InputManager : SingletonMonoBehaviour<InputManager>
{
    public Tap tapinfo;

    private bool isPlayOnApplication;

    private Vector2 previousTapPos;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ReceiveInput();
    }

    public void ReceiveInput()
    {
        Ray cameraRay = Camera.main.ScreenPointToRay(previousTapPos);
        RaycastHit2D hit = Physics2D.Raycast(cameraRay.origin,cameraRay.direction);

        //タップしている場所の座標を変換
        var tapObj = hit ? hit.collider.gameObject : null;

        //タッチされているかのチェック
        if (Input.touchCount > 0)
        {
            //押した瞬間
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                previousTapPos = Input.GetTouch(0).position;
            }
            //押している間
            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                tapinfo = new Tap(true, previousTapPos, ChangeScreenPosIntoWorldPos(Input.GetTouch(0).position), tapObj);
                previousTapPos = Input.GetTouch(0).position;
            }
            //離した瞬間
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                tapinfo = new Tap(false, previousTapPos, ChangeScreenPosIntoWorldPos(Input.GetTouch(0).position), tapObj);
                previousTapPos = Vector2.zero;
            }

            isPlayOnApplication = true;
        }

        if (!isPlayOnApplication)
        {
            //押した瞬間
            if (Input.GetMouseButtonDown(0))
            {
                previousTapPos = Input.mousePosition;
            }
            //押している間
            if (Input.GetMouseButton(0))
            {
                tapinfo = new Tap(true, previousTapPos, ChangeScreenPosIntoWorldPos((Vector2)Input.mousePosition), tapObj);
                previousTapPos = Input.mousePosition;
            }
            //離した瞬間
            if (Input.GetMouseButtonUp(0))
            {
                tapinfo = new Tap(false, previousTapPos, ChangeScreenPosIntoWorldPos((Vector2)Input.mousePosition), tapObj);
                previousTapPos = Vector2.zero;
            }
        }
    }

    public Vector2 ChangeScreenPosIntoWorldPos(Vector2 screenPos)
    {
        return Camera.main.ScreenToWorldPoint(screenPos);
    }
}
