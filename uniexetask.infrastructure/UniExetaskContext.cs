using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using uniexetask.core.Models;

namespace uniexetask.infrastructure;

public partial class UniExetaskContext : DbContext
{
    public UniExetaskContext()
    {
    }

    public UniExetaskContext(DbContextOptions<UniExetaskContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Campus> Campuses { get; set; }

    public virtual DbSet<ChatGroup> ChatGroups { get; set; }

    public virtual DbSet<ChatMessage> ChatMessages { get; set; }

    public virtual DbSet<Document> Documents { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<GroupInvite> GroupInvites { get; set; }

    public virtual DbSet<GroupMember> GroupMembers { get; set; }

    public virtual DbSet<Label> Labels { get; set; }

    public virtual DbSet<MeetingSchedule> MeetingSchedules { get; set; }

    public virtual DbSet<Mentor> Mentors { get; set; }

    public virtual DbSet<Nofitication> Nofitications { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<RegForm> RegForms { get; set; }

    public virtual DbSet<Requirement> Requirements { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RoleReg> RoleRegs { get; set; }

    public virtual DbSet<Score> Scores { get; set; }

    public virtual DbSet<ScoreComponent> ScoreComponents { get; set; }

    public virtual DbSet<Sponsor> Sponsors { get; set; }

    public virtual DbSet<SponsorshipDetail> SponsorshipDetails { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<core.Models.Task> Tasks { get; set; }

    public virtual DbSet<TaskAssign> TaskAssigns { get; set; }

    public virtual DbSet<User> Users { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Campus>(entity =>
        {
            entity.HasKey(e => e.CampusId).HasName("PK__CAMPUS__01989FD17450F5FC");

            entity.ToTable("CAMPUS");

            entity.Property(e => e.CampusId).HasColumnName("campus_id");
            entity.Property(e => e.CampusCode)
                .HasMaxLength(50)
                .HasColumnName("campus_code");
            entity.Property(e => e.CampusName)
                .HasMaxLength(100)
                .HasColumnName("campus_name");
            entity.Property(e => e.Location)
                .HasMaxLength(100)
                .HasColumnName("location");
        });

        modelBuilder.Entity<ChatGroup>(entity =>
        {
            entity.HasKey(e => e.ChatGroupId).HasName("PK__CHAT_GRO__F18D357989BB6FEC");

            entity.ToTable("CHAT_GROUP");

            entity.Property(e => e.ChatGroupId).HasColumnName("chat_group_id");
            entity.Property(e => e.ChatboxName)
                .HasMaxLength(50)
                .HasColumnName("chatbox_name");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.OwnerId).HasColumnName("owner_id");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasColumnName("type");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ChatGroupCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CHAT_GROU__creat__3E52440B");

            entity.HasOne(d => d.Owner).WithMany(p => p.ChatGroupOwners)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CHAT_GROU__owner__3F466844");
        });

        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__CHAT_MES__0BBF6EE6D5819BAC");

            entity.ToTable("CHAT_MESSAGE");

            entity.Property(e => e.MessageId).HasColumnName("message_id");
            entity.Property(e => e.ChatGroupId).HasColumnName("chat_group_id");
            entity.Property(e => e.MessageContent)
                .HasMaxLength(4000)
                .HasColumnName("message_content");
            entity.Property(e => e.SendDatetime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("send_datetime");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.ChatGroup).WithMany(p => p.ChatMessages)
                .HasForeignKey(d => d.ChatGroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CHAT_MESS__chat___4316F928");

            entity.HasOne(d => d.User).WithMany(p => p.ChatMessages)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CHAT_MESS__user___440B1D61");
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.DocumentId).HasName("PK__DOCUMENT__9666E8AC8D64E23A");

            entity.ToTable("DOCUMENT");

            entity.Property(e => e.DocumentId).HasColumnName("document_id");
            entity.Property(e => e.IsFinancialReport).HasColumnName("is_financial_report");
            entity.Property(e => e.Name)
                .HasMaxLength(250)
                .HasColumnName("name");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasColumnName("type");
            entity.Property(e => e.UploadBy).HasColumnName("upload_by");
            entity.Property(e => e.Url)
                .HasMaxLength(250)
                .HasColumnName("url");

            entity.HasOne(d => d.Project).WithMany(p => p.Documents)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DOCUMENT__projec__5FB337D6");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__EVENT__2370F7273B4BEFEB");

            entity.ToTable("EVENT");

            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .HasColumnName("description");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.Location)
                .HasMaxLength(250)
                .HasColumnName("location");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.RegUrl)
                .HasMaxLength(250)
                .HasColumnName("reg_url");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("PK__GROUP__D57795A05278D317");

            entity.ToTable("GROUP");

            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.GroupName)
                .HasMaxLength(250)
                .HasColumnName("group_name");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");

            entity.HasOne(d => d.Project).WithMany(p => p.Groups)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GROUP__project_i__68487DD7");

            entity.HasMany(d => d.Mentors).WithMany(p => p.Groups)
                .UsingEntity<Dictionary<string, object>>(
                    "MentorGroup",
                    r => r.HasOne<Mentor>().WithMany()
                        .HasForeignKey("MentorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__MENTOR_GR__mento__6C190EBB"),
                    l => l.HasOne<Group>().WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__MENTOR_GR__group__6B24EA82"),
                    j =>
                    {
                        j.HasKey("GroupId", "MentorId").HasName("PK__MENTOR_G__FB2AB24F7E44C056");
                        j.ToTable("MENTOR_GROUP");
                        j.IndexerProperty<int>("GroupId").HasColumnName("group_id");
                        j.IndexerProperty<int>("MentorId").HasColumnName("mentor_id");
                    });
        });

        modelBuilder.Entity<GroupInvite>(entity =>
        {
            entity.HasKey(e => new { e.GroupId, e.NotificationId }).HasName("PK__GROUP_IN__3B720DE23F5D9E5D");

            entity.ToTable("GROUP_INVITE");

            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.InviteeId).HasColumnName("invitee_id");
            entity.Property(e => e.InviterId).HasColumnName("inviter_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime")
                .HasColumnName("updated_date");

            entity.HasOne(d => d.Group).WithMany(p => p.GroupInvites)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GROUP_INV__group__01142BA1");

            entity.HasOne(d => d.Notification).WithMany(p => p.GroupInvites)
                .HasForeignKey(d => d.NotificationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GROUP_INV__notif__02084FDA");
        });

        modelBuilder.Entity<GroupMember>(entity =>
        {
            entity.HasKey(e => new { e.GroupId, e.StudentId }).HasName("PK__GROUP_ME__67D4A5C9A712393D");

            entity.ToTable("GROUP_MEMBER");

            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasColumnName("role");

            entity.HasOne(d => d.Group).WithMany(p => p.GroupMembers)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GROUP_MEM__group__75A278F5");

            entity.HasOne(d => d.Student).WithMany(p => p.GroupMembers)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GROUP_MEM__stude__76969D2E");
        });

        modelBuilder.Entity<Label>(entity =>
        {
            entity.HasKey(e => e.LabelId).HasName("PK__LABEL__E44FFA58B163EF4C");

            entity.ToTable("LABEL");

            entity.Property(e => e.LabelId).HasColumnName("label_id");
            entity.Property(e => e.LabelName)
                .HasMaxLength(50)
                .HasColumnName("label_name");
        });

        modelBuilder.Entity<MeetingSchedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__MEETING___C46A8A6FAF8403D7");

            entity.ToTable("MEETING_SCHEDULE");

            entity.Property(e => e.ScheduleId).HasColumnName("schedule_id");
            entity.Property(e => e.Content)
                .HasMaxLength(250)
                .HasColumnName("content");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.Location).HasColumnName("location");
            entity.Property(e => e.MeetingDate)
                .HasColumnType("datetime")
                .HasColumnName("meeting_date");
            entity.Property(e => e.MentorId).HasColumnName("mentor_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasColumnName("type");

            entity.HasOne(d => d.Group).WithMany(p => p.MeetingSchedules)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MEETING_S__group__70DDC3D8");

            entity.HasOne(d => d.Mentor).WithMany(p => p.MeetingSchedules)
                .HasForeignKey(d => d.MentorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MEETING_S__mento__71D1E811");
        });

        modelBuilder.Entity<Mentor>(entity =>
        {
            entity.HasKey(e => e.MentorId).HasName("PK__MENTOR__E5D27EF3FA65BBD5");

            entity.ToTable("MENTOR");

            entity.Property(e => e.MentorId).HasColumnName("mentor_id");
            entity.Property(e => e.Specialty)
                .HasMaxLength(250)
                .HasColumnName("specialty");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Mentors)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MENTOR__user_id__36B12243");
        });

        modelBuilder.Entity<Nofitication>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__NOFITICA__E059842F37124868");

            entity.ToTable("NOFITICATION");

            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Message)
                .HasMaxLength(250)
                .HasColumnName("message");
            entity.Property(e => e.ReceiverId).HasColumnName("receiver_id");
            entity.Property(e => e.SenderId).HasColumnName("sender_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasColumnName("type");

            entity.HasOne(d => d.Receiver).WithMany(p => p.NofiticationReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__NOFITICAT__recei__7C4F7684");

            entity.HasOne(d => d.Sender).WithMany(p => p.NofiticationSenders)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__NOFITICAT__sende__7B5B524B");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.PermissionId).HasName("PK__PERMISSI__E5331AFA8C93D821");

            entity.ToTable("PERMISSION");

            entity.Property(e => e.PermissionId).HasColumnName("permission_id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__PROJECT__BC799E1F1BC788C7");

            entity.ToTable("PROJECT");

            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .HasColumnName("description");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.Score).HasColumnName("score");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.SubjectId).HasColumnName("subject_id");
            entity.Property(e => e.TopicCode)
                .HasMaxLength(10)
                .HasColumnName("topic_code");
            entity.Property(e => e.TopicName)
                .HasMaxLength(50)
                .HasColumnName("topic_name");

            entity.HasOne(d => d.Subject).WithMany(p => p.Projects)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PROJECT__subject__4AB81AF0");

            entity.HasMany(d => d.Labels).WithMany(p => p.Projects)
                .UsingEntity<Dictionary<string, object>>(
                    "ProjectLabel",
                    r => r.HasOne<Label>().WithMany()
                        .HasForeignKey("LabelId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__PROJECT_L__label__5812160E"),
                    l => l.HasOne<Project>().WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__PROJECT_L__proje__571DF1D5"),
                    j =>
                    {
                        j.HasKey("ProjectId", "LabelId").HasName("PK__PROJECT___223D61BA779FB8C0");
                        j.ToTable("PROJECT_LABEL");
                        j.IndexerProperty<int>("ProjectId").HasColumnName("project_id");
                        j.IndexerProperty<int>("LabelId").HasColumnName("label_id");
                    });
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("PK__REFRESH___CB3C9E17087E9927");

            entity.ToTable("REFRESH_TOKEN");

            entity.Property(e => e.TokenId).HasColumnName("token_id");
            entity.Property(e => e.Created)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created");
            entity.Property(e => e.Expires)
                .HasColumnType("datetime")
                .HasColumnName("expires");
            entity.Property(e => e.Revoked)
                .HasColumnType("datetime")
                .HasColumnName("revoked");
            entity.Property(e => e.Status)
                .HasDefaultValue(true)
                .HasColumnName("status");
            entity.Property(e => e.Token)
                .HasMaxLength(50)
                .HasColumnName("token");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__REFRESH_T__user___10566F31");
        });

        modelBuilder.Entity<RegForm>(entity =>
        {
            entity.HasKey(e => e.RegFormId).HasName("PK__REG_FORM__5DCD302487DE6AA7");

            entity.ToTable("REG_FORM");

            entity.Property(e => e.RegFormId).HasColumnName("reg_form_id");
            entity.Property(e => e.ContentReg)
                .HasMaxLength(250)
                .HasColumnName("content_reg");
            entity.Property(e => e.RoleRegId).HasColumnName("role_reg_id");
            entity.Property(e => e.UserRegId).HasColumnName("user_reg_id");

            entity.HasOne(d => d.RoleReg).WithMany(p => p.RegForms)
                .HasForeignKey(d => d.RoleRegId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__REG_FORM__role_r__160F4887");

            entity.HasOne(d => d.UserReg).WithMany(p => p.RegForms)
                .HasForeignKey(d => d.UserRegId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__REG_FORM__user_r__151B244E");
        });

        modelBuilder.Entity<Requirement>(entity =>
        {
            entity.HasKey(e => e.RequirementId).HasName("PK__REQUIREM__2A73C1ADD4E04EE9");

            entity.ToTable("REQUIREMENT");

            entity.Property(e => e.RequirementId).HasColumnName("requirement_id");
            entity.Property(e => e.Content)
                .HasMaxLength(250)
                .HasColumnName("content");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.RequestorId).HasColumnName("requestor_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");

            entity.HasOne(d => d.Project).WithMany(p => p.Requirements)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__REQUIREME__proje__5BE2A6F2");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__ROLE__760965CC1B39327B");

            entity.ToTable("ROLE");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");

            entity.HasMany(d => d.Permissions).WithMany(p => p.Roles)
                .UsingEntity<Dictionary<string, object>>(
                    "RolePermission",
                    r => r.HasOne<Permission>().WithMany()
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__ROLE_PERM__permi__30F848ED"),
                    l => l.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__ROLE_PERM__role___300424B4"),
                    j =>
                    {
                        j.HasKey("RoleId", "PermissionId").HasName("PK__ROLE_PER__C85A546326CEF69D");
                        j.ToTable("ROLE_PERMISSION");
                        j.IndexerProperty<int>("RoleId").HasColumnName("role_id");
                        j.IndexerProperty<int>("PermissionId").HasColumnName("permission_id");
                    });
        });

        modelBuilder.Entity<RoleReg>(entity =>
        {
            entity.HasKey(e => e.RoleRegId).HasName("PK__ROLE_REG__830F110C858BD82B");

            entity.ToTable("ROLE_REG");

            entity.Property(e => e.RoleRegId).HasColumnName("role_reg_id");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .HasColumnName("description");
            entity.Property(e => e.RoleRegName).HasColumnName("role_reg_name");
        });

        modelBuilder.Entity<Score>(entity =>
        {
            entity.HasKey(e => e.ScoreId).HasName("PK__SCORE__8CA19050904F7428");

            entity.ToTable("SCORE");

            entity.Property(e => e.ScoreId).HasColumnName("score_id");
            entity.Property(e => e.ComponentId).HasColumnName("component_id");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.RatingStatus).HasColumnName("rating_status");
            entity.Property(e => e.Score1).HasColumnName("score");
            entity.Property(e => e.ScoredBy)
                .HasMaxLength(100)
                .HasColumnName("scored_by");

            entity.HasOne(d => d.Component).WithMany(p => p.Scores)
                .HasForeignKey(d => d.ComponentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SCORE__component__0A9D95DB");

            entity.HasOne(d => d.Project).WithMany(p => p.Scores)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SCORE__project_i__0B91BA14");
        });

        modelBuilder.Entity<ScoreComponent>(entity =>
        {
            entity.HasKey(e => e.ComponentId).HasName("PK__SCORE_CO__AEB1DA595A137514");

            entity.ToTable("SCORE_COMPONENT");

            entity.Property(e => e.ComponentId).HasColumnName("component_id");
            entity.Property(e => e.ComponentName)
                .HasMaxLength(250)
                .HasColumnName("component_name");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .HasColumnName("description");
            entity.Property(e => e.MilestoneName)
                .HasMaxLength(100)
                .HasColumnName("milestone_name");
            entity.Property(e => e.Percentage).HasColumnName("percentage");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime")
                .HasColumnName("updated_date");
        });

        modelBuilder.Entity<Sponsor>(entity =>
        {
            entity.HasKey(e => e.SponsorId).HasName("PK__SPONSOR__BE37D45462C557E8");

            entity.ToTable("SPONSOR");

            entity.Property(e => e.SponsorId).HasColumnName("sponsor_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Sponsors)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SPONSOR__user_id__398D8EEE");
        });

        modelBuilder.Entity<SponsorshipDetail>(entity =>
        {
            entity.HasKey(e => e.SponsorshipDetailId).HasName("PK__SPONSORS__A62594A8AFF9595F");

            entity.ToTable("SPONSORSHIP_DETAIL");

            entity.Property(e => e.SponsorshipDetailId).HasColumnName("sponsorship_detail_id");
            entity.Property(e => e.AmountMoney)
                .HasColumnType("money")
                .HasColumnName("amount_money");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.SponsorId).HasColumnName("sponsor_id");
            entity.Property(e => e.SponsorshipStatus)
                .HasMaxLength(20)
                .HasColumnName("sponsorship_status");

            entity.HasOne(d => d.Project).WithMany(p => p.SponsorshipDetails)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SPONSORSH__proje__6383C8BA");

            entity.HasOne(d => d.Sponsor).WithMany(p => p.SponsorshipDetails)
                .HasForeignKey(d => d.SponsorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SPONSORSH__spons__6477ECF3");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__STUDENT__2A33069A4B4A1BDD");

            entity.ToTable("STUDENT");

            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.IsEligible).HasColumnName("is_eligible");
            entity.Property(e => e.Major)
                .HasMaxLength(250)
                .HasColumnName("major");
            entity.Property(e => e.StudentCode)
                .HasMaxLength(10)
                .HasColumnName("student_code");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Students)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__STUDENT__user_id__33D4B598");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.SubjectId).HasName("PK__SUBJECT__5004F6602A4FC01C");

            entity.ToTable("SUBJECT");

            entity.Property(e => e.SubjectId).HasColumnName("subject_id");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .HasColumnName("description");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.SubjectCode)
                .HasMaxLength(50)
                .HasColumnName("subject_code");
            entity.Property(e => e.SubjectName)
                .HasMaxLength(50)
                .HasColumnName("subject_name");
        });

        modelBuilder.Entity<core.Models.Task>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__TASK__0492148DABAB5D5F");

            entity.ToTable("TASK");

            entity.Property(e => e.TaskId).HasColumnName("task_id");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .HasColumnName("description");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.TaskName)
                .HasMaxLength(50)
                .HasColumnName("task_name");

            entity.HasOne(d => d.Project).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TASK__project_id__4E88ABD4");
        });

        modelBuilder.Entity<TaskAssign>(entity =>
        {
            entity.HasKey(e => e.TaskAssignId).HasName("PK__TASK_ASS__8736D5688BEAA89D");

            entity.ToTable("TASK_ASSIGN");

            entity.Property(e => e.TaskAssignId).HasColumnName("task_assign_id");
            entity.Property(e => e.CompletionDate)
                .HasColumnType("datetime")
                .HasColumnName("completion_date");
            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.TaskId).HasColumnName("task_id");

            entity.HasOne(d => d.Student).WithMany(p => p.TaskAssigns)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TASK_ASSI__stude__52593CB8");

            entity.HasOne(d => d.Task).WithMany(p => p.TaskAssigns)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TASK_ASSI__task___5165187F");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__USER__B9BE370F2FA07ADC");

            entity.ToTable("USER");

            entity.HasIndex(e => e.Email, "UQ__USER__AB6E616420E44DB9").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CampusId).HasColumnName("campus_id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Status)
                .HasDefaultValue(true)
                .HasColumnName("status");

            entity.HasOne(d => d.Campus).WithMany(p => p.Users)
                .HasForeignKey(d => d.CampusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__USER__campus_id__2C3393D0");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__USER__role_id__2D27B809");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
