using UnityEditor;
using UnityEngine;

public class SceneMenuListener
{
    private static Color s_SelectColor = Color.green;
    private static EventType s_LasType = EventType.Layout;
    private static EventType m_CurrenType;

    [InitializeOnLoadMethod]
    public static void Init()
    {
        //* Scene *//
        SceneEditorUpdateManager.AddUpdateAction(ShowMenu);
        SceneEditorUpdateManager.AddUpdateAction(DrawSelectionGos);
        SceneView.onSceneGUIDelegate += new SceneView.OnSceneFunc(OnSceneViewGUI);
    }


    #region Scene

    public static void OnSceneViewGUI(SceneView sceneView)
    {
        SceneEditorUpdateManager.Update();
    }

    public static void ShowMenu()
    {
        if (Event.current != null && Event.current.button == 1)
        {
            m_CurrenType = Event.current.type;

            if (s_LasType == EventType.MouseDown && m_CurrenType == EventType.MouseUp)
            {
                SceneEditorMenuMgr.Instance().Show();
                Event.current.Use();
            }
            else if (m_CurrenType != EventType.Layout && m_CurrenType != EventType.Repaint)
            {
                s_LasType = m_CurrenType;
            }
        }

    }

    public static void DrawSelectionGos()
    {
        if (Selection.transforms == null)
        {
            return;
        }

        RectTransform rectTransform;
        for (var index = 0; index < Selection.transforms.Length; index++)
        {
            rectTransform = Selection.transforms[index] as RectTransform;
            if (rectTransform == null)
            {
                continue;
            }

            SceneEditorUpdateManager.DrawCollider(rectTransform.position, rectTransform.rect, s_SelectColor);
        }

    }

    #endregion
}
