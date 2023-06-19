using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using PersistenceService.Models;

namespace PersistenceService.Data.ApplicationDb;

#pragma warning disable CS8618
public class ApplicationDbContext : DbContext
{
    public DbSet<Channel> Channels { get; set; }
    public DbSet<ChannelInvite> ChannelInvites { get; set; }
    public DbSet<ChannelMember> ChannelMembers { get; set; }
    public DbSet<ChannelMessage> ChannelMessages { get; set; }
    public DbSet<ChannelMessageLaterFlag> ChannelMessageLaterFlags { get; set; }
    public DbSet<ChannelMessageMention> ChannelMessageMentions { get; set; }
    public DbSet<ChannelMessageNotification> ChannelMessageNotifications { get; set; }
    public DbSet<ChannelMessageReaction> ChannelMessageReactions { get; set; }
    public DbSet<ChannelMessageReply> ChannelMessageReplies { get; set; }
    public DbSet<DirectMessage> DirectMessages { get; set; }
    public DbSet<DirectMessageGroup> DirectMessageGroups { get; set; }
    public DbSet<DirectMessageGroupMember> DirectMessageGroupMembers { get; set; }
    public DbSet<DirectMessageLaterFlag> DirectMessageLaterFlags { get; set; }
    public DbSet<DirectMessageMention> DirectMessageMentions { get; set; }
    public DbSet<DirectMessageNotification> DirectMessageNotifications { get; set; }
    public DbSet<DirectMessageReaction> DirectMessageReactions { get; set; }
    public DbSet<DirectMessageReply> DirectMessageReplies { get; set; }
    public DbSet<Models.File> Files { get; set; }
    public DbSet<Theme> Themes { get; set; }
    public DbSet<Models.Thread> Threads { get; set; }
    public DbSet<ThreadWatch> ThreadWatches { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Workspace> Workspaces { get; set; }
    public DbSet<WorkspaceAdminPermissions> WorkspaceAdminPermissions { get; set; }
    public DbSet<WorkspaceInvite> WorkspaceInvites { get; set; }
    public DbSet<WorkspaceMember> WorkspaceMembers { get; set; }
    public DbSet<WorkspaceSearch> WorkspaceSearches { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public ApplicationDbContext() { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<ChannelMessage>()
            .HasMany(e => e.Replies)
            .WithOne(e => e.ChannelMessage)
            .HasForeignKey(e => e.ChannelMessageId)
            .IsRequired();

        modelBuilder
            .Entity<DirectMessage>()
            .HasMany(e => e.Replies)
            .WithOne(e => e.DirectMessage)
            .HasForeignKey(e => e.DirectMessageId)
            .IsRequired();

        modelBuilder
            .Entity<ChannelMessage>()
            .HasOne(e => e.Thread)
            .WithMany(e => e.Messages)
            .HasForeignKey(e => e.ThreadId);

        modelBuilder
            .Entity<Channel>()
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()");

        modelBuilder
            .Entity<ChannelInvite>()
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()");

        modelBuilder
            .Entity<ChannelMessage>()
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()");

        modelBuilder
            .Entity<ChannelMessageLaterFlag>()
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()");

        modelBuilder
            .Entity<ChannelMessageMention>()
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()");

        modelBuilder
            .Entity<ChannelMessageNotification>()
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()");

        modelBuilder
            .Entity<ChannelMessageReaction>()
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()");

        modelBuilder
            .Entity<DirectMessage>()
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()");

        modelBuilder
            .Entity<DirectMessageGroup>()
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()");

        modelBuilder
            .Entity<DirectMessageLaterFlag>()
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()");

        modelBuilder
            .Entity<DirectMessageMention>()
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()");

        modelBuilder
            .Entity<DirectMessageNotification>()
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()");

        modelBuilder
            .Entity<DirectMessageReaction>()
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()");

        modelBuilder
            .Entity<DirectMessageReaction>()
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()");

        modelBuilder
            .Entity<Models.File>()
            .Property(e => e.UploadedAt)
            .HasDefaultValueSql("now()");

        modelBuilder
            .Entity<User>()
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()");

        modelBuilder
            .Entity<Workspace>()
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()");

        modelBuilder
            .Entity<WorkspaceInvite>()
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()");

        modelBuilder
            .Entity<WorkspaceMember>()
            .Property(e => e.JoinedAt)
            .HasDefaultValueSql("now()");

        modelBuilder
            .Entity<WorkspaceSearch>()
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()");
    }
}

public class ApplicationDbContextFactory
    : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        DotNetEnv.Env.Load();
        var optionsBuilder =
            new DbContextOptionsBuilder<ApplicationDbContext>();
        string connectionString =
            Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
            ?? "Failed to load DB_CONNECTION_STRING environment variable";
        optionsBuilder.UseNpgsql(connectionString);
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
