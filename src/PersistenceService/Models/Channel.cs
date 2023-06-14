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

    public File? Avatar { get; set; }

    public Guid? AvatarId { get; set; }

    [DefaultValue(1)]
    public int AllowedChannelPostersMask { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }

    public User? CreatedBy { get; set; }

    public Guid? CreatedById { get; set; }

#pragma warning disable CS8618
    [DefaultValue("")]
    [MaxLength(120)]
    public string Description { get; set; }
#pragma warning restore CS8618

    public ICollection<ChannelMember> ChannelMembers { get; } =
        new List<ChannelMember>();

    public ICollection<ChannelMessage> ChannelMessages { get; } =
        new List<ChannelMessage>();

    public ICollection<File> Files { get; } = new List<File>();

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

    public Workspace Workspace { get; set; }
#pragma warning restore CS8618

    public Guid WorkspaceId { get; set; }
}
