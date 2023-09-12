using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersistenceService.Models;

[Index(nameof(WorkspaceId), nameof(Name), IsUnique = true)]
[Index(nameof(Private))]
public class Channel
{
    public Guid Id { get; set; }

    public bool AllowThreads { get; set; } = true;

    [DeleteBehavior(DeleteBehavior.SetNull)]
    public File? Avatar { get; set; }

    [ForeignKey(nameof(Avatar))]
    public Guid? AvatarId { get; set; }

    public int AllowedPostersMask { get; set; } = 1;

    [Column(TypeName = "timestamp")]
    public DateTime CreatedAt { get; set; }

#pragma warning disable CS8618
    [DeleteBehavior(DeleteBehavior.SetNull)]
    public User? CreatedBy { get; set; }

    [ForeignKey(nameof(CreatedBy))]
    public Guid? CreatedById { get; set; }

    [ConcurrencyCheck]
    public Guid ConcurrencyStamp { get; set; }

    [MaxLength(120)]
    public string? Description { get; set; }
#pragma warning restore CS8618

    public ICollection<ChannelMember> ChannelMembers { get; } =
        new List<ChannelMember>();

    public ICollection<ChannelMessage> ChannelMessages { get; } =
        new List<ChannelMessage>();

#pragma warning disable CS8618
    [MaxLength(40)]
    public string Name { get; set; }
#pragma warning restore CS8618

    public int NumMembers { get; set; }

    public bool Private { get; set; } = false;

#pragma warning disable CS8618
    [MaxLength(40)]
    public string? Topic { get; set; }

    [DeleteBehavior(DeleteBehavior.Cascade)]
    public Workspace Workspace { get; set; }
#pragma warning restore CS8618

    [ForeignKey(nameof(Workspace))]
    public Guid WorkspaceId { get; set; }
}
