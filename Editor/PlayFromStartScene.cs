using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class PlayFromStartScene
{
    static PlayFromStartScene()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            string prevScene = EditorPrefs.GetString("prevScenePath", "");
            if (!string.IsNullOrEmpty(prevScene) &&
                System.IO.File.Exists(prevScene))
            {
                EditorSceneManager.OpenScene(prevScene);
            }
            EditorPrefs.DeleteKey("prevScenePath");
        }
    }
}