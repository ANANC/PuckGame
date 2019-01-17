using UnityEngine;

public class SceneEdiotrMenuLockMembersComponentItem : SceneEditorMenuComponentItem
{
    private SceneEdiotrMenuLockModule m_Module;

    protected override void OnInit()
    {
        m_Module = SceneEdiotrMenuLockModule.GetInstance();

        SetDynamicMenu(true);
    }

    protected override void OnDynamicMenuInit()
    {
        if (ChangeDrawLock_Condition())
        {
            AddAchieveItem(m_Module.GetDrawLock() ? "隐藏锁" : "显示锁", ChangeDrawLock);
            AddSeparator();
        }

        int count = m_Module.GetLockGoInstances().Count;

        for (int index = 0; index < count; index++)
        {
            GameObject gameObject = m_Module.GetLockGoInstances()[index];

            string GoPath = gameObject.transform.parent != null
                ? string.Format("path：{0}\\{1}", gameObject.transform.parent.name, gameObject.name)
                : "path：" + gameObject.name;

            AddAchieveItem(GoPath, RemoveLockGo, gameObject);
        }
    }

    private void RemoveLockGo(object goInstance)
    {
        m_Module.RemoveLockGoInstanceGo((GameObject) goInstance);
    }

    private bool ChangeDrawLock_Condition()
    {
        return m_Module.GetLockGoInstances().Count > 0;
    }

    private void ChangeDrawLock()
    {
        m_Module.SetDrawLock(!m_Module.GetDrawLock());
    }

}
