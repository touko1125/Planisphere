using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using DG.Tweening;
using UniRx;

public class BeamComponent : MonoBehaviour
{
    public GameObject lineRendererObjPrefab;

    [SerializeField]
    private PlanetManager planetManager;

    private GameObject Edge;

    public Material currentLineMaterial;

    public Material beforeLineMaterial;

    private Vector3 line_Difference;

    public List<GameObject> currentBeamHitObjects = new List<GameObject>();

    private List<List<GameObject>> beforeLineRendererObjects = new List<List<GameObject>>();

    private List<List<GameObject>> twoBeforeLineRendererObjects = new List<List<GameObject>>();

    public IEnumerator shot_beam_coroutine;

    private int currentTabNum;

    public bool isDrawLine;

    //鏡の処理用
    private bool is_pass_Mirror;

    //全体の処理用
    private bool is_Reflected;

    private Ray beemRay;

    //最終的に描画するときに使用
    private List<List<Vector3>> line_RedererPos_List = new List<List<Vector3>>();

    // Start is called before the first frame update
    void Start()
    {
        Edge = GameObject.Find("Edge");

        for(int i = 0; i < 2; i++)
        {
            beforeLineRendererObjects.Add(new List<GameObject>());
            twoBeforeLineRendererObjects.Add(new List<GameObject>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ThinandDestroyLine(int TabNum)
    {
        //一個前の線を薄く
        for (int i = 0; i < beforeLineRendererObjects[TabNum].Count; i++)
        {
            beforeLineRendererObjects[TabNum][i].GetComponent<LineRenderer>().material = beforeLineMaterial;
        }

        //二個前の線を消す
        for (int i = 0; i < twoBeforeLineRendererObjects[TabNum].Count; i++)
        {
            Destroy(twoBeforeLineRendererObjects[TabNum][i]);
        }
    }

    public IEnumerator ShotBeam(Vector3 beemOriginPos, Vector3 directionPos, int TabNum)
    {
        //今から書き始めるよサイン
        isDrawLine = true;

        //今からビームを描くタブ
        currentTabNum = TabNum;

        //z軸の調整
        beemOriginPos = new Vector3(beemOriginPos.x, beemOriginPos.y, Const.rayDepth);
        directionPos = new Vector3(directionPos.x, directionPos.y,0);

        //レイヤーの指定
        int layerNum1 = LayerMask.NameToLayer("BeamCollision");
        int layerNum2 = LayerMask.NameToLayer("Edge");

        beemRay = new Ray(beemOriginPos,directionPos);

        Debug.Log("bbbbb");

        //一番新しいところに描画位置を収納
        line_RedererPos_List.Add(new List<Vector3>());

        //line_RendererPos_Listのかずはリストの中のリストの要素(Vector3型)2n個とリストの中のリスト(List<Vector3>型)の1個
        line_RedererPos_List[line_RedererPos_List.Count - 1].Add(beemOriginPos);
        line_RedererPos_List[line_RedererPos_List.Count - 1].Add(directionPos);

        Debug.Log(line_RedererPos_List[line_RedererPos_List.Count - 1][0]);
        Debug.Log(line_RedererPos_List[line_RedererPos_List.Count - 1][1]);

        //Scene画面に描画
        Debug.DrawRay(beemRay.origin,beemRay.direction*Const.radius, Color.white, 5.0f);

        //ぶつかった奴を格納
        List<GameObject> rayHitObj=new List<GameObject>();

        //ぶつかった場所を格納
        List<Vector3> rayHitPos = new List<Vector3>();

        Debug.Log("beforeHit");

        if (Physics2D.Raycast(beemRay.origin, beemRay.direction).collider)
        {
            Debug.Log("Hit!");
            foreach (RaycastHit2D hit in Physics2D.RaycastAll(beemRay.origin, beemRay.direction,Mathf.Infinity,1<<layerNum1|1<<layerNum2))
            {
                Debug.Log(hit.transform.gameObject);
                //確認済みでなければ追加
                if (!hit.transform.gameObject.GetComponent<HitObjComponent>().isChecked)
                {
                    rayHitObj.Add(hit.transform.gameObject);
                    rayHitPos.Add(hit.point);
                }
            }

            Debug.Log("あたったかず"+rayHitObj.Count);

            if (rayHitObj.Count > 0)
            {
                //縁だけで当たってる時を外す
                if (!(rayHitObj.Count == 1 && rayHitObj[0] == Edge))
                {

                    StartCoroutine(JudgeHitObjType(rayHitObj, rayHitPos, beemOriginPos, directionPos));

                    //いったんここで終了
                    yield break;
                }
                else
                {
                    Debug.Log(is_Reflected);

                    //鏡を通った後に縁に当たる
                    if (is_Reflected)
                    {
                        Debug.Log("EdgeEnd");
                        //終点を縁に
                        line_RedererPos_List[line_RedererPos_List.Count - 1][1] = rayHitPos[0] - beemOriginPos;
                    }
                }
            }
        }

        twoBeforeLineRendererObjects[currentTabNum].Clear();

        if (beforeLineRendererObjects[currentTabNum].Count > 0)
        {
            for (int i = 0; i < beforeLineRendererObjects[currentTabNum].Count; i++)
            {
                twoBeforeLineRendererObjects[currentTabNum].Add(beforeLineRendererObjects[currentTabNum][i]);
            }
        }

        beforeLineRendererObjects[currentTabNum].Clear();

        for (int i = 0; i < line_RedererPos_List.Count; i++)
        {
            //z軸の調整
            line_RedererPos_List[i][1] = new Vector3(line_RedererPos_List[i][1].x, line_RedererPos_List[i][1].y,0);

            line_Difference = line_RedererPos_List[i][1];

            while (line_Difference.magnitude > 0.1f)
            {
                line_Difference = line_Difference / 2f;
            }

            int differenceNum = (int)(line_RedererPos_List[i][1].magnitude / line_Difference.magnitude);

            GameObject lineRendererObj = Instantiate(lineRendererObjPrefab, Vector3.zero, Quaternion.identity);

            beforeLineRendererObjects[currentTabNum].Add(lineRendererObj);

            LineRenderer lineRenderer = lineRendererObj.GetComponent<LineRenderer>();

            lineRenderer.alignment = LineAlignment.TransformZ;

            lineRenderer.SetWidth(4.0f, 4.0f);

            lineRenderer.textureMode = LineTextureMode.Tile;

            lineRenderer.material = currentLineMaterial;

            for (int n = 0; n < differenceNum; n++)
            {
                yield return new WaitForSeconds(0.02f);
                lineRenderer.positionCount = n + 1;

                lineRenderer.SetPosition(n, line_RedererPos_List[i][0] + line_Difference);
                line_RedererPos_List[i][0] = line_RedererPos_List[i][0] + line_Difference;
            }

            is_pass_Mirror = false;

            is_Reflected = false;

            yield return new WaitForSeconds(0.2f);
        }

        //次のビームに備えて状態のリセット
        for(int i = 0; i < currentBeamHitObjects.Count; i++)
        {
            currentBeamHitObjects[i].GetComponent<HitObjComponent>().isChecked = false;
        }

        //今から打つビームに当たった奴を入れる用のリセット
        currentBeamHitObjects.Clear();

        line_RedererPos_List.Clear();

        isDrawLine = false;
    }

    public IEnumerator JudgeHitObjType(List<GameObject> hitObjects,List<Vector3> hitPos,Vector3 raystartPos,Vector3 rayDirection)
    {
        Vector3[] nextSetBeamPos = new Vector3[2];

        is_pass_Mirror = false;

        //初期化
        nextSetBeamPos[0] = raystartPos;
        nextSetBeamPos[1] = rayDirection;

        //一番最近の二個を消す
        line_RedererPos_List.RemoveAt(line_RedererPos_List.Count-1);

        Debug.Log("Judge");

        for(int i = 0; i < hitObjects.Count; i++)
        {
            Debug.Log(hitObjects[i]);

            if (!currentBeamHitObjects.Contains(hitObjects[i])) currentBeamHitObjects.Add(hitObjects[i]);
        }

        //審査
        for (int i=0;i<hitObjects.Count; i++)
        {
            switch (hitObjects[i].GetComponent<HitObjComponent>().objType)
            {
                case GameManager.ObjType.Planet:

                    //びっくりする
                    hitObjects[i].GetComponent<PlanetComponent>().ChangeFace(GameManager.PlanetFace.surprise);

                    //チェック済み
                    hitObjects[i].GetComponent<HitObjComponent>().isChecked = true;

                    break;
                case GameManager.ObjType.Mirror:

                    //鏡の表面裏面ともにビームが通っているかどうか
                    if (hitObjects.Contains(hitObjects[i].GetComponent<HitObjComponent>().pairMirror))
                    {
                        GameObject backMirror = hitObjects[hitObjects.IndexOf(hitObjects[i].GetComponent<HitObjComponent>().pairMirror)];

                        Debug.Log("Mirror");

                        if (hitObjects.IndexOf(backMirror) > hitObjects.IndexOf(hitObjects[i]))
                        {
                            is_pass_Mirror = true;
                        }
                        else
                        {
                            hitObjects[i].GetComponent<HitObjComponent>().isChecked = true;
                        }
                    }
                    //鏡の表面だけ通過
                    else
                    {
                        is_pass_Mirror = true;
                    }

                    //鏡の表面を通っていたら反射
                    if (is_pass_Mirror)
                    {
                        is_Reflected = true;

                        //二回呼び出してたら二回lineRendererリストに追加されてたからくっしょん
                        var mirrorAngle = getMirrorAngleCalculation(hitObjects[i], hitPos[i], raystartPos, rayDirection);

                        nextSetBeamPos[0] = mirrorAngle[0];
                        nextSetBeamPos[1] = mirrorAngle[1];

                        Debug.Log(nextSetBeamPos[0]);
                        Debug.Log(nextSetBeamPos[1]);

                        //鏡に複数回当たれなくなるけどチェック付ける
                        hitObjects[i].GetComponent<HitObjComponent>().isChecked = true;
                    }

                    break;
                case GameManager.ObjType.Obstacle:

                    //鏡の表面裏面ともにビームが通っているかどうか
                    if (hitObjects.Contains(hitObjects[i].GetComponent<HitObjComponent>().pairMirror))
                    {
                        GameObject forwardMirror = hitObjects[hitObjects.IndexOf(hitObjects[i].GetComponent<HitObjComponent>().pairMirror)];

                        if (hitObjects.IndexOf(forwardMirror) > hitObjects.IndexOf(hitObjects[i]))
                        {
                            //終点を障害物の位置に
                            nextSetBeamPos[1] = hitPos[i] - raystartPos;
                        }
                    }
                    else
                    {
                        //終点を障害物の位置に
                        nextSetBeamPos[1] = hitPos[i] - raystartPos;
                    }

                    //鏡の背面に当たったことは次はなかったことに
                    hitObjects[i].GetComponent<HitObjComponent>().isChecked = true;

                    break;
                //case GameManager.ObjType.Edge:

                //    if (is_pass_Mirror)
                //    {
                //        //nextSetBeamPos[1] = hitPos[i] - raystartPos;
                //    }
                //    break;
            }
        }

        yield return null;

        Debug.Log(nextSetBeamPos[0]);
        Debug.Log(nextSetBeamPos[1]);

        //描画審査
        shot_beam_coroutine = ShotBeam(nextSetBeamPos[0],nextSetBeamPos[1],currentTabNum);

        StartCoroutine(shot_beam_coroutine);
    }

    public Vector3[] getMirrorAngleCalculation(GameObject hitObj,Vector3 hitPos,Vector3 raystartPos,Vector3 rayDirection)
    {
        //クオータニオン→オイラー角→ラジアン        
        Vector3 mirrorVector = new Vector3(Mathf.Cos(hitObj.transform.parent.transform.rotation.eulerAngles.z * Mathf.Deg2Rad), Mathf.Sin(hitObj.transform.parent.rotation.eulerAngles.z * Mathf.Deg2Rad));

        //法線ベクトル?
        System.Numerics.Vector3 nomalVector = new System.Numerics.Vector3(1, -(mirrorVector.x / mirrorVector.y),0);

        UnityEngine.Vector3 nomalUnityVector = new UnityEngine.Vector3(1, -(mirrorVector.x / mirrorVector.y), 0);

        var u = System.Numerics.Vector3.Multiply(2 * Vector3.Dot(-rayDirection, nomalUnityVector), nomalVector);

        var k = new Vector3(u.X, u.Y, u.Z);

        //反射ベクトル求められている?
        Vector3 reflectVector = rayDirection + k;

        //一番新しいところに描画位置(ビーム始点からかがみまで)を収納
        line_RedererPos_List.Add(new List<Vector3>());

        //line_RendererPos_Listのかずはリストの中のリストの要素(Vector3型)2n個とリストの中のリスト(List<Vector3>型)の1個
        line_RedererPos_List[line_RedererPos_List.Count - 1].Add(raystartPos);
        line_RedererPos_List[line_RedererPos_List.Count - 1].Add(hitPos - raystartPos);

        Vector3[] nextSetBeamPos=new Vector3[2];

        nextSetBeamPos[0] = hitPos;
        nextSetBeamPos[1] = reflectVector;

        return nextSetBeamPos;
    }
}
