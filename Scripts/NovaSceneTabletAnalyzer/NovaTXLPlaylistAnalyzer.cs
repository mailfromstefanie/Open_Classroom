#if UNITY_EDITOR
using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Read-only diagnostic tool for VideoTXL playlist wiring.
/// It does NOT modify the scene or any VideoTXL objects.
///
/// Menu:
/// Nova -> VideoTXL -> Analyze Playlist Wiring
///
/// Output:
/// Assets/NovaSceneReports/TXL_Playlist_Diagnostic_*.txt
/// </summary>
public static class NovaTXLPlaylistAnalyzer
{
    private const string OutputFolder = "Assets/NovaSceneReports";

    [MenuItem("Nova/VideoTXL/Analyze Playlist Wiring")]
    public static void AnalyzePlaylistWiring()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (!scene.IsValid())
        {
            EditorUtility.DisplayDialog("Nova VideoTXL Analyzer", "No valid active scene found.", "OK");
            return;
        }

        Directory.CreateDirectory(OutputFolder);

        StringBuilder sb = new StringBuilder(64 * 1024);
        AppendHeader(sb, scene);
        AppendVideoTXLPackageInfo(sb);

        GameObject[] roots = scene.GetRootGameObjects();

        AppendSection(sb, "1. VIDEO TXL SOURCE MANAGERS");
        ScanComponentsByTypeName(sb, roots, "Texel.SourceManager", true);

        AppendSection(sb, "2. VIDEO TXL PLAYLISTS");
        ScanComponentsByTypeName(sb, roots, "Texel.Playlist", true);

        AppendSection(sb, "3. VIDEO TXL PLAYLIST LOAD DATA");
        ScanComponentsByTypeName(sb, roots, "Texel.PlaylistLoadData", true);

        AppendSection(sb, "4. PLAYLIST DATA ASSETS REFERENCED IN SCENE");
        ScanReferencedPlaylistData(sb, roots);

        AppendSection(sb, "5. UNITY BUTTON ONCLICK WIRING");
        ScanButtons(sb, roots);

        AppendSection(sb, "6. LIKELY TABLET / PLAYLIST UI RAYCAST BLOCKERS");
        ScanInterestingGraphics(sb, roots);

        AppendSection(sb, "7. CANVAS GROUPS IN TABLET / PLAYLIST UI");
        ScanInterestingCanvasGroups(sb, roots);

        AppendSection(sb, "8. GRAPHIC RAYCASTERS IN TABLET / PLAYLIST UI");
        ScanInterestingGraphicRaycasters(sb, roots);

        AppendSection(sb, "9. QUICK INTERPRETATION NOTES");
        sb.AppendLine("- This report is read-only evidence. It does not prove runtime VRChat behaviour by itself.");
        sb.AppendLine("- For a dead custom playlist button, compare its Button OnClick target with the PlaylistLoadData entry and Playlist reference.");
        sb.AppendLine("- For playlists leaking into the public VideoTXL UI, inspect SourceManager sources and the generated Video Source UI behaviour.");
        sb.AppendLine("- A Graphic with Raycast Target = true can intercept pointer input if it is visually above the intended button.");
        sb.AppendLine("- A CanvasGroup with Blocks Raycasts = true and Interactable = false can also explain an apparently unclickable area.");
        sb.AppendLine();

        string sceneName = MakeSafeFileName(scene.name);
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string assetPath = OutputFolder + "/TXL_Playlist_Diagnostic_" + sceneName + "_" + timestamp + ".txt";
        File.WriteAllText(assetPath, sb.ToString(), Encoding.UTF8);

        AssetDatabase.Refresh();
        TextAsset report = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
        if (report != null)
        {
            Selection.activeObject = report;
            EditorGUIUtility.PingObject(report);
        }

