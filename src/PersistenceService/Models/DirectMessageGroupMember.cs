using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace PersistenceService.Models;

[Index(nameof(UserId), nameof(DirectMessageGroupId), IsUnique = true)]
public class DirectMessageGroupMember
{
    public Guid Id { get; set; }

#pragma warning disable CS8618
    [JsonIgnore]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public DirectMessageGroup DirectMessageGroup { get; set; }
#pragma warning restore CS8618

    [ForeignKey(nameof(DirectMessageGroup))]
    public Guid DirectMessageGroupId { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime JoinedAt { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime? LastViewedAt { get; set; }

    public bool Starred { get; set; } = false;

#pragma warning disable CS8618
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public User User { get; set; }

    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    [DeleteBehavior(DeleteBehavior.Cascade)]
    public Workspace Workspace { get; set; }

    [ForeignKey(nameof(Workspace))]
    public Guid WorkspaceId { get; set; }
#pragma warning restore CS8618
}
