using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Dsu.Framework;

public class TestSceneLoader : SceneLoaderBase
{
    public Slider loadingBar; // Reference to a UI Slider for progress

    protected override void OnProgress(float progress)
    {
        Debug.Log($"SceneLoader Loading progress: {progress * 100}%");
        if (loadingBar != null)
            loadingBar.value = progress;
    }
}
