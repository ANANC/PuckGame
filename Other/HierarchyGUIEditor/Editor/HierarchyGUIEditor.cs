using UnityEngine;
using UnityEditor;


public class HierarchyGUIEditor
{
    private static HierarchyMarkMgr m_Instance;

    [InitializeOnLoadMethod]
    public static void OnInit()
    {
        m_Instance = HierarchyMarkMgr.GetInstance();

        EditorApplication.hierarchyWindowItemOnGUI += DrawHierarchyIcon;
        EditorApplication.hierarchyWindowChanged += OnChange;
    }

    private static void DrawHierarchyIcon(int instanceID, Rect selectionRect)
    {
        m_Instance.Show(instanceID, selectionRect);
    }


    private static void OnChange()
    {
        m_Instance.OnChange();
    }
}
