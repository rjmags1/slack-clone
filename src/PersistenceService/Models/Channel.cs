using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersistenceService.Models;

[Index(nameof(WorkspaceId), nameof(Name), IsUnique = true)]
[Index(nameof(Private))]
public class Channel
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [DefaultValue(true)]
    public bool AllowThreads { get; set; }

    [DeleteBehavior(DeleteBehavior.SetNull)]
    public File? Avatar { get; set; }

    [ForeignKey(nameof(Avatar))]
    public Guid? AvatarId { get; set; }

    [DefaultValue(1)]
    public int AllowedChannelPostersMask { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime CreatedAt { get; set; }

#pragma warning disable CS8618
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public User CreatedBy { get; set; }

    [ForeignKey(nameof(CreatedBy))]
    public Guid CreatedById { get; set; }

    [ConcurrencyCheck]
    public byte[] ConcurrencyStamp { get; set; }

    [DefaultValue("")]
    [MaxLength(120)]
    public string Description { get; set; }
#pragma warning restore CS8618

    public ICollection<ChannelMember> ChannelMembers { get; } =
        new List<ChannelMember>();

    public ICollection<ChannelMessage> ChannelMessages { get; } =
        new List<ChannelMessage>();

#pragma warning disable CS8618
    [DefaultValue("")]
    [MaxLength(40)]
    public string Name { get; set; }
#pragma warning restore CS8618

    [DefaultValue(1)]
    public int NumMembers { get; set; }

    [DefaultValue(false)]
    public bool Private { get; set; }

#pragma warning disable CS8618
    [DefaultValue("")]
    [MaxLength(40)]
    public string Topic { get; set; }

    [DeleteBehavior(DeleteBehavior.Cascade)]
    public Workspace Workspace { get; set; }
#pragma warning restore CS8618

    [ForeignKey(nameof(Workspace))]
    public Guid WorkspaceId { get; set; }
}
