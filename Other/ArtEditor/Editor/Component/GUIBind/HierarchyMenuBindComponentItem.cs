using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class HierarchyMenuBindComponentItem : SceneEditorMenuComponentItem
{
    private GameObject m_SelectionGameObject = null;
    private List<string> m_CanSelectComponentsList;
    protected override void OnInit()
    {
        SetDynamicMenu(true);
    }

    protected override void OnDynamicMenuInit()
    {
        m_SelectionGameObject = Selection.activeGameObject;

        //所有控件
        List<string> componentsList = new List<string>() { "Transform", "GameObject" };   
        
        //可选控件
        m_CanSelectComponentsList = new List<string>() { "Transform", "GameObject" };

        MonoBehaviour[] monoBehaviours = m_SelectionGameObject.GetComponents<MonoBehaviour>();
        for (var i = 0; i < monoBehaviours.Length; i++)
        {
            if (monoBehaviours[i].GetType().Name.Contains("Transform"))
            {
                continue;
            }
            componentsList.Add(monoBehaviours[i].GetType().Name);
            m_CanSelectComponentsList.Add(monoBehaviours[i].GetType().Name);
        }


        GUIBindBehaviour bind = PrefabUtility.FindPrefabRoot(m_SelectionGameObject).GetComponent<GUIBindBehaviour>();
        GUIBindBehaviour.GUIBindInfo curInfo;
        for (var index = 0; index < bind.m_LuaGUI.Count; index++)
        {
            curInfo = bind.m_LuaGUI[index];
            if (curInfo.m_GameObject == null)
            {
                continue;
            }

            if (curInfo.m_GameObject == m_SelectionGameObject)
            {
                if (curInfo.m_RefType == GUIBindBehaviour.RefType.Transform)
                {
                    m_CanSelectComponentsList.Remove("Transform");
                }

                if (curInfo.m_RefType == GUIBindBehaviour.RefType.GameObject)
                {
                    m_CanSelectComponentsList.Remove("GameObject");
                }

                if (curInfo.m_RefType == GUIBindBehaviour.RefType.MonoBehaviour)
                {
                    m_CanSelectComponentsList.Remove(curInfo.m_Behaviour.GetType().Name);
                }
            }
        }


        for (var index = 0; index < componentsList.Count; index++)
        {
            if (index == 2)
            {
                AddSeparator();
            }

            AddAchieveItem(componentsList[index], AddGUIBindInfoToBehaviour, componentsList[index], m_CanSelectComponentsList.Contains(componentsList[index]));

        }
    }

    private void AddGUIBindInfoToBehaviour(object userData)
    {
        string componentName = userData as string;

        GUIBindBehaviour bind = PrefabUtility.FindPrefabRoot(m_SelectionGameObject).GetComponent<GUIBindBehaviour>();

        bool add = true;
        GUIBindBehaviour.GUIBindInfo curInfo;

        for (var index = 0; index < bind.m_LuaGUI.Count; index++)
        {
            curInfo = bind.m_LuaGUI[index];
            if (curInfo.m_GameObject == null)
            {
                continue;
            }
            
            //命名重复
            if (curInfo.m_GameObject!= m_SelectionGameObject && curInfo.m_GameObject.name == m_SelectionGameObject.name)
            {
                UnityEngine.Debug.LogError($"该对象（{m_SelectionGameObject.name}）的名字重名了，请修改命名。");
                add = false;
                break;
            }
        }

        if (add)
        {
            MonoBehaviour[] monoBehaviours = m_SelectionGameObject.GetComponents<MonoBehaviour>();
            GUIBindBehaviour.GUIBindInfo info = new GUIBindBehaviour.GUIBindInfo();
            info.m_GameObject = Selection.activeGameObject;
            info.m_Name = Selection.activeGameObject.name + componentName;

            if (componentName == "Transform")
            {
                info.m_RefType = GUIBindBehaviour.RefType.Transform;
                info.m_Behaviour = null;
            }
            else if (componentName == "GameObject")
            {
                info.m_RefType = GUIBindBehaviour.RefType.GameObject;
                info.m_Behaviour = null;
            }
            else
            {
                info.m_RefType = GUIBindBehaviour.RefType.MonoBehaviour;
                for (var i = 0; i < monoBehaviours.Length; i++)
                {
                    if (monoBehaviours[i].GetType().Name == componentName)
                    {
                        info.m_Behaviour = monoBehaviours[i];
                        break;
                    }
                }
            }

            bind.m_LuaGUI.Add(info);
        }
    }
}


