using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class SceneEdiotrMenuLockModule
{

    private SceneEdiotrMenuLockModule()
    {
        Selection.selectionChanged += SelectOnChange;
        SceneEditorUpdateManager.AddUpdateAction(EditorDrawLockMark);
    }

    private static SceneEdiotrMenuLockModule s_Instance;

    public static SceneEdiotrMenuLockModule GetInstance()
    {
        if (s_Instance == null)
        {
            s_Instance = new SceneEdiotrMenuLockModule();
        }

        return s_Instance;
    }

    private int m_ActiveInstanceId = 0;
    private List<GameObject> m_LockGos = null;
    private List<int> m_LockInstances = null;


    private bool m_DrawLock = true;
    private readonly Color m_LockColor = Color.red;

    public void AddLockGoInstanceGo(GameObject instanceGo)
    {
        if (m_LockGos == null)
        {
            m_LockGos = new List<GameObject>();
            m_LockInstances = new List<int>();
        }

        if (!m_LockGos.Contains(instanceGo))
        {
            m_LockGos.Add(instanceGo);
            m_LockInstances.Add(instanceGo.GetInstanceID());
        }
    }

    public void RemoveLockGoInstanceGo(GameObject instanceGo)
    {
        if (m_LockGos == null)
        {
            return;
        }

        if (m_LockGos.Contains(instanceGo))
        {
            m_LockGos.Remove(instanceGo);
            m_LockInstances.Remove(instanceGo.GetInstanceID());

            Selection.activeGameObject = instanceGo;
        }
    }

    public void RemoveCurrentLockGo()
    {
        if (m_LockGos == null || m_ActiveInstanceId == 0)
        {
            return;
        }

        if (m_LockInstances.Contains(m_ActiveInstanceId))
        {
            Object goObject = EditorUtility.InstanceIDToObject(m_ActiveInstanceId);
            GameObject gameObject = goObject as GameObject;

            RemoveLockGoInstanceGo(gameObject);
        }
    }

    public List<GameObject> GetLockGoInstances()
    {
        if (m_LockGos == null)
        {
            return new List<GameObject>();
        }

        return m_LockGos;
    }


    public void ClearAll()
    {
        if (m_LockGos == null)
        {
            return;
        }

        m_LockGos.Clear();
        m_LockInstances.Clear();
    }

    public bool UnLockActiveGameObect()
    {
        SetActiveGameObject();

        if (m_LockGos == null || m_LockGos.Count == 0)
        {
            return false;
        }

        if (m_ActiveInstanceId == 0)
        {
            return false;
        }

        return m_LockInstances.Contains(m_ActiveInstanceId);
    }

    public bool HasLockTargetGameObject(int instanceId)
    {
        if (m_LockInstances == null)
        {
            return false;
        }

        return m_LockInstances.Contains(instanceId);
    }

    public void SetDrawLock(bool draw)
    {
        m_DrawLock = draw;
    }

    public bool GetDrawLock()
    {
        return m_DrawLock;
    }

    private void SelectOnChange()
    {
        if (m_LockGos == null || m_LockGos.Count == 0)
        {
            return;
        }

        SceneEditorUpdateManager.AddActiveAction(LockExamination);
    }

    private void LockExamination()
    {
        int[] selectionGoInstanceIds = Selection.instanceIDs;

        GetLockGoInstances();

        List<int> newSelectionGoInstanceIds = new List<int>();
        for (var index = 0; index < selectionGoInstanceIds.Length; index++)
        {
            if (!m_LockInstances.Contains(selectionGoInstanceIds[index]))
            {
                newSelectionGoInstanceIds.Add(selectionGoInstanceIds[index]);
            }
        }

        Selection.instanceIDs = newSelectionGoInstanceIds.ToArray();
    }

    private void SetActiveGameObject()
    {
        if (m_LockGos == null || m_LockGos.Count == 0)
        {
            return;
        }

        if (Event.current == null)
        {
            return;
        }

        GameObject selectGameObject = HandleUtility.PickGameObject(Event.current.mousePosition, false);

        if (selectGameObject == null)
        {
            m_ActiveInstanceId = 0;
            return;
        }

        m_ActiveInstanceId = (selectGameObject).GetInstanceID();
    }

    private void EditorDrawLockMark()
    {
        if (!m_DrawLock || m_LockGos == null || m_LockGos.Count == 0)
        {
            return;
        }

        RectTransform rectTransform;
        Vector3 transformPos;
        Rect transformRect;
        Vector2 center;

        float lockWidth;
        float lockHeight;
        float lockTop;

        List<GameObject> Temp = new List<GameObject>();
        for (var index = 0; index < m_LockGos.Count; index++)
        {
            if (m_LockGos[index] == null)
            {
                Temp.Add(m_LockGos[index]);
                continue;
            }

            rectTransform = m_LockGos[index].transform as RectTransform;

            if (rectTransform == null)
            {
                Renderer renderer = m_LockGos[index].GetComponent<Renderer>();
                if (renderer == null)
                {
                    continue;
                }

                transformPos = m_LockGos[index].transform.position;
                center = Vector3.zero;
                transformRect = new Rect(-renderer.bounds.size.x * 0.5f, -renderer.bounds.size.y * 0.5f,
                                         renderer.bounds.size.x, renderer.bounds.size.y);
            }
            else
            {
                transformRect = rectTransform.rect;
                transformPos = rectTransform.position;
                center = transformRect.center;
            }

            //边框
            SceneEditorUpdateManager.DrawCollider(transformPos, transformRect, m_LockColor);

            lockWidth = transformRect.width * 0.4f;
            lockHeight = transformRect.height * 0.2f;

            //锁
            SceneEditorUpdateManager.DrawCollider(transformPos,
                                                  new Rect(center.x - lockWidth * 0.5f, center.y - lockHeight * 0.5f,
                                                           lockWidth, lockHeight),
                                                  m_LockColor);

            lockWidth = lockWidth * 0.2f;
            lockHeight = lockHeight * 0.5f;
            lockTop = lockHeight * 2;

            Handles.DrawLines(new Vector3[]
            {
                transformPos + new Vector3(center.x + lockWidth, center.y + lockHeight),
                transformPos + new Vector3(center.x + lockWidth, center.y + lockTop),

                transformPos + new Vector3(center.x - lockWidth, center.y + lockHeight),
                transformPos + new Vector3(center.x - lockWidth, center.y + lockTop),

                transformPos + new Vector3(center.x + lockWidth, center.y + lockTop),
                transformPos + new Vector3(center.x - lockWidth, center.y + lockTop)
            });
        }

        for (int index = 0; index < Temp.Count; index++)
        {
            RemoveLockGoInstanceGo(Temp[index]);
        }
    }
}
