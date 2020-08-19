using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public bool isClearGame;

    public bool isPauseGame;

    public List<List<Vector2>> planetPosList = new List<List<Vector2>>();

    public int clearStageNum;

    // Start is called before the first frame update
    void Start()
    {
        clearStageNum = PlayerPrefs.GetInt(Const.clearStageNumKey, -1);

        planetPosList = PlayerPrefsUtility.LoadMultidimensionalList<Vector2>(Const.planetCollectionPosKey);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveClearStageNum()
    {
        PlayerPrefs.SetInt(Const.clearStageNumKey,clearStageNum);

        PlayerPrefs.Save();
    }

    public void SaveCollectionPlanetPos()
    {
        PlayerPrefsUtility.SaveMultiDimensionalList<Vector2>(Const.planetCollectionPosKey,planetPosList);

        var n = PlayerPrefsUtility.LoadMultidimensionalList<Vector2>(Const.planetCollectionPosKey);

        Debug.Log(n.Count);
    }
}
