using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

//Foldout 
public class GUIBindWindow : EditorWindow
{
    private static GUIBindWindow m_Instance = null;

    private static string TemplePath = "Assets/GUIBindEditor/GUIBindTemple.cs";

    private static string[] ButtonEvent = new[] { "onClick", "onDown", "onUp", "onBeginDrag", "onDrag", "onEndDrag" };
    private static string[] SliderEvent = new[] { "onValueChanged" };
    private static string[] ToggleEvent = new[] { "onValueChanged" };

    private Object m_InputObject;
    private Vector2 m_ScrollViewPos;

    private GameObject m_Root;
    private List<GameObject> m_RecordGos;
    private List<Transform> m_RecordTrans;
    private List<MonoBehaviour> m_RecordBehaviours;
    private SelectInfo m_CurInfo;
    private List<BindCell> m_BindCells;
    private Dictionary<object, BindCell> m_DualCells;
    private List<BindCell> m_UnabelCells;
    private List<BindCell> m_deleteCells;

    private bool m_IsPrefab;

    [MenuItem("Test/GuiBind")]
    private static void Open()
    {
        if (m_Instance != null)
        {
            EditorWindow.Destroy(m_Instance);
        }

        m_Instance = EditorWindow.GetWindow<GUIBindWindow>();
        m_Instance.OnInit();
    }

    private void OnFocus()
    {
        if (m_Instance == null)
        {
            Open();
        }
        else
        {
            NewCurInfo(Selection.activeGameObject);
        }
    }

    private void OnDestroy()
    {
        Selection.selectionChanged -= selectionChange;
        EditorApplication.hierarchyWindowChanged += hierarchyWindowChanged;
        m_Instance = null;
    }

    private void OnInit()
    {
        m_Root = null;
        m_CurInfo = null;

        m_RecordGos = new List<GameObject>();
        m_RecordTrans = new List<Transform>();
        m_RecordBehaviours = new List<MonoBehaviour>();
        m_BindCells = new List<BindCell>();
        m_DualCells = new Dictionary<object, BindCell>();
        m_UnabelCells = new List<BindCell>();
        m_deleteCells = new List<BindCell>();

        m_InputObject = null;

        Selection.selectionChanged += selectionChange;
        EditorApplication.hierarchyWindowChanged += hierarchyWindowChanged;

    }

    private void selectionChange()
    {
        NewCurInfo(Selection.activeGameObject);
        m_Instance.Focus();
        EditorWindow.FocusWindowIfItsOpen(Type.GetType("UnityEditor.SceneHierarchyWindow,UnityEditor"));
    }

    private void hierarchyWindowChanged()
    {
        m_IsPrefab = m_Root != null && PrefabUtility.GetPrefabObject(m_Root) != null;

        if (m_IsPrefab)
        {
            RefreshUnableBindCell();
            selectionChange();
        }

    }

    #region GUI

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        {
            m_InputObject = EditorGUILayout.ObjectField(m_InputObject, typeof(GameObject));
            if (m_InputObject != null && m_InputObject != m_Root)
            {
                SetRoot((GameObject) m_InputObject);
            }

        }

        if (m_BindCells.Count > 0)
        {
            if (GUILayout.Button("清空无用引用", GUILayout.Width(100), GUILayout.Height(30)))
            {
                ClearAllUnableBindCell();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("生成C#脚本", GUILayout.Height(30)))
            {
                RefreshBindCell();
                OutPut();
            }
        }

        EditorGUILayout.EndHorizontal();

        if (m_Root && !m_IsPrefab)
        {
            EditorGUILayout.HelpBox("操作对象不是预制体", MessageType.Info);
            return;
        }

        m_deleteCells.Clear();

        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.BeginVertical("box", GUILayout.Width(120));
            OnGUI_Components();
            EditorGUILayout.EndVertical();

            m_ScrollViewPos = EditorGUILayout.BeginScrollView(m_ScrollViewPos);
            EditorGUILayout.BeginVertical("box", GUILayout.Width(694));
            OnGUI_ExportList();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
        EditorGUILayout.EndHorizontal();

