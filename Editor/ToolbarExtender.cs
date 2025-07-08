using UnityEditor;
using UnityEngine;
using System;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Reflection;

public static class ToolbarExtender
{
    private static readonly Type ToolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
    private static ScriptableObject currentToolbar;

    public static Action OnToolbarGUI;

    static ToolbarExtender()
    {
        EditorApplication.update += OnUpdate;
    }

    private static void OnUpdate()
    {
        if (currentToolbar == null)
        {
            var toolbars = Resources.FindObjectsOfTypeAll(ToolbarType);
            if (toolbars.Length == 0) return;

            currentToolbar = (ScriptableObject)toolbars[0];
            var rootProperty = ToolbarType.GetField(
                "m_Root",
                BindingFlags.NonPublic | BindingFlags.Instance
            );

            if (rootProperty == null) return;

            var rootValue = rootProperty.GetValue(currentToolbar);
            var root = rootValue as VisualElement;

            if (root != null)
            {
                var leftContainer = root.Q("ToolbarZonePlayMode");
                if (leftContainer != null)
                {
                    var container = new IMGUIContainer(() => {
                        OnToolbarGUI?.Invoke();
                    });
                    container.style.marginRight = 5;

                    leftContainer.Insert(1, container);
                }
            }
        }
    }
}