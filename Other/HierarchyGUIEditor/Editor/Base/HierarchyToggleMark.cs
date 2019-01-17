using UnityEngine;

public abstract class HierarchyToggleMark : HierarchyMark
{
    public bool m_ToggleValue;

    public HierarchyToggleMark() : base()
    {
        m_Type = HierarchyMarkType.Toggle;
    }

    public override void OnRenderer()
    {
        bool toggle = GUI.Toggle(m_RendereRect, m_ToggleValue, GUIContent.none, m_GUIStyle);

        if (toggle != m_ToggleValue)
        {
            m_ToggleValue = toggle;
            ToggleOnChange();
        }
    }

    public virtual void ToggleOnChange() {}

}
