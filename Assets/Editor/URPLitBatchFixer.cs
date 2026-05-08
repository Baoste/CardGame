using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

public class URPLitBatchFixer : EditorWindow
{
    private Object folderToScan;

    [MenuItem("Tools/Smart URP Lit Fixer")]
    public static void ShowWindow()
    {
        GetWindow<URPLitBatchFixer>("Smart URP Lit Fixer");
    }

    private void OnGUI()
    {
        GUILayout.Label("智能批量升级材质并绑定贴图 (URP Lit)", EditorStyles.boldLabel);
        folderToScan = EditorGUILayout.ObjectField("资源文件夹", folderToScan, typeof(Object), false);

        if (GUILayout.Button("执行智能批量升级"))
        {
            if (folderToScan == null)
            {
                EditorUtility.DisplayDialog("提示", "请选择一个文件夹！", "确定");
                return;
            }

            string folderPath = AssetDatabase.GetAssetPath(folderToScan);
            ProcessAllMaterials(folderPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("完成", "智能批量材质升级完成！", "OK");
        }
    }

    private void ProcessAllMaterials(string folderPath)
    {
        // 找到所有材质
        string[] materialGuids = AssetDatabase.FindAssets("t:Material", new[] { folderPath });
        int count = 0;

        foreach (string guid in materialGuids)
        {
            string matPath = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
            if (mat == null) continue;

            // 升级 Shader
            mat.shader = Shader.Find("Universal Render Pipeline/Lit");

            // 自动扫描材质上所有贴图
            List<string> texProps = new List<string> { "_MainTex", "_BumpMap", "_MetallicGlossMap", "_OcclusionMap", "_ParallaxMap" };
            foreach (var prop in texProps)
            {
                if (!mat.HasProperty(prop)) continue;

                Texture tex = mat.GetTexture(prop);
                if (tex != null)
                {
                    // 对于 Normal Map，确保启用关键词
                    if (prop == "_BumpMap")
                        mat.EnableKeyword("_NORMALMAP");

                    // 对于 Metallic Map，启用关键词
                    if (prop == "_MetallicGlossMap")
                        mat.EnableKeyword("_METALLICGLOSSMAP");
                }
            }

            EditorUtility.SetDirty(mat);
            count++;
        }

        Debug.Log($"处理完成，共处理 {count} 个材质！");
    }
}