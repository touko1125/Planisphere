using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlanetComponent : MonoBehaviour
{
    //個人情報
    public Enum.PlanetFace planetFace;
    public Enum.PlanetMode planetMode;
    public int planetCount;
    public float angle;

    private List<GameObject> FaceParent=new List<GameObject>();
    private List<GameObject> currentFace=new List<GameObject>();

    private int devidedCount;
    private List<GameObject> devidedPrefabObjects=new List<GameObject>();
    private GameObject devideSpriteObjPrefab;
    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform child in this.gameObject.transform)
        {
            if (child.name == "Face")
            {
                FaceParent.Add(child.gameObject);

                currentFace.Add(child.GetChild((int)planetFace).gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeFace(Enum.PlanetFace face)
    {
        for(int i = 0; i < FaceParent.Count; i++)
        {
            //今の表情を非表示に
            currentFace[i].SetActive(false);

            //個人情報の更新
            planetFace = face;

            FaceParent[i].transform.GetChild((int)face).gameObject.SetActive(true);

            //新しい表情の登録
            currentFace[i] = FaceParent[i].transform.GetChild((int)face).gameObject;
        }
    }

    public bool AbleDevide()
    {
        return devidedCount < planetCount;
    }

    public IEnumerator DevidePlanet()
    {
        if (!AbleDevide()) yield break; ;

        //初回はPrefab用に取得
        if (devidedCount == 0)
        {
            devideSpriteObjPrefab = gameObject.transform.Find("Prefab").gameObject;

            devidedPrefabObjects.Add(devideSpriteObjPrefab);
        }
        
        //切られた数増やす
        devidedCount++;

        var newPrefab = Instantiate(devideSpriteObjPrefab, gameObject.transform.position, Quaternion.identity);

        //親は自分
        newPrefab.transform.parent = gameObject.transform;

        devidedPrefabObjects.Add(newPrefab);

        //惑星の処理
        for(int i = 0; i < devidedPrefabObjects.Count; i++)
        {
            //ランダムな位置に設置
            devidedPrefabObjects[i].transform.localPosition = new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f), 0);

            //サイズを一回り小さく
            devidedPrefabObjects[i].transform.localScale = new Vector3(1 - (0.2f * devidedCount), 1 - (0.2f * devidedCount), 1);
        }

        //分けないと変になった?
        for (int i = 0; i < devidedPrefabObjects.Count; i++)
        {
            devidedPrefabObjects[i].transform.Find("Effect").gameObject.SetActive(true);

            yield return devidedPrefabObjects[i].transform.Find("Effect").DOScale(new Vector3(1, 1, 1), 0.4f).SetEase(Ease.OutCirc).WaitForCompletion();
        }

        //初期化
        for (int i = 0; i < devidedPrefabObjects.Count; i++)
        {
            devidedPrefabObjects[i].transform.Find("Effect").localScale=new Vector3(0.4f,0.4f,1);

            devidedPrefabObjects[i].transform.Find("Effect").gameObject.SetActive(false);
        }
    }
}
