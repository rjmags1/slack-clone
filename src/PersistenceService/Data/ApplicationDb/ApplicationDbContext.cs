using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PersistenceService.Models;

namespace PersistenceService.Data.ApplicationDb;

#pragma warning disable CS8618
public class ApplicationDbContext
    : IdentityDbContext<User, IdentityRole<Guid>, Guid>
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
    public DbSet<Workspace> Workspaces { get; set; }
    public DbSet<WorkspaceAdminPermissions> WorkspaceAdminPermissions { get; set; }
    public DbSet<WorkspaceInvite> WorkspaceInvites { get; set; }
    public DbSet<WorkspaceMember> WorkspaceMembers { get; set; }
    public DbSet<WorkspaceSearch> WorkspaceSearches { get; set; }

    private bool _test = false;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public ApplicationDbContext(bool test)
    {
        _test = test;
    }

    public ApplicationDbContext() { }

    protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder
    )
    {
        DotNetEnv.Env.Load();
        if (_test)
        {
            optionsBuilder.UseNpgsql(
                Environment.GetEnvironmentVariable("TEST_DB_CONNECTION_STRING")
            );
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .Entity<ChannelMessage>()
            .HasMany(e => e.Replies)
            .WithOne(e => e.MessageRepliedTo)
            .HasForeignKey(e => e.MessageRepliedToId)
            .IsRequired();

        modelBuilder
            .Entity<ChannelMessageReply>()
            .HasOne(e => e.ChannelMessage)
            .WithOne()
            .IsRequired();

        modelBuilder
            .Entity<DirectMessage>()
            .HasMany(e => e.Replies)
            .WithOne(e => e.MessageRepliedTo)
            .HasForeignKey(e => e.MessageRepliedToId)
            .IsRequired();

        modelBuilder
            .Entity<ChannelMessage>()
            .HasOne(e => e.Thread)
            .WithMany(e => e.Messages)
            .HasForeignKey(e => e.ThreadId);

        modelBuilder
            .Entity<User>()
            .Property(e => e.UserName)
            .HasMaxLength(30)
            .IsRequired();

        modelBuilder
            .Entity<User>()
            .Property(e => e.NormalizedUserName)
            .HasMaxLength(30)
            .IsRequired();

        modelBuilder
            .Entity<User>()
            .Property(e => e.Email)
            .HasMaxLength(320)
            .IsRequired();

        modelBuilder
            .Entity<User>()
            .Property(e => e.NormalizedEmail)
            .HasMaxLength(320)
            .IsRequired();

        modelBuilder
            .Entity<User>()
            .Property(e => e.PasswordHash)
            .HasMaxLength(128)
            .IsRequired();

        modelBuilder
            .Entity<User>()
            .Property(e => e.PhoneNumber)
            .HasMaxLength(20);

        modelBuilder
            .Entity<User>()
            .Property(e => e.ConcurrencyStamp)
            .HasMaxLength(36)
            .IsRequired();

        modelBuilder
            .Entity<User>()
            .Property(e => e.SecurityStamp)
            .HasMaxLength(36)
            .IsRequired();

        modelBuilder
            .Entity<User>()
            .Property(e => e.LockoutEnd)
            .HasColumnType("timestamp")
            .HasConversion(new DateTimeOffsetTimestampConverter());

        modelBuilder
            .Entity<Workspace>()
            .Property(e => e.NumMembers)
            .HasDefaultValueSql("1");

        modelBuilder
            .Entity<Channel>()
            .Property(e => e.NumMembers)
            .HasDefaultValueSql("1");

        modelBuilder
            .Entity<ChannelInvite>()
            .Property(e => e.ChannelInviteStatus)
            .HasDefaultValueSql("1");

        modelBuilder
            .Entity<ChannelMember>()
            .Property(e => e.EnableNotifications)
            .HasDefaultValueSql("true");

        modelBuilder
            .Entity<Models.Thread>()
            .Property(e => e.NumMessages)
            .HasDefaultValueSql("2");

        modelBuilder
            .Entity<WorkspaceInvite>()
            .Property(e => e.WorkspaceInviteStatus)
            .HasDefaultValueSql("1");

        modelBuilder
            .Entity<WorkspaceAdminPermissions>()
            .Property(e => e.WorkspaceAdminPermissionsMask)
            .HasDefaultValueSql("1");

        modelBuilder
            .Entity<ChannelMessage>()
            .Property(e => e.Draft)
            .HasDefaultValueSql("true");

        modelBuilder
            .Entity<ChannelMessageLaterFlag>()
            .Property(e => e.ChannelLaterFlagStatus)
            .HasDefaultValueSql("1");

        modelBuilder
            .Entity<DirectMessage>()
            .Property(e => e.Draft)
            .HasDefaultValueSql("true");

        modelBuilder
            .Entity<DirectMessageLaterFlag>()
            .Property(e => e.DirectMessageLaterFlagStatus)
            .HasDefaultValueSql("1");

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            string[] timestampPropertyNames =
            {
                "CreatedAt",
                "JoinedAt",
                "UploadedAt"
            };
            foreach (var p in entityType.ClrType.GetProperties())
            {
                if (timestampPropertyNames.Contains(p.Name))
                {
                    modelBuilder
                        .Entity(entityType.ClrType)
                        .Property(p.Name)
                        .HasDefaultValueSql("now()");
                }
                if (p.Name == "Id" && p.PropertyType == typeof(Guid))
                {
                    modelBuilder
                        .Entity(entityType.ClrType)
                        .Property(p.Name)
                        .HasDefaultValueSql("gen_random_uuid()");
                }
                if (
                    p.Name == "ConcurrencyStamp"
                    && p.PropertyType == typeof(Guid)
                )
                {
                    modelBuilder
                        .Entity(entityType.ClrType)
                        .Property(p.Name)
                        .HasDefaultValueSql("gen_random_uuid()");
                }
            }
        }
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

public class DateTimeOffsetTimestampConverter
    : ValueConverter<DateTimeOffset, DateTime>
{
    public DateTimeOffsetTimestampConverter()
        : base(
            value => value.UtcDateTime,
            value => new DateTimeOffset(value, TimeSpan.Zero)
        ) { }
}
