using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public enum PlanetMode
    {
        SinglePlanet,
        DoublePlanet,
        BlackHole
    }

    public enum PlanetFace
    {
        nomal,
        smile,
        surprise
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
