using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;

namespace PersistenceService.Stores;

public class DirectMessageGroupStore : Store
{
    public DirectMessageGroupStore(ApplicationDbContext context)
        : base(context) { }

    public async Task<List<DirectMessageGroup>> InsertDirectMessageGroups(
        List<DirectMessageGroup> directMessageGroups,
        List<List<Guid>> members
    )
    {
        using var transaction = _context.Database.BeginTransaction();
        try
        {
            _context.AddRange(directMessageGroups);
            await _context.SaveChangesAsync();
            foreach (
                (
                    DirectMessageGroup dmg,
                    List<Guid> groupMembers
                ) in directMessageGroups.Zip(members)
            )
            {
                foreach (Guid memberId in groupMembers)
                {
                    _context.Add(
                        new DirectMessageGroupMember
                        {
                            DirectMessageGroup = dmg,
                            UserId = memberId
                        }
                    );
                }
            }
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(e.Message);
        }

        return directMessageGroups;
    }

    public async Task<List<DirectMessageGroup>> InsertTestDirectMessageGroups(
        int numTestDirectMessageGroups
    )
    {
        string emailPrefix = "test-email-dmg@test.com";
        string usernamePrefix = "dgcreator-uname";
        List<List<User>> testMembers = new List<List<User>>();
        List<DirectMessageGroup> testGroups = new List<DirectMessageGroup>();
        Workspace directMessageGroupWorkspace = new Workspace
        {
            Description = "test-description",
            Name = "test-workspace-direct-message-group-name"
        };
        for (int i = 0; i < numTestDirectMessageGroups; i++)
        {
            string email1 =
                emailPrefix + DirectMessageGroupStore.GenerateRandomString(15);
            string email2 =
                emailPrefix + DirectMessageGroupStore.GenerateRandomString(15);
            string username1 =
                usernamePrefix
                + DirectMessageGroupStore.GenerateRandomString(10);
            string username2 =
                usernamePrefix
                + DirectMessageGroupStore.GenerateRandomString(10);
            User dmgUser1 = new User
            {
                FirstName = "test-ccreator-fname",
                LastName = "test-dmg-creator-lname",
                Timezone = UserStore.timezones[0].Id,
                UserName = username1,
                Email = email1,
                NormalizedEmail = email1.ToUpper(),
                NormalizedUserName = username1.ToUpper(),
                PhoneNumber = "1-234-456-7890",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                    UserStore.testPassword,
                    4
                ),
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
            };
            User dmgUser2 = new User
            {
                FirstName = "test-ccreator-fname",
                LastName = "test-dmg-creator-lname",
                Timezone = UserStore.timezones[0].Id,
                UserName = username2,
                Email = email2,
                NormalizedEmail = email2.ToUpper(),
                NormalizedUserName = username2.ToUpper(),
                PhoneNumber = "1-234-456-7890",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                    UserStore.testPassword,
                    4
                ),
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
            };
            testGroups.Add(
                new DirectMessageGroup
                {
                    Workspace = directMessageGroupWorkspace,
                }
            );
            testMembers.Add(new List<User> { dmgUser1, dmgUser2 });
        }
        using var transaction = _context.Database.BeginTransaction();
        try
        {
            _context.AddRange(testGroups);
            _context.AddRange(testMembers.SelectMany(pair => pair).ToList());
            await _context.SaveChangesAsync();
            foreach (
                (
                    DirectMessageGroup dmg,
                    List<User> groupMembers
                ) in testGroups.Zip(testMembers)
            )
            {
                foreach (User member in groupMembers)
                {
                    _context.Add(
                        new DirectMessageGroupMember
                        {
                            DirectMessageGroup = dmg,
                            User = member
                        }
                    );
                }
            }
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(e.Message);
        }

        return testGroups;
    }
}
