
public class SceneEditorMenuMgr : EditorMenuRegisterMgr
{

    private SceneEditorMenuMgr() {}

    private static SceneEditorMenuMgr s_Instance;

    public static SceneEditorMenuMgr Instance()
    {
        if (s_Instance == null)
        {
            s_Instance = new SceneEditorMenuMgr();
        }

        return s_Instance;
    }

    protected override void DestoryEvent()
    {
        s_Instance = null;
    }

    protected override void ResisterComponent()
    {
        //对齐
        AddComponent(new SceneEditorMuneAlignComponent());

        //锁
        AddComponent(new SceneEditorMuneLockComponent());

    }

}
