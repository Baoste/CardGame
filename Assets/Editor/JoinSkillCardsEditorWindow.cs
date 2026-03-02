using Game.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class JoinSkillCardsEditorWindow : EditorWindow
{
    [Serializable]
    private class SkillCardsWrapper
    {
        public List<Card> cards;
    }

    private List<TextAsset> jsonAssets = new List<TextAsset>();
    private Vector2 scroll;
    private string mergedJson = "";
    private bool autoPretty = true;

    [MenuItem("Tools/Join Skill Cards")]
    public static void Open()
    {
        var win = GetWindow<JoinSkillCardsEditorWindow>("Join Skill Cards");
        win.minSize = new Vector2(520, 360);
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(6);

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Add Selected Assets", GUILayout.Width(160)))
                AddSelectedAssets();

            if (GUILayout.Button("Add From Folder", GUILayout.Width(140)))
                AddFromFolder();

            if (GUILayout.Button("Clear List", GUILayout.Width(100)))
                ClearList();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Merge -> Preview", GUILayout.Width(140)))
                MergeToPreview();
        }

        EditorGUILayout.Space(8);

        using (new EditorGUILayout.VerticalScope("box"))
        {
            EditorGUILayout.LabelField("Selected JSON Assets", EditorStyles.boldLabel);

            scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(140));
            for (int i = 0; i < jsonAssets.Count; i++)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    jsonAssets[i] = (TextAsset)EditorGUILayout.ObjectField(jsonAssets[i], typeof(TextAsset), false);
                    if (GUILayout.Button("Remove", GUILayout.Width(80)))
                    {
                        jsonAssets.RemoveAt(i);
                        i--;
                    }
                }
            }
            EditorGUILayout.EndScrollView();

            if (jsonAssets.Count == 0)
                EditorGUILayout.HelpBox("No JSON assets added. Select JSON assets in Project window and click 'Add Selected Assets', or use 'Add From Folder'.", MessageType.Info);
        }

        EditorGUILayout.Space(8);

        using (new EditorGUILayout.HorizontalScope())
        {
            autoPretty = EditorGUILayout.ToggleLeft("Pretty JSON", autoPretty, GUILayout.Width(120));

            if (GUILayout.Button("Copy to Clipboard", GUILayout.Width(140)))
            {
                if (string.IsNullOrEmpty(mergedJson)) MergeToPreview();
                EditorGUIUtility.systemCopyBuffer = mergedJson;
                ShowNotification(new GUIContent("Copied merged JSON to clipboard"));
            }

            if (GUILayout.Button("Export SkillCardsT.json", GUILayout.Width(180)))
            {
                if (string.IsNullOrEmpty(mergedJson)) MergeToPreview();
                ExportMergedJson();
            }

            GUILayout.FlexibleSpace();
        }

        EditorGUILayout.Space(6);

        EditorGUILayout.LabelField("Merged JSON Preview", EditorStyles.boldLabel);
        mergedJson = EditorGUILayout.TextArea(mergedJson, GUILayout.MinHeight(140));
    }

    private void AddSelectedAssets()
    {
        var objs = Selection.objects;
        int added = 0;
        foreach (var o in objs)
        {
            if (o is TextAsset ta && !jsonAssets.Contains(ta))
            {
                jsonAssets.Add(ta);
                added++;
            }
        }

        if (added == 0)
            ShowNotification(new GUIContent("No TextAsset JSON selected or already added"));
        else
            ShowNotification(new GUIContent($"Added {added} asset(s)"));
    }

    private void AddFromFolder()
    {
        var folder = EditorUtility.OpenFolderPanel("Select folder (inside project) containing json", Application.dataPath, "");
        if (string.IsNullOrEmpty(folder)) return;

        // ensure folder is inside project Assets
        if (!folder.StartsWith(Application.dataPath, StringComparison.OrdinalIgnoreCase))
        {
            EditorUtility.DisplayDialog("Invalid folder", "Please select a folder inside this Unity project's Assets folder.", "OK");
            return;
        }

        var relativePath = "Assets" + folder.Substring(Application.dataPath.Length);
        var guids = AssetDatabase.FindAssets("t:TextAsset", new[] { relativePath });
        int added = 0;
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (Path.GetExtension(path).Equals(".json", StringComparison.OrdinalIgnoreCase))
            {
                var ta = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                if (ta != null && !jsonAssets.Contains(ta))
                {
                    jsonAssets.Add(ta);
                    added++;
                }
            }
        }

        ShowNotification(new GUIContent($"Added {added} json asset(s) from folder"));
    }

    private void ClearList()
    {
        if (EditorUtility.DisplayDialog("Clear list?", "Remove all selected json assets from list?", "Yes", "No"))
        {
            jsonAssets.Clear();
            mergedJson = "";
        }
    }

    private void MergeToPreview()
    {
        var cards = new List<Card>();
        foreach (var ta in jsonAssets)
        {
            if (ta == null) continue;
            try
            {
                var wrapper = JsonUtility.FromJson<CardEditorWindow.CardWrapper>(ta.text);
                if (wrapper?.card != null)
                {
                    cards.Add(wrapper.card);
                }
                else
                {
                    Debug.LogWarning($"Skipped {ta.name}: invalid wrapper.card is null");
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed parse {ta.name}: {e.Message}");
            }
        }

        var outWrapper = new SkillCardsWrapper { cards = cards };
        mergedJson = JsonUtility.ToJson(outWrapper, autoPretty);
        ShowNotification(new GUIContent($"Merged {cards.Count} card(s)"));
    }

    private void ExportMergedJson()
    {
        var projectPath = EditorUtility.SaveFilePanelInProject("Export SkillCardsT.json", "SkillCardsT.json", "json", "Export merged skill cards json");
        if (string.IsNullOrEmpty(projectPath)) return;

        try
        {
            File.WriteAllText(projectPath, mergedJson);
            AssetDatabase.Refresh();
            ShowNotification(new GUIContent("Saved SkillCardsT.json"));
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            EditorUtility.DisplayDialog("Save failed", e.Message, "OK");
        }
    }
}