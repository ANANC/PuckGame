using UnityEditor;
using UnityEngine;


public class SceneEdiotrMenuLockComponentItem : SceneEditorMenuComponentItem
{
    private GameObject[] m_SelectionGoInstanceIds;
    private SceneEdiotrMenuLockModule m_Module;

    protected override void OnInit()
    {
        m_Module = SceneEdiotrMenuLockModule.GetInstance();

        SetDynamicMenu(true);
    }

    protected override void OnDynamicMenuInit()
    {
        if (m_Module.UnLockActiveGameObect())
        {
            AddAchieveItem("解锁", RemoveCurrentLockGo);
        }
        else
        {
            AddAchieveItem("锁定当前选中", AddLockGo, AddLockGo_Condition);
        }

        AddAchieveItem("清理全部锁", ClearAll, ClearAll_Condition);
    }

    protected override void OnStart()
    {
        m_SelectionGoInstanceIds = Selection.gameObjects;
    }

    protected bool AddLockGo_Condition()
    {
        return Selection.gameObjects.Length > 0;
    }

    private void AddLockGo()
    {
        for (var index = 0; index < m_SelectionGoInstanceIds.Length; index++)
        {
            m_Module.AddLockGoInstanceGo(m_SelectionGoInstanceIds[index]);
        }

        Selection.objects = new Object[] {};
    }

    private bool ClearAll_Condition()
    {
        return m_Module.GetLockGoInstances().Count > 0;
    }

    private void ClearAll()
    {
        m_Module.ClearAll();
    }

    private void RemoveCurrentLockGo()
    {
        m_Module.RemoveCurrentLockGo();
    }

}
