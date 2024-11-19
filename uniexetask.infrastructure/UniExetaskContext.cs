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

    public virtual DbSet<Criterion> Criteria { get; set; }

    public virtual DbSet<Document> Documents { get; set; }

    public virtual DbSet<ExpenseReport> ExpenseReports { get; set; }

    public virtual DbSet<Feature> Features { get; set; }

    public virtual DbSet<Funding> Fundings { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<GroupInvite> GroupInvites { get; set; }

    public virtual DbSet<GroupMember> GroupMembers { get; set; }

    public virtual DbSet<Label> Labels { get; set; }

    public virtual DbSet<MeetingSchedule> MeetingSchedules { get; set; }

    public virtual DbSet<MemberScore> MemberScores { get; set; }

    public virtual DbSet<Mentor> Mentors { get; set; }

    public virtual DbSet<Milestone> Milestones { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectProgress> ProjectProgresses { get; set; }

    public virtual DbSet<ProjectScore> ProjectScores { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<RegMemberForm> RegMemberForms { get; set; }

    public virtual DbSet<RegTopicForm> RegTopicForms { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<core.Models.Task> Tasks { get; set; }

    public virtual DbSet<TaskAssign> TaskAssigns { get; set; }

    public virtual DbSet<TaskDetail> TaskDetails { get; set; }

    public virtual DbSet<TaskProgress> TaskProgresses { get; set; }

    public virtual DbSet<Timeline> Timelines { get; set; }

    public virtual DbSet<Topic> Topics { get; set; }

    public virtual DbSet<UsagePlan> UsagePlans { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Workshop> Workshops { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Campus>(entity =>
        {
            entity.HasKey(e => e.CampusId).HasName("PK__CAMPUS__01989FD1E4672C4F");

            entity.ToTable("CAMPUS");

            entity.Property(e => e.CampusId).HasColumnName("campus_id");
            entity.Property(e => e.CampusCode)
                .HasMaxLength(50)
                .HasColumnName("campus_code");
            entity.Property(e => e.CampusName)
                .HasMaxLength(100)
                .HasColumnName("campus_name");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.Location)
                .HasMaxLength(100)
                .HasColumnName("location");
        });

        modelBuilder.Entity<ChatGroup>(entity =>
        {
            entity.HasKey(e => e.ChatGroupId).HasName("PK__CHAT_GRO__F18D35790D88BFA3");

            entity.ToTable("CHAT_GROUP");

            entity.Property(e => e.ChatGroupId).HasColumnName("chat_group_id");
            entity.Property(e => e.ChatGroupAvatar)
                .HasMaxLength(255)
                .HasColumnName("chat_group_avatar");
            entity.Property(e => e.ChatGroupName)
                .HasMaxLength(50)
                .HasColumnName("chat_group_name");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.LatestActivity)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("latest_activity");
            entity.Property(e => e.OwnerId).HasColumnName("owner_id");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasColumnName("type");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ChatGroupCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CHAT_GROU__creat__46E78A0C");

            entity.HasOne(d => d.Owner).WithMany(p => p.ChatGroupOwners)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CHAT_GROU__owner__47DBAE45");
        });

        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__CHAT_MES__0BBF6EE69BC9AC8C");

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
                .HasConstraintName("FK__CHAT_MESS__chat___4BAC3F29");

            entity.HasOne(d => d.User).WithMany(p => p.ChatMessages)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CHAT_MESS__user___4CA06362");
        });

        modelBuilder.Entity<Criterion>(entity =>
        {
            entity.HasKey(e => e.CriteriaId).HasName("PK__CRITERIA__401F949D10D70F43");

            entity.ToTable("CRITERIA");

            entity.Property(e => e.CriteriaId).HasColumnName("criteria_id");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.CriteriaName)
                .HasMaxLength(250)
                .HasColumnName("criteria_name");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .HasColumnName("description");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.MilestoneId).HasColumnName("milestone_id");
            entity.Property(e => e.Percentage).HasColumnName("percentage");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime")
                .HasColumnName("updated_date");

            entity.HasOne(d => d.Milestone).WithMany(p => p.Criteria)
                .HasForeignKey(d => d.MilestoneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CRITERIA__milest__3A4CA8FD");
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.DocumentId).HasName("PK__DOCUMENT__9666E8AC906A1D18");

            entity.ToTable("DOCUMENT");

            entity.Property(e => e.DocumentId).HasColumnName("document_id");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
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
                .HasConstraintName("FK__DOCUMENT__projec__06CD04F7");
        });

        modelBuilder.Entity<ExpenseReport>(entity =>
        {
            entity.HasKey(e => e.ExpenseReportId).HasName("PK__EXPENSE___E936821A75D9CD8F");

            entity.ToTable("EXPENSE_REPORT");

            entity.Property(e => e.ExpenseReportId).HasColumnName("expense_report_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.ReceiptUrl)
                .HasMaxLength(500)
                .HasColumnName("receipt_url");
            entity.Property(e => e.SpentAmount).HasColumnName("spent_amount");
            entity.Property(e => e.SpentDate)
                .HasColumnType("datetime")
                .HasColumnName("spent_date");
            entity.Property(e => e.UsagePlanId).HasColumnName("usage_plan_id");

            entity.HasOne(d => d.UsagePlan).WithMany(p => p.ExpenseReports)
                .HasForeignKey(d => d.UsagePlanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EXPENSE_R__usage__1332DBDC");
        });

        modelBuilder.Entity<Feature>(entity =>
        {
            entity.HasKey(e => e.FeatureId).HasName("PK__FEATURE__7906CBD711BC0A59");

            entity.ToTable("FEATURE");

            entity.Property(e => e.FeatureId).HasColumnName("feature_id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Funding>(entity =>
        {
            entity.HasKey(e => e.FundingId).HasName("PK__FUNDING__32013D9D20010818");

            entity.ToTable("FUNDING");

            entity.Property(e => e.FundingId).HasColumnName("funding_id");
            entity.Property(e => e.AmountMoney).HasColumnName("amount_money");
            entity.Property(e => e.ApprovedDate)
                .HasColumnType("datetime")
                .HasColumnName("approved_date");
            entity.Property(e => e.DocumentId).HasColumnName("document_id");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");

            entity.HasOne(d => d.Document).WithMany(p => p.Fundings)
                .HasForeignKey(d => d.DocumentId)
                .HasConstraintName("FK__FUNDING__documen__0C85DE4D");

            entity.HasOne(d => d.Project).WithMany(p => p.Fundings)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FUNDING__project__0B91BA14");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("PK__GROUP__D57795A044B10559");

            entity.ToTable("GROUP");

            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.GroupName)
                .HasMaxLength(250)
                .HasColumnName("group_name");
            entity.Property(e => e.HasMentor).HasColumnName("hasMentor");
            entity.Property(e => e.IsCurrentPeriod)
                .HasDefaultValue(true)
                .HasColumnName("isCurrentPeriod");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.SubjectId).HasColumnName("subject_id");

            entity.HasOne(d => d.Subject).WithMany(p => p.Groups)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GROUP__subject_i__59FA5E80");

            entity.HasMany(d => d.Mentors).WithMany(p => p.Groups)
                .UsingEntity<Dictionary<string, object>>(
                    "MentorGroup",
                    r => r.HasOne<Mentor>().WithMany()
                        .HasForeignKey("MentorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__MENTOR_GR__mento__17036CC0"),
                    l => l.HasOne<Group>().WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__MENTOR_GR__group__160F4887"),
                    j =>
                    {
                        j.HasKey("GroupId", "MentorId").HasName("PK__MENTOR_G__FB2AB24F0A864714");
                        j.ToTable("MENTOR_GROUP");
                        j.IndexerProperty<int>("GroupId").HasColumnName("group_id");
                        j.IndexerProperty<int>("MentorId").HasColumnName("mentor_id");
                    });
        });

        modelBuilder.Entity<GroupInvite>(entity =>
        {
            entity.HasKey(e => new { e.GroupId, e.NotificationId }).HasName("PK__GROUP_IN__3B720DE2DB4B288A");

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
                .HasConstraintName("FK__GROUP_INV__group__2BFE89A6");

            entity.HasOne(d => d.Notification).WithMany(p => p.GroupInvites)
                .HasForeignKey(d => d.NotificationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GROUP_INV__notif__2CF2ADDF");
        });

        modelBuilder.Entity<GroupMember>(entity =>
        {
            entity.HasKey(e => new { e.GroupId, e.StudentId }).HasName("PK__GROUP_ME__67D4A5C9A0585853");

            entity.ToTable("GROUP_MEMBER");

            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasColumnName("role");

            entity.HasOne(d => d.Group).WithMany(p => p.GroupMembers)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GROUP_MEM__group__208CD6FA");

            entity.HasOne(d => d.Student).WithMany(p => p.GroupMembers)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GROUP_MEM__stude__2180FB33");
        });

        modelBuilder.Entity<Label>(entity =>
        {
            entity.HasKey(e => e.LabelId).HasName("PK__LABEL__E44FFA58838E196B");

            entity.ToTable("LABEL");

            entity.Property(e => e.LabelId).HasColumnName("label_id");
            entity.Property(e => e.LabelName)
                .HasMaxLength(50)
                .HasColumnName("label_name");
        });

        modelBuilder.Entity<MeetingSchedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__MEETING___C46A8A6F94ED2859");

            entity.ToTable("MEETING_SCHEDULE");

            entity.Property(e => e.ScheduleId).HasColumnName("schedule_id");
            entity.Property(e => e.Content)
                .HasMaxLength(250)
                .HasColumnName("content");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.Location).HasColumnName("location");
            entity.Property(e => e.MeetingDate)
                .HasColumnType("datetime")
                .HasColumnName("meeting_date");
            entity.Property(e => e.MentorId).HasColumnName("mentor_id");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasColumnName("type");

            entity.HasOne(d => d.Group).WithMany(p => p.MeetingSchedules)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MEETING_S__group__1BC821DD");

            entity.HasOne(d => d.Mentor).WithMany(p => p.MeetingSchedules)
                .HasForeignKey(d => d.MentorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MEETING_S__mento__1CBC4616");
        });

        modelBuilder.Entity<MemberScore>(entity =>
        {
            entity.HasKey(e => e.MemberScoreId).HasName("PK__MEMBER_S__A2363F573E79AD84");

            entity.ToTable("MEMBER_SCORE");

            entity.Property(e => e.MemberScoreId).HasColumnName("member_score_id");
            entity.Property(e => e.Comment)
                .HasMaxLength(250)
                .HasColumnName("comment");
            entity.Property(e => e.MilestoneId).HasColumnName("milestone_id");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.Score).HasColumnName("score");
            entity.Property(e => e.ScoredBy).HasColumnName("scored_by");
            entity.Property(e => e.ScoringDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("scoring_date");
            entity.Property(e => e.StudentId).HasColumnName("student_id");

            entity.HasOne(d => d.Milestone).WithMany(p => p.MemberScores)
                .HasForeignKey(d => d.MilestoneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MEMBER_SC__miles__46B27FE2");

            entity.HasOne(d => d.Project).WithMany(p => p.MemberScores)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MEMBER_SC__proje__45BE5BA9");

            entity.HasOne(d => d.ScoredByNavigation).WithMany(p => p.MemberScores)
                .HasForeignKey(d => d.ScoredBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MEMBER_SC__score__47A6A41B");

            entity.HasOne(d => d.Student).WithMany(p => p.MemberScores)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MEMBER_SC__stude__44CA3770");
        });

        modelBuilder.Entity<Mentor>(entity =>
        {
            entity.HasKey(e => e.MentorId).HasName("PK__MENTOR__E5D27EF3F8EFA97E");

            entity.ToTable("MENTOR");

            entity.Property(e => e.MentorId).HasColumnName("mentor_id");
            entity.Property(e => e.Specialty)
                .HasMaxLength(250)
                .HasColumnName("specialty");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Mentors)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MENTOR__user_id__3A81B327");
        });

        modelBuilder.Entity<Milestone>(entity =>
        {
            entity.HasKey(e => e.MilestoneId).HasName("PK__MILESTON__67592EB714633E8E");

            entity.ToTable("MILESTONE");

            entity.Property(e => e.MilestoneId).HasColumnName("milestone_id");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .HasColumnName("description");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.MilestoneName)
                .HasMaxLength(100)
                .HasColumnName("milestone_name");
            entity.Property(e => e.Percentage).HasColumnName("percentage");
            entity.Property(e => e.SubjectId).HasColumnName("subject_id");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime")
                .HasColumnName("updated_date");

            entity.HasOne(d => d.Subject).WithMany(p => p.Milestones)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MILESTONE__subje__3587F3E0");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__NOTIFICA__E059842F1A6B4952");

            entity.ToTable("NOTIFICATION");

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

            entity.HasOne(d => d.Receiver).WithMany(p => p.NotificationReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__NOTIFICAT__recei__2739D489");

            entity.HasOne(d => d.Sender).WithMany(p => p.NotificationSenders)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__NOTIFICAT__sende__2645B050");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.PermissionId).HasName("PK__PERMISSI__E5331AFAE06A2741");

            entity.ToTable("PERMISSION");

            entity.Property(e => e.PermissionId).HasColumnName("permission_id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.FeatureId).HasColumnName("feature_id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");

            entity.HasOne(d => d.Feature).WithMany(p => p.Permissions)
                .HasForeignKey(d => d.FeatureId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PERMISSIO__descr__2E1BDC42");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__PROJECT__BC799E1F9436416A");

            entity.ToTable("PROJECT");

            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.IsCurrentPeriod)
                .HasDefaultValue(true)
                .HasColumnName("isCurrentPeriod");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.SubjectId).HasColumnName("subject_id");
            entity.Property(e => e.TopicId).HasColumnName("topic_id");

            entity.HasOne(d => d.Group).WithMany(p => p.Projects)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PROJECT__group_i__628FA481");

            entity.HasOne(d => d.Subject).WithMany(p => p.Projects)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PROJECT__subject__60A75C0F");

            entity.HasOne(d => d.Topic).WithMany(p => p.Projects)
                .HasForeignKey(d => d.TopicId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PROJECT__topic_i__619B8048");

            entity.HasMany(d => d.Labels).WithMany(p => p.Projects)
                .UsingEntity<Dictionary<string, object>>(
                    "ProjectLabel",
                    r => r.HasOne<Label>().WithMany()
                        .HasForeignKey("LabelId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__PROJECT_L__label__02FC7413"),
                    l => l.HasOne<Project>().WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__PROJECT_L__proje__02084FDA"),
                    j =>
                    {
                        j.HasKey("ProjectId", "LabelId").HasName("PK__PROJECT___223D61BA6EF404C4");
                        j.ToTable("PROJECT_LABEL");
                        j.IndexerProperty<int>("ProjectId").HasColumnName("project_id");
                        j.IndexerProperty<int>("LabelId").HasColumnName("label_id");
                    });
        });

        modelBuilder.Entity<ProjectProgress>(entity =>
        {
            entity.HasKey(e => e.ProjectProgressId).HasName("PK__PROJECT___A8484F6A5940062A");

            entity.ToTable("PROJECT_PROGRESS");

            entity.Property(e => e.ProjectProgressId).HasColumnName("project_progress_id");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.Note)
                .HasMaxLength(250)
                .HasColumnName("note");
            entity.Property(e => e.ProgressPercentage)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("progress_percentage");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime")
                .HasColumnName("updated_date");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectProgresses)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PROJECT_P__proje__68487DD7");
        });

        modelBuilder.Entity<ProjectScore>(entity =>
        {
            entity.HasKey(e => e.ProjectScoreId).HasName("PK__PROJECT___636E7782C3114154");

            entity.ToTable("PROJECT_SCORE");

            entity.Property(e => e.ProjectScoreId).HasColumnName("project_score_id");
            entity.Property(e => e.Comment)
                .HasMaxLength(250)
                .HasColumnName("comment");
            entity.Property(e => e.CriteriaId).HasColumnName("criteria_id");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.Score).HasColumnName("score");
            entity.Property(e => e.ScoredBy).HasColumnName("scored_by");
            entity.Property(e => e.ScoringDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("scoring_date");

            entity.HasOne(d => d.Criteria).WithMany(p => p.ProjectScores)
                .HasForeignKey(d => d.CriteriaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PROJECT_S__crite__3E1D39E1");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectScores)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PROJECT_S__proje__3F115E1A");

            entity.HasOne(d => d.ScoredByNavigation).WithMany(p => p.ProjectScores)
                .HasForeignKey(d => d.ScoredBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PROJECT_S__score__40058253");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("PK__REFRESH___CB3C9E175F6E057B");

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
            entity.Property(e => e.Token).HasColumnName("token");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__REFRESH_T__user___4D5F7D71");
        });

        modelBuilder.Entity<RegMemberForm>(entity =>
        {
            entity.HasKey(e => e.RegMemberId).HasName("PK__REG_MEMB__8BAC7116731AD990");

            entity.ToTable("REG_MEMBER_FORM");

            entity.Property(e => e.RegMemberId).HasColumnName("reg_member_id");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .HasColumnName("description");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Group).WithMany(p => p.RegMemberForms)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__REG_MEMBE__group__531856C7");
        });

        modelBuilder.Entity<RegTopicForm>(entity =>
        {
            entity.HasKey(e => e.RegTopicId).HasName("PK__REG_TOPI__E20C4927FF66B5B0");

            entity.ToTable("REG_TOPIC_FORM");

            entity.Property(e => e.RegTopicId).HasColumnName("reg_topic_id");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .HasColumnName("description");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TopicCode)
                .HasMaxLength(50)
                .HasColumnName("topic_code");
            entity.Property(e => e.TopicName)
                .HasMaxLength(100)
                .HasColumnName("topic_name");

            entity.HasOne(d => d.Group).WithMany(p => p.RegTopicForms)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__REG_TOPIC__group__503BEA1C");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__ROLE__760965CCC489E7BC");

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
                        .HasConstraintName("FK__ROLE_PERM__permi__37A5467C"),
                    l => l.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__ROLE_PERM__role___36B12243"),
                    j =>
                    {
                        j.HasKey("RoleId", "PermissionId").HasName("PK__ROLE_PER__C85A546333DC1EB9");
                        j.ToTable("ROLE_PERMISSION");
                        j.IndexerProperty<int>("RoleId").HasColumnName("role_id");
                        j.IndexerProperty<int>("PermissionId").HasColumnName("permission_id");
                    });
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__STUDENT__2A33069A16AA7B4D");

            entity.ToTable("STUDENT");

            entity.HasIndex(e => e.StudentCode, "UQ__STUDENT__6DF33C450AA5FAC5").IsUnique();

            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.IsCurrentPeriod)
                .HasDefaultValue(true)
                .HasColumnName("isCurrentPeriod");
            entity.Property(e => e.LecturerId).HasColumnName("lecturer_id");
            entity.Property(e => e.Major)
                .HasMaxLength(250)
                .HasColumnName("major");
            entity.Property(e => e.StudentCode)
                .HasMaxLength(10)
                .HasColumnName("student_code");
            entity.Property(e => e.SubjectId).HasColumnName("subject_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Lecturer).WithMany(p => p.Students)
                .HasForeignKey(d => d.LecturerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__STUDENT__lecture__403A8C7D");

            entity.HasOne(d => d.Subject).WithMany(p => p.Students)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__STUDENT__subject__412EB0B6");

            entity.HasOne(d => d.User).WithMany(p => p.Students)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__STUDENT__user_id__3F466844");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.SubjectId).HasName("PK__SUBJECT__5004F660EDB1A3CA");

            entity.ToTable("SUBJECT");

            entity.Property(e => e.SubjectId).HasColumnName("subject_id");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.SubjectCode)
                .HasMaxLength(50)
                .HasColumnName("subject_code");
            entity.Property(e => e.SubjectName)
                .HasMaxLength(50)
                .HasColumnName("subject_name");
        });

        modelBuilder.Entity<core.Models.Task>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__TASK__0492148D9173DC65");

            entity.ToTable("TASK");

            entity.Property(e => e.TaskId).HasColumnName("task_id");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .HasColumnName("description");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
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
                .HasConstraintName("FK__TASK__project_id__6C190EBB");
        });

        modelBuilder.Entity<TaskAssign>(entity =>
        {
            entity.HasKey(e => e.TaskAssignId).HasName("PK__TASK_ASS__8736D568519161B5");

            entity.ToTable("TASK_ASSIGN");

            entity.Property(e => e.TaskAssignId).HasColumnName("task_assign_id");
            entity.Property(e => e.AssignedDate)
                .HasColumnType("datetime")
                .HasColumnName("assigned_date");
            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.TaskId).HasColumnName("task_id");

            entity.HasOne(d => d.Student).WithMany(p => p.TaskAssigns)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TASK_ASSI__stude__76969D2E");

            entity.HasOne(d => d.Task).WithMany(p => p.TaskAssigns)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TASK_ASSI__task___75A278F5");
        });

        modelBuilder.Entity<TaskDetail>(entity =>
        {
            entity.HasKey(e => e.TaskDetailId).HasName("PK__TASK_DET__2B12CFB9064BA992");

            entity.ToTable("TASK_DETAIL");

            entity.Property(e => e.TaskDetailId).HasColumnName("task_detail_id");
            entity.Property(e => e.IsCompleted).HasColumnName("isCompleted");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.ProgressPercentage)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("progress_percentage");
            entity.Property(e => e.TaskDetailName)
                .HasMaxLength(250)
                .HasColumnName("task_detail_name");
            entity.Property(e => e.TaskId).HasColumnName("task_id");

            entity.HasOne(d => d.Task).WithMany(p => p.TaskDetails)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TASK_DETA__task___7D439ABD");
        });

        modelBuilder.Entity<TaskProgress>(entity =>
        {
            entity.HasKey(e => e.TaskProgressId).HasName("PK__TASK_PRO__26A7535EA7EFFC91");

            entity.ToTable("TASK_PROGRESS");

            entity.Property(e => e.TaskProgressId).HasColumnName("task_progress_id");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.Note)
                .HasMaxLength(250)
                .HasColumnName("note");
            entity.Property(e => e.ProgressPercentage)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("progress_percentage");
            entity.Property(e => e.TaskId).HasColumnName("task_id");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime")
                .HasColumnName("updated_date");

            entity.HasOne(d => d.Task).WithMany(p => p.TaskProgresses)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TASK_PROG__task___72C60C4A");
        });

        modelBuilder.Entity<Timeline>(entity =>
        {
            entity.HasKey(e => e.TimelineId).HasName("PK__TIMELINE__DC6F55B09C207929");

            entity.ToTable("TIMELINE");

            entity.Property(e => e.TimelineId).HasColumnName("timeline_id");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .HasColumnName("description");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");
            entity.Property(e => e.SubjectId).HasColumnName("subject_id");
            entity.Property(e => e.TimelineName)
                .HasMaxLength(100)
                .HasColumnName("timeline_name");

            entity.HasOne(d => d.Subject).WithMany(p => p.Timelines)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TIMELINE__subjec__534D60F1");
        });

        modelBuilder.Entity<Topic>(entity =>
        {
            entity.HasKey(e => e.TopicId).HasName("PK__TOPIC__D5DAA3E9A6AFACB1");

            entity.ToTable("TOPIC");

            entity.Property(e => e.TopicId).HasColumnName("topic_id");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .HasColumnName("description");
            entity.Property(e => e.TopicCode)
                .HasMaxLength(50)
                .HasColumnName("topic_code");
            entity.Property(e => e.TopicName)
                .HasMaxLength(100)
                .HasColumnName("topic_name");
        });

        modelBuilder.Entity<UsagePlan>(entity =>
        {
            entity.HasKey(e => e.UsagePlanId).HasName("PK__USAGE_PL__E27451F91C1241D9");

            entity.ToTable("USAGE_PLAN");

            entity.Property(e => e.UsagePlanId).HasColumnName("usage_plan_id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.FundingId).HasColumnName("funding_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.Funding).WithMany(p => p.UsagePlans)
                .HasForeignKey(d => d.FundingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__USAGE_PLA__fundi__10566F31");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__USER__B9BE370FB8EB9113");

            entity.ToTable("USER");

            entity.HasIndex(e => e.Email, "UQ__USER__AB6E61649ADD9072").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Avatar).HasColumnName("avatar");
            entity.Property(e => e.CampusId).HasColumnName("campus_id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Campus).WithMany(p => p.Users)
                .HasForeignKey(d => d.CampusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__USER__campus_id__31EC6D26");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__USER__role_id__32E0915F");

            entity.HasMany(d => d.ChatGroups).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserChatGroup",
                    r => r.HasOne<ChatGroup>().WithMany()
                        .HasForeignKey("ChatGroupId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__USER_CHAT__chat___5070F446"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__USER_CHAT__user___4F7CD00D"),
                    j =>
                    {
                        j.HasKey("UserId", "ChatGroupId").HasName("PK__USER_CHA__36A6E458DB27912E");
                        j.ToTable("USER_CHAT_GROUP");
                        j.IndexerProperty<int>("UserId").HasColumnName("user_id");
                        j.IndexerProperty<int>("ChatGroupId").HasColumnName("chat_group_id");
                    });
        });

        modelBuilder.Entity<Workshop>(entity =>
        {
            entity.HasKey(e => e.WorkshopId).HasName("PK__WORKSHOP__EA6B05590BABA251");

            entity.ToTable("WORKSHOP");

            entity.Property(e => e.WorkshopId).HasColumnName("workshop_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.Location).HasColumnName("location");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.RegUrl).HasColumnName("reg_url");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
