using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetManager : MonoBehaviour
{
    public List<GameObject> planetList = new List<GameObject>();

    [SerializeField]
    private BeamComponent beamComponent;

    [SerializeField]
    private StageProduction stageProduction;

    [SerializeField]
    private GameObject lineRendererObjPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
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
            if (planet.GetComponent<PlanetComponent>().planetFace != GameManager.PlanetFace.surprise) isAllPlanetSmile = false;
        }

        if (isAllPlanetSmile)
        {
            beamComponent.ResetLine();

            yield return StartCoroutine(ConnectPlanet());

            yield return new WaitForSeconds(0.5f);

            IEnumerator clearProduction = stageProduction.ClearProduction(planetList);

            stageProduction.StartCoroutine(clearProduction);

            GameManager.Instance.isClearGame = true;
        }
    }

    public IEnumerator ConnectPlanet()
    {
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

            LineRenderer lineRenderer = lineRendererObj.GetComponent<LineRenderer>();

            lineRenderer.alignment = LineAlignment.TransformZ;

            lineRenderer.SetWidth(4.0f, 4.0f);

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
            yield return new WaitForSeconds(0.0f);
        }
    }
}
