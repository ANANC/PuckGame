using System;

public class HierarchyMarkRegister
{
    public readonly static Type[] Registers =
    {
        //锁
        typeof(HierarchyLockMark),
        //Bind
        typeof(HierarchyGUIBindTargetMark),
        //Event
        typeof(HierarchyGUIEventMark),
        //根显示
        typeof(HierarchyRootMark),
    };
}
