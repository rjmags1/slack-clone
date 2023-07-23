using GraphQL.Types;

namespace SlackCloneGraphQL.Types;

public class WorkspaceType : ObjectGraphType<Workspace>
{
    public WorkspaceType(SlackCloneData data)
    {
        Name = "Workspace";
        //
    }
}

public class Workspace { }
