using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TapClass;

public class BeamComponent : MonoBehaviour
{
    public GameObject lineRendererObjPrefab;

    public Material lineMaterial;

    private Vector3 line_Difference;

    private List<GameObject> currentBeamHitObjects = new List<GameObject>();

    private List<GameObject> beforeLineRendererObjects = new List<GameObject>();

    public IEnumerator shot_beam_coroutine;

    public bool isDrawLine;

    private Ray beemRay;

    //最終的に描画するときに使用
    private List<List<Vector3>> line_RedererPos_List = new List<List<Vector3>>(); 

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator ShotBeam(Vector3 beemOriginPos,Vector3 directionPos)
    {
        //今から書き始めるよサイン
        isDrawLine = true;

        //z軸の調整
        beemOriginPos = new Vector3(beemOriginPos.x, beemOriginPos.y, Const.rayDepth);
        directionPos = new Vector3(directionPos.x, directionPos.y,0);

        //レイヤーの指定
        int layerNum = LayerMask.NameToLayer("Default");

        beemRay = new Ray(beemOriginPos,directionPos);

        //一番新しいところに描画位置を収納
        line_RedererPos_List.Add(new List<Vector3>());

        //line_RendererPos_Listのかずはリストの中のリストの要素(Vector3型)2n個とリストの中のリスト(List<Vector3>型)の1個
        line_RedererPos_List[line_RedererPos_List.Count - 1].Add(beemOriginPos);
        line_RedererPos_List[line_RedererPos_List.Count - 1].Add(directionPos);

        //Scene画面に描画
        Debug.DrawRay(beemRay.origin,beemRay.direction*10, Color.white, 5.0f);

        //ぶつかった奴を格納
        List<GameObject> rayHitObj=new List<GameObject>();

        //ぶつかった場所を格納
        List<Vector3> rayHitPos = new List<Vector3>();

        RaycastHit beemHit;

        if (Physics.Raycast(beemRay, out beemHit,beemRay.direction.magnitude*Const.radius))
        {
            Debug.Log("Hit!");
            foreach (RaycastHit hit in Physics.RaycastAll(beemRay.origin,beemRay.direction))
            {
                //確認済みでなければ追加
                if(!hit.transform.gameObject.GetComponent<HitObjComponent>().isChecked) 
                rayHitObj.Add(hit.transform.gameObject);
                rayHitPos.Add(hit.point);
            }

            if (rayHitObj.Count > 0)
            {
                //今から打つビームに当たった奴を入れる用のリセット
                currentBeamHitObjects.Clear();

                JudgeHitObjType(rayHitObj,rayHitPos,beemOriginPos,directionPos);

                //いったんここで終了
                yield break;
            }
        }

        for(int i = 0; i < line_RedererPos_List.Count; i++)
        {

            line_Difference = line_RedererPos_List[i][1];

            while (line_Difference.magnitude > 0.1f)
            {
                line_Difference = line_Difference / 2f;
            }

            int differenceNum = (int)(line_RedererPos_List[i][1].magnitude / line_Difference.magnitude);

            GameObject lineRendererObj = Instantiate(lineRendererObjPrefab, Vector3.zero, Quaternion.identity);

            beforeLineRendererObjects.Add(lineRendererObj);

            LineRenderer lineRenderer = lineRendererObj.GetComponent<LineRenderer>();

            lineRenderer.alignment = LineAlignment.TransformZ;

            lineRenderer.textureMode = LineTextureMode.Tile;

            lineRenderer.material = lineMaterial;

            for (int n = 0; n < differenceNum; n++)
            {
                yield return new WaitForSeconds(0.02f);
                lineRenderer.positionCount = n + 1;

                lineRenderer.SetPosition(n, line_RedererPos_List[i][0] + line_Difference);
                line_RedererPos_List[i][0] = line_RedererPos_List[i][0] + line_Difference;
            }

            yield return new WaitForSeconds(0.2f);
        }

        //次のビームに備えて状態のリセット
        for(int i = 0; i < currentBeamHitObjects.Count; i++)
        {
            currentBeamHitObjects[i].GetComponent<HitObjComponent>().isChecked = false;
        }

        line_RedererPos_List.Clear();

        isDrawLine = false;
    }

