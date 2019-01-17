using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class HierarchyMarkEditor : EditorWindow
{
    private static HierarchyMarkEditor m_Instance;

    private List<HierarchyMarkMgr.MarkEditorInfo> m_RecordEditorInfos = null;

    private HierarchyMark m_CurMark = null;
    private HierarchyMarkMgr.MarkEditorInfo m_EditorInfo = null;
    private int m_RegisterSelectIndex = -1;
    private Vector2 m_MarkScrollViewVector2;
    private Vector2 m_InfoScrollViewVector2;
    private bool m_UseUnityStyle = true;
    private bool m_MoreGUIStyleSet = false;
    private Object[] m_IconObject = new Object[8];

    [MenuItem("Window/Seasun/HierarchyMarkSetting")]
    private static void Open()
    {
        if (m_Instance != null)
        {
            m_Instance.Close();
            m_Instance = null;
        }

        m_Instance = EditorWindow.GetWindow<HierarchyMarkEditor>();
        m_Instance.minSize = new Vector2(400, 300);
        m_Instance.GetMarkInfoFromJson();
    }

    private void OnDestroy()
    {
        m_Instance = null;

        m_RegisterSelectIndex = -1;
        m_CurMark = null;
        m_EditorInfo = null;
    }


    public void OnGUI()
    {

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical("Box", GUILayout.MaxWidth(200));
        {
            m_MarkScrollViewVector2 = EditorGUILayout.BeginScrollView(m_MarkScrollViewVector2);
            {
                for (var index = 0; index < HierarchyMarkRegister.Registers.Length; index++)
                {
                    GUI.backgroundColor = m_RegisterSelectIndex == index ? Color.grey : Color.white;
                    if (GUILayout.Button(HierarchyMarkRegister.Registers[index].Name))
                    {
                        m_RegisterSelectIndex = index;
                        Type type = HierarchyMarkRegister.Registers[index];
                        m_CurMark = type.Assembly.CreateInstance(type.Name) as HierarchyMark;
                        UpdateCurMarkInfo();
                    }

                    GUI.backgroundColor = Color.white;

                }
            }
            EditorGUILayout.EndScrollView();
        }
        EditorGUILayout.EndVertical();

        if (m_CurMark == null)
        {
            m_RegisterSelectIndex = -1;
            EditorGUILayout.LabelField("请选择对象");
            return;
        }

        EditorGUILayout.BeginVertical();
        {
            m_InfoScrollViewVector2 = EditorGUILayout.BeginScrollView(m_InfoScrollViewVector2);
            {
                MarkBaseInfoGUI();

                EditorGUILayout.Space();

                if (GUILayout.Button("保存", GUILayout.Height(30)))
                {
                    KeepCurInfo();
                    KeepMarkInfoToJson();
                }
            }
            EditorGUILayout.EndScrollView();
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
    }

    private void MarkBaseInfoGUI()
    {
        EditorGUILayout.BeginHorizontal("Box", GUILayout.Height(20));
        {
            EditorGUILayout.LabelField(m_CurMark.m_Name);
            MarkTypeGUI();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("基础信息");
        EditorGUILayout.BeginHorizontal();
        {
            bool toggle = GUILayout.Toggle(m_UseUnityStyle, "Unity内置样式");
            toggle = !GUILayout.Toggle(!toggle, "自定义样式");

            if (toggle != m_UseUnityStyle)
            {
                m_UseUnityStyle = toggle;
                UpdateGUIStyle();
            }
        }
        EditorGUILayout.EndHorizontal();

        if (m_UseUnityStyle)
        {
            string name = EditorGUILayout.TextField("样式名称：", m_EditorInfo.m_UnityStyleName);
            if (name != m_EditorInfo.m_UnityStyleName)
            {
                MrakUnityStyle(name);
            }
        }
        else
        {
            MarkIconInfoGUI("Normal", 0, ref m_EditorInfo.m_Normal);
            MarkIconInfoGUI("OnNormal", 1, ref m_EditorInfo.m_OnNormal);
            m_MoreGUIStyleSet = EditorGUILayout.Foldout(m_MoreGUIStyleSet, "更多设置");
            if (m_MoreGUIStyleSet)
            {
                MarkIconInfoGUI("Hover", 2, ref m_EditorInfo.m_Hover);
                MarkIconInfoGUI("OnHover", 3, ref m_EditorInfo.m_OnHover);
                MarkIconInfoGUI("Active", 4, ref m_EditorInfo.m_Active);
                MarkIconInfoGUI("OnActive", 5, ref m_EditorInfo.m_OnActive);
                MarkIconInfoGUI("Focused", 6, ref m_EditorInfo.m_Focused);
                MarkIconInfoGUI("OnFocused", 7, ref m_EditorInfo.m_OnFocused);
            }
        }

        EditorGUILayout.Space();

    }

    private void KeepCurInfo()
    {
        if (m_EditorInfo == null || m_CurMark == null)
        {
            return;
        }

        m_EditorInfo.m_UseUnity = m_UseUnityStyle;
    }

    private void UpdateCurMarkInfo()
    {
        string curTypeName = m_CurMark.GetType().Name;
        HierarchyMarkMgr.MarkEditorInfo info = m_RecordEditorInfos.Find(editorInfo => editorInfo.m_Name == curTypeName);
        if (info != null)
        {
            m_EditorInfo = info;
            m_UseUnityStyle = m_EditorInfo.m_UseUnity;
            UpdateGUIStyle();
        }
        else
        {
            m_EditorInfo = new HierarchyMarkMgr.MarkEditorInfo();
            m_EditorInfo.m_Name = m_CurMark.GetType().Name;
            m_RecordEditorInfos.Add(m_EditorInfo);
        }
    }

    private void UpdateGUIStyle()
    {
        if (m_UseUnityStyle)
        {
            MrakUnityStyle(m_EditorInfo.m_UnityStyleName);
        }
        else
        {
            UpdateSelfGUIStyle();
        }

        m_IconObject[0] = m_CurMark.m_GUIStyle.normal.background;
        m_IconObject[1] = m_CurMark.m_GUIStyle.onNormal.background;
        m_IconObject[2] = m_CurMark.m_GUIStyle.hover.background;
        m_IconObject[3] = m_CurMark.m_GUIStyle.onHover.background;
        m_IconObject[4] = m_CurMark.m_GUIStyle.active.background;
        m_IconObject[5] = m_CurMark.m_GUIStyle.onActive.background;
        m_IconObject[6] = m_CurMark.m_GUIStyle.focused.background;
        m_IconObject[7] = m_CurMark.m_GUIStyle.onFocused.background;
    }


    private void MrakUnityStyle(string styleName)
    {
        m_EditorInfo.m_UnityStyleName = styleName;
        if (!string.IsNullOrEmpty(m_EditorInfo.m_UnityStyleName))
        {
            m_CurMark.m_GUIStyle = new GUIStyle(m_EditorInfo.m_UnityStyleName);
        }
    }

    private void MarkIconInfoGUI(string text, int index, ref string path)
    {
        EditorGUILayout.LabelField(string.Format("【{0}】 Path: {1}", text, path));
        Object inputObject = EditorGUILayout.ObjectField(m_IconObject[index], typeof(Texture)) as Object;
        if (m_IconObject[index] != inputObject)
        {
            m_IconObject[index] = inputObject;
            if (inputObject != null)
            {
                path = AssetDatabase.GetAssetPath(m_IconObject[index]);
            }
            else
            {
                path = null;
            }

            UpdateSelfGUIStyle();
        }
    }

    private void UpdateSelfGUIStyle()
    {
        m_CurMark.m_GUIStyle.normal.background = string.IsNullOrEmpty(m_EditorInfo.m_Normal) ? null : AssetDatabase.LoadAssetAtPath<Texture2D>(m_EditorInfo.m_Normal);
        m_CurMark.m_GUIStyle.onNormal.background = string.IsNullOrEmpty(m_EditorInfo.m_OnNormal) ? null : AssetDatabase.LoadAssetAtPath<Texture2D>(m_EditorInfo.m_OnNormal);
        m_CurMark.m_GUIStyle.hover.background = string.IsNullOrEmpty(m_EditorInfo.m_Hover) ? null : AssetDatabase.LoadAssetAtPath<Texture2D>(m_EditorInfo.m_Hover);
        m_CurMark.m_GUIStyle.onHover.background = string.IsNullOrEmpty(m_EditorInfo.m_OnHover) ? null : AssetDatabase.LoadAssetAtPath<Texture2D>(m_EditorInfo.m_OnHover);
        m_CurMark.m_GUIStyle.active.background = string.IsNullOrEmpty(m_EditorInfo.m_Active) ? null : AssetDatabase.LoadAssetAtPath<Texture2D>(m_EditorInfo.m_Active);
        m_CurMark.m_GUIStyle.onActive.background = string.IsNullOrEmpty(m_EditorInfo.m_OnActive) ? null : AssetDatabase.LoadAssetAtPath<Texture2D>(m_EditorInfo.m_OnActive);
        m_CurMark.m_GUIStyle.focused.background = string.IsNullOrEmpty(m_EditorInfo.m_Focused) ? null : AssetDatabase.LoadAssetAtPath<Texture2D>(m_EditorInfo.m_Focused);
        m_CurMark.m_GUIStyle.onFocused.background = string.IsNullOrEmpty(m_EditorInfo.m_OnFocused) ? null : AssetDatabase.LoadAssetAtPath<Texture2D>(m_EditorInfo.m_OnFocused);
    }

    private void MarkTypeGUI()
    {
        if (m_CurMark.m_Type == HierarchyMarkType.Label)
        {
            EditorGUILayout.LabelField(string.Empty, m_CurMark.m_GUIStyle, GUILayout.Width(18), GUILayout.Height(18));
        }
        else if (m_CurMark.m_Type == HierarchyMarkType.Toggle)
        {
            ((HierarchyToggleMark) m_CurMark).m_ToggleValue = EditorGUILayout.Toggle(((HierarchyToggleMark) m_CurMark).m_ToggleValue, m_CurMark.m_GUIStyle, GUILayout.Width(18), GUILayout.Height(18));
        }
    }


    private void KeepMarkInfoToJson()
    {
        HierarchyMarkMgr.MarkEditorInfos infos = new HierarchyMarkMgr.MarkEditorInfos();
        infos.m_Infos = m_RecordEditorInfos;

        HierarchyMarkMgr.GetInstance().KeepMarkInfoToJson(infos);
    }

    public void GetMarkInfoFromJson()
    {
        HierarchyMarkMgr.MarkEditorInfos infos = HierarchyMarkMgr.GetInstance().ReadMarkInfoFromJson();
        if (infos != null)
        {
            m_RecordEditorInfos = infos.m_Infos;
        }

        if (m_RecordEditorInfos == null)
        {
            m_RecordEditorInfos = new List<HierarchyMarkMgr.MarkEditorInfo>();
        }
    }
}

