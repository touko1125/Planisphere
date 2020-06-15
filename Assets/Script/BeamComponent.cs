using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BeamComponent : MonoBehaviour
{

    private LineRenderer lineRenderer;

    public Material lineMaterial;

    private int differenceNum;

    private Vector3 distance;

    private Vector3 differenceLine;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartShotBeam(GameObject shotbeamTabObj)
    {
        Debug.Log("bbbb");

        StartCoroutine(ShotBeam(shotbeamTabObj));
    }

    public IEnumerator ShotBeam(GameObject shotbeamTabObj)
    {
        lineRenderer.positionCount=0;

        //頂点が最初の点なので単位円からは90度回転している?
        Vector2 direction = new Vector2(-Mathf.Sin(shotbeamTabObj.transform.parent.rotation.z), -Mathf.Cos(shotbeamTabObj.transform.parent.rotation.z));

        //調整
        Vector3 beemStartPos = new Vector3(shotbeamTabObj.transform.position.x+(direction/5.0f).x, shotbeamTabObj.transform.position.y + (direction / 5.0f).y, -1);
        Vector3 beemEndPos = new Vector3(direction.x * 2.5f, direction.y * 2.5f, -1);

        //レイヤーの指定
        int rayerNum = LayerMask.NameToLayer("BeamCollision");

        Ray beemRay = new Ray(beemStartPos,beemEndPos);

        RaycastHit beemRaycast;

        //太くする
        Physics.SphereCast(beemRay, 3.0f,out beemRaycast,rayerNum);

        //Scene画面に描画
        Debug.DrawRay(beemStartPos, beemEndPos, Color.white, 5.0f);

        distance = beemEndPos - beemStartPos;
        differenceLine = distance;

        while (differenceLine.magnitude > 0.1f)
        {
            differenceLine = differenceLine / 2f;
        }

        differenceNum = (int)(distance.magnitude / differenceLine.magnitude);

        lineRenderer.alignment = LineAlignment.TransformZ;

        lineRenderer.textureMode = LineTextureMode.Tile;

        lineRenderer.SetWidth(1.5f, 1.5f);

        lineRenderer.material = lineMaterial;

        for (int i = 0; i < differenceNum; i++)
        {
            yield return new WaitForSeconds(0.02f);
            lineRenderer.positionCount=i+1;

            lineRenderer.SetPosition(i, beemStartPos + differenceLine);
            beemStartPos = beemStartPos + differenceLine;
        }
    }
}
