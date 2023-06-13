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

    [DefaultValue(1)]
    public int DirectMessageLaterFlagStatus { get; set; }

#pragma warning disable CS8618
    public DirectMessageGroup DirectMessageGroup { get; set; }
#pragma warning restore CS8618

    public Guid DirectMessageGroupId { get; set; }

#pragma warning disable CS8618
    public DirectMessage DirectMessage { get; set; }
#pragma warning restore CS8618

    public Guid DirectMessageId { get; set; }

#pragma warning disable CS8618
    public User User { get; set; }
#pragma warning restore CS8618

    public Guid UserId { get; set; }

#pragma warning disable CS8618
    public Workspace Workspace { get; set; }
#pragma warning restore CS8618

    public Guid WorkspaceId { get; set; }
}