        for (var index = 0; index < m_deleteCells.Count; index++)
        {
            DeleteBindCell(m_deleteCells[index]);
        }

    }

    private void OnGUI_Components()
    {
        EditorGUILayout.LabelField("当前对象");

        if (m_CurInfo == null)
        {
            return;
        }

        if (m_Root == null)
        {
            EditorGUILayout.HelpBox("请赋值预制体", MessageType.Info);
            return;
        }

        if (!m_CurInfo.m_Enable)
        {
            EditorGUILayout.HelpBox("当前选中对象超出可操作范围", MessageType.Info);
            return;
        }

        if (m_CurInfo.m_Target != m_Root)
        {
            GUI.backgroundColor = m_CurInfo.m_EnableGo ? Color.white : Color.gray;
            if (GUILayout.Button("GameObject"))
            {
                if (!m_CurInfo.m_EnableGo)
                {
                    RemoveBindCell(m_CurInfo.m_Target);
                    return;
                }

                AddBindCell(m_CurInfo.m_Target);
            }

            GUI.backgroundColor = m_CurInfo.m_EnableTra ? Color.white : Color.gray;
            if (GUILayout.Button("Transform"))
            {
                if (!m_CurInfo.m_EnableTra)
                {
                    RemoveBindCell(m_CurInfo.m_Target.transform);
                    return;
                }

                AddBindCell(m_CurInfo.m_Target.transform);
            }
        }

        for (var index = 0; index < m_CurInfo.m_Behaviours.Length; index++)
        {
            GUI.backgroundColor = m_CurInfo.m_UnableBehaviours[index] == 0 ? Color.white : Color.gray;
            if (GUILayout.Button(m_CurInfo.m_Behaviours[index].GetType().Name))
            {
                if (m_CurInfo.m_UnableBehaviours[index] == 0)
                {
                    AddBindCell(m_CurInfo.m_Behaviours[index]);
                }
                else
                {
                    RemoveBindCell(m_CurInfo.m_Behaviours[index]);
                }
            }
        }
        
        GUI.backgroundColor = Color.white;
        
    }

    private void OnGUI_ExportList()
    {
        EditorGUILayout.LabelField("记录控件");

        if (m_BindCells.Count == 0)
        {
            return;
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("对象",GUILayout.Width(140));
        GUILayout.Label("控件");
        GUILayout.Label("命名", GUILayout.Width(180));
        EditorGUILayout.EndHorizontal();
        
        int buttonInitPosY = 45;

        BindCell curCell;
        for (var index = 0; index < m_UnabelCells.Count; index++)
        {
            curCell = m_UnabelCells[index];

            GUI.backgroundColor = Color.red;
            EditorGUILayout.BeginHorizontal("helpbox");

            int[] eventX;
            OnGUI_EventCell(curCell.m_Event, buttonInitPosY, out eventX);

            EditorGUILayout.LabelField(curCell.m_Name, GUILayout.Width(130));
            EditorGUILayout.LabelField(curCell.GetComponentName());
            EditorGUILayout.LabelField(curCell.GetName(), GUILayout.Width(180));

            GUI.backgroundColor = Color.white;
            if (GUI.Button(new Rect(664, buttonInitPosY , 30, 22), "x"))
            {
                RemoveBindCell(curCell);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("path:root/"+curCell.m_Path);

            buttonInitPosY += 46;
        }

        GUI.backgroundColor = Color.white;

        for (int index = 0,cellcount = 0; index < m_BindCells.Count; index++)
        {
            curCell = m_BindCells[index];
            if (m_UnabelCells.Contains(curCell))
            {
                continue;
            }

            EditorGUILayout.BeginHorizontal("helpbox");

            int[] eventX = null;
            OnGUI_EventCell(curCell.m_Event, buttonInitPosY + 26 * cellcount,out eventX);
            OnGUI_BindCellBackground(eventX, buttonInitPosY + 26 * cellcount, curCell);

            EditorGUILayout.LabelField(curCell.m_Target.name, GUILayout.Width(130));
            EditorGUILayout.LabelField(curCell.GetComponentName());
            EditorGUILayout.LabelField(curCell.GetName(), GUILayout.Width(180));

            GUI.backgroundColor = Color.white;
            if (GUI.Button(new Rect(664, buttonInitPosY + 26 * cellcount, 30, 22), "x"))
            {
                RemoveBindCell(curCell);
            }

            EditorGUILayout.EndHorizontal();

            cellcount += 1;
        }


    }

    private void OnGUI_EventCell(EventCell target,int y,out int[] x)
    {
        x = new[] {0, 0};

        string[] componentEvents = target.ComponentEvents();

        if (componentEvents == null || componentEvents.Length == 0)
        {
            return;
        }

        System.Collections.Generic.List<string> options = new List<string>() {"Null"};
        options.AddRange(componentEvents);
        string[] optionsArray = options.ToArray();

        int[] unableEvents = target.GetUnableEvent();
        int[] enableEvents = target.GetEnableEvent();

        int curEventLength = unableEvents!=null ? unableEvents.Length + 1: 1;
        int[] curEvnets = new int[curEventLength];

        int xInit = 200;

        GUIStyle popup = EditorStyles.popup;
        popup.fixedHeight = 22;

        GUI.depth -= 1;
        if (unableEvents != null && unableEvents.Length > 0)
        {
            x[0] = xInit;
            for (var index = 0; index < unableEvents.Length; index++)
            {
                curEvnets[index] = EditorGUI.Popup(new Rect(x[0], y, 50, 40), unableEvents[index] + 1, optionsArray, popup);
                x[0] += 50;
            }
        }

        if (enableEvents != null && enableEvents.Length > 0)
        {
            x[0] = x[0] == 0 ? xInit : x[0];
            curEvnets[curEventLength - 1] = EditorGUI.Popup(new Rect(x[0], y, 50, 40), 0, optionsArray, popup);
            x[0] += 50;
        }

        x[1] = x[0];
        x[0] = xInit-8;



        target.Clear();
        for (var index = 0; index < curEvnets.Length; index++)
        {
            int curIndex = curEvnets[index] - 1;
            if (curIndex >= 0)
            {
                target.AddIndex(curIndex);
            }
        }

    }

    private void OnGUI_BindCellBackground(int[] eventX,int y,BindCell curCell)
    {
        GUI.backgroundColor = new Color(0.87f, 0.87f, 0.87f);
        if (eventX[0] == 0)
        {
            if (GUI.Button(new Rect(8, y, 656, 22), ""))
            {
            Ping(curCell.m_Target);
        }
    }
      else
      {
          if (GUI.Button(new Rect(8, y, eventX[0], 22), ""))
          {
              Ping(curCell.m_Target);
}
          if (GUI.Button(new Rect(eventX[1], y, 664 - eventX[1], 22), ""))
          {
              Ping(curCell.m_Target);
          }
      }
    }

    #endregion

    #region 获取

    private void GetCode(string filePath)
    {
        string menberRegex = "(?<name>[\\w\\W]+) = m_Transform.Find\\(\"(?<path>[\\w\\W]+)\"\\)";
        Dictionary<string,BindCell> cells = new Dictionary<string, BindCell>();
        using (StreamReader reader = File.OpenText(filePath))
        {
            string line;
            int state = 0;
            while ((line = reader.ReadLine()) != null)
            {
            
                if (line.Contains("Menber End")|| line.Contains("Init End") || line.Contains("EventListener End"))
                {
                    state = 0;
                }

                if (state == 1)
                {
                    string[] content = line.Split(' ');
                    if (content.Length < 7)
                    {
                        continue;
                    }
                    BindCell cell = new BindCell();
                    cell.m_BehaviourType = content[5];
                    cell.m_Name = content[6];
                    cells.Add(cell.m_Name,cell);
                }

                if (state == 2)
                {
                    Match match = Regex.Match(line, menberRegex);
                    if (match.Groups.Count > 1)
                    {
                        string name = match.Groups["name"].Value.Replace(" ", string.Empty);
                        if (!cells.ContainsKey(name))
                        {
                            continue;
                        }

                        BindCell cell = cells[name];
                        cell.m_Path = match.Groups["path"].Value;
                        cell.m_Name = cell.m_Name.Replace("m_", string.Empty).Replace(cell.m_BehaviourType, string.Empty);
                        Transform target = m_Root.transform.Find(cell.m_Path);
                        object component = null;
                        if (target != null)
                        {
                            cell.SetRoot(target.gameObject);
                            cell.SetBehaviour(null);

                            if (cell.m_BehaviourType == "GameObject")
                            {
                                cell.m_Type = BindCell.type.gameobject;
                                component = cell.m_Target;
                                AddComponent(cell.m_Target);
                            }
                            else if (cell.m_BehaviourType == "Transform")
                            {
                                cell.m_Type = BindCell.type.transform;

                                component = cell.m_Target.transform;
                                AddComponent(cell.m_Target.transform);
                            }
                            else
                            {
                                cell.m_Type = BindCell.type.behaviour;
                                component = cell.m_Target.GetComponent(cell.m_BehaviourType);
                                if (component != null)
                                {
                                    cell.SetBehaviour((MonoBehaviour)component);
                                    AddComponent(cell.m_Behaviour);
                                }
                            }
                        }

                        m_BindCells.Add(cell);
                        if (component != null)
                        {
                            m_DualCells.Add(component, cell);
                        }
                    }
                }

                if (state == 3)
                {
                    BindCell curCell;
                    for (var index = 0; index < m_BindCells.Count; index++)
                    {
                        curCell = m_BindCells[index];
                        if (curCell.m_Event.GetEventType() == EventCell.type.Other)
                        {
                            continue;
                        }

                        if (!line.Contains(curCell.GetName()))
                        {
                            continue;
                        }

                        int id = curCell.m_Event.GetComponentId(line);
                        if (id != -1)
                        {
                            curCell.m_Event.AddIndex(id);
                        }
                    }
                }

                if (line.Contains("Menber Start"))
                {
                    state = 1;
                }

                if (line.Contains("Init Start"))
                {
                    state = 2;
                }

                if (line.Contains("EventListener Start"))
                {
                    state = 3;
                }
            }
        }
        
        RefreshUnableBindCell();
    }

    private string GetTargetPath(Transform target, string path)
    {
        if (target == m_Root.transform)
        {
            path = string.Empty;
            return path;
        }

        if (string.IsNullOrEmpty(path))
        {
            path = target.name;
        }

        Transform parent = target.parent;
        if (parent == m_Root.transform)
        {
            return path;
        }
        else
        {
            path = string.Format("{0}/{1}", parent.name, path);
            return GetTargetPath(parent, path);
        }
    }

    private void Ping(GameObject target)
    {
        EditorGUIUtility.PingObject(target);
        Selection.activeGameObject = target;
    }

    #endregion

    #region 设置

    private void SetRoot(GameObject target)
    {
        m_BindCells.Clear();
        m_DualCells.Clear();
        m_UnabelCells.Clear();
        m_deleteCells.Clear();

        if (target == null)
        {
            m_Root = null;
            m_InputObject = null;
            return;
        }

        object prefabRoot = PrefabUtility.GetPrefabObject(target);
        if (prefabRoot == null)
        {
            Debug.LogError("对象并不是预制体！");
            m_InputObject = null;
            return;
        }

        m_IsPrefab = true;

        m_Root = PrefabUtility.FindPrefabRoot(target);
        m_InputObject = m_Root;

        if (File.Exists(TemplePath))
        {
            GetCode(TemplePath);
        }

        NewCurInfo(Selection.activeGameObject);
    }

    private void AddBindCell(object target)
    {
        BindCell cell = new BindCell();

        bool enabel = false;
        if (target is GameObject)
        {
            enabel = AddComponent((GameObject)target);
            cell.SetRoot((GameObject)target);
            cell.m_Type = BindCell.type.gameobject;
        }
        if (target is Transform)
        {
            enabel = AddComponent((Transform)target);
            cell.SetRoot(((Transform)target).gameObject);
            cell.m_Type = BindCell.type.transform;

        }
        if (target is MonoBehaviour)
        {
            enabel = AddComponent((MonoBehaviour)target);
            cell.SetRoot(((MonoBehaviour)target).gameObject);
            cell.m_Type = BindCell.type.behaviour;
            cell.SetBehaviour((MonoBehaviour)target);
        }

        if (!enabel)
        {
            return;
        }

        cell.m_Path = GetTargetPath(cell.m_Target.transform, cell.m_Path);

        m_BindCells.Add(cell);
        m_DualCells.Add(target, cell);

        NewCurInfo(m_CurInfo.m_Target != null ? m_CurInfo.m_Target : null);

    }

    private bool AddComponent(GameObject target)
    {
        if (!m_RecordGos.Contains(target))
        {
            m_RecordGos.Add(target);
            return true;
        }

        return false;
    }

    private bool AddComponent(Transform target)
    {
        if (!m_RecordTrans.Contains(target))
        {
            m_RecordTrans.Add(target);
            return true;
        }

        return false;
    }

    private bool AddComponent(MonoBehaviour target)
    {
        if (!m_RecordBehaviours.Contains(target))
        {
            m_RecordBehaviours.Add(target);
            return true;
        }

        return false;
    }

    private void RemoveBindCell(BindCell target)
    {
        if (!m_deleteCells.Contains(target))
        {
            m_deleteCells.Add(target);
        }
    }

    private void RemoveBindCell(object target)
    {
        if (m_DualCells.ContainsKey(target))
        {
            RemoveBindCell(m_DualCells[target]);
        }
    }

    private void DeleteBindCell(BindCell target)
    {
        if (m_BindCells.Contains(target))
        {
            m_BindCells.Remove(target);
            m_UnabelCells.Remove(target);
            object bindBehaviour = target.GetComponent();
            if (bindBehaviour!=null && m_DualCells.ContainsKey(bindBehaviour))
            {
                m_DualCells.Remove(bindBehaviour);
            }
            if (bindBehaviour is GameObject)
            {
                RemoveComponent((GameObject) bindBehaviour);
            }

            if (bindBehaviour is Transform)
            {
                RemoveComponent((Transform) bindBehaviour);
            }

            if (bindBehaviour is MonoBehaviour)
            {
                RemoveComponent((MonoBehaviour) bindBehaviour);
            }
        }

        NewCurInfo(m_CurInfo.m_Target != null ? m_CurInfo.m_Target : null);
    }

    private void RemoveComponent(GameObject target)
    {
        if (m_RecordGos.Contains(target))
        {
            m_RecordGos.Remove(target);
        }
    }
    private void RemoveComponent(Transform target)
    {
        if (m_RecordTrans.Contains(target))
        {
            m_RecordTrans.Remove(target);
        }
    }

    private void RemoveComponent(MonoBehaviour target)
    {
        if (m_RecordBehaviours.Contains(target))
        {
            m_RecordBehaviours.Remove(target);
        }
    }

    private void NewCurInfo( GameObject target)
    {
        if (target == null)
        {
            m_CurInfo = null;
            return;
        }

        m_CurInfo = new SelectInfo();
        m_CurInfo.m_Target = target;
        m_CurInfo.m_Enable = true;

        if (m_Root == null)
        {
            m_CurInfo.m_Enable = false;
        }
        GameObject prefabRoot = PrefabUtility.FindPrefabRoot(target);
        if (prefabRoot != m_Root)
        {
            m_CurInfo.m_Enable = false;
        }
        
        if (!m_CurInfo.m_Enable)
        {
            return;
        }

        m_CurInfo.m_EnableGo = !m_RecordGos.Contains(m_CurInfo.m_Target);
        m_CurInfo.m_EnableTra = !m_RecordTrans.Contains(m_CurInfo.m_Target.transform);

        m_CurInfo.m_Behaviours = m_CurInfo.m_Target.GetComponents<MonoBehaviour>();
        m_CurInfo.m_UnableBehaviours = m_CurInfo.m_Behaviours != null ? new int[m_CurInfo.m_Behaviours.Length] : null; 
        for (var index = 0; index < m_CurInfo.m_Behaviours.Length; index++)
        {
            if (m_RecordBehaviours.Contains(m_CurInfo.m_Behaviours[index]))
            {
                m_CurInfo.m_UnableBehaviours[index] = 1;
            }
            else
            {
                m_CurInfo.m_UnableBehaviours[index] = 0;
            }
        }

        m_CurInfo.m_Path = GetTargetPath(m_CurInfo.m_Target.transform, m_CurInfo.m_Path);
    }



    private void RefreshUnableBindCell()
    {
        m_UnabelCells.Clear();

        BindCell curCell;
        for (var index = 0; index < m_BindCells.Count; index++)
        {
            curCell = m_BindCells[index];
            if (!curCell.IsAble())
            {
                m_UnabelCells.Add(curCell);
            }
        }
    }

    private void RefreshBindCell()
    {
        BindCell curCell;

        for (var index = 0; index < m_BindCells.Count; index++)
        {
            curCell = m_BindCells[index];
            if (curCell.IsAble())
            {
                curCell.SetRoot(curCell.m_Target);
                curCell.SetBehaviour(curCell.m_Behaviour);
                curCell.m_Path = string.Empty;
                curCell.m_Path = GetTargetPath(curCell.m_Target.transform, curCell.m_Path);
            }
        }
    }

    private void ClearAllUnableBindCell()
    {
        RefreshUnableBindCell();
        
        for (var index = 0; index < m_UnabelCells.Count; index++)
        {
            m_BindCells.Remove(m_UnabelCells[index]);
        }

        m_UnabelCells.Clear();
    }

    #endregion


    #region 输出

    private void OutPut()
    {
        if (!File.Exists(TemplePath))
        {
            Debug.Log("找不到："+ TemplePath);
            return;
        }
        StringBuilder codeStringBuilder = new StringBuilder();

        StringBuilder memberStrBuilder = new StringBuilder();
        StringBuilder initStrBuilder = new StringBuilder();
        StringBuilder listenerBuilder = new StringBuilder();

        BindCell curCell;
        for (var index = 0; index < m_BindCells.Count; index++)
        {
            curCell = m_BindCells[index];

            memberStrBuilder.Append(curCell.GetMenberLine());
            initStrBuilder.Append(curCell.GetInitLine());
            listenerBuilder.Append(curCell.GetListenerLine());
        }

        List<string> existListenerFuns = new List<string>();
        string listenerFunRegex = "void (?<funName>[\\w\\W]+)\\(";

        using (StreamReader reader = File.OpenText(TemplePath))
        {
            bool replace = false;
            bool check = false;

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Contains("Menber End"))
                {
                    replace = false;
                    codeStringBuilder.Append(memberStrBuilder);
                }

                if (line.Contains("Init End"))
                {
                    replace = false;
                    codeStringBuilder.Append(initStrBuilder);
                }

                if (line.Contains("EventListener End"))
                {
                    replace = false;
                    codeStringBuilder.Append(listenerBuilder);
                }

                if (line.Contains("EventListenerFun End"))
                {
                    check = false;

                    BindCell cell;
                    for (var index = 0; index < m_BindCells.Count; index++)
                    {
                        cell = m_BindCells[index];
                        codeStringBuilder.Append(cell.GetListenerFunLine(existListenerFuns));
                    }
                }

                if (!replace)
                {
                    codeStringBuilder.Append(string.IsNullOrEmpty(line) || line == "\n" ? "\n" : line + "\n");
                }


                if (line.Contains("Menber Start") || line.Contains("Init Start")  || line.Contains("EventListener Start"))
                {
                    replace = true;
                }

                if (line.Contains("EventListenerFun Start"))
                {
                    check = true;
                }

                if (check)
                {
                    if (line.Contains("void"))
                    {
                        Match match = Regex.Match(line, listenerFunRegex);
                        if (match.Groups.Count > 1)
                        {
                            existListenerFuns.Add(match.Groups["funName"].Value);
                        }
                    }
                }
            }

        }

        if (codeStringBuilder.Length > 0)
        {
            StreamWriter writter = File.CreateText(TemplePath);
            writter.Write(codeStringBuilder.ToString());
            writter.Close();
        }
        
        Debug.Log("生成完毕");
    }

    #endregion

    private class SelectInfo
    {
        public GameObject m_Target;
        public bool m_Enable;
        public MonoBehaviour[] m_Behaviours = null;
        public int[] m_UnableBehaviours = null;
        public bool m_EnableGo;
        public bool m_EnableTra;
        public string m_Path;

        public SelectInfo()
        {
            m_Behaviours = null;
            m_UnableBehaviours = null;
            m_Path = string.Empty;
        }
    }

    private class BindCell
    {
        public GameObject m_Target;
        public MonoBehaviour m_Behaviour = null;
        public type m_Type;

        public enum type
        {
            gameobject,
            transform,
            behaviour
        }

        public string m_Path = null;
        public string m_Name = null;
        public string m_BehaviourType = null;

        public EventCell m_Event = null;

        public BindCell()
        {
            m_Event = new EventCell();
        }


        public bool IsAble()
        {
            return m_Target != null && (m_Type != type.behaviour || m_Behaviour != null);
        }

        public void SetRoot(GameObject root)
        {
            m_Target = root;
            if (m_Target != null)
            {
                m_Name = m_Target.name;
            }
        }

        public void SetBehaviour(MonoBehaviour behaviour)
        {
            m_Behaviour = behaviour;
            if (m_Behaviour != null)
            {
                m_BehaviourType = m_Behaviour.GetType().Name;
            }

            m_Event.SetEventType(behaviour);
        }

        public string GetName()
        {
            if (IsAble())
            {
                return m_Target.name.Replace(" ", string.Empty) + GetComponentName();
            }
            else
            {
                return m_Name + m_BehaviourType;
            }
        }

        public string GetComponentName()
        {
            if (m_Type == type.gameobject)
            {
                return "GameObject";
            }
            else if (m_Type == type.transform)
            {
                return "Transform";
            }
            else
            {
                if (IsAble())
                {
                    return m_Behaviour.GetType().Name;
                }
                else
                {
                    return m_BehaviourType;
                }
            }
        }

        public object GetComponent()
        {
            if (m_Type == type.gameobject)
            {
                return m_Target;
            }
            else if (m_Type == type.transform)
            {
                return m_Target.transform;
            }
            else
            {
                return m_Behaviour;
            }
        }

        public string GetMenberLine()
        {
            return string.Format("    private {1} m_{0} = null;\n", GetName(), GetComponentName());
        }

        public string GetInitLine()
        {
            if (m_Type == BindCell.type.gameobject)
            {
                return string.Format("        m_{0} = m_Transform.Find(\"{1}\").gameObject;\n", GetName(), m_Path);
            }
            else if (m_Type == BindCell.type.transform)
            {
                return string.Format("        m_{0} = m_Transform.Find(\"{1}\");\n", GetName(), m_Path);
            }
            else
            {
                if (string.IsNullOrEmpty(m_Path))
                {
                    return string.Format("        m_{0} = m_Transform.GetComponent<{1}>();\n", GetName(), GetComponentName());
                }
                else
                {
                    return string.Format("        m_{0} = m_Transform.Find(\"{1}\").GetComponent<{2}>();\n", GetName(), m_Path, GetComponentName());
                }
            }
        }
        

        public StringBuilder GetListenerLine()
        {
            StringBuilder listenerBuilder = new StringBuilder();
            int[] selectIds = m_Event.GetUnableEvent();
            if (selectIds != null && selectIds.Length > 0)
            {
                for (var index = 0; index < selectIds.Length; index++)
                {
                    listenerBuilder.Append(m_Event.GetListenerLine(this,index));
                }
            }

            return listenerBuilder;
        }

        public StringBuilder GetListenerFunLine(List<string> existFunNames)
        {
            StringBuilder listenerBuilder = new StringBuilder();

            int[] selectIds = m_Event.GetUnableEvent();

            string funName;
            if (selectIds != null && selectIds.Length > 0)
            {
                for (var index = 0; index < selectIds.Length; index++)
                {
                    funName = m_Event.GetFuncName(this, index);
                    if (existFunNames.Contains(funName))
                    {
                        continue;
                    }
                    listenerBuilder.Append(m_Event.GetListenerFunLine(this, index));
                }
            }

            return listenerBuilder;
        }
    }

    private class EventCell
    {
        private List<int> m_EventIds = null;
        private type m_Type;

        public enum type
        {
            Other,
            Button,
            Slider,
            Toggle
        }

        public EventCell()
        {
            m_Type = type.Other;
            m_EventIds = new List<int>();
        }

        public type GetEventType()
        {
            return m_Type;
        }

        public void SetEventType(MonoBehaviour behaviour)
        {
            if (behaviour is Button)
            {
                m_Type = type.Button;
            }
            else if (behaviour is Slider)
            {
                m_Type = type.Slider;
            }
            else if (behaviour is Toggle)
            {
                m_Type = type.Toggle;
            }
            else
            {
                m_Type = type.Other;
            }
        }

        public void Clear()
        {
            m_EventIds.Clear();
        }

        public void AddIndex(int index)
        {
            if (!m_EventIds.Contains(index))
            {
                m_EventIds.Add(index);
            }
        }

        public int[] GetUnableEvent()
        {
            if (m_EventIds.Count == 0)
            {
                return null;
            }

            return m_EventIds.ToArray();
        }

        public int[] GetEnableEvent()
        {
            int[] events = null;

            string[] componentEvents = ComponentEvents();

            if (componentEvents != null)
            {
                int length = componentEvents.Length - m_EventIds.Count;
                if (length != 0)
                {
                    events = new int[length];
                    for (int index = 0, eventIndex = 0; index < componentEvents.Length; index++)
                    {
                        if (!m_EventIds.Contains(index))
                        {
                            events[eventIndex] = index;
                            eventIndex += 1;
                        }
                    }
                }
            }

            return events;
        }

        public string[] ComponentEvents()
        {
            if (m_Type == type.Button)
            {
                return ButtonEvent;
            }
            else if (m_Type == type.Slider)
            {
                return SliderEvent;
            }
            else if (m_Type == type.Toggle)
            {
                return ToggleEvent;
            }

            return null;
        }

        public string GetFuncName(BindCell target, int id)
        {
            string[] componentEvents = ComponentEvents();
            return target.GetName() + componentEvents[id];
        }

        public string GetListenerLine(BindCell target, int index)
        {
            string[] componentEvents = ComponentEvents();

            int id = m_EventIds[index];
            if (m_Type == type.Button)
            {
                if (id == 0) //onclick
                {
                    return string.Format("        m_{0}.{1}.AddListener({0}{1});\n", target.GetName(), componentEvents[id]);
                }
                else
                {
                    return string.Format("        EventTriggerListener.Get(m_{0}.gameObject).{1} = {0}{1};\n", target.GetName(), componentEvents[id]);
                }
            }
            else if (m_Type == type.Slider)
            {
                return string.Format("        m_{0}.{1}.AddListener({0}{1});\n", target.GetName(), componentEvents[id]);
            }
            else if (m_Type == type.Toggle)
            {
                return string.Format("        m_{0}.{1}.AddListener({0}{1});\n", target.GetName(), componentEvents[id]);
            }

            return null;
        }

        public string GetListenerFunLine(BindCell target, int index)
        {
            int id = m_EventIds[index];
            if (m_Type == type.Button)
            {
                switch (id)
                {
                    case 0://click
                        return string.Format("    private void {0}()\n    {{\n\n    }}\n\n", GetFuncName(target, id));
                    case 1://Down
                    case 2://Up
                    case 3://BeginDrag
                    case 4://Drag
                    case 5://EndDrag
                        return string.Format("    private void {0}(GameObject obj, PointerEventData eventData)\n    {{\n\n    }}\n\n", GetFuncName(target, id));
                }
            }
            else if (m_Type == type.Slider)
            {
                return string.Format("    private void {0}(float value)\n    {{\n\n    }}\n\n", GetFuncName(target, id));
            }
            else if (m_Type == type.Toggle)
            {
                return string.Format("    private void {0}(bool value)\n    {{\n\n    }}\n\n", GetFuncName(target, id));
            }

            return null;
        }

        public int GetComponentId(string line)
        {
            string[] componentEvents = ComponentEvents();
            if (m_Type == type.Button)
            {
                if (line.Contains("AddListener"))
                {
                    return 0;//click
                }

                string regex = "\\)\\.(?<name>[\\w\\W]+) =";
                Match match = Regex.Match(line, regex);
                if (match.Groups.Count > 1)
                {
                    string name = match.Groups["name"].Value;
                    for (var index = 0; index < componentEvents.Length; index++)
                    {
                        if (componentEvents[index] == name)
                        {
                            return index;
                        }
                    }
                }
            }
            else if (m_Type == type.Slider)
            {
                string regex = "\\.(?<name>[\\w\\W]+)\\.AddListener";
                Match match = Regex.Match(line, regex);
                if (match.Groups.Count > 1)
                {
                    string name = match.Groups["name"].Value;
                    for (var index = 0; index < componentEvents.Length; index++)
                    {
                        if (componentEvents[index] == name)
                        {
                            return index;
                        }
                    }
                }
            }
            else if (m_Type == type.Toggle)
            {
                string regex = "\\.(?<name>[\\w\\W]+)\\.AddListener";
                Match match = Regex.Match(line, regex);
                if (match.Groups.Count > 1)
                {
                    string name = match.Groups["name"].Value;
                    for (var index = 0; index < componentEvents.Length; index++)
                    {
                        if (componentEvents[index] == name)
                        {
                            return index;
                        }
                    }
                }
            }

            return -1;
        }
    }

}
