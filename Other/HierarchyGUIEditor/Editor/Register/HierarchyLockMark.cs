
public class HierarchyLockMark : HierarchyToggleMark
{
    private SceneEdiotrMenuLockModule m_LockInstance;

    public override void OnInit()
    {
        m_Name = "锁";

        m_LockInstance = SceneEdiotrMenuLockModule.GetInstance();
    }

    public override bool RendererCondition()
    {
        m_ToggleValue = m_LockInstance.HasLockTargetGameObject(m_InstanceId);
        return m_ToggleValue;
    }

    public override void ToggleOnChange()
    {
        if (!m_ToggleValue)
        {
            m_LockInstance.RemoveLockGoInstanceGo(m_GameObject);
        }
    }
}
