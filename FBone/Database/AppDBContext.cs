using DocumentFormat.OpenXml.Vml.Office;
using FBone.Database.Entities;
using FBone.Models.NMode;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FBone.Database
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options)
            : base(options)
        {
        }
        public DbSet<tUser> tUser { get; set; }
        public DbSet<tPosition> tPosition { get; set; }
        public DbSet<tActCause> tActCause { get; set; }
        public DbSet<tActImpact> tActImpact { get; set; }
        public DbSet<tActProtect> tActProtect { get; set; }
        public DbSet<tArea> tArea { get; set; }
        public DbSet<tAct> tAct { get; set; }
        public DbSet<tActItems> tActItems { get; set; }
        public DbSet<Tag> Tag { get; set; }
        public DbSet<Event> Event { get; set; }
        public DbSet<Device> Device { get; set; }
        public DbSet<tFacility> tFacility { get; set; }
        public DbSet<ActHistory> ActHistory { get; set; }
        public DbSet<NodeReferences> NodeReferences { get; set; }
        public DbSet<EmailTemplate> EmailTemplate { get; set; }
        public DbSet<RequestLog> RequestLogs { get; set; }
        public virtual DbSet<Area> Areas { get; set; }
        public virtual DbSet<Lcn> Lcns { get; set; }
        public virtual DbSet<NModeCondition> NModeConditions { get; set; }

        public virtual DbSet<NModeRecord> NModeRecords { get; set; }
        public virtual DbSet<NModeResult> NModeResults { get; set; }
        public virtual DbSet<NModeChangeRecord> NModeChangeRecords { get; set; }
        public virtual DbSet<NMTotalRecord> NMTotalRecords { get; set; }
        public virtual DbSet<Audit> Audits { get; set; }        
        public virtual DbSet<AuditFile> AuditFiles { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tAct>()
                .Property(u => u.OriginalLang).IsRequired(true).HasDefaultValue("ru");

            modelBuilder.Entity<EmailTemplate>()
                .HasIndex(b => b.EmailId)
                .IsUnique()
                .HasDatabaseName("Index_EmailId");

            // modelBuilder.Entity<tAct>().Property(a => a.ClosedByUser).IsRequired(false);
            //modelBuilder.Entity<Event>()
            //    .Property(u => u.EventDateTimeClear).IsRequired(false);

            modelBuilder.Entity<Event>().ToTable(i => i.HasCheckConstraint("CK_Properties_EventSetDate_EventClearDate", "[EventDateTimeClear] > [EventDateTimeSet]"));
            //modelBuilder.Entity<Event>()
            //    .HasCheckConstraint("CK_Properties_EventSetDate_EventClearDate", "[EventDateTimeClear] > [EventDateTimeSet]");

            //one-to-many tUser-tPosition
            modelBuilder.Entity<tUser>()
                .HasOne(u => u.Position)
                .WithMany(p => p.Users)
                .HasForeignKey(u => u.PositionId)
                .OnDelete(DeleteBehavior.Restrict);

            //one-to-many area-act
            modelBuilder.Entity<tAct>()
                .HasOne(u => u.Area)
                .WithMany(p => p.Acts)
                .HasForeignKey(u => u.AreaId)
                .OnDelete(DeleteBehavior.Restrict);

            //one-to-many area-NodeRef
            modelBuilder.Entity<NodeReferences>()
                .HasOne(u => u.Area)
                .WithMany(p => p.References)
                .HasForeignKey(u => u.AreaId)
                .OnDelete(DeleteBehavior.Restrict);

            //one-to-many Tag-Area
            modelBuilder.Entity<Tag>()
                .HasOne(u => u.Area)
                .WithMany(p => p.Tags)
                .HasForeignKey(u => u.AreaId)
                .OnDelete(DeleteBehavior.Restrict);

            //one-to-many Event-Tag
            modelBuilder.Entity<Event>()
                .HasOne(u => u.Tag)
                .WithMany(p => p.Events)
                .HasForeignKey(u => u.TagId)
                .OnDelete(DeleteBehavior.Restrict);

            //one-to-many Device-Tag
            modelBuilder.Entity<Tag>()
                .HasOne(u => u.Device)
                .WithMany(p => p.Tags)
                .HasForeignKey(u => u.DeviceId)
                .OnDelete(DeleteBehavior.Restrict);

            //one-to-many Event-Tag
            //modelBuilder.Entity<tArea>()
            //    .HasOne(u => u.Notifier)
            //    .WithMany(p => p.NotifyAreas)
            //    .HasForeignKey(u => u.NotifyPos24H)
            //    .OnDelete(DeleteBehavior.Restrict);

            //one-to-many Act-ActItems
            modelBuilder.Entity<tActItems>()
                .HasOne(u => u.Act)
                .WithMany(p => p.ActItems)
                .HasForeignKey(u => u.ActId)
                .OnDelete(DeleteBehavior.Restrict);

            //one-to-many Facility-Area
            modelBuilder.Entity<tArea>()
                .HasOne(u => u.Facility)
                .WithMany(p => p.Areas)
                .HasForeignKey(u => u.FacilityId)
                .OnDelete(DeleteBehavior.Restrict);

            //one-to-many FacilityShiftEng-Area
            modelBuilder.Entity<tArea>()
                .HasOne(u => u.ShiftEngFacility)
                .WithMany(p => p.AreasShiftEng)
                .HasForeignKey(u => u.ShiftEngFacilityId)
                .OnDelete(DeleteBehavior.Restrict);

            //one-to-many Facility-User
            modelBuilder.Entity<tUser>()
                .HasOne(u => u.Facility)
                .WithMany(p => p.Users)
                .HasForeignKey(u => u.FacilityId)
                .OnDelete(DeleteBehavior.Restrict);

            //one-to-Many User-ActHistory
            modelBuilder.Entity<ActHistory>()
                .HasOne(u => u.User)
                .WithMany(p => p.ActHistories)
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            //one-to-Many Act-ActHistory
            modelBuilder.Entity<ActHistory>()
                .HasOne(u => u.Act)
                .WithMany(p => p.ActHistories)
                .HasForeignKey(u => u.ActId)
                .OnDelete(DeleteBehavior.Restrict);

            //one-to-Many User-Act Creators
            modelBuilder.Entity<tAct>()
                .HasOne(u => u.CreateByUser)
                .WithMany(p => p.ActCreators)
                .HasForeignKey(u => u.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            //one-to-Many User-Act Closers
            modelBuilder.Entity<tAct>()
                .HasOne(u => u.ClosedByUser)
                .WithMany(p => p.ActClosers)
                .HasForeignKey(u => u.ClosedByUserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            //one-to-one Event-ActItems
            modelBuilder.Entity<Event>()
                .HasOne(u => u.ActItem)
                .WithOne(p => p.Event)
                .HasForeignKey<Event>(u => u.ActItemId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            modelBuilder.Entity<Area>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(50);
                entity.Property(e => e.StartingHour).HasDefaultValue(true);
                entity.Property(e => e.SplitToShift).HasDefaultValue(true);
                entity.Property(e => e.InterpolatedValuesCount).HasDefaultValue(true);
            });

            modelBuilder.Entity<Lcn>(entity =>
            {
                entity.ToTable("LCNs");

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<NModeCondition>(entity =>
            {
                entity.ToTable("NModeConditions");
                //entity.HasKey(e => e.Id).HasName("PK_Conditions");
                entity.Property(e => e.NModeRecordId).HasColumnName("NModeRecordId");
                entity.Property(e => e.Operator).HasMaxLength(3);
                entity.Property(e => e.Tagname).HasMaxLength(50);
                entity.Property(e => e.Value).HasMaxLength(10);

                entity.HasOne(d => d.NModeRecord).WithMany(p => p.Conditions)
                    .HasForeignKey(d => d.NModeRecordId)
                    .HasConstraintName("FK_Conditions_NModes");
            });

            modelBuilder.Entity<NModeRecord>(entity =>
            {
                entity.Property(e => e.ConditionORed)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasColumnName("ConditionORed");
                entity.Property(e => e.Descriptor).HasMaxLength(50);
                entity.Property(e => e.Nmode)
                    .HasMaxLength(50)
                    .HasColumnName("NMode");
                entity.Property(e => e.Operator)
                  .HasMaxLength(5)
                  .HasDefaultValue("=");
                entity.Property(e => e.Tagname).HasMaxLength(50);
                entity.Property(e => e.NModeORed)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasColumnName("NModeORed");
                entity.HasOne(d => d.Area).WithMany(p => p.NModeRecords)
                    .HasForeignKey(d => d.AreaId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_NModeRecords_Areas");

                entity.HasOne(d => d.Lcn).WithMany(p => p.NModeRecords)
                    .HasForeignKey(d => d.LcnId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_NModeRecords_LCNs");
            });

            modelBuilder.Entity<NModeResult>(entity =>
            {
                entity.HasOne(rr => rr.Record).WithMany(r => r.Results)
                    .HasForeignKey(r => r.RecordId);
                entity.Property(rr => rr.Evaluation).HasDefaultValue(true);

            });
            modelBuilder.Entity<NModeChangeRecord>(entity =>
            {
                entity.HasOne(rr => rr.Record).WithMany(r => r.Changes)
                    .HasForeignKey(r => r.RecordId);
            });
            modelBuilder.Entity<NMTotalRecord>(entity =>
            {
                entity.ToTable("NMTotalRecords");

                entity.Property(e => e.Tagname).HasMaxLength(50);

                entity.HasOne(d => d.Area).WithMany(p => p.NMTotalRecords)
                    .HasForeignKey(d => d.AreaId)
                    .HasConstraintName("FK_NMTotalRecords_Areas");

                entity.HasOne(d => d.Lcn).WithMany(p => p.NMTotalRecords)
                    .HasForeignKey(d => d.LcnId)
                    .HasConstraintName("FK_NMTotalRecords_LCNs");

                entity.HasOne(d => d.Parent).WithMany(p => p.SubTotals).HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK_NMTotalRecords_NMTotalRecords");
            });

        }
    }
}
