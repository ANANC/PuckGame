using System.Collections.Generic;

public abstract class EditorMenuRegisterMgr
{

    public EditorMenuRegisterMgr()
    {
        OnInit();
    }

    private List<SceneEditorMuneComponent> m_Components = null;

    #region Start

    protected void OnInit()
    {
        m_Components = new List<SceneEditorMuneComponent>();

        ResisterComponent();
    }

    protected abstract void ResisterComponent();

    protected void AddComponent<T>(T component) where T : SceneEditorMuneComponent
    {
        m_Components.Add(component);
    }

    #endregion

    #region Destory

    public void OnDestroy()
    {
        m_Components.Clear();
        m_Components = null;

        DestoryEvent();
    }

    protected abstract void DestoryEvent();

    #endregion

    public void Show()
    {
        SceneViewMenuTool.Instance().Start();

        SceneEditorMuneComponent component;
        for (var index = 0; index < m_Components.Count; index++)
        {
            component = m_Components[index];
            component.CreateGenericMenu();
        }

        SceneViewMenuTool.Instance().Show();
    }
}
