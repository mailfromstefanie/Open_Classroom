#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NovaHierarchySidebar : EditorWindow
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

    private Type hierarchyWindowType;
    private MethodInfo setExpandedMethod;

    [MenuItem("Nova/Editor Tools/Hierarchy Sidebar")]
    public static void OpenWindow()
    {
        NovaHierarchySidebar window = GetWindow<NovaHierarchySidebar>();
        window.titleContent = new GUIContent("Nova Side");
        window.minSize = new Vector2(52, 240);
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

    private void OnGUI()
    {
        DrawTopButtons();
        DrawBookmarkButtons();
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

    private void DrawTopButtons()
    {
        EditorGUILayout.Space(4);

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("+", GUILayout.Width(22), GUILayout.Height(22)))
            {
                AddSelectedObject();
            }

            if (GUILayout.Button("R", GUILayout.Width(22), GUILayout.Height(22)))
            {
                Load();
                RefreshObjectCache();
                Repaint();
            }
        }

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("C", GUILayout.Width(22), GUILayout.Height(22)))
            {
                CollapseAllHierarchyObjects();
            }

            if (GUILayout.Button("?", GUILayout.Width(22), GUILayout.Height(22)))
            {
                ShowHelp();
            }
        }

        EditorGUILayout.Space(4);
    }

    private void DrawBookmarkButtons()
    {
        if (data.bookmarks == null || data.bookmarks.Count == 0)
        {
            EditorGUILayout.HelpBox("+", MessageType.None);
            return;
        }

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        for (int i = 0; i < data.bookmarks.Count; i++)
        {
            BookmarkData bookmark = data.bookmarks[i];

            if (bookmark == null)
                continue;

            UnityEngine.Object obj = GetCachedObject(bookmark.globalId);
            bool missing = obj == null;

            string buttonText = MakeShortButtonLabel(bookmark.label);

            Rect buttonRect = GUILayoutUtility.GetRect(
                40,
                150,
                GUILayout.Width(40),
                GUILayout.Height(150)
            );

            if (DrawRotatedButton(buttonRect, buttonText, missing))
            {
                if (obj != null)
                {
                    SelectAndPingObject(obj);
                    SetHierarchyExpanded(obj, true);
                }
            }

            HandleContextClick(buttonRect, i, bookmark, obj);

            EditorGUILayout.Space(4);
        }

        EditorGUILayout.EndScrollView();
    }

    private string MakeShortButtonLabel(string label)
    {
        if (string.IsNullOrWhiteSpace(label))
            return "?";

        string clean = label.Trim();

        if (clean.Length > 20)
            clean = clean.Substring(0, 20);

        return clean;
    }

    private bool DrawRotatedButton(Rect rect, string text, bool missing)
    {
        Event currentEvent = Event.current;
        int controlId = GUIUtility.GetControlID(FocusType.Passive, rect);

        bool clicked = false;

        switch (currentEvent.GetTypeForControl(controlId))
        {
            case EventType.MouseDown:
                if (rect.Contains(currentEvent.mousePosition) && currentEvent.button == 0)
                {
                    GUIUtility.hotControl = controlId;
                    currentEvent.Use();
                }
                break;

            case EventType.MouseUp:
                if (GUIUtility.hotControl == controlId)
                {
                    GUIUtility.hotControl = 0;

                    if (rect.Contains(currentEvent.mousePosition))
                    {
                        clicked = true;
                        currentEvent.Use();
                    }
                }
                break;

            case EventType.Repaint:
                DrawRotatedButtonVisual(rect, text, missing, controlId);
                break;
        }

        return clicked;
    }

    private void DrawRotatedButtonVisual(Rect rect, string text, bool missing, int controlId)
    {
        Event currentEvent = Event.current;

        bool isHot = controlId == GUIUtility.hotControl;
        bool isHover = rect.Contains(currentEvent.mousePosition);

        Color oldBackgroundColor = GUI.backgroundColor;

        if (missing)
        {
            GUI.backgroundColor = new Color(0.70f, 0.25f, 0.25f, 1f);
        }

        GUI.skin.button.Draw(
            rect,
            GUIContent.none,
            isHover,
            isHot,
            false,
            false
        );

        GUI.backgroundColor = oldBackgroundColor;

        Matrix4x4 oldMatrix = GUI.matrix;

        Vector2 pivot = new Vector2(
            rect.x + rect.width * 0.5f,
            rect.y + rect.height * 0.5f
        );

        GUIUtility.RotateAroundPivot(90f, pivot);

        // Important:
        // Because the text is rotated, width and height are swapped.
        // This rect gives the text more safe space inside the button.
        Rect rotatedTextRect = new Rect(
            pivot.x - rect.height * 0.5f + 8f,
            pivot.y - rect.width * 0.5f + 10f,
            rect.height - 16f,
            rect.width - 20f
        );

        GUIStyle textStyle = new GUIStyle(EditorStyles.boldLabel);
        textStyle.alignment = TextAnchor.MiddleCenter;
        textStyle.clipping = TextClipping.Clip;
        textStyle.wordWrap = false;
        textStyle.normal.textColor = EditorGUIUtility.isProSkin
            ? new Color(0.92f, 0.92f, 0.92f, 1f)
            : Color.black;

        // Smaller text than before.
        // This keeps it clearly inside the button.
        textStyle.fontSize = GetBestFittingFontSize(
            textStyle,
            text,
            rotatedTextRect,
            9,
            5
        );

        GUI.Label(rotatedTextRect, text, textStyle);

        GUI.matrix = oldMatrix;
    }

    private int GetBestFittingFontSize(
        GUIStyle style,
        string text,
        Rect availableRect,
        int maxFontSize,
        int minFontSize)
    {
        if (string.IsNullOrEmpty(text))
            return maxFontSize;

        GUIContent content = new GUIContent(text);

        for (int size = maxFontSize; size >= minFontSize; size--)
        {
            style.fontSize = size;

            Vector2 textSize = style.CalcSize(content);

            if (textSize.x <= availableRect.width - 2f &&
                textSize.y <= availableRect.height - 2f)
            {
                return size;
            }
        }

        return minFontSize;
    }

    private void HandleContextClick(Rect rect, int index, BookmarkData bookmark, UnityEngine.Object obj)
    {
        Event currentEvent = Event.current;

        if (currentEvent.type != EventType.ContextClick)
            return;

        if (!rect.Contains(currentEvent.mousePosition))
            return;

        GenericMenu menu = new GenericMenu();

        if (obj != null)
        {
            menu.AddItem(new GUIContent("Select"), false, () =>
            {
                SelectAndPingObject(obj);
            });

            menu.AddItem(new GUIContent("Open"), false, () =>
            {
                SelectAndPingObject(obj);
                SetHierarchyExpanded(obj, true);
            });

            menu.AddItem(new GUIContent("Close"), false, () =>
            {
                SelectAndPingObject(obj);
                SetHierarchyExpanded(obj, false);
            });

            menu.AddItem(new GUIContent("Use Object Name"), false, () =>
            {
                bookmark.label = obj.name;
                Save();
                Repaint();
            });
        }
        else
        {
            menu.AddDisabledItem(new GUIContent("Missing object"));
        }

        menu.AddSeparator("");

        menu.AddItem(new GUIContent("Move Up"), false, () =>
        {
            MoveBookmark(index, -1);
        });

        menu.AddItem(new GUIContent("Move Down"), false, () =>
        {
            MoveBookmark(index, 1);
        });

        menu.AddSeparator("");

        menu.AddItem(new GUIContent("Remove Bookmark"), false, () =>
        {
            data.bookmarks.RemoveAt(index);
            Save();
            RefreshObjectCache();
            Repaint();
        });

        menu.ShowAsContext();
        currentEvent.Use();
    }

    private void AddSelectedObject()
    {
        Load();

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
                    "Dit object staat al in je Nova Hierarchy bookmarks.",
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

        Save();
        RefreshObjectCache();
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
        RefreshObjectCache();
        Repaint();
    }

    private void RefreshObjectCache()
    {
        objectCache.Clear();

        if (data.bookmarks == null)
            return;

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
            Debug.LogWarning("NovaHierarchySidebar: Unity kon de Hierarchy open/close methode niet vinden. Select en Ping werken nog steeds.");
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
            Debug.LogWarning("NovaHierarchySidebar: Open/Close werkte niet in deze Unity-versie. Select en Ping werken nog steeds.\n" + exception.Message);
        }
    }

    private void CollapseAllHierarchyObjects()
    {
        if (hierarchyWindowType == null || setExpandedMethod == null)
        {
            Debug.LogWarning("NovaHierarchySidebar: Unity kon de Hierarchy open/close methode niet vinden.");
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

    private void ShowHelp()
    {
        EditorUtility.DisplayDialog(
            "Nova Hierarchy Sidebar",
            "Knoppen:\n\n+ = selected object toevoegen\nR = refresh\nC = alles dichtklappen\n? = uitleg\n\nBookmark knop:\nLeft click = selecteer en open\nRight click = extra opties zoals close, remove, up/down",
            "OK"
        );
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