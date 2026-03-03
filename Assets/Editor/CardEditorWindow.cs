using Game.Domain;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;

public class CardEditorWindow : EditorWindow
{
    [InitializeOnLoadMethod]
    private static void InitEditor()
    {
        JsonBootstrap.Init();
    }

    [Serializable]
    public class CardWrapper
    {
        public Card card;
    }


    private Card current = new Card
    {
        effects = new List<EffectOp>()
    };

    private Vector2 scroll;

    // JSON 面板
    private string jsonText = "";
    private bool foldJson = true;

    // 基本校验提示
    private string validationMsg = "";

    // UI state
    private List<bool> effectFoldouts = new List<bool>();

    [MenuItem("Tools/Card Editor")]
    public static void Open()
    {
        var win = GetWindow<CardEditorWindow>("Card Editor");
        win.minSize = new Vector2(540, 600);
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(6);
        DrawToolbar();

        EditorGUILayout.Space(8);
        scroll = EditorGUILayout.BeginScrollView(scroll);

        DrawCardBaseInfo();
        EditorGUILayout.Space(10);
        DrawEffects();
        EditorGUILayout.Space(10);
        DrawValidation();

        EditorGUILayout.Space(10);
        DrawJsonPanel();

        EditorGUILayout.EndScrollView();
    }

