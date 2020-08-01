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

    public enum ObjType
    {
        Planet,
        Mirror,
        Obstacle,
        Edge
    }

    public bool isClearGame;

    public enum Stage
    {
        Cmi,
        Tri,
        Cyg,
        Del,
        Cnc,
        Crt,
        Crv,
        Crb,
        Cep,
        Sgr,
        Lep,
        Lyra,
        Umi,
        Cas,
        Gem,
        Ori,
        Her,
        Peg,
        Cra,
        Uma,
        Cma,
        Aur,
        Lup,
        Psc,
        Cet,
        Dra,
        Cen,
        Ari,
        Leo,
        And,
        Aql,
        Tau,
        Oph,
        Sge,
        Equ,
        Ara,
        Psa,
        Vir,
        Boo,
        Lib,
        Cap,
        Agr,
        Per,
        Eri,
        Sco,
        Hya,
        Argo
    }

    public int clearStageNum;

    // Start is called before the first frame update
    void Start()
    {
        clearStageNum = PlayerPrefs.GetInt(Const.clearStageNumKey, -1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
