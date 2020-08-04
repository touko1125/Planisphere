using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CSVReader : SingletonMonoBehaviour<CSVReader>
{
    TextAsset csvConstellationList;

    List<string[]> csvDateConstellationList = new List<string[]>();

    // Start is called before the first frame update
    void Start()
    {
        //Resorce下のファイル読み込み
        csvConstellationList = Resources.Load("ConstellationCollection") as TextAsset;
        StringReader reader = new StringReader(csvConstellationList.text);

        //,で分割し一行ずつ読み込みしリストに入れていく
        while (reader.Peek() != -1)
        {
            string line = reader.ReadLine();
            csvDateConstellationList.Add(line.Split(','));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
