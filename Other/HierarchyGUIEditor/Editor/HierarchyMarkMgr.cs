using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class HierarchyMarkMgr
{

    public const string INFO_PATH = "Assets/Third/HierarchyGUIEditor/Editor/Info/HierarchyMarkInfo.txt";
    public const int m_Size = 16;

    private List<HierarchyMark> m_Marks = null;
    private bool m_Dirty = true;
    private bool m_OnChange = true;
    private GameObject InstanceGameObject = null;
    private Rect RendererRect = new Rect(0, 0, m_Size, m_Size);

    private static HierarchyMarkMgr s_Instance;

    private HierarchyMarkMgr() {}

    public static HierarchyMarkMgr GetInstance()
    {
        if (s_Instance == null)
        {
            s_Instance = new HierarchyMarkMgr();
        }

        return s_Instance;
    }

    #region Init

    private void OnInit()
    {
        if (m_Marks != null)
        {
            m_Marks.Clear();
            m_Marks = null;
        }

        m_Marks = new List<HierarchyMark>();
        MarkEditorInfos infos = ReadMarkInfoFromJson();

        Type curType;
        object TypeInstance;
        HierarchyMark curMark;
        MarkEditorInfo curMarkInfo;
        for (int i = 0; i < HierarchyMarkRegister.Registers.Length; i++)
        {
            curType = HierarchyMarkRegister.Registers[i];
            TypeInstance = curType.Assembly.CreateInstance(curType.Name);
            curMark = (HierarchyMark) TypeInstance;

            curMarkInfo = infos.m_Infos.Find(info => info.m_Name == curType.Name);
            if (curMarkInfo == null)
            {
                continue;
            }

            SetHierarchyMarkInfo(curMark, curMarkInfo);

            m_Marks.Add(curMark);
        }

    }

    private void SetHierarchyMarkInfo(HierarchyMark curMark, MarkEditorInfo markInfo)
    {
        if (markInfo.m_UseUnity)
        {
            GUIStyle guiStyle = new GUIStyle(markInfo.m_UnityStyleName);
            curMark.m_GUIStyle = guiStyle;
        }
        else
        {
            curMark.m_GUIStyle.normal.background = string.IsNullOrEmpty(markInfo.m_Normal) ? null : AssetDatabase.LoadAssetAtPath<Texture2D>(markInfo.m_Normal);
            curMark.m_GUIStyle.onNormal.background = string.IsNullOrEmpty(markInfo.m_OnNormal) ? null : AssetDatabase.LoadAssetAtPath<Texture2D>(markInfo.m_OnNormal);
            curMark.m_GUIStyle.hover.background = string.IsNullOrEmpty(markInfo.m_Hover) ? null : AssetDatabase.LoadAssetAtPath<Texture2D>(markInfo.m_Hover);
            curMark.m_GUIStyle.onHover.background = string.IsNullOrEmpty(markInfo.m_OnHover) ? null : AssetDatabase.LoadAssetAtPath<Texture2D>(markInfo.m_OnHover);
            curMark.m_GUIStyle.active.background = string.IsNullOrEmpty(markInfo.m_Active) ? null : AssetDatabase.LoadAssetAtPath<Texture2D>(markInfo.m_Active);
            curMark.m_GUIStyle.onActive.background = string.IsNullOrEmpty(markInfo.m_OnActive) ? null : AssetDatabase.LoadAssetAtPath<Texture2D>(markInfo.m_OnActive);
            curMark.m_GUIStyle.focused.background = string.IsNullOrEmpty(markInfo.m_Focused) ? null : AssetDatabase.LoadAssetAtPath<Texture2D>(markInfo.m_Focused);
            curMark.m_GUIStyle.onFocused.background = string.IsNullOrEmpty(markInfo.m_OnFocused) ? null : AssetDatabase.LoadAssetAtPath<Texture2D>(markInfo.m_OnFocused);
        }
    }

    #endregion

    #region KeepAndRead

    public void KeepMarkInfoToJson(MarkEditorInfos infos)
    {
        string jsonContent = EditorJsonUtility.ToJson(infos, true);
        File.WriteAllText(INFO_PATH, jsonContent);

        EditorUtility.DisplayDialog("保存", "保存完成\n（" + INFO_PATH + ")", "成功");

        m_Dirty = true;
    }

    public MarkEditorInfos ReadMarkInfoFromJson()
    {
        MarkEditorInfos infos = null;

        if (File.Exists(INFO_PATH))
        {
            string jsonContent = File.ReadAllText(INFO_PATH);
            infos = JsonUtility.FromJson<MarkEditorInfos>(jsonContent);
        }

        if (infos == null)
        {
            infos = new MarkEditorInfos();
            infos.m_Infos = new List<MarkEditorInfo>();
        }

        return infos;
    }

    #endregion

    #region Renderer

    public void Show(int instanceID, Rect selectionRect)
    {
        if (EditorApplication.isPlaying)
        {
            return;
        }

        if (m_Dirty)
        {
            OnInit();
            m_Dirty = false;
        }

        InstanceGameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

        RendererRect.x = selectionRect.x + selectionRect.width - m_Size;
        RendererRect.y = selectionRect.y;

        for (int i = 0; i < m_Marks.Count; i++)
        {
            m_Marks[i].m_GameObject = InstanceGameObject;
            m_Marks[i].m_InstanceId = instanceID;

            if (m_OnChange)
            {
                m_Marks[i].HierarchyOnChange();
            }

            if (InstanceGameObject == null)
            {
                m_Marks[i].HierarchyRoot();
                continue;
            }

            if (m_Marks[i].RendererCondition())
            {
                m_Marks[i].m_RendereRect = RendererRect;

                m_Marks[i].OnRenderer();
                RendererRect.x -= m_Size;
            }
        }

        if (m_OnChange)
        {
            m_OnChange = false;
        }
    }

    public void OnChange()
    {
        m_OnChange = true;
    }

    #endregion

    [Serializable]
    public class MarkEditorInfos
    {
        public List<MarkEditorInfo> m_Infos = null;
    }

    [Serializable]
    public class MarkEditorInfo
    {
        public string m_Name = string.Empty;

        public bool m_UseUnity = true;

        public string m_UnityStyleName = string.Empty;

        public string m_Normal = string.Empty;
        public string m_Hover = string.Empty;
        public string m_Active = string.Empty;
        public string m_Focused = string.Empty;
        public string m_OnNormal = string.Empty;
        public string m_OnHover = string.Empty;
        public string m_OnActive = string.Empty;
        public string m_OnFocused = string.Empty;
    }

}
