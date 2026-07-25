#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NovaHierarchyQuickPanel : EditorWindow
{
    [Serializable]
    private class BookmarkData
    {
        public string label;
        public string globalId;
    }

    [Serializable]
    private class BookmarkList
    {
        public List<BookmarkData> bookmarks = new List<BookmarkData>();
    }

    private const string EditorPrefsKey = "NovaHierarchyQuickPanel_Bookmarks_v4";

    private BookmarkList data = new BookmarkList();

    private readonly Dictionary<string, UnityEngine.Object> objectCache =
        new Dictionary<string, UnityEngine.Object>();

    private Vector2 scrollPosition;
    private string searchText = "";

    private Type hierarchyWindowType;
    private MethodInfo setExpandedMethod;

    [MenuItem("Nova/Editor Tools/Hierarchy Quick Panel")]
    public static void OpenWindow()
    {
        NovaHierarchyQuickPanel window = GetWindow<NovaHierarchyQuickPanel>();
        window.titleContent = new GUIContent("Nova Hierarchy");
        window.minSize = new Vector2(360, 300);
        window.Show();
    }

    private void OnEnable()
    {
        CacheReflection();
        Load();
        RefreshObjectCache();
    }

    private void OnFocus()
    {
        Load();
        RefreshObjectCache();
        Repaint();
    }

    private void OnDisable()
    {
        Save();
    }

    private void OnGUI()
    {
        DrawHeader();
        DrawToolbar();
        DrawSearch();
        DrawBookmarkList();
    }

    private void CacheReflection()
    {
        hierarchyWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");

        if (hierarchyWindowType == null)
            return;

        setExpandedMethod = hierarchyWindowType.GetMethod(
            "SetExpanded",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
        );
    }

    private void DrawHeader()
    {
        EditorGUILayout.Space(6);

        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.fontSize = 14;

        EditorGUILayout.LabelField("Nova Hierarchy Quick Panel", titleStyle);

        GUIStyle smallStyle = new GUIStyle(EditorStyles.miniLabel);
        smallStyle.wordWrap = true;

        EditorGUILayout.LabelField(
            "Snel naar belangrijke objecten in je Hierarchy.",
            smallStyle
        );

        EditorGUILayout.Space(4);
    }

    private void DrawToolbar()
    {
        using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
        {
            if (GUILayout.Button("Add Selected", EditorStyles.toolbarButton))
            {
                AddSelectedObject();
            }

            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton))
            {
                Load();
                RefreshObjectCache();
                Repaint();
            }

            if (GUILayout.Button("Remove Missing", EditorStyles.toolbarButton))
            {
                RemoveMissingObjects();
            }

            if (GUILayout.Button("Collapse All", EditorStyles.toolbarButton))
            {
                CollapseAllHierarchyObjects();
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Save", EditorStyles.toolbarButton, GUILayout.Width(45)))
            {
                Save();
            }

            if (GUILayout.Button("Clear", EditorStyles.toolbarButton, GUILayout.Width(45)))
            {
                ClearAllBookmarks();
            }
        }
    }

    private void DrawSearch()
    {
        EditorGUILayout.Space(6);

        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Label("Search", GUILayout.Width(48));
            searchText = EditorGUILayout.TextField(searchText);

            if (GUILayout.Button("X", GUILayout.Width(24)))
            {
                searchText = "";
                GUI.FocusControl(null);
            }
        }

        EditorGUILayout.Space(4);
    }

    private void DrawBookmarkList()
    {
        if (data.bookmarks.Count == 0)
        {
            EditorGUILayout.HelpBox(
                "Nog geen bookmarks. Selecteer een GameObject en klik op Add Selected.",
                MessageType.Info
            );
            return;
        }

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        for (int i = 0; i < data.bookmarks.Count; i++)
        {
            BookmarkData bookmark = data.bookmarks[i];

            if (!PassesSearch(bookmark))
                continue;

            DrawCompactBookmark(i, bookmark);
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawCompactBookmark(int index, BookmarkData bookmark)
    {
        UnityEngine.Object obj = GetCachedObject(bookmark.globalId);
        bool missing = obj == null;

        Color oldBackground = GUI.backgroundColor;

        if (missing)
        {
            GUI.backgroundColor = new Color(0.75f, 0.35f, 0.35f, 1f);
        }

        EditorGUILayout.BeginVertical("box");
        GUI.backgroundColor = oldBackground;

        using (new EditorGUILayout.HorizontalScope())
        {
            bookmark.label = EditorGUILayout.TextField(bookmark.label, GUILayout.MinWidth(90));

            GUI.enabled = !missing;

            if (GUILayout.Button("Select", GUILayout.Width(58), GUILayout.Height(22)))
            {
                SelectAndPingObject(obj);
            }

            if (GUILayout.Button("Open", GUILayout.Width(48), GUILayout.Height(22)))
            {
                SelectAndPingObject(obj);
                SetHierarchyExpanded(obj, true);
            }

            if (GUILayout.Button("Close", GUILayout.Width(50), GUILayout.Height(22)))
            {
                SelectAndPingObject(obj);
                SetHierarchyExpanded(obj, false);
            }

            GUI.enabled = true;

            if (GUILayout.Button("X", GUILayout.Width(24), GUILayout.Height(22)))
            {
                data.bookmarks.RemoveAt(index);
                Save();
                RefreshObjectCache();
                GUIUtility.ExitGUI();
            }
        }

        using (new EditorGUILayout.HorizontalScope())
        {
            GUIStyle miniStyle = new GUIStyle(EditorStyles.miniLabel);
            miniStyle.normal.textColor = missing ? new Color(1f, 0.55f, 0.55f) : new Color(0.65f, 0.65f, 0.65f);

            string status = missing ? "Missing object" : obj.name;
            GUILayout.Label(status, miniStyle);

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Up", EditorStyles.miniButtonLeft, GUILayout.Width(38)))
            {
                MoveBookmark(index, -1);
                GUIUtility.ExitGUI();
            }

            if (GUILayout.Button("Down", EditorStyles.miniButtonMid, GUILayout.Width(48)))
            {
                MoveBookmark(index, 1);
                GUIUtility.ExitGUI();
            }

            GUI.enabled = !missing;

            if (GUILayout.Button("Name", EditorStyles.miniButtonRight, GUILayout.Width(48)))
            {
                bookmark.label = obj.name;
                Save();
            }

            GUI.enabled = true;
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(2);
    }

    private bool PassesSearch(BookmarkData bookmark)
    {
        if (string.IsNullOrWhiteSpace(searchText))
            return true;

        string label = bookmark.label == null ? "" : bookmark.label;
        return label.ToLowerInvariant().Contains(searchText.ToLowerInvariant());
    }

    private void AddSelectedObject()
    {
        GameObject selected = Selection.activeGameObject;

        if (selected == null)
        {
            EditorUtility.DisplayDialog(
                "Geen GameObject geselecteerd",
                "Selecteer eerst een GameObject in je Hierarchy.",
                "OK"
            );
            return;
        }

        GlobalObjectId globalObjectId = GlobalObjectId.GetGlobalObjectIdSlow(selected);
        string idString = globalObjectId.ToString();

        for (int i = 0; i < data.bookmarks.Count; i++)
        {
            if (data.bookmarks[i].globalId == idString)
            {
                EditorUtility.DisplayDialog(
                    "Bestaat al",
                    "Dit object staat al in je Nova Hierarchy Quick Panel.",
                    "OK"
                );
                return;
            }
        }

        data.bookmarks.Add(new BookmarkData
        {
            label = selected.name,
            globalId = idString
        });

        objectCache[idString] = selected;

        Save();
        Repaint();
    }

    private void RemoveMissingObjects()
    {
        RefreshObjectCache();

        int before = data.bookmarks.Count;

        data.bookmarks.RemoveAll(bookmark => GetCachedObject(bookmark.globalId) == null);

        int removed = before - data.bookmarks.Count;

        Save();
        RefreshObjectCache();

        EditorUtility.DisplayDialog(
            "Klaar",
            "Missing bookmarks verwijderd: " + removed,
            "OK"
        );
    }

    private void ClearAllBookmarks()
    {
        bool confirm = EditorUtility.DisplayDialog(
            "Alles verwijderen?",
            "Weet je zeker dat je alle bookmarks wilt verwijderen?",
            "Ja",
            "Nee"
        );

        if (!confirm)
            return;

        data.bookmarks.Clear();
        objectCache.Clear();
        Save();
        Repaint();
    }

    private void MoveBookmark(int index, int direction)
    {
        int newIndex = index + direction;

        if (newIndex < 0 || newIndex >= data.bookmarks.Count)
            return;

        BookmarkData item = data.bookmarks[index];
        data.bookmarks.RemoveAt(index);
        data.bookmarks.Insert(newIndex, item);

        Save();
        Repaint();
    }

    private void RefreshObjectCache()
    {
        objectCache.Clear();

        for (int i = 0; i < data.bookmarks.Count; i++)
        {
            BookmarkData bookmark = data.bookmarks[i];

            if (bookmark == null || string.IsNullOrEmpty(bookmark.globalId))
                continue;

            UnityEngine.Object obj = ResolveObjectFromGlobalId(bookmark.globalId);
            objectCache[bookmark.globalId] = obj;
        }
    }

    private UnityEngine.Object GetCachedObject(string globalId)
    {
        if (string.IsNullOrEmpty(globalId))
            return null;

        if (objectCache.TryGetValue(globalId, out UnityEngine.Object obj))
            return obj;

        return null;
    }

    private UnityEngine.Object ResolveObjectFromGlobalId(string globalIdString)
    {
        if (string.IsNullOrEmpty(globalIdString))
            return null;

        if (!GlobalObjectId.TryParse(globalIdString, out GlobalObjectId globalObjectId))
            return null;

        return GlobalObjectId.GlobalObjectIdentifierToObjectSlow(globalObjectId);
    }

    private void SelectAndPingObject(UnityEngine.Object obj)
    {
        if (obj == null)
            return;

        Selection.activeObject = obj;
        EditorGUIUtility.PingObject(obj);
        FocusHierarchyWindow();
    }

    private void FocusHierarchyWindow()
    {
        if (hierarchyWindowType == null)
            return;

        EditorWindow hierarchyWindow = GetWindow(hierarchyWindowType);
        hierarchyWindow.Focus();
    }

    private void SetHierarchyExpanded(UnityEngine.Object obj, bool expanded)
    {
        if (obj == null)
            return;

        if (hierarchyWindowType == null || setExpandedMethod == null)
        {
            Debug.LogWarning("NovaHierarchyQuickPanel: Unity kon de Hierarchy open/close methode niet vinden. Select en Ping werken nog steeds.");
            return;
        }

        EditorWindow hierarchyWindow = GetWindow(hierarchyWindowType);

        try
        {
            setExpandedMethod.Invoke(
                hierarchyWindow,
                new object[]
                {
                    obj.GetInstanceID(),
                    expanded
                }
            );
        }
        catch (Exception exception)
        {
            Debug.LogWarning("NovaHierarchyQuickPanel: Open/Close werkte niet in deze Unity-versie. Select en Ping werken nog steeds.\n" + exception.Message);
        }
    }

    private void CollapseAllHierarchyObjects()
    {
        if (hierarchyWindowType == null || setExpandedMethod == null)
        {
            Debug.LogWarning("NovaHierarchyQuickPanel: Unity kon de Hierarchy open/close methode niet vinden.");
            return;
        }

        FocusHierarchyWindow();

        int sceneCount = SceneManager.sceneCount;

        for (int sceneIndex = 0; sceneIndex < sceneCount; sceneIndex++)
        {
            Scene scene = SceneManager.GetSceneAt(sceneIndex);

            if (!scene.isLoaded)
                continue;

            GameObject[] rootObjects = scene.GetRootGameObjects();

            for (int i = 0; i < rootObjects.Length; i++)
            {
                CollapseGameObjectAndChildren(rootObjects[i]);
            }
        }

        Repaint();
    }

    private void CollapseGameObjectAndChildren(GameObject gameObject)
    {
        if (gameObject == null)
            return;

        SetHierarchyExpanded(gameObject, false);

        Transform transform = gameObject.transform;

        for (int i = 0; i < transform.childCount; i++)
        {
            CollapseGameObjectAndChildren(transform.GetChild(i).gameObject);
        }
    }

    private void Save()
    {
        string json = JsonUtility.ToJson(data);
        EditorPrefs.SetString(EditorPrefsKey, json);
    }

    private void Load()
    {
        string json = EditorPrefs.GetString(EditorPrefsKey, "");

        if (string.IsNullOrEmpty(json))
        {
            data = new BookmarkList();
            return;
        }

        data = JsonUtility.FromJson<BookmarkList>(json);

        if (data == null)
            data = new BookmarkList();

        if (data.bookmarks == null)
            data.bookmarks = new List<BookmarkData>();
    }
}
#endif