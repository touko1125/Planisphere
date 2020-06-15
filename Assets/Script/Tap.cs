using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TapClass
{
    [System.Serializable]
    public class Tap
    {
        public bool is_tap;   //タップされているか
        public Vector2 start_tapPosition;   //最初のタップ座標
        public Vector2 end_tapPosition;   //最後のタップ座標
        public GameObject tap_Obj;  //タップされたもの

        //positionはnull許容型
        public Tap(bool isTap = false, Vector2? startTapPosition = null, Vector2? endTapPosition = null, GameObject tapObj = null)
        {
            this.is_tap = isTap;

            if (startTapPosition == null)
            {
                this.start_tapPosition = new Vector2(0, 0);
            }
            else
            {
                this.start_tapPosition = (Vector2)startTapPosition;
            }

            if (endTapPosition == null)
            {
                this.end_tapPosition = new Vector2(0, 0);
            }
            else
            {
                this.end_tapPosition = (Vector2)endTapPosition;
            }

            this.tap_Obj = tapObj;
        }
    }
}