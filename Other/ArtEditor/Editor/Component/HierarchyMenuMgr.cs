
public class HierarchyMenuMgr : EditorMenuRegisterMgr
{

    private HierarchyMenuMgr() {}

    private static HierarchyMenuMgr s_Instance;

    public static HierarchyMenuMgr Instance()
    {
        if (s_Instance == null)
        {
            s_Instance = new HierarchyMenuMgr();
        }

        return s_Instance;
    }

    protected override void DestoryEvent()
    {
        s_Instance = null;
    }

    protected override void ResisterComponent()
    {
       AddComponent(new HierarchyMuneBindComponent());
    }

}
