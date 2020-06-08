using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TapClass
{
    [System.Serializable]
    public class Tap
    {
        public bool is_tap;   //タップされているか
        public Vector2 tap_position;   //タップ座標
        public TouchPhase tap_touchPhase;   //タップ状態
        public GameObject tap_Obj;  //タップされたもの

        //positionはnull許容型
        public Tap(bool isTap = false, Vector2? tapPosition = null, TouchPhase tapTouchPhase = TouchPhase.Began, GameObject tapObj = null)
        {
            this.is_tap = isTap;

            if (tapPosition == null)
            {
                this.tap_position = new Vector2(0, 0);
            }
            else
            {
                this.tap_position = (Vector2)tapPosition;
            }

            this.tap_touchPhase = tapTouchPhase;

            this.tap_Obj = tapObj;
        }
    }
}