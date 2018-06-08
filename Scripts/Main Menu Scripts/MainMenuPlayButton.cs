using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuPlayButton : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        Debug.Log("Starting Load Co-routine");
        StartCoroutine(LoadAsyncScene());
	}

    IEnumerator LoadAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Testing Level");
        while(asyncLoad.isDone == false)
        {
            yield return null;
        }
    }
}
