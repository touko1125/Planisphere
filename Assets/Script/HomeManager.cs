using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TapClass;

public class HomeManager : MonoBehaviour
{
    private bool isDisplayAnotherScene;

    private string SceneName;

    [SerializeField]
    private GameObject canvas;

    private GameObject stageSelectObj;

    private GameObject collectionObj;

    // Start is called before the first frame update
    void Start()
    {
        stageSelectObj = canvas.transform.Find("StageSelect").gameObject;
        collectionObj = canvas.transform.Find("Collection").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        ObserveSceneTransitionInput();
    }

    public void ObserveSceneTransitionInput()
    {
        if (!InputManager.Instance.tapinfo.is_tap) return;

        if (getTapInfo() == null) return;

        if (getTapInfo().tap_Obj == null) return;

        if (getTapInfo().tap_Obj.name != "Collection" && getTapInfo().tap_Obj.name != "StageSelect") return;

        if (isDisplayAnotherScene) return;

        SceneTransition(getTapInfo().tap_Obj.name);
    }

    public void SceneTransition(string sceneName)
    {
        isDisplayAnotherScene = true;

        SceneName = sceneName;

        switch (SceneName)
        {
            case "StageSelect":
                stageSelectObj.SetActive(true);
                break;
            case "Collection":
                collectionObj.SetActive(true);
                break;
        }

        Application.LoadLevelAdditive(sceneName);
    }

    public void UnloadScene()
    {
        Application.UnloadLevel(SceneName);

        switch (SceneName)
        {
            case "StageSelect":
                stageSelectObj.SetActive(false);
                break;
            case "Collection":
                collectionObj.SetActive(false);
                break;
        }

        isDisplayAnotherScene = false;
    }

    public Tap getTapInfo()
    {
        return InputManager.Instance.tapinfo;
    }
}
