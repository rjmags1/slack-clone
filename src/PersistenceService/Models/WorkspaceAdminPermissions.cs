using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersistenceService.Models;

[PrimaryKey(nameof(AdminId), nameof(WorkspaceId))]
public class WorkspaceAdminPermissions
{
#pragma warning disable CS8618
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public User Admin { get; set; }
#pragma warning restore CS8618

    [ForeignKey(nameof(Admin))]
    public Guid AdminId { get; set; }

#pragma warning disable CS8618
    [ConcurrencyCheck]
    public Guid ConcurrencyStamp { get; set; }
#pragma warning restore CS8618

    [DefaultValue(1)]
    public int WorkspaceAdminPermissionsMask { get; set; }

#pragma warning disable CS8618
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public Workspace Workspace { get; set; }
#pragma warning restore CS8618

    [ForeignKey(nameof(Workspace))]
    public Guid WorkspaceId { get; set; }
}
