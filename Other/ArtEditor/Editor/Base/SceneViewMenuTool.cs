using UnityEditor;
using UnityEngine;

public class SceneViewMenuTool
{
    private SceneViewMenuTool() {}
    private static SceneViewMenuTool s_Instance;

    public static SceneViewMenuTool Instance()
    {
        if (s_Instance == null)
        {
            s_Instance = new SceneViewMenuTool();
        }

        return s_Instance;
    }

    private GenericMenu m_Menu = null;

    public void Start()
    {
        m_Menu = new GenericMenu();
    }

    public void AddMenuItem(string menuPath, bool able, GenericMenu.MenuFunction callback)
    {
        if (able)
        {
            m_Menu.AddItem(new GUIContent(menuPath), false, callback);
        }
        else
        {
            m_Menu.AddDisabledItem(new GUIContent(menuPath));
        }
    }

    public void AddMenuItem(string menuPath, bool able, GenericMenu.MenuFunction2 callback, object userData)
    {
        if (able)
        {
            m_Menu.AddItem(new GUIContent(menuPath), false, callback, userData);
        }
        else
        {
            m_Menu.AddDisabledItem(new GUIContent(menuPath));
        }
    }

    public void AddSeparator(string path = "")
    {
        m_Menu.AddSeparator(path);
    }

    public void Show()
    {
        m_Menu.ShowAsContext();
    }
}
