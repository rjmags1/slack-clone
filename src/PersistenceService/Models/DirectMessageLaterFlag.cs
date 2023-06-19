using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersistenceService.Models;

[Index(nameof(DirectMessageId), nameof(UserId), IsUnique = true)]
[Index(nameof(WorkspaceId), nameof(UserId))]
public class DirectMessageLaterFlag
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime CreatedAt { get; set; }

    [DefaultValue(1)]
    public int DirectMessageLaterFlagStatus { get; set; }

#pragma warning disable CS8618
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public DirectMessageGroup DirectMessageGroup { get; set; }
#pragma warning restore CS8618

    [ForeignKey(nameof(DirectMessageGroup))]
    public Guid DirectMessageGroupId { get; set; }

#pragma warning disable CS8618
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public DirectMessage DirectMessage { get; set; }
#pragma warning restore CS8618

    [ForeignKey(nameof(DirectMessage))]
    public Guid DirectMessageId { get; set; }

#pragma warning disable CS8618
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public User User { get; set; }
#pragma warning restore CS8618

    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

#pragma warning disable CS8618
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public Workspace Workspace { get; set; }
#pragma warning restore CS8618

    [ForeignKey(nameof(Workspace))]
    public Guid WorkspaceId { get; set; }
}