    public void JudgeHitObjType(List<GameObject> hitObjects,List<Vector3> hitPos,Vector3 raystartPos,Vector3 rayDirection)
    {
        Vector3[] nextSetBeamPos = new Vector3[2];

        //初期化
        nextSetBeamPos[0] = raystartPos;
        nextSetBeamPos[1] = rayDirection;

        Debug.Log(line_RedererPos_List.Count);

        //一番最近の二個を消す
        line_RedererPos_List.RemoveAt(line_RedererPos_List.Count-1);

        Debug.Log(line_RedererPos_List.Count);

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

                    bool is_pass_Mirror = false;

                    //鏡の表面裏面ともにビームが通っているかどうか
                    if (hitObjects.Contains(hitObjects[i].GetComponent<HitObjComponent>().pairMirror))
                    {
                        GameObject backMirror = hitObjects[hitObjects.IndexOf(hitObjects[i].GetComponent<HitObjComponent>().pairMirror)];

                        Debug.Log("Mirror");

                        //鏡の前面ほうが背面よりビーム射出位置から遠かったら背中にビームが当たっているといえる(?)
                        if (Vector3.Distance(hitObjects[i].transform.position, raystartPos) >
                            Vector3.Distance(backMirror.transform.position, raystartPos))
                        {
                            //鏡に当たったことはつぎはなかったことに
                            hitObjects[i].GetComponent<HitObjComponent>().isChecked = true;
                        }
                        else
                        {
                            //鏡の両面通過かつ表面のほうが射出口に近い
                            is_pass_Mirror = true;
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
                        //二回呼び出してたら二回lineRendererリストに追加されてたからくっしょん
                        var mirrorAngle = getMirrorAngleCalculation(hitObjects[i], hitPos[i], raystartPos, rayDirection);

                        nextSetBeamPos[0] = mirrorAngle[0];
                        nextSetBeamPos[1] = mirrorAngle[1];
                    }

                    break;
                case GameManager.ObjType.Obstacle:

                    //鏡の表面裏面ともにビームが通っているかどうか
                    if (hitObjects.Contains(hitObjects[i].GetComponent<HitObjComponent>().pairMirror))
                    {
                        GameObject forwardMirror = hitObjects[hitObjects.IndexOf(hitObjects[i].GetComponent<HitObjComponent>().pairMirror)];

                        //鏡の前面ほうが背面よりビーム射出位置から遠かったら背中にビームが当たっているといえる(?)
                        if (Vector3.Distance(hitObjects[i].transform.position, raystartPos) <
                            Vector3.Distance(forwardMirror.transform.position, raystartPos))
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
            }
        }

        //描画審査
        shot_beam_coroutine = ShotBeam(nextSetBeamPos[0],nextSetBeamPos[1]);

        StartCoroutine(shot_beam_coroutine);
    }

    public Vector3[] getMirrorAngleCalculation(GameObject hitObj,Vector3 hitPos,Vector3 raystartPos,Vector3 rayDirection)
    {
        //クオータニオン→オイラー角→ラジアン        
        Vector3 mirrorVector = new Vector3(Mathf.Cos(hitObj.transform.rotation.eulerAngles.z * Mathf.Deg2Rad), Mathf.Sin(hitObj.transform.rotation.eulerAngles.z * Mathf.Deg2Rad));

        Debug.Log(hitObj.transform.rotation.eulerAngles.z * Mathf.Deg2Rad);

        Debug.Log(Mathf.Cos(hitObj.transform.rotation.eulerAngles.z * Mathf.Deg2Rad));

        Debug.Log(mirrorVector);

        Debug.Log(Vector3.Angle(rayDirection, mirrorVector));

        //なす角が90度以上なら調整
        float angle = Vector3.Angle(rayDirection, mirrorVector) > 180 ? 90 - Vector3.Angle(rayDirection, mirrorVector) : Vector3.Angle(rayDirection, mirrorVector);

        Debug.Log(angle);

        //一番新しいところに描画位置(ビーム始点からかがみまで)を収納
        line_RedererPos_List.Add(new List<Vector3>());

        //line_RendererPos_Listのかずはリストの中のリストの要素(Vector3型)2n個とリストの中のリスト(List<Vector3>型)の1個
        line_RedererPos_List[line_RedererPos_List.Count - 1].Add(raystartPos);
        line_RedererPos_List[line_RedererPos_List.Count - 1].Add(hitPos - raystartPos);

        Vector3[] nextSetBeamPos=new Vector3[2];

        nextSetBeamPos[0] = hitPos;
        nextSetBeamPos[1] = new Vector3(-Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * Const.radius;

        return nextSetBeamPos;
    }
}
