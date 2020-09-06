using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Text;
using System;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public bool isClearGame;

    public bool isPauseGame;

    public List<List<Vector2>> planetPosList = new List<List<Vector2>>();

    public int clearStageNum;

    // Start is called before the first frame update
    void Start()
    {

        PlayerPrefs.DeleteAll();
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

    public void UnloadScene(string sceneName)
    {
        Debug.Log("aaaa");

        Application.UnloadLevel(sceneName);
        Resources.UnloadUnusedAssets();
    }

    public void SaveCollectionPlanetPos()
    {
        PlayerPrefsUtility.SaveMultiDimensionalList<Vector2>(Const.planetCollectionPosKey,planetPosList);

        WritePlanetPosString(planetPosList);

        var n = PlayerPrefsUtility.LoadMultidimensionalList<Vector2>(Const.planetCollectionPosKey);
    }

    public void WritePlanetPosString(List<List<Vector2>> vector2DimentialList)
    {
        StreamWriter streamWriter = new StreamWriter(@"planetPosCSV.csv",true, Encoding.GetEncoding("Shift_JIS"));

        for (int i = 0; i < vector2DimentialList.Count; i++)
        {
            for(int n = 0; n < vector2DimentialList[i].Count; n++)
            {
                string[] str = { i.ToString(), "(" + vector2DimentialList[i][n].x.ToString() + "." + vector2DimentialList[i][n].y.ToString() + ")" };
                var str2 = string.Join(",", str);

                Debug.Log(str2);
                streamWriter.WriteLine(str2);
            }
        }

        streamWriter.Close();
    }
}
