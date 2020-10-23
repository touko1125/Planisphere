using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitObjComponent : MonoBehaviour
{
    public Enum.ObjType objType;

    public bool isChecked;

    //これが鏡とブラックホールの時にだけ使う
    public GameObject pairObj;

}
