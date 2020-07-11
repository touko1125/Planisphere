using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageProduction : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClearProduction(List<GameObject> planetObjects)
    {
        for(int i = 0; i < planetObjects.Count; i++)
        {
            planetObjects[i].GetComponent<PlanetComponent>().ChangeFace(GameManager.PlanetFace.smile);
        }
        Debug.Log("Clear");
    }
}