    private void DrawToolbar()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("New", GUILayout.Width(80)))
            {
                current = new Card { effects = new List<EffectOp>() };
                jsonText = "";
                validationMsg = "";
                effectFoldouts.Clear();
                GUI.FocusControl(null);
            }

            if (GUILayout.Button("Copy as JSON", GUILayout.Width(120)))
            {
                ValidateCurrent();
                jsonText = ExportCurrentToJson(pretty: true);
                EditorGUIUtility.systemCopyBuffer = jsonText;
                ShowNotification(new GUIContent("Copied JSON to clipboard"));
            }

            if (GUILayout.Button("Import JSON -> Current", GUILayout.Width(170)))
            {
                ImportJsonToCurrent(jsonText);
                ValidateCurrent();
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Export .json file", GUILayout.Width(140)))
            {
                var path = EditorUtility.SaveFilePanel("Save card json", Application.dataPath, $"card_{current.name}.json", "json");
                if (!string.IsNullOrEmpty(path))
                {
                    System.IO.File.WriteAllText(path, ExportCurrentToJson(pretty: true));
                    AssetDatabase.Refresh();
                    ShowNotification(new GUIContent("Saved file"));
                }
            }
        }
    }

    private void DrawCardBaseInfo()
    {
        EditorGUILayout.LabelField("Card Base Info", EditorStyles.boldLabel);
        using (new EditorGUILayout.VerticalScope("box"))
        {
            current.id = EditorGUILayout.IntField("ID", current.id);
            current.name = EditorGUILayout.TextField("Name", current.name);
            current.description = EditorGUILayout.TextField("Description", current.description);

            current.type = (CardType)EditorGUILayout.EnumPopup("Type", current.type);
            current.point = EditorGUILayout.IntField("Point", current.point);
            current.count = EditorGUILayout.IntField("Count (in deck)", current.count);
        }
    }

    private void DrawEffects()
    {
        EditorGUILayout.LabelField("Effects", EditorStyles.boldLabel);

        using (new EditorGUILayout.VerticalScope("box"))
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("+ Add Effect", GUILayout.Width(120)))
                {
                    current.effects ??= new List<EffectOp>();
                    current.effects.Add(new EffectOp
                    {
                        type = EffectType.DrawPoint,
                        source = new ParticipantSpec { filter = new NoneCondition() },
                        target = new ParticipantSpec { filter = new NoneCondition() },
                        value = new NoneValue()
                    });
                    effectFoldouts.Add(true);
                }

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Clear Effects", GUILayout.Width(120)))
                {
                    if (EditorUtility.DisplayDialog("Clear?", "Remove all effects?", "Yes", "No"))
                    {
                        current.effects?.Clear();
                        effectFoldouts.Clear();
                    }
                }
            }

            EditorGUILayout.Space(6);

            if (current.effects == null) current.effects = new List<EffectOp>();
            EnsureEffectFoldoutsCount();

            if (current.effects.Count == 0)
            {
                EditorGUILayout.HelpBox("No effects yet. Click '+ Add Effect'.", MessageType.Info);
                return;
            }

            for (int i = 0; i < current.effects.Count; i++)
            {
                var op = current.effects[i];

                using (new EditorGUILayout.VerticalScope("box"))
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        effectFoldouts[i] = EditorGUILayout.Foldout(effectFoldouts[i], $"Effect #{i} [{op.type}]", true);

                        GUILayout.FlexibleSpace();

                        GUI.enabled = i > 0;
                        if (GUILayout.Button("↑", GUILayout.Width(28)))
                        {
                            Swap(current.effects, i, i - 1);
                            Swap(effectFoldouts, i, i - 1);
                        }
                        GUI.enabled = i < current.effects.Count - 1;
                        if (GUILayout.Button("↓", GUILayout.Width(28)))
                        {
                            Swap(current.effects, i, i + 1);
                            Swap(effectFoldouts, i, i + 1);
                        }
                        GUI.enabled = true;

                        if (GUILayout.Button("X", GUILayout.Width(28)))
                        {
                            current.effects.RemoveAt(i);
                            effectFoldouts.RemoveAt(i);
                            break;
                        }
                    }

                    if (!effectFoldouts[i]) continue;

                    op.type = (EffectType)EditorGUILayout.EnumPopup("Type", op.type);

                    // Source editor
                    EditorGUILayout.Space(4);
                    EditorGUILayout.LabelField("Source", EditorStyles.boldLabel);
                    if (op.source == null) op.source = new ParticipantSpec { filter = new AllCondition() };
                    DrawParticipantSpecEditor(op.source);

                    EditorGUILayout.Space(4);
                    EditorGUILayout.LabelField("Target", EditorStyles.boldLabel);
                    if (op.target == null) op.target = new ParticipantSpec { filter = new AllCondition() };
                    DrawParticipantSpecEditor(op.target);

                    EditorGUILayout.Space(4);
                    EditorGUILayout.LabelField("Value", EditorStyles.boldLabel);
                    if (op.value == null) op.value = new NoneValue();
                    DrawValueExprEditor(ref op.value);

                    // write back (class is reference but keep style consistency)
                    current.effects[i] = op;
                }
            }
        }
    }

    private void DrawParticipantSpecEditor(ParticipantSpec spec)
    {
        using (new EditorGUILayout.VerticalScope("box"))
        {
            spec.participantType = (ParticipantType)EditorGUILayout.EnumFlagsField("Participant Type", spec.participantType);
            spec.participantSelectionMode = (ParticipantSelectionMode)EditorGUILayout.EnumPopup("Selection Mode", spec.participantSelectionMode);

            EditorGUILayout.Space(4);
            // filter (ConditionExpr) - always present (default to AllCondition)
            EditorGUILayout.LabelField("Filter (Condition)", EditorStyles.miniBoldLabel);
            if (spec.filter == null)
            {
                spec.filter = new AllCondition();
            }
            DrawConditionExprEditor(ref spec.filter);

            EditorGUILayout.Space(4);
            // maxCandidateCount / maxSelectCount (ValueExpr)
            EditorGUILayout.LabelField("Max Candidate Count", EditorStyles.miniBoldLabel);
            if (spec.maxCandidateCountWhenRandom == null) spec.maxCandidateCountWhenRandom = new NoneValue();
            DrawValueExprEditor(ref spec.maxCandidateCountWhenRandom);

            EditorGUILayout.LabelField("Max Select Count", EditorStyles.miniBoldLabel);
            if (spec.maxSelectCount == null) spec.maxSelectCount = new NoneValue();
            DrawValueExprEditor(ref spec.maxSelectCount);
        }
    }

    private void DrawValueExprEditor(ref ValueExpr value)
    {
        using (new EditorGUILayout.VerticalScope("box"))
        {
            // Type selector
            int sel = ValueExprTypeToIndex(value);
            int newSel = EditorGUILayout.Popup("Value Type", sel, new[] { "NoneValue", "ConstValue", "VariableValue", "BinaryValue" });
            if (newSel != sel)
            {
                value = CreateValueExprByIndex(newSel);
            }

            // Draw fields per concrete type
            switch (value)
            {
                case NoneValue _:
                    EditorGUILayout.LabelField("No value", EditorStyles.helpBox);
                    break;

                case ConstValue cv:
                    cv.value = EditorGUILayout.IntField("Value", cv.value);
                    break;

                case VariableValue vv:
                    vv.source = (ValueSource)EditorGUILayout.EnumPopup("Source", vv.source);
                    break;

                case BinaryValue bv:
                    bv.op = (BinaryOp)EditorGUILayout.EnumPopup("Op", bv.op);
                    if (bv.left == null) bv.left = new NoneValue();
                    if (bv.right == null) bv.right = new NoneValue();

                    EditorGUILayout.LabelField("Left", EditorStyles.miniBoldLabel);
                    DrawValueExprEditor(ref bv.left);
                    EditorGUILayout.LabelField("Right", EditorStyles.miniBoldLabel);
                    DrawValueExprEditor(ref bv.right);
                    break;
            }
        }
    }

    private void DrawConditionExprEditor(ref ConditionExpr cond)
    {
        using (new EditorGUILayout.VerticalScope("box"))
        {
            int sel = ConditionExprTypeToIndex(cond);
            int newSel = EditorGUILayout.Popup("Condition Type", sel, new[] { "NoneCondition", "AllCondition", "CompareCondition", "AndCondition" });
            if (newSel != sel)
            {
                cond = CreateConditionExprByIndex(newSel);
            }

            switch (cond)
            {
                case NoneCondition _:
                    EditorGUILayout.LabelField("Always false", EditorStyles.helpBox);
                    break;

                case AllCondition _:
                    EditorGUILayout.LabelField("Always true", EditorStyles.helpBox);
                    break;

                case CompareCondition cc:
                    if (cc.left == null) cc.left = new NoneValue();
                    if (cc.right == null) cc.right = new NoneValue();

                    EditorGUILayout.LabelField("Left", EditorStyles.miniBoldLabel);
                    DrawValueExprEditor(ref cc.left);
                    EditorGUILayout.LabelField("Right", EditorStyles.miniBoldLabel);
                    DrawValueExprEditor(ref cc.right);

                    cc.op = (CompareOp)EditorGUILayout.EnumPopup("Compare Op", cc.op);
                    break;

                case AndCondition ac:
                    if (ac.a == null) ac.a = new AllCondition();
                    if (ac.b == null) ac.b = new AllCondition();

                    EditorGUILayout.LabelField("A", EditorStyles.miniBoldLabel);
                    DrawConditionExprEditor(ref ac.a);
                    EditorGUILayout.LabelField("B", EditorStyles.miniBoldLabel);
                    DrawConditionExprEditor(ref ac.b);
                    break;
            }
        }
    }

    private void EnsureEffectFoldoutsCount()
    {
        if (effectFoldouts == null) effectFoldouts = new List<bool>();
        while (effectFoldouts.Count < current.effects.Count)
            effectFoldouts.Add(true);
        while (effectFoldouts.Count > current.effects.Count)
            effectFoldouts.RemoveAt(effectFoldouts.Count - 1);
    }

    private int ValueExprTypeToIndex(ValueExpr v)
    {
        return v switch
        {
            NoneValue _ => 0,
            ConstValue _ => 1,
            VariableValue _ => 2,
            BinaryValue _ => 3,
            _ => 0
        };
    }

    private ValueExpr CreateValueExprByIndex(int idx)
    {
        return idx switch
        {
            0 => new NoneValue(),
            1 => new ConstValue(),
            2 => new VariableValue(),
            3 => new BinaryValue(),
            _ => new NoneValue()
        };
    }

    private int ConditionExprTypeToIndex(ConditionExpr c)
    {
        return c switch
        {
            NoneCondition _ => 0,
            AllCondition _ => 1,
            CompareCondition _ => 2,
            AndCondition _ => 3,
            _ => 0
        };
    }

    private ConditionExpr CreateConditionExprByIndex(int idx)
    {
        return idx switch
        {
            0 => new NoneCondition(),
            1 => new AllCondition(),
            2 => new CompareCondition(),
            3 => new AndCondition(),
            _ => new NoneCondition()
        };
    }

    private void DrawValidation()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Validate", GUILayout.Width(100)))
            {
                ValidateCurrent();
            }

            GUILayout.FlexibleSpace();
        }

        if (!string.IsNullOrEmpty(validationMsg))
        {
            var msgType = validationMsg.StartsWith("OK") ? MessageType.Info : MessageType.Warning;
            EditorGUILayout.HelpBox(validationMsg, msgType);
        }
    }

    private void DrawJsonPanel()
    {
        foldJson = EditorGUILayout.Foldout(foldJson, "JSON Panel", true);
        if (!foldJson) return;

        using (new EditorGUILayout.VerticalScope("box"))
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Refresh JSON", GUILayout.Width(120)))
                {
                    ValidateCurrent();
                    jsonText = ExportCurrentToJson(pretty: true);
                }

                if (GUILayout.Button("Pretty", GUILayout.Width(80)))
                {
                    // 重新 pretty 一遍（JsonUtility 本身 prettyPrint）
                    ImportJsonToCurrent(jsonText);
                    jsonText = ExportCurrentToJson(pretty: true);
                }

                GUILayout.FlexibleSpace();
            }

            EditorGUILayout.Space(6);
            jsonText = EditorGUILayout.TextArea(jsonText, GUILayout.MinHeight(180));
        }
    }

    private void ValidateCurrent()
    {
        var errs = new List<string>();

        if (current.id <= 0) errs.Add("id should be > 0");
        if (string.IsNullOrWhiteSpace(current.name)) errs.Add("name is empty");
        if (current.count < 0) errs.Add("count should be >= 0");

        current.effects ??= new List<EffectOp>();
        for (int i = 0; i < current.effects.Count; i++)
        {
            var op = current.effects[i];
            if (op == null) errs.Add($"effects[{i}] is null");
            else
            {
                if (op.value == null) errs.Add($"effects[{i}].value is null");
                if (op.target == null) errs.Add($"effects[{i}].target is null");
                // additional rule examples
                // if (op.type == EffectType.ModifyPoint && (op.value is ConstValue cv && cv.value == 0))
                //     errs.Add($"effects[{i}].value is 0 for ModifyPoint");
            }
        }

        validationMsg = errs.Count == 0
            ? "OK: Card looks valid."
            : "Warnings:\n- " + string.Join("\n- ", errs);
    }

    private string ExportCurrentToJson(bool pretty)
    {
        var wrapper = new CardWrapper { card = current };
        return JsonConvert.SerializeObject(wrapper);
    }

    private void ImportJsonToCurrent(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            ShowNotification(new GUIContent("JSON is empty"));
            return;
        }

        try
        {
            var wrapper = JsonConvert.DeserializeObject<CardWrapper>(json);
            if (wrapper?.card == null)
            {
                ShowNotification(new GUIContent("Invalid JSON: wrapper.card is null"));
                return;
            }

            wrapper.card.effects ??= new List<EffectOp>();
            current = wrapper.card;
            effectFoldouts.Clear();
            GUI.FocusControl(null);
            ShowNotification(new GUIContent("Imported JSON"));
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            ShowNotification(new GUIContent("Import failed, check Console"));
        }
    }

    private static void Swap<T>(List<T> list, int a, int b)
    {
        (list[a], list[b]) = (list[b], list[a]);
    }
}