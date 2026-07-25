#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NovaSceneTabletAnalyzer : EditorWindow
{
    private bool includeInactiveObjects = true;
    private bool includeFullScriptCode = true;
    private bool includeSerializedFields = true;
    private bool includeMaterials = true;
    private bool includeUIInfo = true;

    private Vector2 scroll;

    [MenuItem("Nova/Scene Tools/Analyze Current Scene")]
    public static void ShowWindow()
    {
        NovaSceneTabletAnalyzer window = GetWindow<NovaSceneTabletAnalyzer>();
        window.titleContent = new GUIContent("Nova Scene Analyzer");
        window.minSize = new Vector2(420, 320);
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Nova Scene Analyzer", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Deze tool leest de huidige scene uit en maakt TXT, Markdown, JSON en CSV rapporten. " +
            "Handig om aan Nova te geven zodat zij precies kan zien wat gekoppeld is.",
            MessageType.Info
        );

        scroll = EditorGUILayout.BeginScrollView(scroll);

        includeInactiveObjects = EditorGUILayout.ToggleLeft("Include inactive GameObjects", includeInactiveObjects);
        includeSerializedFields = EditorGUILayout.ToggleLeft("Include serialized fields and references", includeSerializedFields);
        includeMaterials = EditorGUILayout.ToggleLeft("Include renderer/material info", includeMaterials);
        includeUIInfo = EditorGUILayout.ToggleLeft("Include UI Button/Slider/Toggle info", includeUIInfo);
        includeFullScriptCode = EditorGUILayout.ToggleLeft("Include full script code at end", includeFullScriptCode);

        EditorGUILayout.Space(12);

        if (GUILayout.Button("Analyze Current Scene", GUILayout.Height(38)))
        {
            AnalyzeCurrentScene();
        }

        EditorGUILayout.Space(12);

        EditorGUILayout.HelpBox(
            "Output komt in:\nAssets/NovaSceneReports/\n\nStuur vooral het .txt of .md bestand naar Nova.",
            MessageType.None
        );

        EditorGUILayout.EndScrollView();
    }

    private void AnalyzeCurrentScene()
    {
        Scene scene = SceneManager.GetActiveScene();

        if (!scene.IsValid())
        {
            EditorUtility.DisplayDialog("Nova Scene Analyzer", "No valid active scene found.", "OK");
            return;
        }

        string folder = "Assets/NovaSceneReports";
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        string safeSceneName = MakeSafeFileName(scene.name);
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

        string basePath = Path.Combine(folder, safeSceneName + "_SceneReport_" + timestamp);

        List<GameObject> allObjects = GetAllSceneObjects(scene, includeInactiveObjects);
        List<ScriptInfo> usedScripts = CollectUsedScripts(allObjects);

        string txt = BuildTextReport(scene, allObjects, usedScripts);
        string md = BuildMarkdownReport(scene, allObjects, usedScripts);
        string json = BuildJsonReport(scene, allObjects, usedScripts);
        string csv = BuildCsvReport(allObjects);

        File.WriteAllText(basePath + ".txt", txt, Encoding.UTF8);
        File.WriteAllText(basePath + ".md", md, Encoding.UTF8);
        File.WriteAllText(basePath + ".json", json, Encoding.UTF8);
        File.WriteAllText(basePath + ".csv", csv, Encoding.UTF8);

        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog(
            "Nova Scene Analyzer",
            "Scene analysis complete.\n\nCreated files:\n" +
            basePath + ".txt\n" +
            basePath + ".md\n" +
            basePath + ".json\n" +
            basePath + ".csv",
            "OK"
        );

        Debug.Log("[Nova Scene Analyzer] Report created at: " + basePath);
    }

    private List<GameObject> GetAllSceneObjects(Scene scene, bool includeInactive)
    {
        List<GameObject> result = new List<GameObject>();

        GameObject[] roots = scene.GetRootGameObjects();

        foreach (GameObject root in roots)
        {
            AddObjectRecursive(root, result, includeInactive);
        }

        result.Sort((a, b) => string.Compare(GetHierarchyPath(a), GetHierarchyPath(b), StringComparison.OrdinalIgnoreCase));

        return result;
    }

    private void AddObjectRecursive(GameObject obj, List<GameObject> list, bool includeInactive)
    {
        if (obj == null) return;

        if (includeInactive || obj.activeInHierarchy || obj.activeSelf)
        {
            list.Add(obj);
        }

        for (int i = 0; i < obj.transform.childCount; i++)
        {
            AddObjectRecursive(obj.transform.GetChild(i).gameObject, list, includeInactive);
        }
    }

    private List<ScriptInfo> CollectUsedScripts(List<GameObject> objects)
    {
        Dictionary<string, ScriptInfo> scripts = new Dictionary<string, ScriptInfo>();

        foreach (GameObject obj in objects)
        {
            Component[] components = obj.GetComponents<Component>();

            foreach (Component component in components)
            {
                if (component == null) continue;

                MonoBehaviour mono = component as MonoBehaviour;
                if (mono == null) continue;

                MonoScript monoScript = MonoScript.FromMonoBehaviour(mono);
                if (monoScript == null) continue;

                string assetPath = AssetDatabase.GetAssetPath(monoScript);
                if (string.IsNullOrEmpty(assetPath)) continue;

                if (!scripts.ContainsKey(assetPath))
                {
                    ScriptInfo info = new ScriptInfo();
                    info.className = monoScript.GetClass() != null ? monoScript.GetClass().Name : monoScript.name;
                    info.assetPath = assetPath;
                    info.usedOnObjects = new List<string>();

                    if (includeFullScriptCode)
                    {
                        try
                        {
                            info.sourceCode = File.ReadAllText(assetPath);
                        }
                        catch
                        {
                            info.sourceCode = "[Could not read script file]";
                        }
                    }

                    scripts.Add(assetPath, info);
                }

                scripts[assetPath].usedOnObjects.Add(GetHierarchyPath(obj));
            }
        }

        List<ScriptInfo> result = new List<ScriptInfo>(scripts.Values);
        result.Sort((a, b) => string.Compare(a.className, b.className, StringComparison.OrdinalIgnoreCase));
        return result;
    }

    private string BuildTextReport(Scene scene, List<GameObject> objects, List<ScriptInfo> usedScripts)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("NOVA SCENE ANALYSIS REPORT");
        sb.AppendLine("==========================");
        sb.AppendLine();
        sb.AppendLine("Scene: " + scene.name);
        sb.AppendLine("Scene path: " + scene.path);
        sb.AppendLine("Generated: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        sb.AppendLine("Unity version: " + Application.unityVersion);
        sb.AppendLine("Object count: " + objects.Count);
        sb.AppendLine("Script count: " + usedScripts.Count);
        sb.AppendLine();

        sb.AppendLine("IMPORTANT");
        sb.AppendLine("---------");
        sb.AppendLine("This report shows GameObjects, components, serialized fields, object references and used script code.");
        sb.AppendLine("Use this file to let Nova inspect the current scene setup.");
        sb.AppendLine();

        sb.AppendLine("ROOT OBJECTS");
        sb.AppendLine("------------");

        foreach (GameObject root in scene.GetRootGameObjects())
        {
            sb.AppendLine("- " + root.name);
        }

        sb.AppendLine();
        sb.AppendLine("HIERARCHY AND COMPONENTS");
        sb.AppendLine("========================");
        sb.AppendLine();

        foreach (GameObject obj in objects)
        {
            AppendGameObjectText(sb, obj);
        }

        sb.AppendLine();
        sb.AppendLine("USED SCRIPTS");
        sb.AppendLine("============");
        sb.AppendLine();

        foreach (ScriptInfo script in usedScripts)
        {
            sb.AppendLine("Script: " + script.className);
            sb.AppendLine("Path: " + script.assetPath);
            sb.AppendLine("Used on:");

            foreach (string objPath in script.usedOnObjects)
            {
                sb.AppendLine("  - " + objPath);
            }

            sb.AppendLine();
        }

        if (includeFullScriptCode)
        {
            sb.AppendLine();
            sb.AppendLine("FULL SCRIPT CODE");
            sb.AppendLine("================");
            sb.AppendLine();

            foreach (ScriptInfo script in usedScripts)
            {
                sb.AppendLine("============================================================");
                sb.AppendLine("SCRIPT: " + script.className);
                sb.AppendLine("PATH: " + script.assetPath);
                sb.AppendLine("============================================================");
                sb.AppendLine();
                sb.AppendLine(script.sourceCode);
                sb.AppendLine();
            }
        }

        return sb.ToString();
    }

    private string BuildMarkdownReport(Scene scene, List<GameObject> objects, List<ScriptInfo> usedScripts)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("# Nova Scene Analysis Report");
        sb.AppendLine();
        sb.AppendLine("## Scene Info");
        sb.AppendLine();
        sb.AppendLine("- Scene: `" + scene.name + "`");
        sb.AppendLine("- Scene path: `" + scene.path + "`");
        sb.AppendLine("- Generated: `" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "`");
        sb.AppendLine("- Unity version: `" + Application.unityVersion + "`");
        sb.AppendLine("- Object count: `" + objects.Count + "`");
        sb.AppendLine("- Script count: `" + usedScripts.Count + "`");
        sb.AppendLine();

        sb.AppendLine("## Hierarchy And Components");
        sb.AppendLine();

        foreach (GameObject obj in objects)
        {
            sb.AppendLine("### " + EscapeMarkdown(GetHierarchyPath(obj)));
            sb.AppendLine();
            sb.AppendLine("- Active Self: `" + obj.activeSelf + "`");
            sb.AppendLine("- Active In Hierarchy: `" + obj.activeInHierarchy + "`");
            sb.AppendLine("- Tag: `" + obj.tag + "`");
            sb.AppendLine("- Layer: `" + LayerMask.LayerToName(obj.layer) + "`");
            sb.AppendLine("- Transform:");
            sb.AppendLine("  - Local Position: `" + FormatVector3(obj.transform.localPosition) + "`");
            sb.AppendLine("  - Local Rotation: `" + FormatVector3(obj.transform.localEulerAngles) + "`");
            sb.AppendLine("  - Local Scale: `" + FormatVector3(obj.transform.localScale) + "`");
            sb.AppendLine();

            Component[] components = obj.GetComponents<Component>();
            sb.AppendLine("Components:");

            foreach (Component component in components)
            {
                if (component == null)
                {
                    sb.AppendLine("- Missing Script / Missing Component");
                    continue;
                }

                sb.AppendLine("- `" + component.GetType().Name + "`");

                if (includeSerializedFields)
                {
                    List<string> fields = GetSerializedFields(component);
                    foreach (string field in fields)
                    {
                        sb.AppendLine("  - " + field);
                    }
                }
            }

            sb.AppendLine();
        }

        sb.AppendLine("## Used Scripts");
        sb.AppendLine();

        foreach (ScriptInfo script in usedScripts)
        {
            sb.AppendLine("### " + script.className);
            sb.AppendLine();
            sb.AppendLine("- Path: `" + script.assetPath + "`");
            sb.AppendLine("- Used on:");

            foreach (string objPath in script.usedOnObjects)
            {
                sb.AppendLine("  - `" + objPath + "`");
            }

            sb.AppendLine();
        }

        if (includeFullScriptCode)
        {
            sb.AppendLine("## Full Script Code");
            sb.AppendLine();

            foreach (ScriptInfo script in usedScripts)
            {
                sb.AppendLine("### " + script.className);
                sb.AppendLine();
                sb.AppendLine("Path: `" + script.assetPath + "`");
                sb.AppendLine();
                sb.AppendLine("```csharp");
                sb.AppendLine(script.sourceCode);
                sb.AppendLine("```");
                sb.AppendLine();
            }
        }

        return sb.ToString();
    }

    private string BuildJsonReport(Scene scene, List<GameObject> objects, List<ScriptInfo> usedScripts)
    {
        SceneReportJson report = new SceneReportJson();
        report.sceneName = scene.name;
        report.scenePath = scene.path;
        report.generated = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        report.unityVersion = Application.unityVersion;
        report.objects = new List<GameObjectJson>();
        report.scripts = usedScripts;

        foreach (GameObject obj in objects)
        {
            GameObjectJson objJson = new GameObjectJson();
            objJson.name = obj.name;
            objJson.path = GetHierarchyPath(obj);
            objJson.activeSelf = obj.activeSelf;
            objJson.activeInHierarchy = obj.activeInHierarchy;
            objJson.tag = obj.tag;
            objJson.layer = LayerMask.LayerToName(obj.layer);
            objJson.localPosition = FormatVector3(obj.transform.localPosition);
            objJson.localRotation = FormatVector3(obj.transform.localEulerAngles);
            objJson.localScale = FormatVector3(obj.transform.localScale);
            objJson.components = new List<ComponentJson>();

            Component[] components = obj.GetComponents<Component>();
            foreach (Component component in components)
            {
                ComponentJson compJson = new ComponentJson();

                if (component == null)
                {
                    compJson.type = "Missing Script / Missing Component";
                    compJson.fields = new List<string>();
                }
                else
                {
                    compJson.type = component.GetType().Name;
                    compJson.fields = includeSerializedFields ? GetSerializedFields(component) : new List<string>();
                }

                objJson.components.Add(compJson);
            }

            report.objects.Add(objJson);
        }

        return JsonUtility.ToJson(report, true);
    }

    private string BuildCsvReport(List<GameObject> objects)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("Path,Name,ActiveSelf,ActiveInHierarchy,Tag,Layer,ComponentCount,Components");

        foreach (GameObject obj in objects)
        {
            Component[] components = obj.GetComponents<Component>();
            List<string> componentNames = new List<string>();

            foreach (Component component in components)
            {
                if (component == null)
                {
                    componentNames.Add("Missing Script");
                }
                else
                {
                    componentNames.Add(component.GetType().Name);
                }
            }

            sb.Append(EscapeCsv(GetHierarchyPath(obj)));
            sb.Append(",");
            sb.Append(EscapeCsv(obj.name));
            sb.Append(",");
            sb.Append(EscapeCsv(obj.activeSelf.ToString()));
            sb.Append(",");
            sb.Append(EscapeCsv(obj.activeInHierarchy.ToString()));
            sb.Append(",");
            sb.Append(EscapeCsv(obj.tag));
            sb.Append(",");
            sb.Append(EscapeCsv(LayerMask.LayerToName(obj.layer)));
            sb.Append(",");
            sb.Append(EscapeCsv(components.Length.ToString()));
            sb.Append(",");
            sb.Append(EscapeCsv(string.Join(" | ", componentNames.ToArray())));
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private void AppendGameObjectText(StringBuilder sb, GameObject obj)
    {
        sb.AppendLine("------------------------------------------------------------");
        sb.AppendLine("GameObject: " + GetHierarchyPath(obj));
        sb.AppendLine("------------------------------------------------------------");
        sb.AppendLine("Name: " + obj.name);
        sb.AppendLine("Active Self: " + obj.activeSelf);
        sb.AppendLine("Active In Hierarchy: " + obj.activeInHierarchy);
        sb.AppendLine("Tag: " + obj.tag);
        sb.AppendLine("Layer: " + LayerMask.LayerToName(obj.layer));
        sb.AppendLine("Local Position: " + FormatVector3(obj.transform.localPosition));
        sb.AppendLine("Local Rotation: " + FormatVector3(obj.transform.localEulerAngles));
        sb.AppendLine("Local Scale: " + FormatVector3(obj.transform.localScale));
        sb.AppendLine();

        Component[] components = obj.GetComponents<Component>();

        sb.AppendLine("Components:");

        foreach (Component component in components)
        {
            if (component == null)
            {
                sb.AppendLine("  - Missing Script / Missing Component");
                continue;
            }

            sb.AppendLine("  - " + component.GetType().FullName);

            if (includeSerializedFields)
            {
                List<string> fields = GetSerializedFields(component);
                foreach (string field in fields)
                {
                    sb.AppendLine("      " + field);
                }
            }
        }

        sb.AppendLine();
    }

    private List<string> GetSerializedFields(Component component)
    {
        List<string> result = new List<string>();

        if (component == null) return result;

        try
        {
            SerializedObject serializedObject = new SerializedObject(component);
            SerializedProperty property = serializedObject.GetIterator();

            bool enterChildren = true;

            while (property.NextVisible(enterChildren))
            {
                enterChildren = false;

                if (property.name == "m_Script")
                {
                    UnityEngine.Object scriptObj = property.objectReferenceValue;
                    string scriptPath = scriptObj != null ? AssetDatabase.GetAssetPath(scriptObj) : "None";
                    result.Add("Script = " + scriptPath);
                    continue;
                }

                string line = property.displayName + " (" + property.name + ") = " + SerializedPropertyToString(property);

                result.Add(line);
            }
        }
        catch (Exception ex)
        {
            result.Add("[Could not read serialized fields: " + ex.Message + "]");
        }

        return result;
    }

    private string SerializedPropertyToString(SerializedProperty property)
    {
        switch (property.propertyType)
        {
            case SerializedPropertyType.Integer:
                return property.intValue.ToString();

            case SerializedPropertyType.Boolean:
                return property.boolValue.ToString();

            case SerializedPropertyType.Float:
                return property.floatValue.ToString();

            case SerializedPropertyType.String:
                return "\"" + property.stringValue + "\"";

            case SerializedPropertyType.Color:
                return property.colorValue.ToString();

            case SerializedPropertyType.ObjectReference:
                return ObjectReferenceToString(property.objectReferenceValue);

            case SerializedPropertyType.LayerMask:
                return property.intValue.ToString();

            case SerializedPropertyType.Enum:
                return property.enumDisplayNames != null && property.enumValueIndex >= 0 && property.enumValueIndex < property.enumDisplayNames.Length
                    ? property.enumDisplayNames[property.enumValueIndex]
                    : property.enumValueIndex.ToString();

            case SerializedPropertyType.Vector2:
                return property.vector2Value.ToString();

            case SerializedPropertyType.Vector3:
                return property.vector3Value.ToString();

            case SerializedPropertyType.Vector4:
                return property.vector4Value.ToString();

            case SerializedPropertyType.Rect:
                return property.rectValue.ToString();

            case SerializedPropertyType.ArraySize:
                return property.intValue.ToString();

            case SerializedPropertyType.Character:
                return property.intValue.ToString();

            case SerializedPropertyType.AnimationCurve:
                return property.animationCurveValue != null ? property.animationCurveValue.ToString() : "None";

            case SerializedPropertyType.Bounds:
                return property.boundsValue.ToString();

            case SerializedPropertyType.Quaternion:
                return property.quaternionValue.eulerAngles.ToString();

            case SerializedPropertyType.ExposedReference:
                return ObjectReferenceToString(property.exposedReferenceValue);

            case SerializedPropertyType.FixedBufferSize:
                return property.intValue.ToString();

            case SerializedPropertyType.Vector2Int:
                return property.vector2IntValue.ToString();

            case SerializedPropertyType.Vector3Int:
                return property.vector3IntValue.ToString();

            case SerializedPropertyType.RectInt:
                return property.rectIntValue.ToString();

            case SerializedPropertyType.BoundsInt:
                return property.boundsIntValue.ToString();

            case SerializedPropertyType.ManagedReference:
                return property.managedReferenceFullTypename;

            case SerializedPropertyType.Generic:
                if (property.isArray)
                {
                    return "Array, size = " + property.arraySize;
                }
                return "Generic";

            default:
                return "[Unsupported property type: " + property.propertyType + "]";
        }
    }

    private string ObjectReferenceToString(UnityEngine.Object obj)
    {
        if (obj == null) return "None";

        GameObject go = obj as GameObject;
        if (go != null)
        {
            return "GameObject: " + GetHierarchyPath(go);
        }

        Component component = obj as Component;
        if (component != null)
        {
            return "Component: " + component.GetType().Name + " on " + GetHierarchyPath(component.gameObject);
        }

        string assetPath = AssetDatabase.GetAssetPath(obj);
        if (!string.IsNullOrEmpty(assetPath))
        {
            return obj.GetType().Name + ": " + obj.name + " | AssetPath: " + assetPath;
        }

        return obj.GetType().Name + ": " + obj.name;
    }

    private string GetHierarchyPath(GameObject obj)
    {
        if (obj == null) return "None";

        string path = obj.name;
        Transform current = obj.transform.parent;

        while (current != null)
        {
            path = current.name + "/" + path;
            current = current.parent;
        }

        return path;
    }

    private string FormatVector3(Vector3 value)
    {
        return value.x.ToString("0.###") + ", " + value.y.ToString("0.###") + ", " + value.z.ToString("0.###");
    }

    private string MakeSafeFileName(string input)
    {
        if (string.IsNullOrEmpty(input)) return "UntitledScene";

        foreach (char c in Path.GetInvalidFileNameChars())
        {
            input = input.Replace(c, '_');
        }

        return input;
    }

    private string EscapeCsv(string value)
    {
        if (value == null) return "";

        value = value.Replace("\"", "\"\"");

        if (value.Contains(",") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r"))
        {
            value = "\"" + value + "\"";
        }

        return value;
    }

    private string EscapeMarkdown(string value)
    {
        if (string.IsNullOrEmpty(value)) return "";
        return value.Replace("#", "\\#");
    }

    [Serializable]
    public class SceneReportJson
    {
        public string sceneName;
        public string scenePath;
        public string generated;
        public string unityVersion;
        public List<GameObjectJson> objects;
        public List<ScriptInfo> scripts;
    }

    [Serializable]
    public class GameObjectJson
    {
        public string name;
        public string path;
        public bool activeSelf;
        public bool activeInHierarchy;
        public string tag;
        public string layer;
        public string localPosition;
        public string localRotation;
        public string localScale;
        public List<ComponentJson> components;
    }

    [Serializable]
    public class ComponentJson
    {
        public string type;
        public List<string> fields;
    }

    [Serializable]
    public class ScriptInfo
    {
        public string className;
        public string assetPath;
        public List<string> usedOnObjects;
        public string sourceCode;
    }
}
#endif