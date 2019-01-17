

public class SceneEditorMuneAlignComponent : SceneEditorMuneComponent
{
    protected override string SetComponentName()
    {
        return "对齐";
    }

    protected override SceneEditorMenuComponentItem CreateItem()
    {
        return new SceneEdiotrMenuAlignComponentItem();
    }
    
}

