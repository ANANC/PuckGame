using UnityEditor;
using UnityEngine;

public class HierarchyGUIBindTargetMark : HierarchyLabelMark
{
    public override void OnInit()
    {
        m_Name = "GUIBind绑定对象";
    }

    public override bool RendererCondition()
    {
        GameObject root = PrefabUtility.FindPrefabRoot(m_GameObject);
        if (root == null)
        {
            return false;
        }

        GUIBindBehaviour bindBehaviour = root.GetComponent<GUIBindBehaviour>();
        if (bindBehaviour == null)
        {
            return false;
        }

        return bindBehaviour.m_LuaGUI.Exists(info => info.m_GameObject == m_GameObject);
    }
}
