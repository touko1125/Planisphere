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

    private List<List<GameObject>> currentBeamPlanetObjects = new List<List<GameObject>>();

    private List<List<GameObject>> beforeLineRendererObjects = new List<List<GameObject>>();

    private List<List<GameObject>> twoBeforeLineRendererObjects = new List<List<GameObject>>();

    public IEnumerator shot_beam_coroutine;

    //ビーム検査コルーチン監視用
    private int shot_beam_coroutineNum;

    private int currentTabNum;

    public bool isDrawLine;

    //鏡の処理用
    private bool is_pass_Mirror;

    //全体の処理用
    private bool is_Reflected;

    //微小変化ビーム用
    private bool isDeltaLine;

    private Ray beemRay;

    //最終的に描画するときに使用
    private List<List<Vector3>> line_RedererPos_List = new List<List<Vector3>>();

    private int TestNum;

    //よくない　爆発した時の最後の線
    private int bombedLastLineNum;

    // Start is called before the first frame update
    void Start()
    {
        Edge = GameObject.Find("Edge");

        for(int i = 0; i < 2; i++)
        {
            beforeLineRendererObjects.Add(new List<GameObject>());
            twoBeforeLineRendererObjects.Add(new List<GameObject>());
            currentBeamPlanetObjects.Add(new List<GameObject>());
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

    public void RestPlanetState(int TabNum)
    {
        for(int i=0;i < currentBeamPlanetObjects[TabNum].Count; i++)
        {
            currentBeamPlanetObjects[TabNum][i].GetComponent<PlanetComponent>().ChangeFace(Enum.PlanetFace.nomal);
        }

        currentBeamPlanetObjects[TabNum].Clear();

        //片割れ
        int anotherTabNum = TabNum == 0 ? 1 : 0;

        for (int i = 0; i < currentBeamPlanetObjects[anotherTabNum].Count; i++)
        {
            currentBeamPlanetObjects[anotherTabNum][i].GetComponent<PlanetComponent>().ChangeFace(Enum.PlanetFace.surprise);
        }
    }

    public void ResetLine()
    {
        for (int i = 0; i < 2; i++)
        {
            for (int n = beforeLineRendererObjects[i].Count-1;-1 < n; n--)
            {
                GameObject beforeLine = beforeLineRendererObjects[i][n];

                beforeLineRendererObjects[i].Remove(beforeLine);

                Destroy(beforeLine);
            }

            for (int n = twoBeforeLineRendererObjects[i].Count-1; -1 < n; n--)
            {
                GameObject twoBeforeLine = twoBeforeLineRendererObjects[i][n];

                twoBeforeLineRendererObjects[i].Remove(twoBeforeLine);

                Destroy(twoBeforeLine);
            }
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

        Debug.Log(beemOriginPos);
        Debug.Log(directionPos);

        //レイヤーの指定
        int layerNum1 = LayerMask.NameToLayer("BeamCollision");
        int layerNum2 = LayerMask.NameToLayer("Edge");

        beemRay = new Ray(beemOriginPos,directionPos);

        //一番新しいところに描画位置を収納
        line_RedererPos_List.Add(new List<Vector3>());

        //line_RendererPos_Listのかずはリストの中のリストの要素(Vector3型)2n個とリストの中のリスト(List<Vector3>型)の1個
        line_RedererPos_List[line_RedererPos_List.Count - 1].Add(beemOriginPos);
        line_RedererPos_List[line_RedererPos_List.Count - 1].Add(directionPos);

        Debug.Log(line_RedererPos_List[line_RedererPos_List.Count - 1][0]);
        Debug.Log(line_RedererPos_List[line_RedererPos_List.Count - 1][1]);

        //Scene画面に描画
        Debug.DrawRay(beemRay.origin,beemRay.direction*Const.radius, Color.white,directionPos.magnitude);

        //ぶつかった奴を格納
        List<GameObject> rayHitObj=new List<GameObject>();

        //ぶつかった場所を格納
        List<Vector3> rayHitPos = new List<Vector3>();

        if (Physics2D.Raycast(beemRay.origin, beemRay.direction).collider)
        {
            foreach (RaycastHit2D hit in Physics2D.RaycastAll(beemRay.origin, beemRay.direction,directionPos.magnitude,1<<layerNum1|1<<layerNum2))
            {
                //確認済みでなければ追加
                if (!hit.transform.gameObject.GetComponent<HitObjComponent>().isChecked)
                {
                    rayHitObj.Add(hit.transform.gameObject);
                    rayHitPos.Add(hit.point);
                }
            }

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
                    //鏡を通った後に縁に当たる
                    if (is_Reflected)
                    {
                        //終点を縁に
                        line_RedererPos_List[line_RedererPos_List.Count - 1][1] = rayHitPos[0] - beemOriginPos;
                    }
                    else
                    {
                        //爆発した時のすり抜けたビーム用(なぜかLineRendererPosListの一番最後じゃなかった)
                        if (isDeltaLine) line_RedererPos_List[bombedLastLineNum][1] = rayHitPos[0] - line_RedererPos_List[bombedLastLineNum][0]; //終点を縁に
                    }
                }
            }
        }

        if(shot_beam_coroutineNum>0) shot_beam_coroutineNum--;

        //全てのビームの審査が終わるまで待つ
        if (shot_beam_coroutineNum > 0) yield break; 

        twoBeforeLineRendererObjects[currentTabNum].Clear();

        if (beforeLineRendererObjects[currentTabNum].Count > 0)
        {
            for (int i = 0; i < beforeLineRendererObjects[currentTabNum].Count; i++)
            {
                twoBeforeLineRendererObjects[currentTabNum].Add(beforeLineRendererObjects[currentTabNum][i]);
            }
        }

        beforeLineRendererObjects[currentTabNum].Clear();

        //応急処置　爆発以外はRenderingにこんなに入んないはず…
        var waitPerBeam = line_RedererPos_List.Count > 50 ? 0 : 0.1f;

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

            TestNum++;

            lineRendererObj.name = TestNum.ToString();

            beforeLineRendererObjects[currentTabNum].Add(lineRendererObj);

            LineRenderer lineRenderer = lineRendererObj.GetComponent<LineRenderer>();

            lineRenderer.alignment = LineAlignment.TransformZ;

            lineRenderer.SetWidth(4.0f, 4.0f);

            lineRenderer.textureMode = LineTextureMode.Tile;

            lineRenderer.material = currentLineMaterial;

            for (int n = 0; n < differenceNum; n++)
            {
                yield return new WaitForSeconds(waitPerBeam/10);
                lineRenderer.positionCount = n + 1;

                lineRenderer.SetPosition(n, line_RedererPos_List[i][0] + line_Difference);
                line_RedererPos_List[i][0] = line_RedererPos_List[i][0] + line_Difference;
            }

            yield return new WaitForSeconds(waitPerBeam);
        }

        //次のビームに備えて状態のリセット
        for(int i = 0; i < currentBeamHitObjects.Count; i++)
        {
            currentBeamHitObjects[i].GetComponent<HitObjComponent>().isChecked = false;

            //今のビームで貫かれた星のリスト
            if (currentBeamHitObjects[i].GetComponent<HitObjComponent>().objType==Enum.ObjType.Planet) currentBeamPlanetObjects[currentTabNum].Add(currentBeamHitObjects[i]);
        }

        //今から打つビームに当たった奴を入れる用のリセット
        currentBeamHitObjects.Clear();

        //ビーム検査コルーチン監視用リストクリア
        shot_beam_coroutineNum=0;

        line_RedererPos_List.Clear();

        isDrawLine = false;

        is_pass_Mirror = false;

        is_Reflected = false;

        isDeltaLine = false;
    }

    public IEnumerator JudgeHitObjType(List<GameObject> hitObjects,List<Vector3> hitPos,Vector3 raystartPos,Vector3 rayDirection)
    {
        Vector3[] nextSetBeamPos = new Vector3[2];

        //鏡を通っているか
        is_pass_Mirror = false;

        //線の変更をすでにされているか
        var isBendLine = false;

        //初期化
        nextSetBeamPos[0] = raystartPos;
        nextSetBeamPos[1] = rayDirection;

        //一番最近の二個を消す
        line_RedererPos_List.RemoveAt(line_RedererPos_List.Count-1);

        List<GameObject> inLineMirrorObj = new List<GameObject>();

        List<GameObject> inLineBlackHole = new List<GameObject>();

        List<GameObject> inLineAsteroid = new List<GameObject>();

        for (int i = 0; i < hitObjects.Count; i++)
        {
            Debug.Log(i +"番目は"+ hitObjects[i]);

            if (!currentBeamHitObjects.Contains(hitObjects[i]))
            {
                currentBeamHitObjects.Add(hitObjects[i]);

                if (hitObjects[i].GetComponent<HitObjComponent>().objType == Enum.ObjType.Mirror) inLineMirrorObj.Add(hitObjects[i]);
                if (hitObjects[i].GetComponent<HitObjComponent>().objType == Enum.ObjType.Planet)
                {
                    if (hitObjects[i].GetComponent<PlanetComponent>().planetMode == Enum.PlanetMode.BlackHoleOut
                        || hitObjects[i].GetComponent<PlanetComponent>().planetMode == Enum.PlanetMode.BlackHoleSafe) inLineBlackHole.Add(hitObjects[i]);

                    if (hitObjects[i].GetComponent<PlanetComponent>().planetMode == Enum.PlanetMode.Asteroid
                        || hitObjects[i].GetComponent<PlanetComponent>().planetMode == Enum.PlanetMode.Range) inLineAsteroid.Add(hitObjects[i]);
                }
            }
        }

        //審査
        for (int i=0;i<hitObjects.Count; i++)
        {
            switch (hitObjects[i].GetComponent<HitObjComponent>().objType)
            {
                case Enum.ObjType.Planet:

                    //びっくりする
                    hitObjects[i].GetComponent<PlanetComponent>().ChangeFace(Enum.PlanetFace.surprise);

                    //チェック済み
                    hitObjects[i].GetComponent<HitObjComponent>().isChecked = true;

                    if (hitObjects[i].GetComponent<PlanetComponent>().planetMode != Enum.PlanetMode.SinglePlanet)
                    {
                        if (!isBendLine)
                        {
                            switch (hitObjects[i].GetComponent<PlanetComponent>().planetMode)
                            {
                                case Enum.PlanetMode.DoublePlanet:
                                    if (isBendLine) break; ;

                                    yield return DoublePlanet(hitObjects[i],hitPos[i],raystartPos,rayDirection);
                                    yield break;
                                case Enum.PlanetMode.Range:

                                    var pairAsteroid = hitObjects[i].GetComponent<HitObjComponent>().pairObj;

                                    if (inLineAsteroid.Contains(pairAsteroid))
                                    {
                                        if (isBendLine) break;

                                        hitObjects[i].GetComponent<PlanetComponent>().StartCoroutine("DevidePlanet");

                                        yield return BombAsteroid(hitObjects[i], hitPos[i], raystartPos, rayDirection);
                                        yield break;

                                    }

                                    break;
                                case Enum.PlanetMode.BlackHoleSafe:
                                case Enum.PlanetMode.BlackHoleOut:

                                    var pairBlackHole = hitObjects[i].GetComponent<HitObjComponent>().pairObj;

                                    if (inLineBlackHole.Contains(pairBlackHole))
                                    {
                                        if (!isBendLine && hitObjects[i].GetComponent<PlanetComponent>().planetMode == Enum.PlanetMode.BlackHoleOut)
                                        {
                                            is_Reflected = false;

                                            isBendLine = true;

                                            //終点を障害物の位置に
                                            nextSetBeamPos[1] = hitPos[i] - raystartPos;
                                        }
                                    }
                                    else
                                    {
                                        if (isBendLine) break;

                                        yield return BlackHole(hitObjects[i], hitPos[i], raystartPos, rayDirection);
                                        yield break;
                                    }

                                    break;
                            }
                        }
                    }

                    break;
                case Enum.ObjType.Mirror:

                    //線上に鏡が二個以上あるときの処理
                    if (inLineMirrorObj.Count > 1)
                    {
                        //二個目以降の鏡
                        if (inLineMirrorObj.IndexOf(hitObjects[i]) > 0)
                        {
                            hitObjects[i].GetComponent<HitObjComponent>().isChecked = true;

                            break;
                        }
                    }

                    //鏡の表面裏面ともにビームが通っているかどうか
                    if (hitObjects.Contains(hitObjects[i].GetComponent<HitObjComponent>().pairObj))
                    {
                        GameObject backMirror = hitObjects[hitObjects.IndexOf(hitObjects[i].GetComponent<HitObjComponent>().pairObj)];

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
                        //鏡に複数回当たれなくなるけどチェック付ける
                        hitObjects[i].GetComponent<HitObjComponent>().isChecked = true;

                        if (isBendLine) break;

                        is_Reflected = true;

                        //二回呼び出してたら二回lineRendererリストに追加されてたからくっしょん
                        var mirrorAngle = getMirrorAngleCalculation(hitObjects[i], hitPos[i], raystartPos, rayDirection);

                        Debug.Log("Bend");

                        isBendLine = true;

                        nextSetBeamPos[0] = mirrorAngle[0];
                        nextSetBeamPos[1] = mirrorAngle[1];
                    }

                    break;
                case Enum.ObjType.Obstacle:

                    //鏡の表面裏面ともにビームが通っているかどうか
                    if (hitObjects.Contains(hitObjects[i].GetComponent<HitObjComponent>().pairObj))
                    {
                        GameObject forwardMirror = hitObjects[hitObjects.IndexOf(hitObjects[i].GetComponent<HitObjComponent>().pairObj)];

                        //線上に鏡が二個以上あるときの処理
                        if (inLineMirrorObj.Count > 1)
                        {
                            //二個目以降の鏡
                            if (inLineMirrorObj.IndexOf(forwardMirror) > 0)
                            {
                                hitObjects[i].GetComponent<HitObjComponent>().isChecked = true;

                                break;
                            }
                        }

                        if (hitObjects.IndexOf(forwardMirror) > hitObjects.IndexOf(hitObjects[i]))
                        {
                            if (!isBendLine)
                            {
                                Debug.Log("Bend");

                                is_Reflected = false;

                                isBendLine = true;

                                //終点を障害物の位置に
                                nextSetBeamPos[1] = hitPos[i] - raystartPos;
                            }
                        }
                    }
                    else
                    {
                        if (!isBendLine)
                        {
                            isBendLine = true;

                            Debug.Log("Bend");

                            //終点を障害物の位置に
                            nextSetBeamPos[1] = hitPos[i] - raystartPos;
                        }
                    }

                    //鏡の背面に当たったことは次はなかったことに
                    hitObjects[i].GetComponent<HitObjComponent>().isChecked = true;

                    break;
            }
        }

        yield return null;

        Debug.Log(nextSetBeamPos[0]);
        Debug.Log(nextSetBeamPos[1]);

        //描画審査
        shot_beam_coroutine = ShotBeam(nextSetBeamPos[0],nextSetBeamPos[1],currentTabNum);

        StartCoroutine(shot_beam_coroutine);
    }

    public IEnumerator DoublePlanet(GameObject hitObject,Vector3 hitPos,Vector3 raystartPos,Vector3 rayDirection)
    {
        var nextSetBeamPos = new Vector3[2];

        var changedLine = new List<List<Vector3>>();

        changedLine = getBendLine(hitObject, hitPos, raystartPos, rayDirection);

        is_Reflected = true;

        shot_beam_coroutineNum = changedLine.Count;

        for (int n = 0; n < changedLine.Count; n++)
        {
            nextSetBeamPos[0] = changedLine[n][0];
            nextSetBeamPos[1] = changedLine[n][1];

            //描画審査
            shot_beam_coroutine = ShotBeam(nextSetBeamPos[0], nextSetBeamPos[1], currentTabNum);

            StartCoroutine(shot_beam_coroutine);
        }

        Debug.Log("aaaa");

        yield return new WaitForSeconds(0);
    }

    public IEnumerator BlackHole(GameObject hitObject, Vector3 hitPos, Vector3 raystartPos, Vector3 rayDirection)
    {
        var changedLine = new Vector3[2];

        changedLine = getBendBlackHoleLine(hitObject, hitPos, raystartPos, rayDirection);

        is_Reflected = true;

        shot_beam_coroutine = ShotBeam(changedLine[0], changedLine[1], currentTabNum);

        StartCoroutine(shot_beam_coroutine);

        yield return new WaitForSeconds(0);
    }

    public IEnumerator BombAsteroid(GameObject hitObject, Vector3 hitPos, Vector3 raystartPos, Vector3 rayDirection)
    {
        var nextSetBeamPos = new Vector3[2];

        var changedLine = new List<List<Vector3>>();

        changedLine = getBombedLine(hitObject, hitPos, raystartPos, rayDirection);

        shot_beam_coroutineNum = changedLine.Count;

        for (int n = 0; n < changedLine.Count; n++)
        {
            nextSetBeamPos[0] = changedLine[n][0];
            nextSetBeamPos[1] = changedLine[n][1];

            //描画審査
            shot_beam_coroutine = ShotBeam(nextSetBeamPos[0], nextSetBeamPos[1], currentTabNum);

            StartCoroutine(shot_beam_coroutine);
        }

        yield return new WaitForSeconds(0);
    }

    public List<List<Vector3>> getBombedLine(GameObject hitObj, Vector3 hitPos, Vector3 rayStartPos, Vector3 rayDirectionPos)
    {
        //一番新しいところに描画位置(ビーム始点からかがみまで)を収納
        line_RedererPos_List.Add(new List<Vector3>());

        //とりあえず現地点までの情報
        line_RedererPos_List[line_RedererPos_List.Count - 1].Add(rayStartPos);
        line_RedererPos_List[line_RedererPos_List.Count - 1].Add(hitPos - rayStartPos);

        var bombedLine = new List<List<Vector3>>();

        var firstRad = Mathf.Atan2((hitObj.transform.position-hitPos).x, (hitObj.transform.position-hitPos).y);

        Debug.Log(firstRad*Mathf.Rad2Deg);

        //z軸の調整
        var prePos = new Vector3(hitPos.x,hitPos.y,Const.rayDepth);
        var centerPos = new Vector3(hitObj.transform.position.x,hitObj.transform.position.y,0);

        //ビームの入射位置の間反対のとこにあるやつ
        var specialPos = Vector3.zero;

        for(int i = 0; i < Const.cireclePersantage; i++)
        {
            var rad = Mathf.Deg2Rad*(i * (360 / Const.cireclePersantage));

            var newPos = centerPos + new Vector3(-Mathf.Sin(rad+firstRad), -Mathf.Cos(rad+firstRad))*0.8f;

            if (i == Const.cireclePersantage / 2) specialPos = newPos;

            bombedLine.Add(new List<Vector3>());

            bombedLine[bombedLine.Count - 1].Add(prePos);
            bombedLine[bombedLine.Count - 1].Add((newPos-prePos)*1.2f);

            prePos = newPos;
        }

        bombedLastLineNum = Const.cireclePersantage + line_RedererPos_List.Count;

        bombedLine.Add(new List<Vector3>());

        bombedLine[bombedLine.Count - 1].Add(specialPos);
        bombedLine[bombedLine.Count - 1].Add(rayDirectionPos-specialPos);

        isDeltaLine = true;

        return bombedLine;
    }

    public List<List<Vector3>> getBendLine(GameObject hitObj, Vector3 hitPos, Vector3 rayStartPos, Vector3 rayDirectionPos)
    {
        //一番新しいところに描画位置(ビーム始点からかがみまで)を収納
        line_RedererPos_List.Add(new List<Vector3>());

        //とりあえず現地点までの情報
        line_RedererPos_List[line_RedererPos_List.Count - 1].Add(rayStartPos);
        line_RedererPos_List[line_RedererPos_List.Count - 1].Add(hitPos - rayStartPos);

        Debug.Log(line_RedererPos_List[line_RedererPos_List.Count - 1][1]);

        float currentAngle = Mathf.Atan2((hitPos - rayStartPos).x, (hitPos - rayStartPos).y) * Mathf.Rad2Deg;

        int planetCount = hitObj.GetComponent<PlanetComponent>().planetCount;

        bool isOdd = planetCount % 2 == 1;

        Debug.Log("奇数" + isOdd);

        Debug.Log(planetCount);

        var bendTestLine = new List<List<Vector3>>();

        int vectorNum = 0;

        for (int i = 0; i < planetCount / 2; i++)
        {
            float angleValue = hitObj.GetComponent<PlanetComponent>().angle * (i + 1);

            float angleRight = 90 - currentAngle + angleValue;

            float angleLeft = 90 - currentAngle - angleValue;

            Debug.Log(currentAngle);

            Debug.Log(angleRight);

            Debug.Log(angleLeft);

            Vector3 rightVector = new Vector3(Mathf.Cos(angleRight * Mathf.Deg2Rad), Mathf.Sin(angleRight * Mathf.Deg2Rad), 0) * 3f;

            Vector3 leftVector = new Vector3(Mathf.Cos(angleLeft * Mathf.Deg2Rad), Mathf.Sin(angleLeft * Mathf.Deg2Rad), 0) * 3f;

            Debug.Log(rightVector);

            Debug.Log(leftVector);

            bendTestLine.Add(new List<Vector3>());

            bendTestLine[vectorNum].Add(hitPos);

            bendTestLine[vectorNum].Add(rightVector + hitPos);

            vectorNum++;

            bendTestLine.Add(new List<Vector3>());

            bendTestLine[vectorNum].Add(hitPos);

            bendTestLine[vectorNum].Add(leftVector + hitPos);

            vectorNum++;
        }

        if (isOdd)
        {
            Vector3 centerVector = new Vector3(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle)) + (rayDirectionPos - rayStartPos);

            List<Vector3> topVectorList = new List<Vector3>();

            topVectorList.Add(hitPos);

            topVectorList.Add(centerVector);

            //先頭に挿入
            bendTestLine.Insert((int)(planetCount / 2), topVectorList);
        }

        return bendTestLine;
    }

    public Vector3[] getBendBlackHoleLine(GameObject hitObj, Vector3 hitPos, Vector3 rayStartPos, Vector3 rayDirectionPos)
    {
        //一番新しいところに描画位置(ビーム始点からかがみまで)を収納
        line_RedererPos_List.Add(new List<Vector3>());

        //とりあえず現地点までの情報
        line_RedererPos_List[line_RedererPos_List.Count - 1].Add(rayStartPos);
        line_RedererPos_List[line_RedererPos_List.Count - 1].Add(hitPos - rayStartPos);

        var bendedLine = new Vector3[2];

        var objCenter_hitPosVector = hitPos - hitObj.transform.position;

        var direction_hitPoshitPosAngle = Vector3.Angle(objCenter_hitPosVector, -rayDirectionPos);

        Debug.Log(direction_hitPoshitPosAngle);

        var hitPos_ObjCenterAngle= Mathf.Atan2(-objCenter_hitPosVector.x,-objCenter_hitPosVector.y) * Mathf.Rad2Deg;

        var angleValue = hitObj.GetComponent<PlanetComponent>().angle;

        Debug.Log(Vector3.Distance(hitObj.transform.position + new Vector3(Mathf.Cos((90 - (hitPos_ObjCenterAngle + angleValue)) * Mathf.Deg2Rad), Mathf.Sin((90 - (hitPos_ObjCenterAngle + angleValue)) * Mathf.Deg2Rad), 0) * 0.1f, rayStartPos)
            > Vector3.Distance(hitObj.transform.position + new Vector3(Mathf.Cos((90 - (hitPos_ObjCenterAngle - angleValue)) * Mathf.Deg2Rad), Mathf.Sin((90 - (hitPos_ObjCenterAngle - angleValue)) * Mathf.Deg2Rad), 0) * 0.1f, rayStartPos));

        //超苦肉の策　ビーム射出地点から離れた方向に曲がる
        var reflectPos = Vector3.Distance(hitObj.transform.position + new Vector3(Mathf.Cos((90-(hitPos_ObjCenterAngle+angleValue)) * Mathf.Deg2Rad), Mathf.Sin((90-(hitPos_ObjCenterAngle+angleValue)) * Mathf.Deg2Rad), 0) * 0.15f,rayStartPos)
            > Vector3.Distance(hitObj.transform.position + new Vector3(Mathf.Cos((90 - (hitPos_ObjCenterAngle - angleValue)) * Mathf.Deg2Rad), Mathf.Sin((90 - (hitPos_ObjCenterAngle - angleValue)) * Mathf.Deg2Rad), 0) * 0.15f, rayStartPos)
            ? hitObj.transform.position + new Vector3(Mathf.Cos((90 - (hitPos_ObjCenterAngle + angleValue)) * Mathf.Deg2Rad), Mathf.Sin((90 - (hitPos_ObjCenterAngle + angleValue)) * Mathf.Deg2Rad), 0) * 0.15f
            : hitObj.transform.position + new Vector3(Mathf.Cos((90 - (hitPos_ObjCenterAngle - angleValue)) * Mathf.Deg2Rad), Mathf.Sin((90 - (hitPos_ObjCenterAngle - angleValue)) * Mathf.Deg2Rad), 0) * 0.15f;

        var directionNormarized = Vector3.Distance(new Vector3(-(reflectPos - hitObj.transform.position).y, (reflectPos - hitObj.transform.position).x).normalized*3f+reflectPos,rayStartPos)
            < Vector3.Distance(new Vector3((reflectPos - hitObj.transform.position).y,-(reflectPos - hitObj.transform.position).x).normalized * 3f + reflectPos, rayStartPos)
            ? new Vector3((reflectPos - hitObj.transform.position).y, -(reflectPos - hitObj.transform.position).x).normalized
            : new Vector3(-(reflectPos - hitObj.transform.position).y, (reflectPos - hitObj.transform.position).x).normalized;

        Debug.Log("方向単位ベクトル"+directionNormarized);

        bendedLine[0] = reflectPos;
        bendedLine[1] = reflectPos + directionNormarized*3f;

        return bendedLine;
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
