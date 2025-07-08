using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class ToolbarButton
{
    private static int selectedSceneIndex;
    private static string[] sceneNames;

    static ToolbarButton()
    {
        ToolbarExtender.OnToolbarGUI = OnToolbarGUI;
        RefreshScenes();

        string savedPath = EditorPrefs.GetString("customStartScenePath", "");
        if (!string.IsNullOrEmpty(savedPath))
        {
            var scenes = EditorBuildSettings.scenes
                .Where(s => s.enabled)
                .Select(s => s.path)
                .ToArray();

            selectedSceneIndex = Array.IndexOf(scenes, savedPath);
            if (selectedSceneIndex < 0) selectedSceneIndex = 0;
        }
    }

    private static void RefreshScenes()
    {
        var scenes = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .ToArray();

        sceneNames = scenes
            .Select(s => System.IO.Path.GetFileNameWithoutExtension(s.path))
            .ToArray();
    }

    private static void OnToolbarGUI()
    {
        if (sceneNames == null || sceneNames.Length == 0)
        {
            RefreshScenes();
            if (sceneNames == null || sceneNames.Length == 0) return;
        }

        var style = new GUIStyle("Command")
        {
            fixedWidth = 32,
            fixedHeight = 22,
            imagePosition = ImagePosition.ImageAbove,
            fontSize = 8,
            padding = new RectOffset(0, 0, 1, 0),
            margin = new RectOffset(0, 5, 0, 0)
        };

        var buttonContent = new GUIContent(
            EditorGUIUtility.IconContent("d_PlayButton").image,
            "Play from custom scene\n(Right-click to select scene)"
        );

        if (GUILayout.Button(buttonContent, style))
        {
            if (Event.current.button == 0)
            {
                PlayFromSelectedScene();
            }
            else if (Event.current.button == 1)
            {
                ShowSceneMenu();
            }
        }
    }

    private static void PlayFromSelectedScene()
    {
        if (selectedSceneIndex < 0 || selectedSceneIndex >= sceneNames.Length)
        {
            selectedSceneIndex = 0;
        }

        EditorPrefs.SetString("prevScenePath", EditorSceneManager.GetActiveScene().path);

        string scenePath = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .ElementAt(selectedSceneIndex).path;

        EditorSceneManager.OpenScene(scenePath);
        EditorApplication.isPlaying = true;
    }

    private static void ShowSceneMenu()
    {
        GenericMenu menu = new GenericMenu();

        for (int i = 0; i < sceneNames.Length; i++)
        {
            int index = i;
            menu.AddItem(
                new GUIContent(sceneNames[index]),
                index == selectedSceneIndex,
                () => {
                    selectedSceneIndex = index;
                    string path = EditorBuildSettings.scenes
                        .Where(s => s.enabled)
                        .ElementAt(index).path;

                    EditorPrefs.SetString("customStartScenePath", path);
                }
            );
        }

        menu.ShowAsContext();
    }
}