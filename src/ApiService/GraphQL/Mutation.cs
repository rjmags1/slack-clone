using GraphQL;
using GraphQL.Types;
using Common.SlackCloneGraphQL.Types;

namespace SlackCloneGraphQL;

public class SlackCloneMutation : ObjectGraphType
{
    public SlackCloneMutation(SlackCloneData data)
    {
        Name = "Mutation";
        Field<WorkspaceType>("createWorkspace")
            .Argument<NonNullGraphType<WorkspaceInputType>>("workspace")
            .Argument<NonNullGraphType<IdGraphType>>("creatorId")
            .ResolveAsync(async context =>
            {
                var workspaceInfo = context.GetArgument<WorkspaceInput>(
                    "workspace"
                );
                var creatorId = context.GetArgument<Guid>("creatorId");
                return await data.CreateWorkspace(workspaceInfo, creatorId);
            });
        Field<FileType>("createAvatar")
            .Argument<NonNullGraphType<FileInputType>>("file")
            .ResolveAsync(async context =>
            {
                var fileInfo = context.GetArgument<FileInput>("file");
                return await data.CreateAvatar(fileInfo);
            });
    }
}
