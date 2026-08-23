#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Read-only VideoTXL 2.5.x playlist diagnostic tool.
/// It never changes scene objects or package files.
///
/// Menu: Nova -> VideoTXL -> Analyze Playlist Wiring
/// Output: Assets/NovaSceneReports/TXL_Playlist_Diagnostic_*.txt
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

        sb.AppendLine("NOVA VIDEOTXL PLAYLIST DIAGNOSTIC");
        sb.AppendLine("=================================");
        sb.AppendLine("Generated: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        sb.AppendLine("Scene: " + scene.name);
        sb.AppendLine("Scene path: " + scene.path);
        sb.AppendLine("Unity: " + Application.unityVersion);
        sb.AppendLine();
        sb.AppendLine("READ ONLY: this tool does not change the scene.");
        sb.AppendLine();

        AppendPackageInfo(sb);

        List<Component> all = GetAllComponents(scene);
        List<Component> sourceManagers = FindByType(all, "Texel.SourceManager");
        List<Component> playlists = FindByType(all, "Texel.Playlist");
        List<Component> loadButtons = FindByType(all, "Texel.PlaylistLoadData");
        List<Component> videoSourceUis = FindByType(all, "Texel.VideoSourceUI");

        Section(sb, "1. SOURCE MANAGERS AND SOURCES");
        if (sourceManagers.Count == 0) sb.AppendLine("NONE FOUND\n");
        foreach (Component manager in sourceManagers)
        {
            sb.AppendLine("SourceManager: " + PathOf(manager.gameObject));
            SerializedObject so = new SerializedObject(manager);
            SerializedProperty sources = so.FindProperty("sources");
            if (sources == null || !sources.isArray)
            {
                sb.AppendLine("  sources: <not found>");
                sb.AppendLine();
                continue;
            }

            sb.AppendLine("  Source count: " + sources.arraySize);
            for (int i = 0; i < sources.arraySize; i++)
            {
                UnityEngine.Object source = sources.GetArrayElementAtIndex(i).objectReferenceValue;
                sb.AppendLine("  [" + i + "] " + Describe(source));
                if (source != null)
                {
                    SerializedObject sourceSo = new SerializedObject(source);
                    string sourceName = GetString(sourceSo, "sourceName");
                    bool sourceEnabled = GetBool(sourceSo, "sourceEnabled", true);
                    UnityEngine.Object data = GetObject(sourceSo, "playlistData");
                    UnityEngine.Object catalog = GetObject(sourceSo, "playlistCatalog");
                    sb.AppendLine("       sourceName: " + (string.IsNullOrEmpty(sourceName) ? "<empty/default>" : sourceName));
                    sb.AppendLine("       sourceEnabled (serialized default): " + sourceEnabled);
                    if (data != null) sb.AppendLine("       playlistData: " + Describe(data));
                    if (catalog != null) sb.AppendLine("       playlistCatalog: " + Describe(catalog));
                }
            }
            sb.AppendLine();
        }

        Section(sb, "2. PLAYLISTS");
        if (playlists.Count == 0) sb.AppendLine("NONE FOUND\n");
        foreach (Component playlist in playlists)
        {
            SerializedObject so = new SerializedObject(playlist);
            UnityEngine.Object data = GetObject(so, "playlistData");
            UnityEngine.Object catalog = GetObject(so, "playlistCatalog");
            UnityEngine.Object queue = GetObject(so, "queue");

            sb.AppendLine("Playlist: " + PathOf(playlist.gameObject));
            sb.AppendLine("  Source Name: " + EmptyAsDefault(GetString(so, "sourceName")));
            sb.AppendLine("  Source Enabled (serialized default): " + GetBool(so, "sourceEnabled", true));
            sb.AppendLine("  Playlist Data: " + Describe(data));
            sb.AppendLine("  Playlist Catalog: " + Describe(catalog));
            sb.AppendLine("  Queue: " + Describe(queue));
            sb.AppendLine("  Immediate: " + GetBool(so, "immediate", false));
            sb.AppendLine("  Auto Advance: " + GetBool(so, "autoAdvance", true));
            sb.AppendLine("  Track Catalog Mode: " + GetBool(so, "trackCatalogMode", false));
            sb.AppendLine();
        }

        Section(sb, "3. CUSTOM PLAYLIST LOAD BUTTONS - IMPORTANT");
        sb.AppendLine("VideoTXL 2.5.1 PlaylistLoadData._Load() behaves like this:");
        sb.AppendLine("  Playlist has PlaylistCatalog -> load through catalog");
        sb.AppendLine("  Playlist has NO PlaylistCatalog -> load PlaylistData directly");
        sb.AppendLine();
        sb.AppendLine("If a Playlist has a catalog but the button's PlaylistData is NOT inside that catalog,");
        sb.AppendLine("VideoTXL 2.5.1 resolves catalog index -1 and loads an empty/null playlist.");
        sb.AppendLine("That can look exactly like a button that stopped working after the update.");
        sb.AppendLine();

        if (loadButtons.Count == 0) sb.AppendLine("NONE FOUND\n");
        foreach (Component loader in loadButtons)
        {
            SerializedObject so = new SerializedObject(loader);
            UnityEngine.Object playlistObj = GetObject(so, "playlist");
            UnityEngine.Object dataObj = GetObject(so, "playlistData");

            sb.AppendLine("PlaylistLoadData: " + PathOf(loader.gameObject));
            sb.AppendLine("  Playlist: " + Describe(playlistObj));
            sb.AppendLine("  Playlist Data: " + Describe(dataObj));

            if (playlistObj == null)
            {
                sb.AppendLine("  RESULT: ERROR - Playlist reference is null.");
                sb.AppendLine();
                continue;
            }

            SerializedObject playlistSo = new SerializedObject(playlistObj);
            UnityEngine.Object catalogObj = GetObject(playlistSo, "playlistCatalog");
            sb.AppendLine("  Target Playlist Catalog: " + Describe(catalogObj));

            if (catalogObj == null)
            {
                sb.AppendLine("  ROUTE: direct _LoadData(playlistData)");
                sb.AppendLine("  RESULT: no catalog-membership problem detected.");
            }
            else
            {
                int catalogIndex = FindObjectInArray(catalogObj, "playlists", dataObj);
                sb.AppendLine("  ROUTE: _LoadFromCatalogueData(playlistData)");
                sb.AppendLine("  Catalog index for this Playlist Data: " + catalogIndex);

                if (dataObj == null)
                {
                    sb.AppendLine("  RESULT: ERROR - Playlist Data is null.");
                }
                else if (catalogIndex < 0)
                {
                    sb.AppendLine("  RESULT: *** LIKELY BROKEN IN VIDEOTXL 2.5.1 ***");
                    sb.AppendLine("  REASON: Playlist has a catalog, but this Playlist Data is not in that catalog.");
                    sb.AppendLine("  _Load() will use the catalog route, resolve -1, then load no Playlist Data.");
                }
                else
                {
                    sb.AppendLine("  RESULT: catalog membership is valid.");
                }
            }
            sb.AppendLine();
        }

        Section(sb, "4. VIDEOSOURCE UI - WHY SOURCE BUTTONS APPEAR");
        sb.AppendLine("VideoTXL 2.5.x VideoSourceUI builds source selector buttons at runtime from SourceManager sources.");
        sb.AppendLine("Enabled compatible sources can therefore become visible in the stock player UI.");
        sb.AppendLine();

        if (videoSourceUis.Count == 0) sb.AppendLine("NONE FOUND\n");
        foreach (Component ui in videoSourceUis)
        {
            SerializedObject so = new SerializedObject(ui);
            UnityEngine.Object managerObj = GetObject(so, "sourceManager");
            UnityEngine.Object buttonRoot = GetObject(so, "buttonRoot");
            UnityEngine.Object contentRoot = GetObject(so, "contentRoot");
            UnityEngine.Object templateRoot = GetObject(so, "templateRoot");

            sb.AppendLine("VideoSourceUI: " + PathOf(ui.gameObject));
            sb.AppendLine("  Source Manager: " + Describe(managerObj));
            sb.AppendLine("  Button Root: " + Describe(buttonRoot));
            sb.AppendLine("  Content Root: " + Describe(contentRoot));
            sb.AppendLine("  Template Root: " + Describe(templateRoot));

            if (managerObj != null)
            {
                SerializedObject managerSo = new SerializedObject(managerObj);
                SerializedProperty sources = managerSo.FindProperty("sources");
                if (sources != null && sources.isArray)
                {
                    sb.AppendLine("  Sources potentially represented by the stock UI:");
                    for (int i = 0; i < sources.arraySize; i++)
                    {
                        UnityEngine.Object source = sources.GetArrayElementAtIndex(i).objectReferenceValue;
                        if (source == null) continue;
                        SerializedObject sourceSo = new SerializedObject(source);
                        string sourceName = GetString(sourceSo, "sourceName");
                        bool enabled = GetBool(sourceSo, "sourceEnabled", true);
                        sb.AppendLine("    [" + i + "] " + (string.IsNullOrEmpty(sourceName) ? source.name : sourceName) + " | serialized enabled=" + enabled + " | " + Describe(source));
                    }
                }
            }
            sb.AppendLine();
        }

        Section(sb, "5. UNITY BUTTON ONCLICK WIRING");
        ScanButtons(sb, scene);

        Section(sb, "6. SIMPLE UI INPUT CHECKS");
        ScanCanvasGroups(sb, scene);

        Section(sb, "7. SUMMARY FOR NOVA");
        sb.AppendLine("Look first for any line containing:");
        sb.AppendLine("  *** LIKELY BROKEN IN VIDEOTXL 2.5.1 ***");
        sb.AppendLine();
        sb.AppendLine("If those lines exist, the custom PlaylistLoadData buttons have a concrete catalog mismatch.");
        sb.AppendLine("If no mismatch is found, inspect the OnClick section and CanvasGroup section next.");
        sb.AppendLine("The stock VideoSourceUI source-name leak is a separate issue from the custom load-button issue.");
        sb.AppendLine();

        string safeScene = MakeSafeFileName(scene.name);
        string stamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string assetPath = OutputFolder + "/TXL_Playlist_Diagnostic_" + safeScene + "_" + stamp + ".txt";
        File.WriteAllText(assetPath, sb.ToString(), Encoding.UTF8);
        AssetDatabase.Refresh();

        TextAsset report = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
        if (report != null)
        {
            Selection.activeObject = report;
            EditorGUIUtility.PingObject(report);
        }

        Debug.Log("[Nova VideoTXL Analyzer] Created: " + assetPath);
        EditorUtility.DisplayDialog("Nova VideoTXL Analyzer", "Done.\n\nCreated:\n" + assetPath + "\n\nSend this TXT file to Nova.", "OK");
    }

    private static void AppendPackageInfo(StringBuilder sb)
    {
        Section(sb, "VIDEOTXL PACKAGE");
        try
        {
            PackageInfo[] packages = PackageInfo.GetAllRegisteredPackages();
            bool found = false;
            foreach (PackageInfo p in packages)
            {
                if (p == null) continue;
                if ((p.name ?? "").IndexOf("texelsaur.video", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (p.displayName ?? "").IndexOf("VideoTXL", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    found = true;
                    sb.AppendLine("Name: " + p.name);
                    sb.AppendLine("Display Name: " + p.displayName);
                    sb.AppendLine("Version: " + p.version);
                    sb.AppendLine("Resolved Path: " + p.resolvedPath);
                    sb.AppendLine();
                }
            }
            if (!found) sb.AppendLine("VideoTXL package not identified through Package Manager.\n");
        }
        catch (Exception ex)
        {
            sb.AppendLine("Package query failed: " + ex.Message + "\n");
        }
    }

    private static void ScanButtons(StringBuilder sb, Scene scene)
    {
        int shown = 0;
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            Button[] buttons = root.GetComponentsInChildren<Button>(true);
            foreach (Button button in buttons)
            {
                string path = PathOf(button.gameObject);
                bool interesting = path.IndexOf("playlist", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                   path.IndexOf("vip", StringComparison.OrdinalIgnoreCase) >= 0;

                int count = button.onClick.GetPersistentEventCount();
                for (int i = 0; i < count && !interesting; i++)
                {
                    string method = button.onClick.GetPersistentMethodName(i) ?? "";
                    UnityEngine.Object target = button.onClick.GetPersistentTarget(i);
                    if (method.IndexOf("Load", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        (target != null && target.name.IndexOf("Playlist", StringComparison.OrdinalIgnoreCase) >= 0))
                        interesting = true;
                }

                if (!interesting) continue;
                shown++;
                sb.AppendLine("Button: " + path);
                sb.AppendLine("  enabled: " + button.enabled);
                sb.AppendLine("  interactable: " + button.interactable);
                sb.AppendLine("  activeInHierarchy: " + button.gameObject.activeInHierarchy);
                sb.AppendLine("  OnClick count: " + count);
                for (int i = 0; i < count; i++)
                {
                    sb.AppendLine("    [" + i + "] state=" + button.onClick.GetPersistentListenerState(i));
                    sb.AppendLine("        target=" + Describe(button.onClick.GetPersistentTarget(i)));
                    sb.AppendLine("        method=" + button.onClick.GetPersistentMethodName(i));
                }
                sb.AppendLine();
            }
        }
        sb.AppendLine("Focused buttons reported: " + shown + "\n");
    }

    private static void ScanCanvasGroups(StringBuilder sb, Scene scene)
    {
        int shown = 0;
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            CanvasGroup[] groups = root.GetComponentsInChildren<CanvasGroup>(true);
            foreach (CanvasGroup group in groups)
            {
                string path = PathOf(group.gameObject);
                if (path.IndexOf("tablet", StringComparison.OrdinalIgnoreCase) < 0 &&
                    path.IndexOf("playlist", StringComparison.OrdinalIgnoreCase) < 0 &&
                    path.IndexOf("vip", StringComparison.OrdinalIgnoreCase) < 0)
                    continue;

                shown++;
                sb.AppendLine("CanvasGroup: " + path);
                sb.AppendLine("  alpha=" + group.alpha);
                sb.AppendLine("  interactable=" + group.interactable);
                sb.AppendLine("  blocksRaycasts=" + group.blocksRaycasts);
                sb.AppendLine("  ignoreParentGroups=" + group.ignoreParentGroups);
                sb.AppendLine("  activeInHierarchy=" + group.gameObject.activeInHierarchy);
                sb.AppendLine();
            }
        }
        if (shown == 0) sb.AppendLine("No relevant CanvasGroups found.\n");
    }

    private static int FindObjectInArray(UnityEngine.Object holder, string propertyName, UnityEngine.Object wanted)
    {
        if (holder == null || wanted == null) return -1;
        try
        {
            SerializedObject so = new SerializedObject(holder);
            SerializedProperty array = so.FindProperty(propertyName);
            if (array == null || !array.isArray) return -1;
            for (int i = 0; i < array.arraySize; i++)
            {
                if (array.GetArrayElementAtIndex(i).objectReferenceValue == wanted)
                    return i;
            }
        }
        catch { }
        return -1;
    }

    private static List<Component> GetAllComponents(Scene scene)
    {
        List<Component> result = new List<Component>();
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            Component[] comps = root.GetComponentsInChildren<Component>(true);
            foreach (Component c in comps)
                if (c != null) result.Add(c);
        }
        return result;
    }

    private static List<Component> FindByType(List<Component> components, string fullName)
    {
        List<Component> result = new List<Component>();
        foreach (Component c in components)
            if (c != null && c.GetType().FullName == fullName) result.Add(c);
        return result;
    }

    private static UnityEngine.Object GetObject(SerializedObject so, string propertyName)
    {
        if (so == null) return null;
        SerializedProperty p = so.FindProperty(propertyName);
        return p != null && p.propertyType == SerializedPropertyType.ObjectReference ? p.objectReferenceValue : null;
    }

    private static string GetString(SerializedObject so, string propertyName)
    {
        if (so == null) return "";
        SerializedProperty p = so.FindProperty(propertyName);
        return p != null && p.propertyType == SerializedPropertyType.String ? p.stringValue : "";
    }

    private static bool GetBool(SerializedObject so, string propertyName, bool fallback)
    {
        if (so == null) return fallback;
        SerializedProperty p = so.FindProperty(propertyName);
        return p != null && p.propertyType == SerializedPropertyType.Boolean ? p.boolValue : fallback;
    }

    private static string Describe(UnityEngine.Object obj)
    {
        if (obj == null) return "<null>";
        Component c = obj as Component;
        if (c != null) return PathOf(c.gameObject) + " [" + c.GetType().FullName + "]";
        GameObject go = obj as GameObject;
        if (go != null) return PathOf(go) + " [GameObject]";
        string path = AssetDatabase.GetAssetPath(obj);
        return obj.name + " [" + obj.GetType().FullName + "]" + (string.IsNullOrEmpty(path) ? "" : " @ " + path);
    }

    private static string PathOf(GameObject go)
    {
        if (go == null) return "<null>";
        string path = go.name;
        Transform t = go.transform.parent;
        while (t != null)
        {
            path = t.name + "/" + path;
            t = t.parent;
        }
        return path;
    }

    private static string EmptyAsDefault(string value)
    {
        return string.IsNullOrEmpty(value) ? "<empty/default>" : value;
    }

    private static void Section(StringBuilder sb, string title)
    {
        sb.AppendLine();
        sb.AppendLine(title);
        sb.AppendLine(new string('=', title.Length));
        sb.AppendLine();
    }

    private static string MakeSafeFileName(string value)
    {
        foreach (char c in Path.GetInvalidFileNameChars()) value = value.Replace(c, '_');
        return value;
    }
}
#endif
