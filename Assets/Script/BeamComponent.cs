using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TapClass;

public class BeamComponent : MonoBehaviour
{

    private LineRenderer lineRenderer;

    public Material lineMaterial;

    private int differenceNum;

    private Vector3 distance;

    private Vector3 differenceLine;

    public IEnumerator shot_beam_coroutine;

    public bool isDrawLine;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator ShotBeam(Vector3 beemOriginPos,Vector3 directionPos)
    {
        //今から書き始めるよサイン
        isDrawLine = true;

        //LineRendererの頂点の初期化
        lineRenderer.positionCount=0;

        //z軸の調整
        beemOriginPos = new Vector3(beemOriginPos.x, beemOriginPos.y, Const.rayDepth);
        directionPos = new Vector3(directionPos.x, directionPos.y,Const.rayDepth);

        //Rayのベクトル取得
        Vector3 direction = directionPos-beemOriginPos;

        //Rayはどこからどこに打つにしても長さは直径(当たり判定のためのためなので直径の長さ以上必要になることはないはず…)
        Vector3 beemEndPos = new Vector3(direction.x, direction.y, 0);

        //レイヤーの指定
        int layerNum = LayerMask.NameToLayer("BeamCollision");

        Ray beemRay = new Ray(beemOriginPos,beemEndPos*10.0f);

        //Scene画面に描画
        Debug.DrawRay(beemRay.origin,beemRay.direction*10, Color.white, 5.0f);

        //ぶつかった奴を格納
        List<GameObject> rayHitObj=new List<GameObject>();

        RaycastHit beemHit;

        if (Physics.Raycast(beemRay, out beemHit, 10, layerNum))
        {
            Debug.Log("Hit");
            foreach (RaycastHit hit in Physics.RaycastAll(beemRay.origin,beemRay.direction))
            {
                rayHitObj.Add(hit.transform.gameObject);
            }

            JudgeHitObjType(rayHitObj.ToArray());

            //いったんここで終了
            yield break;
        }

        differenceLine = direction*Const.radius;

        while (differenceLine.magnitude > 0.1f)
        {
            differenceLine = differenceLine / 2f;
        }

        differenceNum = (int)(direction.magnitude*Const.radius / differenceLine.magnitude);

        lineRenderer.alignment = LineAlignment.TransformZ;

        lineRenderer.textureMode = LineTextureMode.Tile;

        lineRenderer.material = lineMaterial;

        for (int i = 0; i < differenceNum; i++)
        {
            yield return new WaitForSeconds(0.02f);
            lineRenderer.positionCount=i+1;

            lineRenderer.SetPosition(i, beemOriginPos + differenceLine);
            beemOriginPos = beemOriginPos + differenceLine;
        }

        isDrawLine = false;
    }

    public void JudgeHitObjType(GameObject[] hitObjects)
    {

    }
}
