using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HierarchyRootMark : HierarchyLabelMark
{
    private bool m_HasRecord = false;
    private List<GameObject> m_LastRendererList = null;
    private List<GameObject> m_nowRendererList = new List<GameObject>();
    private Dictionary<GameObject, GameObject> m_RootList = new Dictionary<GameObject, GameObject>();
    private List<GameObject> m_BindGoList =new List<GameObject>();

    public override void OnInit()
    {
        m_Name = "根节点收起时显示子节点标志";
    }

    public override void HierarchyOnChange()
    {

        m_BindGoList.Clear();
        m_RootList.Clear();

        GameObject[] allGameObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        GameObject curGameObject;
        for (int i = 0; i < allGameObjects.Length; i++)
        {
            curGameObject = allGameObjects[i];

            GameObject prefab = PrefabUtility.FindPrefabRoot(curGameObject);
            if (prefab == null || prefab == m_GameObject)
            {
                continue;
            }

            GUIBindBehaviour bindBehaviour = prefab.GetComponent<GUIBindBehaviour>();
            if (bindBehaviour == null)
            {
                continue;
            }

            if (bindBehaviour.m_LuaGUI.Exists(info => info.m_GameObject == curGameObject))
            {
                m_BindGoList.Add(curGameObject);
                GameObject rootParentGo = prefab.transform.parent != null ? prefab.transform.parent.gameObject : null;

                GameObject root;
                while (curGameObject.transform.parent != null) 
                {
                    root = curGameObject.transform.parent.gameObject;
                    if (root == rootParentGo)
                    {
                        break;
                    }

                    if (m_RootList.ContainsKey(root))
                    {
                       break;
                    }

                    m_RootList.Add(root, curGameObject);
                    curGameObject = root;
                }
            }
        }

    }

    public override void HierarchyRoot()
    {
        if (!m_HasRecord && m_LastRendererList != null)
        {
            m_HasRecord = true;
        }

        if (m_LastRendererList == null)
        {
            m_LastRendererList = new List<GameObject>();
        }

        m_LastRendererList.Clear();
        for (int i = 0; i < m_nowRendererList.Count; i++)
        {
            m_LastRendererList.Add(m_nowRendererList[i]);
        }

        m_nowRendererList.Clear();
    }

    public override bool RendererCondition()
    {
        m_nowRendererList.Add(m_GameObject);

        if (!m_HasRecord)
        {
            return false;
        }

        if (m_RootList.ContainsKey(m_GameObject) && !m_LastRendererList.Contains(m_RootList[m_GameObject]) && !m_BindGoList.Contains(m_GameObject))
            return true;

        return false;

    }


}
