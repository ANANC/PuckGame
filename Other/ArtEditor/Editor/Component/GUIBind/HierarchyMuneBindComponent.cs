public class HierarchyMuneBindComponent : SceneEditorMuneComponent
{
    protected override string SetComponentName()
    {
        return "";
    }

    protected override SceneEditorMenuComponentItem CreateItem()
    {
        return new HierarchyMenuBindComponentItem();
    }
    
}

