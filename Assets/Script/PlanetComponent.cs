using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetComponent : MonoBehaviour
{
    //個人情報
    public GameManager.PlanetFace planetFace;
    public GameManager.PlanetMode planetMode;
    public int planetCount;
    public float angle;

    private List<GameObject> FaceParent=new List<GameObject>();
    private List<GameObject> currentFace=new List<GameObject>();
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

    public void ChangeFace(GameManager.PlanetFace face)
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
}
