using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersistenceService.Models;

[Index(nameof(WorkspaceId))]
public class DirectMessageGroup
{
    public Guid Id { get; set; }

#pragma warning disable CS8618
    [ConcurrencyCheck]
    public Guid ConcurrencyStamp { get; set; }
#pragma warning restore CS8618

    [Column(TypeName = "timestamp")]
    public DateTime CreatedAt { get; set; }

    public ICollection<DirectMessageGroupMember> DirectMessageGroupMembers { get; } =
        new List<DirectMessageGroupMember>();

    public ICollection<DirectMessage> DirectMessages { get; } =
        new List<DirectMessage>();

    public ICollection<File> Files { get; } = new List<File>();

    public int Size { get; set; } = 2;

#pragma warning disable CS8618
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public Workspace Workspace { get; set; }
#pragma warning restore CS8618

    [ForeignKey(nameof(Workspace))]
    public Guid WorkspaceId { get; set; }
}
