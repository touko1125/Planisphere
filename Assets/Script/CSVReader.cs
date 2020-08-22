using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CSVReader : SingletonMonoBehaviour<CSVReader>
{
    TextAsset csvConstellationList;

    private List<TextAsset> textAssetList = new List<TextAsset>();

    private List<List<string[]>> csvDateList = new List<List<string[]>>();

    public bool isSetCsvDate;

    // Start is called before the first frame update
    void Start()
    {
        InitializationCSVFile();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializationCSVFile()
    {
        //Resorces下ファイルの読み込み
        textAssetList.Add(Resources.Load(Const.csvConstellationKey) as TextAsset);
        textAssetList.Add(Resources.Load(Const.csvStoryKey) as TextAsset);

        for (int i = 0; i < textAssetList.Count; i++)
        {
            var reader = new StringReader(textAssetList[i].text);

            csvDateList.Add(new List<string[]>());
            
            //,で分割し一行ずつ読み込みしリストに入れていく
            while (reader.Peek() != -1)
            {
                string line = reader.ReadLine();
                csvDateList[i].Add(line.Split(','));
            }
        }

        isSetCsvDate = true;
    }

    public string getCollectionTitle(int collectionNum)
    {
        return csvDateList[0][collectionNum][1];
    }

    public string getCollectionExplanation(int collectionNum)
    {
        return csvDateList[0][collectionNum][2];
    }

    public string getStoryText(int collectionNum)
    {
        return csvDateList[1][collectionNum][0];
    }

    public string getStoryPerson(int collectionNum)
    {
        return csvDateList[1][collectionNum][1];
    }

    public string getStoryBackGround(int collectionNum)
    {
        return csvDateList[1][collectionNum][2];
    }

    public string getStoryPersonPos(int collectionNum)
    {
        return csvDateList[1][collectionNum][3];
    }
}
