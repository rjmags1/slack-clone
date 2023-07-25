using PersistenceService.Stores;
using Models = PersistenceService.Models;
using SlackCloneGraphQL.Types;

namespace SlackCloneGraphQL;

public class SlackCloneData
{
    private IServiceProvider _provider { get; set; }

    public SlackCloneData(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task<User> GetUserById(
        Guid userId,
        IEnumerable<string> requestedFields
    )
    {
        using var scope = _provider.CreateScope();
        UserStore userStore =
            scope.ServiceProvider.GetRequiredService<UserStore>();
        Models.User dbUser =
            await userStore.FindByIdAsyncWithEagerNavPropLoading(
                userId,
                requestedFields
            );

        return ModelToObjectConverters.ConvertUser(dbUser, requestedFields);
    }
}
