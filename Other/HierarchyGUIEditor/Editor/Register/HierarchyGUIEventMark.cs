using CoreFramework;

public class HierarchyGUIEventMark : HierarchyLabelMark
{
    public override void OnInit()
    {
        m_Name = "绑定GUIEventTrigger";
    }

    public override bool RendererCondition()
    {
        return m_GameObject.GetComponent<GUIEventTrigger>() != null;
    }
    
}
