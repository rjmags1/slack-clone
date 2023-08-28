using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersistenceService.Models;

[Index(nameof(UserId))]
[Index(nameof(WorkspaceId))]
public class Star
{
    public Guid Id { get; set; }

#pragma warning disable CS8618
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public User User { get; set; }

    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    [DeleteBehavior(DeleteBehavior.Cascade)]
    public Channel? Channel { get; set; }

    [ForeignKey(nameof(Channel))]
    public Guid? ChannelId { get; set; }

    [DeleteBehavior(DeleteBehavior.Cascade)]
    public DirectMessageGroup? DirectMessageGroup { get; set; }

    [ForeignKey(nameof(DirectMessageGroup))]
    public Guid? DirectMessageGroupId { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime CreatedAt { get; set; }

    [DeleteBehavior(DeleteBehavior.Cascade)]
    public Workspace Workspace { get; set; }
#pragma warning restore CS8618
    [ForeignKey(nameof(Workspace))]
    public Guid WorkspaceId { get; set; }
}
