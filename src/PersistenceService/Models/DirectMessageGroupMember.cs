using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersistenceService.Models;

[Index(nameof(UserId), nameof(DirectMessageGroupId), IsUnique = true)]
public class DirectMessageGroupMember
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

#pragma warning disable CS8618
    public DirectMessageGroup DirectMessageGroup { get; set; }
#pragma warning restore CS8618

    public Guid DirectMessageGroupId { get; set; }

    public DateTime? LastViewedGroupMessagesAt { get; set; }

#pragma warning disable CS8618
    public User User { get; set; }
#pragma warning restore CS8618

    public Guid UserId { get; set; }
}
