using UnityEngine;

public abstract class HierarchyLabelMark : HierarchyMark
{
    public HierarchyLabelMark():base()
    {
        m_Type = HierarchyMarkType.Label;
    }

    public override void OnRenderer()
    {
        GUI.Label(m_RendereRect, string.Empty,m_GUIStyle);
    }
}
