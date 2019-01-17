using UnityEngine;

public enum HierarchyMarkType
{
    Label,
    Toggle,
}

public abstract class HierarchyMark
{
    public GameObject m_GameObject;
    public int m_InstanceId;
    public string m_Name;
    public HierarchyMarkType m_Type;
    public GUIStyle m_GUIStyle = null;
    public Rect m_RendereRect;

    public HierarchyMark()
    {
        m_GUIStyle = new GUIStyle();
        OnInit();
    }

    public abstract void OnInit();

    /// <summary>
    /// 界面变更
    /// </summary>
    public virtual void HierarchyOnChange() {}

    /// <summary>
    /// 界面根
    /// </summary>
    public virtual void HierarchyRoot() {}

    /// <summary>
    /// 渲染前条件
    /// </summary>
    public virtual bool RendererCondition()
    {
        return true;
    }

    /// <summary>
    /// 渲染
    /// </summary>
    public abstract void OnRenderer();
}
