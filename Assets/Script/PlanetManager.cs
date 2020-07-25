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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!GameManager.Instance.isClearGame) ObservePlanet();
    }

    public void ObservePlanet()
    {
        if (beamComponent.isDrawLine) return;

        bool isAllPlanetSmile=true;

        foreach(GameObject planet in planetList)
        {
            if (planet.GetComponent<PlanetComponent>().planetFace != GameManager.PlanetFace.surprise) isAllPlanetSmile = false;
        }

        if (isAllPlanetSmile)
        {
            IEnumerator clearProduction = stageProduction.ClearProduction(planetList);

            stageProduction.StartCoroutine(clearProduction);

            GameManager.Instance.isClearGame = true;
        }
    }
}
