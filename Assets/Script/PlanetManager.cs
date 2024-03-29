﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class PlanetManager : MonoBehaviour
{
    public List<GameObject> planetList = new List<GameObject>();

    [SerializeField]
    private BeamComponent beamComponent;

    [SerializeField]
    private StageProduction stageProduction;

    [SerializeField]
    private GameObject lineRendererObjPrefab;

    [SerializeField]
    private Material lineRendererMaterial;

    private List<GameObject> lineRendererObjects = new List<GameObject>();

    private GameObject coverObj;
    // Start is called before the first frame update
    void Start()
    {
        coverObj = GameObject.Find("Cover");
    }

    // Update is called once per frame
    void Update()
    {
        if(!GameManager.Instance.isClearGame) StartCoroutine(ObservePlanet());
    }

    public IEnumerator ObservePlanet()
    {
        if (beamComponent.isDrawLine) yield break;

        bool isAllPlanetSmile=true;

        foreach(GameObject planet in planetList)
        {
            if (planet.GetComponent<PlanetComponent>().planetFace != Enum.PlanetFace.surprise) isAllPlanetSmile = false;
        }

        if (isAllPlanetSmile)
        {
            GameManager.Instance.isClearGame = true;

            beamComponent.ResetLine();

            if(coverObj!=null) coverObj.SetActive(false);   //カバー消す

            yield return StartCoroutine(ConnectPlanet());

            IEnumerator clearProduction = stageProduction.ClearProduction(planetList);

            stageProduction.StartCoroutine(clearProduction);

            for (int i = 0; i < planetList.Count; i++)
            {
                GameManager.Instance.planetPosList[(int)(Enum.Stage)System.Enum.Parse(typeof(Enum.Stage), SceneManager.GetActiveScene().name)].Add(planetList[i].transform.position);
            }

            GameManager.Instance.SaveCollectionPlanetPos();
        }
    }

    public IEnumerator ConnectPlanet()
    {
        lineRendererObjects.Clear();

        List<Vector2> drawConnectLinePosList = new List<Vector2>();
        for(int i = 0; i < planetList.Count; i++)
        {
            drawConnectLinePosList.Add(planetList[i].transform.position);
        }

        //n個星があったらn-1個線を引く
        for(int i = 0; i < drawConnectLinePosList.Count-1; i++)
        {

            //線のベクトル取得
            Vector2 lineVector = drawConnectLinePosList[i + 1] - drawConnectLinePosList[i];

            //線のLineRendererの生成
            GameObject lineRendererObj = Instantiate(lineRendererObjPrefab, Vector3.zero, Quaternion.identity);

            lineRendererObjects.Add(lineRendererObj);

            LineRenderer lineRenderer = lineRendererObj.GetComponent<LineRenderer>();

            lineRenderer.alignment = LineAlignment.TransformZ;

            lineRenderer.material = lineRendererMaterial;

            lineRenderer.material.color = new Color(1, 1, 1, 0);

            lineRenderer.SetWidth(0.02f, 0.02f);

            lineRenderer.textureMode = LineTextureMode.Tile;

            Vector2 lineDifference=lineVector;

            while (lineDifference.magnitude > 0.1f)
            {
                lineDifference = lineDifference / 2;
            }

            int differenceNum = (int)(lineVector.magnitude / lineDifference.magnitude);

            for(int n = 0; n < differenceNum; n++)
            {
                lineRenderer.positionCount=n+1;
                lineRenderer.SetPosition(n, drawConnectLinePosList[i] + (lineDifference * n));
            }
        }

        var waitTime = 2f;

        for (int i = 0; i < lineRendererObjects.Count; i++)
        {
            //yield return StartCoroutine(FadeLine(line.GetComponent<LineRenderer>()));

            var lineMaterial = lineRendererObjects[i].GetComponent<LineRenderer>().material;

            //LIneの表示
            DOTween.ToAlpha(() => lineMaterial.color,
                            color => lineMaterial.color = color,
                            1,
                            waitTime);
        }

        yield return new WaitForSeconds(waitTime);

        yield break;
    }
}
