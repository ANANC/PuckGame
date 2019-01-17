using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


public class SceneEdiotrMenuAlignComponentItem : SceneEditorMenuComponentItem
{
    private GameObject[] m_SelectionGameObjects = null;

    protected override void OnInit()
    {
        AddAchieveItem("左对齐", AlignLeft);
        AddAchieveItem("右对齐", AlignRight);
        AddAchieveItem("顶对齐", AlignTop);
        AddAchieveItem("底对齐", AlignBottom);

        AddSeparator();

        AddAchieveItem("水平居中对齐", AlignCenterInHorziontal);
        AddAchieveItem("垂直居中对齐", AlignCenterInVertical);

        AddSeparator();

        AddAchieveItem("水平均匀排序", AlignSortInHorziontal);
        AddAchieveItem("垂直均匀排序", AlignSortInVertical);
    }

    protected override bool GeneralCondition()
    {
        return Selection.gameObjects.Length > 1;
    }

    protected override void OnStart()
    {
        m_SelectionGameObjects = Selection.gameObjects;
    }

    private void AlignLeft()
    {
        float minX = Mathf.Min(m_SelectionGameObjects.Select(gameObject => gameObject.transform.position.x).ToArray());

        Transform transform;
        Vector3 transformPos;
        for (var index = 0; index < m_SelectionGameObjects.Length; index++)
        {
            transform = m_SelectionGameObjects[index].transform;
            transformPos = transform.position;
            transformPos.x = minX;
            m_SelectionGameObjects[index].transform.position = transformPos;
        }
    }

    private void AlignRight()
    {
        float maxX = Mathf.Max(m_SelectionGameObjects.Select(gameObject => gameObject.transform.position.x).ToArray());

        Transform transform;
        Vector3 transformPos;
        for (var index = 0; index < m_SelectionGameObjects.Length; index++)
        {
            transform = m_SelectionGameObjects[index].transform;
            transformPos = transform.position;
            transformPos.x = maxX;
            m_SelectionGameObjects[index].transform.position = transformPos;
        }
    }

    private void AlignTop()
    {
        float maxY = Mathf.Max(m_SelectionGameObjects.Select(gameObject => gameObject.transform.position.y).ToArray());

        Transform transform;
        Vector3 transformPos;
        for (var index = 0; index < m_SelectionGameObjects.Length; index++)
        {
            transform = m_SelectionGameObjects[index].transform;
            transformPos = transform.position;
            transformPos.y = maxY;
            m_SelectionGameObjects[index].transform.position = transformPos;
        }
    }

    private void AlignBottom()
    {
        float minY = Mathf.Min(m_SelectionGameObjects.Select(gameObject => gameObject.transform.position.y).ToArray());

        Transform transform;
        Vector3 transformPos;
        for (var index = 0; index < m_SelectionGameObjects.Length; index++)
        {
            transform = m_SelectionGameObjects[index].transform;
            transformPos = transform.position;
            transformPos.y = minY;
            m_SelectionGameObjects[index].transform.position = transformPos;
        }
    }

    private void AlignCenterInHorziontal()
    {
        float[] posXArray = m_SelectionGameObjects.Select(gameObject => gameObject.transform.position.x).ToArray();
        float minX = Mathf.Min(posXArray);
        float maxX = Mathf.Max(posXArray);

        float centerX = (maxX + minX) * 0.5f;

        Transform transform;
        Vector3 transformPos;
        for (var index = 0; index < m_SelectionGameObjects.Length; index++)
        {
            transform = m_SelectionGameObjects[index].transform;
            transformPos = transform.position;
            transformPos.x = centerX;
            m_SelectionGameObjects[index].transform.position = transformPos;
        }
    }

    private void AlignCenterInVertical()
    {
        float[] posXArray = m_SelectionGameObjects.Select(gameObject => gameObject.transform.position.y).ToArray();
        float minY = Mathf.Min(posXArray);
        float maxY = Mathf.Max(posXArray);

        float centerY = (maxY + minY) * 0.5f;

        Transform transform;
        Vector3 transformPos;
        for (var index = 0; index < m_SelectionGameObjects.Length; index++)
        {
            transform = m_SelectionGameObjects[index].transform;
            transformPos = transform.position;
            transformPos.y = centerY;
            m_SelectionGameObjects[index].transform.position = transformPos;
        }
    }

    private void AlignSortInHorziontal()
    {
        float[] posXArray = m_SelectionGameObjects.Select(gameObject => gameObject.transform.position.x).ToArray();
        float minX = Mathf.Min(posXArray);
        float maxX = Mathf.Max(posXArray);

        float distance = (maxX - minX) / (m_SelectionGameObjects.Length - 1);

        List<GameObject> sortGameObjects = m_SelectionGameObjects.ToList();
        sortGameObjects.Sort((left, right) => (left.transform.position.x < right.transform.position.x) ? -1 : 1);

        Transform transform;
        Vector3 transformPos;
        for (var index = 0; index < sortGameObjects.Count; index++)
        {
            transform = sortGameObjects[index].transform;
            transformPos = transform.position;
            transformPos.x = minX + distance * index;
            sortGameObjects[index].transform.position = transformPos;
        }
        
    }

    private void AlignSortInVertical()
    {
        float[] posYArray = m_SelectionGameObjects.Select(gameObject => gameObject.transform.position.y).ToArray();

        float minY = Mathf.Min(posYArray);
        float maxY = Mathf.Max(posYArray);

        float distance = (maxY - minY) / (m_SelectionGameObjects.Length - 1);

        List<GameObject> sortGameObjects = m_SelectionGameObjects.ToList();
        sortGameObjects.Sort((left, right) => (left.transform.position.y < right.transform.position.y) ? -1 : 1);

        Transform transform;
        Vector3 transformPos;
        for (var index = 0; index < sortGameObjects.Count; index++)
        {
            transform = sortGameObjects[index].transform;
            transformPos = transform.position;
            transformPos.y = minY + distance * index;
            sortGameObjects[index].transform.position = transformPos;
        }
    }
}


