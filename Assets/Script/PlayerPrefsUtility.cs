//  PlayerPrefsUtility.cs
//  http://kan-kikuchi.hatenablog.com/entry/PlayerPrefsUtility
//
//  Created by kan kikuchi on 2015.10.22.

using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
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
        var inListListSerialized = new List<string>();

        //各星座ごとのリストのなかのVector2型の要素をstring型に変換
        //それをリストに入れたやつをSerialize
        for(int i = 0; i < value.Count; i++)
        {
            var vectorStringList = new List<string>();

            for(int n = 0; n < value[i].Count; n++)
            {
                vectorStringList.Add(SerializeVector2(value[i][n]));
            }

            inListListSerialized.Add(Serialize(vectorStringList));
        }

        string serializedList = Serialize(inListListSerialized);
        PlayerPrefs.SetString(key, serializedList);
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
            string serizlizedList = PlayerPrefs.GetString(key);
            return Deserialize<List<T>>(serizlizedList);
        }

        return new List<T>();
    }

    public static List<List<T>> LoadMultidimensionalList<T>(string key)
    {
        var multidimensionalList = new List<List<T>>();

        if (PlayerPrefs.HasKey(key))
        {
            //星座ごとの惑星の位置のリストがstring型にされたものをDeserializeする
            string serializedList = PlayerPrefs.GetString(key);
            var inListListSerialized = Deserialize<List<string>>(serializedList);

            //string型からそれぞれの星座の位置のList<string>をDeserialize
            for (int i = 0; i < inListListSerialized.Count; i++)
            {
                var vectorStringList = Deserialize<List<string>>(inListListSerialized[i]);

                multidimensionalList.Add(new List<T>());

                for (int n = 0; n < vectorStringList.Count; n++)
                {
                    multidimensionalList[i].Add(DeserializeVector2<T>(vectorStringList[n]));
                }
            }

            return multidimensionalList;
        }

        var defaultMultidimensionalList = new List<List<T>>();

        for(int i = 0; i <= (int)Enum.Stage.Argo; i++)
        {
            defaultMultidimensionalList.Add(new List<T>());
        }

        return defaultMultidimensionalList;
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