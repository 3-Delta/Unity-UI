using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// https://networm.me/2018/12/02/unity-show-asset-relationship/
public class ShowAssetRelationship : EditorWindow
{
    [MenuItem("Tools/Misc/Show Asset Relationship")]
    private static void ShowWindow()
    {
        var window = GetWindow(typeof(ShowAssetRelationship));
        window.Show();
    }

    private void OnGUI()
    {
        _targetDependent = EditorGUILayout.ObjectField("Dependent: ", _targetDependent, typeof(Object), false);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Find"))
        {
            _results = Find(AssetDatabase.GetAssetPath(_targetDependent),
                AssetDatabase.GetAssetPath(_targetDependency));
            if (_results.Count == 0)
            {
                ShowNotification(new GUIContent("Could not find relationship between two assets"));
            }
        }

        if (GUILayout.Button("Swap"))
        {
            var temp = _targetDependent;
            _targetDependent = _targetDependency;
            _targetDependency = temp;
        }

        if (GUILayout.Button("Clear"))
        {
            _targetDependent = null;
            _targetDependency = null;
            _results = null;
        }

        GUILayout.EndHorizontal();

        if (_results != null)
        {
            GUILayout.BeginHorizontal();

            foreach (var result in _results)
            {
                GUILayout.BeginVertical();

                foreach (var asset in result.Routine)
                {
                    EditorGUILayout.ObjectField("", asset, typeof(Object), false);
                }

                GUILayout.EndVertical();
            }

            GUILayout.EndHorizontal();
        }

        _targetDependency = EditorGUILayout.ObjectField("Dependeny: ", _targetDependency, typeof(Object), false);
    }

    private List<Result> Find(string targetDependent, string targetDependency)
    {
        var stack = new Stack<string>();
        stack.Push(targetDependent);
        var results = new List<Result>();

        var dependencies = AssetDatabase.GetDependencies(targetDependent, false);
        foreach (var dependency in dependencies)
        {
            RecursiveFind(dependency, targetDependency, stack, results);
        }

        return results;
    }

    private void RecursiveFind(string targetDependent, string targetDependency, Stack<string> stack,
        List<Result> results)
    {
        stack.Push(targetDependent);

        if (targetDependent == targetDependency)
        {
            var result = new Result(stack);
            results.Add(result);
        }
        else
        {
            var dependencies = AssetDatabase.GetDependencies(targetDependent, false);
            foreach (var dependency in dependencies)
            {
                RecursiveFind(dependency, targetDependency, stack, results);
            }
        }

        stack.Pop();
    }

    private class Result
    {
        public readonly List<Object> Routine = new List<Object>();

        public Result(Stack<string> result)
        {
            foreach (var path in result)
            {
                Routine.Insert(0, AssetDatabase.LoadMainAssetAtPath(path));
            }
        }
    }

    private Object _targetDependent;
    private Object _targetDependency;
    private List<Result> _results;
}
