using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetManager : MonoBehaviour
{
    public List<GameObject> planetList = new List<GameObject>();

    [SerializeField]
    private StageProduction stageProduction;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ObservePlanet()
    {
        bool isAllPlanetSmile=true;

        foreach(GameObject planet in planetList)
        {
            if (planet.GetComponent<PlanetComponent>().planetFace != GameManager.PlanetFace.smile) isAllPlanetSmile = false;
        }

        if (isAllPlanetSmile)
        {
            stageProduction.ClearProduction(planetList);
        }
    }
}
