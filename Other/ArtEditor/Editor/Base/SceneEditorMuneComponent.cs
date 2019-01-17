using System.Collections.Generic;

public abstract class SceneEditorMuneComponent
{
    protected string m_ComponentName;

    private List<SceneEditorMuneComponent> m_ChlidComponent = null;
    private SceneEditorMenuComponentItem m_Items = null;
    private int[] m_Specification = null;

    public SceneEditorMuneComponent()
    {
        m_ComponentName = SetComponentName();

        m_ChlidComponent = CreatComponent();
        m_Items = CreateItem();

        m_Specification = SetSpecification();

        OnInit();
    }

    protected abstract string SetComponentName();

    protected virtual SceneEditorMenuComponentItem CreateItem()
    {
        return null;
    }

    protected virtual List<SceneEditorMuneComponent> CreatComponent()
    {
        return null;
    }

    protected virtual int[] SetSpecification()
    {
        return new[] {0, 0, 0, 0};
    }

    protected virtual void OnInit() {}

    public void CreateGenericMenu(string parentComponentName = "")
    {
        AddSeparator(0, parentComponentName);

        if (m_Items != null)
        {
            m_Items.Achieve(parentComponentName == string.Empty
                                ? m_ComponentName
                                : parentComponentName + m_ComponentName);

            AddSeparator(1, parentComponentName);
        }

        if (m_ChlidComponent != null)
        {
            AddSeparator(2, parentComponentName);

            for (var index = 0; index < m_ChlidComponent.Count; index++)
            {
                if (string.IsNullOrEmpty(m_ComponentName))
                {
                    m_ChlidComponent[index].CreateGenericMenu();
                }
                else
                {
                    m_ChlidComponent[index].CreateGenericMenu(string.Format("{0}/", m_ComponentName));
                }
            }
        }

        AddSeparator(3, parentComponentName);
    }

    protected void AddSeparator(int index, string parentComponentName)
    {
        bool hasSeparator = m_Specification != null && m_Specification.Length == 4;

        if (!hasSeparator || m_Specification[index] == 0)
        {
            return;
        }

        SceneViewMenuTool.Instance().AddSeparator(parentComponentName);
    }

}
