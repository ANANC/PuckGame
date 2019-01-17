using System;
using System.Collections.Generic;
using UnityEditor;

public abstract class SceneEditorMenuComponentItem
{
    private Dictionary<string, MenuItem> m_MenuItems = null;
    private bool m_DynamicMenu = false;
    private List<int> m_SeparatorValues = null;

    protected SceneEditorMenuComponentItem()
    {
        m_MenuItems = new Dictionary<string, MenuItem>();

        OnInit();
    }

    protected abstract void OnInit();

    protected virtual void OnDynamicMenuInit() {}

    /// <summary>
    /// 设置动态目录
    /// </summary>
    protected void SetDynamicMenu(bool dynamic = false)
    {
        m_DynamicMenu = dynamic;
    }

    protected virtual bool GeneralCondition()
    {
        return true;
    }

    public void Achieve(string menuComponentName)
    {
        if (m_DynamicMenu)
        {
            m_MenuItems.Clear();
            OnDynamicMenuInit();
        }

        bool generalCondition = GeneralCondition();
        bool hasSeparator = m_SeparatorValues != null && m_SeparatorValues.Count > 0;

        int itemIndex = 0;

        foreach (var menuItem in m_MenuItems)
        {
            if (hasSeparator && m_SeparatorValues.Contains(itemIndex))
            {
                AddSeparator(menuComponentName);
            }

            bool enoughCondition = generalCondition && FunctionConditon(menuItem.Key);

            AddMenuItem(menuComponentName, menuItem.Value.m_MenuItemName, enoughCondition, ItemOnClick, menuItem.Key);

            itemIndex += 1;
        }
    }

    private bool FunctionConditon(string menuItemName)
    {
        if (m_MenuItems[menuItemName].m_Condition != null)
        {
            return m_MenuItems[menuItemName].m_Condition();
        }

        return true;
    }

    #region OnClickCallback

    private void ItemOnClick(object userData)
    {
        string funcName = userData.ToString();
        ItemFunctionCallback(funcName);
    }

    private void ItemFunctionCallback(string funcName)
    {
        MenuItem menuItem;
        if (m_MenuItems.TryGetValue(funcName, out menuItem) == false)
        {
            return;
        }

        OnStart();

        if (menuItem.m_MenuFunction != null)
        {
            menuItem.m_MenuFunction();
        }

        if (menuItem.m_MenuFunction2 != null)
        {
            menuItem.m_MenuFunction2(menuItem.m_UserData);
        }

        OnEnd();
    }

    protected virtual void OnStart() {}

    protected virtual void OnEnd() {}

    #endregion

    #region AddFuncItem

    protected void AddAchieveItem(string menuName, GenericMenu.MenuFunction function, Func<bool> able = null)
    {
        m_MenuItems.Add(menuName, new MenuItem(menuName, function, able));
    }

    protected void AddAchieveItem(string menuName, GenericMenu.MenuFunction function,bool able)
    {
        m_MenuItems.Add(menuName, new MenuItem(menuName, function, ()=>able));
    }

    protected void AddAchieveItem(string menuName, GenericMenu.MenuFunction2 function, object userData, Func<bool> able = null)
    {
        m_MenuItems.Add(menuName, new MenuItem(menuName, function, userData, able));
    }

    protected void AddAchieveItem(string menuName, GenericMenu.MenuFunction2 function, object userData, bool able)
    {
        m_MenuItems.Add(menuName, new MenuItem(menuName, function, userData, ()=>able));
    }

    protected void AddSeparator()
    {
        if (m_SeparatorValues == null)
        {
            m_SeparatorValues = new List<int>();
        }

        m_SeparatorValues.Add(m_MenuItems.Count);
    }

    #endregion

    #region AddMenuItem

    private void AddMenuItem(string componentName, string menuItemName, bool able, GenericMenu.MenuFunction callback)
    {
        string menuPath;
        if (string.IsNullOrEmpty(componentName))
        {
            menuPath = menuItemName;
        }
        else
        {
            menuPath = string.Format("{0}/{1}", componentName, menuItemName);
        }

        SceneViewMenuTool.Instance().AddMenuItem(menuPath, able, callback);
    }

    private void AddMenuItem(string componentName, string menuItemName, bool able, GenericMenu.MenuFunction2 callback, object userData)
    {
        string menuPath;
        if (string.IsNullOrEmpty(componentName))
        {
            menuPath = menuItemName;
        }
        else
        {
            menuPath = string.Format("{0}/{1}", componentName, menuItemName);
        }

        SceneViewMenuTool.Instance().AddMenuItem(menuPath, able, callback, userData);
   }

    private void AddSeparator(string path)
    {
        SceneViewMenuTool.Instance().AddSeparator(string.Format("{0}/", path));
    }

    #endregion


    private class MenuItem
    {
        public string m_MenuItemName;
        public Func<bool> m_Condition;
        public GenericMenu.MenuFunction m_MenuFunction;
        public GenericMenu.MenuFunction2 m_MenuFunction2;
        public object m_UserData;

        public MenuItem(string menuItemName, GenericMenu.MenuFunction function, Func<bool> condition = null)
        {
            m_MenuItemName = menuItemName;
            m_Condition = condition;
            m_MenuFunction = function;
            m_MenuFunction2 = null;
            m_UserData = null;
        }

        public MenuItem(string menuItemName, GenericMenu.MenuFunction2 function2, object userData, Func<bool> condition = null)
        {
            m_MenuItemName = menuItemName;
            m_Condition = condition;
            m_MenuFunction = null;
            m_MenuFunction2 = function2;
            m_UserData = userData;
        }

    }
}
