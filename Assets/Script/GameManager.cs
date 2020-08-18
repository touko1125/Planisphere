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

        for (int i = 0; i < (int)Enum.Stage.Argo; i++)
        {
            planetPosList.Add(new List<Vector2>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
