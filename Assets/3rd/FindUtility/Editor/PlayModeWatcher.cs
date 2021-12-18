using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class PlaymodeHook
{
    static PlaymodeHook()
    {
        EditorApplication.update += Update;
        EditorApplication.playmodeStateChanged += PlaymodeStateChanged;
    }

    private static void Update()
    {
        if (!EditorApplication.isPlaying &&
            EditorApplication.isPlayingOrWillChangePlaymode)
        {
            BeforeEnterPlaymode();
        }
    }

    private static void PlaymodeStateChanged()
    {
        if (!EditorApplication.isPlaying &&
            !EditorApplication.isPlayingOrWillChangePlaymode)
        {
            AfterExitPlaymode();
        }
    }

    private static void BeforeEnterPlaymode()
    {
        Debug.Log("BeforeEnterPlaymode");
    }

    private static void AfterExitPlaymode()
    {
        Debug.Log("AfterExitPlaymode");
    }
}