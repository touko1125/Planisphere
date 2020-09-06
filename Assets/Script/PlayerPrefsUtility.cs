//  PlayerPrefsUtility.cs
//  http://kan-kikuchi.hatenablog.com/entry/PlayerPrefsUtility
//
//  Created by kan kikuchi on 2015.10.22.

using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Text;
using System;

/// <summary>
/// PlayerPrefsに関する便利クラス
/// </summary>
public static class PlayerPrefsUtility
{

    //=================================================================================
    //保存
    //=================================================================================

    /// <summary>
    /// リストを保存
    /// </summary>
    public static void SaveList<T>(string key, List<T> value)
    {
        string serizlizedList = Serialize<List<T>>(value);
        PlayerPrefs.SetString(key, serizlizedList);
    }


    /// <summary>
    /// 多次元リスト(星の位置セーブ用)を保存
    /// </summary>
    public static void SaveMultiDimensionalList<T>(string key,List<List<T>> value)
    {
        var vector2StringListSerializeList = new List<string>();

        //各星座ごとのリストのなかのVector2型の要素をstring型に変換
        //それをリストに入れたやつをSerialize
        for(int i = 0; i < value.Count; i++)
        {
            var vector2StringList = new List<string>();

            for (int n = 0; n < value[i].Count; n++)
            {
                vector2StringList.Add(SerializeVector2(value[i][n]));
            }

            Another("SerializeVector2StringListCSV.csv", vector2StringList);

            Debug.Log(i);

            vector2StringListSerializeList.Add(Serialize(vector2StringList));

            Debug.Log(vector2StringListSerializeList.Count);

            WritePlanetPosDateCSV("vector2StringListSerializeListCSV.csv", vector2StringListSerializeList);
        }

        var serializedListString = Serialize(vector2StringListSerializeList);

        var serializedList = new List<string>();

        serializedList.Add(serializedListString);

        WritePlanetPosDateCSV("serializedListStringCSV.csv", serializedList);

        PlayerPrefs.SetString(key, serializedListString);
    }

    public static void Another(string folderPass, List<string> strList)
    {
        StreamWriter streamWriter = new StreamWriter(@folderPass,false);

        for (int i = 0; i < strList.Count; i++)
        {
            string[] str2 = strList[i].Split(',');
            var str3=i.ToString() + "," + str2[0] + ":" + str2[1] + "\n";
            Debug.Log(str3);
            streamWriter.Write(str3);
        }

        streamWriter.Flush();
    }

    public static void WritePlanetPosDateCSV(string folderPass,List<string> strList)
    {
        StreamWriter streamWriter = new StreamWriter(@folderPass,false,Encoding.GetEncoding("Shift_JIS"));

        for(int i=0;i<strList.Count;i++)
        {
            string[] str = {i.ToString(),strList[i]};
            var str2 = string.Join(",", str);

            Debug.Log(str2);

            streamWriter.WriteLine(str2);
        }

        streamWriter.Close();
    }

    /// <summary>
    /// ディクショナリーを保存
    /// </summary>
    public static void SaveDict<Key, Value>(string key, Dictionary<Key, Value> value)
    {
        string serizlizedDict = Serialize<Dictionary<Key, Value>>(value);
        PlayerPrefs.SetString(key, serizlizedDict);
    }

    //=================================================================================
    //読み込み
    //=================================================================================

    /// <summary>
    /// リストを読み込み
    /// </summary>
    public static List<T> LoadList<T>(string key)
    {
        //keyがある時だけ読み込む
        if (PlayerPrefs.HasKey(key))
        {
            var serizlizedList = PlayerPrefs.GetString(key);
            return Deserialize<List<T>>(serizlizedList);
        }

        return new List<T>();
    }

    public static List<List<T>> LoadMultidimensionalList<T>(string key)
    {
        var multiDimensionalList = new List<List<T>>();

        if (PlayerPrefs.HasKey(key))
        {
            //星座ごとの惑星の位置のリストがstring型にされたものをDeserializeする
            var serializedListString = PlayerPrefs.GetString(key);
            var vector2StringListSerializeList = Deserialize<List<string>>(serializedListString);

            //string型からそれぞれの星座の位置のList<string>をDeserialize
            for (int i = 0; i < vector2StringListSerializeList.Count; i++)
            {
                var vector2StringList = Deserialize<List<string>>(vector2StringListSerializeList[i]);

                multiDimensionalList.Add(new List<T>());

                for (int n = 0; n < vector2StringList.Count; n++)
                {
                    multiDimensionalList[i].Add(DeserializeVector2<T>(vector2StringList[n]));
                }
            }

            return multiDimensionalList;
        }

        //初期値
        var defaultMultiDimensionalList = new List<List<T>>();

        for(int i = 0; i <= (int)Enum.Stage.Argo; i++)
        {
            defaultMultiDimensionalList.Add(new List<T>());
        }

        return defaultMultiDimensionalList;
    }

    /// <summary>
    /// ディクショナリーを読み込み
    /// </summary>
    public static Dictionary<Key, Value> LoadDict<Key, Value>(string key)
    {
        //keyがある時だけ読み込む
        if (PlayerPrefs.HasKey(key))
        {
            string serizlizedDict = PlayerPrefs.GetString(key);
            return Deserialize<Dictionary<Key, Value>>(serizlizedDict);
        }

        return new Dictionary<Key, Value>();
    }

    //=================================================================================
    //シリアライズ、デシリアライズ
    //=================================================================================

    private static string SerializeVector2<T>(T vec)
    {
        return JsonUtility.ToJson(vec);
    }

    private static T DeserializeVector2<T>(string key)
    {
        return JsonUtility.FromJson<T>(key);
    }

    private static string Serialize<T>(T obj)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        MemoryStream memoryStream = new MemoryStream();
        binaryFormatter.Serialize(memoryStream, obj);
        return Convert.ToBase64String(memoryStream.GetBuffer());
    }

    private static T Deserialize<T>(string str)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(str));
        return (T)binaryFormatter.Deserialize(memoryStream);
    }
}