using System.Collections.Generic;

public class SceneEditorMuneLockComponent : SceneEditorMuneComponent
{
    protected override string SetComponentName()
    {
        return string.Empty;
    }

    protected override SceneEditorMenuComponentItem CreateItem()
    {
        return new SceneEdiotrMenuLockComponentItem();
    }

    protected override List<SceneEditorMuneComponent> CreatComponent()
    {
        return new List<SceneEditorMuneComponent>()
        {
            new SceneEditorMuneLockMembersComponent(),
        };
    }

    protected override int[] SetSpecification()
    {
        return new[] {1, 0, 0, 1};
    }


    public class SceneEditorMuneLockMembersComponent : SceneEditorMuneComponent
    {
        protected override string SetComponentName()
        {
            return "解锁";
        }

        protected override SceneEditorMenuComponentItem CreateItem()
        {
            return new SceneEdiotrMenuLockMembersComponentItem();
        }

    }
}

