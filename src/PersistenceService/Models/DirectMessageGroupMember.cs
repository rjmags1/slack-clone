using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersistenceService.Models;

[Index(nameof(DirectMessageGroupId), nameof(UserId), IsUnique = true)]
[Index(nameof(UserId))]
public class DirectMessageGroupMember
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

#pragma warning disable CS8618
    public DirectMessageGroup DirectMessageGroup { get; set; }
#pragma warning restore CS8618

    public Guid DirectMessageGroupId { get; set; }

    public DateTime? LastViewedAt { get; set; }

#pragma warning disable CS8618
    public User User { get; set; }
#pragma warning restore CS8618

    public Guid UserId { get; set; }
}