        Debug.Log("[Nova VideoTXL Analyzer] Created report: " + assetPath);
        EditorUtility.DisplayDialog(
            "Nova VideoTXL Analyzer",
            "Done.\n\nCreated:\n" + assetPath + "\n\nSend this TXT file to Nova.",
            "OK"
        );
    }

    private static void AppendHeader(StringBuilder sb, Scene scene)
    {
        sb.AppendLine("NOVA VIDEO TXL PLAYLIST DIAGNOSTIC");
        sb.AppendLine("==================================");
        sb.AppendLine("Generated: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        sb.AppendLine("Unity: " + Application.unityVersion);
        sb.AppendLine("Scene: " + scene.name);
        sb.AppendLine("Scene path: " + scene.path);
        sb.AppendLine();
        sb.AppendLine("PURPOSE");
        sb.AppendLine("-------");
        sb.AppendLine("Find the exact wiring around VideoTXL SourceManager, Playlist, PlaylistLoadData,");
        sb.AppendLine("custom Unity Buttons and possible UI raycast blockers after a VideoTXL update.");
        sb.AppendLine("This analyzer only READS the scene.");
        sb.AppendLine();
    }

    private static void AppendVideoTXLPackageInfo(StringBuilder sb)
    {
        AppendSection(sb, "VIDEO TXL PACKAGE INFO");
        try
        {
            PackageInfo[] packages = PackageInfo.GetAllRegisteredPackages();
            bool found = false;
            foreach (PackageInfo package in packages)
            {
                if (package == null) continue;

                string name = package.name ?? "";
                string display = package.displayName ?? "";
                if (name.IndexOf("texelsaur.video", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    display.IndexOf("VideoTXL", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    found = true;
                    sb.AppendLine("Name: " + package.name);
                    sb.AppendLine("Display name: " + package.displayName);
                    sb.AppendLine("Version: " + package.version);
                    sb.AppendLine("Source: " + package.source);
                    sb.AppendLine("Resolved path: " + package.resolvedPath);
                    sb.AppendLine();
                }
            }

            if (!found)
                sb.AppendLine("VideoTXL package was not identified through Unity Package Manager.\n");
        }
        catch (Exception ex)
        {
            sb.AppendLine("Could not query Package Manager: " + ex.Message);
            sb.AppendLine();
        }
    }

    private static void ScanComponentsByTypeName(StringBuilder sb, GameObject[] roots, string fullTypeName, bool dumpSerialized)
    {
        int count = 0;
        foreach (GameObject root in roots)
        {
            Component[] components = root.GetComponentsInChildren<Component>(true);
            foreach (Component component in components)
            {
                if (component == null) continue;
                Type type = component.GetType();
                if (type.FullName != fullTypeName) continue;

                count++;
                sb.AppendLine("------------------------------------------------------------");
                sb.AppendLine("Object: " + GetHierarchyPath(component.gameObject));
                sb.AppendLine("Component: " + type.FullName);
                sb.AppendLine("GameObject activeSelf: " + component.gameObject.activeSelf);
                sb.AppendLine("GameObject activeInHierarchy: " + component.gameObject.activeInHierarchy);
                sb.AppendLine("Component enabled: " + GetEnabledState(component));

                if (dumpSerialized)
                    AppendSerializedProperties(sb, component);

                sb.AppendLine();
            }
        }

        if (count == 0)
            sb.AppendLine("NONE FOUND\n");
        else
            sb.AppendLine("Total found: " + count + "\n");
    }

    private static void ScanReferencedPlaylistData(StringBuilder sb, GameObject[] roots)
    {
        int count = 0;
        foreach (GameObject root in roots)
        {
            Component[] components = root.GetComponentsInChildren<Component>(true);
            foreach (Component component in components)
            {
                if (component == null || component.GetType().FullName != "Texel.PlaylistLoadData")
                    continue;

                try
                {
                    SerializedObject so = new SerializedObject(component);
                    SerializedProperty data = so.FindProperty("playlistData");
                    if (data == null || data.objectReferenceValue == null)
                        continue;

                    count++;
                    UnityEngine.Object obj = data.objectReferenceValue;
                    sb.AppendLine("Load button object: " + GetHierarchyPath(component.gameObject));
                    sb.AppendLine("PlaylistData: " + DescribeObjectReference(obj));
                    AppendSerializedProperties(sb, obj);
                    sb.AppendLine();
                }
                catch (Exception ex)
                {
                    sb.AppendLine("Could not inspect PlaylistData from " + GetHierarchyPath(component.gameObject) + ": " + ex.Message);
                    sb.AppendLine();
                }
            }
        }

        if (count == 0)
            sb.AppendLine("NONE FOUND\n");
    }

    private static void ScanButtons(StringBuilder sb, GameObject[] roots)
    {
        int totalButtons = 0;
        int playlistRelevantButtons = 0;

        foreach (GameObject root in roots)
        {
            Button[] buttons = root.GetComponentsInChildren<Button>(true);
            foreach (Button button in buttons)
            {
                totalButtons++;

                bool interesting = IsInterestingPath(GetHierarchyPath(button.gameObject));
                int persistentCount = button.onClick.GetPersistentEventCount();

                for (int i = 0; i < persistentCount && !interesting; i++)
                {
                    UnityEngine.Object target = button.onClick.GetPersistentTarget(i);
                    string method = button.onClick.GetPersistentMethodName(i) ?? "";
                    string targetText = target != null ? target.name + " " + target.GetType().FullName : "";
                    if (targetText.IndexOf("Playlist", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        method.IndexOf("Load", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        interesting = true;
                    }
                }

                if (!interesting)
                    continue;

                playlistRelevantButtons++;
                sb.AppendLine("------------------------------------------------------------");
                sb.AppendLine("Button: " + GetHierarchyPath(button.gameObject));
                sb.AppendLine("Active self: " + button.gameObject.activeSelf);
                sb.AppendLine("Active in hierarchy: " + button.gameObject.activeInHierarchy);
                sb.AppendLine("Button enabled: " + button.enabled);
                sb.AppendLine("Interactable: " + button.interactable);
                sb.AppendLine("Target Graphic: " + (button.targetGraphic ? GetHierarchyPath(button.targetGraphic.gameObject) + " / " + button.targetGraphic.GetType().Name : "<null>"));
                sb.AppendLine("Persistent OnClick count: " + persistentCount);

                for (int i = 0; i < persistentCount; i++)
                {
                    UnityEngine.Object target = button.onClick.GetPersistentTarget(i);
                    string method = button.onClick.GetPersistentMethodName(i);
                    UnityEngine.Events.UnityEventCallState state = button.onClick.GetPersistentListenerState(i);

                    sb.AppendLine("  OnClick[" + i + "]:");
                    sb.AppendLine("    State: " + state);
                    sb.AppendLine("    Target: " + DescribeObjectReference(target));
                    sb.AppendLine("    Method/Event: " + (string.IsNullOrEmpty(method) ? "<empty>" : method));
                }

                sb.AppendLine();
            }
        }

        sb.AppendLine("Total Unity Buttons in scene: " + totalButtons);
        sb.AppendLine("Buttons included in this focused report: " + playlistRelevantButtons);
        sb.AppendLine();
    }

    private static void ScanInterestingGraphics(StringBuilder sb, GameObject[] roots)
    {
        int count = 0;
        foreach (GameObject root in roots)
        {
            Graphic[] graphics = root.GetComponentsInChildren<Graphic>(true);
            foreach (Graphic graphic in graphics)
            {
                string path = GetHierarchyPath(graphic.gameObject);
                if (!IsInterestingPath(path))
                    continue;

                // Focus on things that can actually affect pointer input.
                if (!graphic.raycastTarget)
                    continue;

                count++;
                sb.AppendLine("Graphic: " + path);
                sb.AppendLine("  Type: " + graphic.GetType().FullName);
                sb.AppendLine("  Enabled: " + graphic.enabled);
                sb.AppendLine("  Active self: " + graphic.gameObject.activeSelf);
                sb.AppendLine("  Active in hierarchy: " + graphic.gameObject.activeInHierarchy);
                sb.AppendLine("  Raycast Target: " + graphic.raycastTarget);
                sb.AppendLine("  Canvas: " + (graphic.canvas ? GetHierarchyPath(graphic.canvas.gameObject) : "<none>"));
                sb.AppendLine();
            }
        }

        if (count == 0)
            sb.AppendLine("No raycast-target Graphics found in likely tablet/playlist paths.\n");
        else
            sb.AppendLine("Total raycast-target Graphics reported: " + count + "\n");
    }

    private static void ScanInterestingCanvasGroups(StringBuilder sb, GameObject[] roots)
    {
        int count = 0;
        foreach (GameObject root in roots)
        {
            CanvasGroup[] groups = root.GetComponentsInChildren<CanvasGroup>(true);
            foreach (CanvasGroup group in groups)
            {
                string path = GetHierarchyPath(group.gameObject);
                if (!IsInterestingPath(path))
                    continue;

                count++;
                sb.AppendLine("CanvasGroup: " + path);
                sb.AppendLine("  Alpha: " + group.alpha);
                sb.AppendLine("  Interactable: " + group.interactable);
                sb.AppendLine("  Blocks Raycasts: " + group.blocksRaycasts);
                sb.AppendLine("  Ignore Parent Groups: " + group.ignoreParentGroups);
                sb.AppendLine("  Active in hierarchy: " + group.gameObject.activeInHierarchy);
                sb.AppendLine();
            }
        }

        if (count == 0)
            sb.AppendLine("No CanvasGroups found in likely tablet/playlist paths.\n");
    }

    private static void ScanInterestingGraphicRaycasters(StringBuilder sb, GameObject[] roots)
    {
        int count = 0;
        foreach (GameObject root in roots)
        {
            GraphicRaycaster[] raycasters = root.GetComponentsInChildren<GraphicRaycaster>(true);
            foreach (GraphicRaycaster raycaster in raycasters)
            {
                string path = GetHierarchyPath(raycaster.gameObject);
                if (!IsInterestingPath(path))
                    continue;

                count++;
                sb.AppendLine("GraphicRaycaster: " + path);
                sb.AppendLine("  Enabled: " + raycaster.enabled);
                sb.AppendLine("  Ignore Reversed Graphics: " + raycaster.ignoreReversedGraphics);
                sb.AppendLine("  Blocking Objects: " + raycaster.blockingObjects);
                sb.AppendLine("  Blocking Mask: " + LayerMaskToString(raycaster.blockingMask));
                sb.AppendLine();
            }
        }

        if (count == 0)
            sb.AppendLine("No GraphicRaycasters found in likely tablet/playlist paths.\n");
    }

    private static void AppendSerializedProperties(StringBuilder sb, UnityEngine.Object obj)
    {
        if (obj == null)
        {
            sb.AppendLine("Serialized fields: <object is null>");
            return;
        }

        try
        {
            SerializedObject so = new SerializedObject(obj);
            SerializedProperty iterator = so.GetIterator();
            bool enterChildren = true;

            sb.AppendLine("Serialized fields:");
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;

                if (iterator.propertyPath == "m_Script")
                    continue;

                sb.AppendLine("  " + iterator.propertyPath + " = " + SerializedValueToString(iterator));
            }
        }
        catch (Exception ex)
        {
            sb.AppendLine("Serialized fields: <could not inspect: " + ex.Message + ">");
        }
    }

    private static string SerializedValueToString(SerializedProperty property)
    {
        try
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    return property.intValue.ToString();
                case SerializedPropertyType.Boolean:
                    return property.boolValue.ToString();
                case SerializedPropertyType.Float:
                    return property.floatValue.ToString("0.#####");
                case SerializedPropertyType.String:
                    return "\"" + property.stringValue + "\"";
                case SerializedPropertyType.Color:
                    return property.colorValue.ToString();
                case SerializedPropertyType.ObjectReference:
                    return DescribeObjectReference(property.objectReferenceValue);
                case SerializedPropertyType.LayerMask:
                    return property.intValue.ToString();
                case SerializedPropertyType.Enum:
                    if (property.enumDisplayNames != null && property.enumValueIndex >= 0 && property.enumValueIndex < property.enumDisplayNames.Length)
                        return property.enumDisplayNames[property.enumValueIndex] + " (" + property.enumValueIndex + ")";
                    return property.enumValueIndex.ToString();
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
                    return ((char)property.intValue).ToString();
                case SerializedPropertyType.AnimationCurve:
                    return "AnimationCurve";
                case SerializedPropertyType.Bounds:
                    return property.boundsValue.ToString();
                case SerializedPropertyType.Quaternion:
                    return property.quaternionValue.eulerAngles.ToString();
                default:
                    if (property.isArray && property.propertyType != SerializedPropertyType.String)
                        return "Array(size=" + property.arraySize + ")";
                    return property.propertyType.ToString();
            }
        }
        catch (Exception ex)
        {
            return "<error reading value: " + ex.Message + ">";
        }
    }

    private static string DescribeObjectReference(UnityEngine.Object obj)
    {
        if (obj == null)
            return "<null>";

        Component component = obj as Component;
        if (component != null)
            return GetHierarchyPath(component.gameObject) + " [" + component.GetType().FullName + "]";

        GameObject go = obj as GameObject;
        if (go != null)
            return GetHierarchyPath(go) + " [GameObject]";

        string assetPath = AssetDatabase.GetAssetPath(obj);
        if (!string.IsNullOrEmpty(assetPath))
            return obj.name + " [" + obj.GetType().FullName + "] @ " + assetPath;

        return obj.name + " [" + obj.GetType().FullName + "]";
    }

    private static string GetEnabledState(Component component)
    {
        Behaviour behaviour = component as Behaviour;
        return behaviour != null ? behaviour.enabled.ToString() : "n/a";
    }

    private static string GetHierarchyPath(GameObject go)
    {
        if (go == null) return "<null>";

        StringBuilder sb = new StringBuilder(go.name);
        Transform current = go.transform.parent;
        while (current != null)
        {
            sb.Insert(0, current.name + "/");
            current = current.parent;
        }
        return sb.ToString();
    }

    private static bool IsInterestingPath(string path)
    {
        if (string.IsNullOrEmpty(path)) return false;
        string p = path.ToLowerInvariant();
        return p.Contains("playlist") ||
               p.Contains("paper_tablet") ||
               p.Contains("paper tablet") ||
               p.Contains("vipcontentroot") ||
               p.Contains("panel (vip)") ||
               p.Contains("video player") ||
               p.Contains("source manager");
    }

    private static string LayerMaskToString(LayerMask mask)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < 32; i++)
        {
            if ((mask.value & (1 << i)) == 0) continue;
            if (sb.Length > 0) sb.Append(", ");
            string layerName = LayerMask.LayerToName(i);
            sb.Append(string.IsNullOrEmpty(layerName) ? i.ToString() : layerName + "(" + i + ")");
        }
        return sb.Length == 0 ? "Nothing" : sb.ToString();
    }

    private static void AppendSection(StringBuilder sb, string title)
    {
        sb.AppendLine();
        sb.AppendLine(title);
        sb.AppendLine(new string('=', title.Length));
        sb.AppendLine();
    }

    private static string MakeSafeFileName(string value)
    {
        foreach (char c in Path.GetInvalidFileNameChars())
            value = value.Replace(c, '_');
        return value;
    }
}
#endif
