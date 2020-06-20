using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetComponent : MonoBehaviour
{
    //個人情報
    public GameManager.PlanetFace planetFace;
    public GameManager.PlanetMode planetMode;

    private GameObject FaceParent;
    private GameObject currentFace;
    // Start is called before the first frame update
    void Start()
    {
        FaceParent = gameObject.transform.Find("Face").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeFace(GameManager.PlanetFace face)
    {
        //今の表情を非表示に
        currentFace.SetActive(false);

        //個人情報の更新
        planetFace = face;

        //Planetくん!新しい顔よ!
        FaceParent.transform.GetChild((int)face).gameObject.SetActive(true);
    }
}
