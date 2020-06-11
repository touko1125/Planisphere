using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BeamComponent : MonoBehaviour
{

    private LineRenderer lineRenderer;

    public Material lineMaterial;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShotBeam(GameObject shotbeamTabObj)
    {
        Debug.Log("aaaa");

        //頂点が最初の点なので単位円からは90度回転している?
        Vector2 direction = new Vector2(Mathf.Sin(shotbeamTabObj.transform.parent.rotation.z), -Mathf.Cos(shotbeamTabObj.transform.parent.rotation.z));

        //レイヤーの指定
        int rayerNum = LayerMask.NameToLayer("BeamCollision");

        Ray beemRay = new Ray(shotbeamTabObj.transform.position, direction);

        RaycastHit beemRaycast;

        //太くする
        Physics.SphereCast(beemRay, 3.0f,out beemRaycast,rayerNum);

        //Scene画面に描画
        Debug.DrawRay(beemRay.origin, beemRay.direction*3.0f, Color.white, 5.0f);

        lineRenderer.SetPosition(0, beemRay.origin);
        lineRenderer.SetPosition(1, beemRay.direction*3.0f);

        lineRenderer.alignment = LineAlignment.Local;

        lineRenderer.textureMode = LineTextureMode.RepeatPerSegment;

        lineRenderer.SetWidth(0.2f,0.2f);

        lineRenderer.material = lineMaterial;

        //lineRenderer.SetColors(Color.white,Color.white);
    }
}
