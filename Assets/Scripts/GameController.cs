using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public void OnQuit()
    {
        #if UNITY_EDITOR
        if (Application.isPlaying)
        {
            EditorApplication.ExitPlaymode();
        }
        #endif
        Application.Quit();
    }
}
