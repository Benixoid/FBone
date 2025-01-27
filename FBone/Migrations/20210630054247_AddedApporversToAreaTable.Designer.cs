﻿// <auto-generated />
using System;
using FBone.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FBone.Migrations
{
    [DbContext(typeof(AppDBContext))]
    [Migration("20210630054247_AddedApporversToAreaTable")]
    partial class AddedApporversToAreaTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.14-servicing-32113")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("FBone.Database.Entities.tAct", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ActNotes");

                    b.Property<int>("AreaId");

                    b.Property<string>("CauseEN");

                    b.Property<string>("CauseKK");

                    b.Property<string>("CauseRU");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<byte>("Crew");

                    b.Property<string>("ImpactEN");

                    b.Property<string>("ImpactKK");

                    b.Property<string>("ImpactRU");

                    b.Property<bool>("IsTranslated");

                    b.Property<string>("OriginalLang")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue("ru");

                    b.Property<string>("ProtectEN");

                    b.Property<string>("ProtectKK");

                    b.Property<string>("ProtectRU");

                    b.Property<int>("StatusId");

                    b.Property<byte>("Type");

                    b.HasKey("Id");

                    b.HasIndex("AreaId");

                    b.ToTable("tAct");
                });

            modelBuilder.Entity("FBone.Database.Entities.tActCause", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name_EN")
                        .IsRequired();

                    b.Property<string>("Name_KK")
                        .IsRequired();

                    b.Property<string>("Name_RU")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("tActCause");
                });

            modelBuilder.Entity("FBone.Database.Entities.tActImpact", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name_EN")
                        .IsRequired();

                    b.Property<string>("Name_KK")
                        .IsRequired();

                    b.Property<string>("Name_RU")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("tActImpact");
                });

            modelBuilder.Entity("FBone.Database.Entities.tActProtect", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name_EN")
                        .IsRequired();

                    b.Property<string>("Name_KK")
                        .IsRequired();

                    b.Property<string>("Name_RU")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("tActProtect");
                });

            modelBuilder.Entity("FBone.Database.Entities.tArea", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Approver1_1");

                    b.Property<int>("Approver1_2");

                    b.Property<int>("Approver1_3");

                    b.Property<int>("Approver1_4");

                    b.Property<int>("Approver1_5");

                    b.Property<int>("Approver1_6");

                    b.Property<int>("Approver1_7");

                    b.Property<int>("Approver2_1");

                    b.Property<int>("Approver2_2");

                    b.Property<int>("Approver2_3");

                    b.Property<int>("Approver2_4");

                    b.Property<int>("Approver2_5");

                    b.Property<int>("Approver2_6");

                    b.Property<int>("Approver2_7");

                    b.Property<int>("Approver3_1");

                    b.Property<int>("Approver3_2");

                    b.Property<int>("Approver3_3");

                    b.Property<int>("Approver3_4");

                    b.Property<int>("Approver3_5");

                    b.Property<int>("Approver3_6");

                    b.Property<int>("Approver3_7");

                    b.Property<int>("Approver4_1");

                    b.Property<int>("Approver4_2");

                    b.Property<int>("Approver4_3");

                    b.Property<int>("Approver4_4");

                    b.Property<int>("Approver4_5");

                    b.Property<int>("Approver4_6");

                    b.Property<int>("Approver4_7");

                    b.Property<string>("ConnectionString");

                    b.Property<string>("EncryptedPassword");

                    b.Property<DateTime>("LastImportDate");

                    b.Property<int>("LastRecordId");

                    b.Property<string>("Name_EN")
                        .IsRequired();

                    b.Property<string>("Name_KK")
                        .IsRequired();

                    b.Property<string>("Name_RU")
                        .IsRequired();

                    b.Property<string>("SQLquery");

                    b.HasKey("Id");

                    b.ToTable("tArea");
                });

            modelBuilder.Entity("FBone.Database.Entities.tPosition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("CanCreateAct");

                    b.Property<bool>("CanTranslateAct");

                    b.Property<bool>("IsShiftEngineer");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("tPosition");
                });

            modelBuilder.Entity("FBone.Database.Entities.tUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AreaId");

                    b.Property<string>("CAI")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<bool>("IsActive");

                    b.Property<bool>("IsAdmin");

                    b.Property<string>("Name_EN")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("Name_KK")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("Name_RU")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("PositionId");

                    b.Property<string>("lang")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("AreaId");

                    b.HasIndex("PositionId");

                    b.ToTable("tUser");
                });

            modelBuilder.Entity("FBone.Database.Entities.tAct", b =>
                {
                    b.HasOne("FBone.Database.Entities.tArea", "Area")
                        .WithMany("Acts")
                        .HasForeignKey("AreaId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("FBone.Database.Entities.tUser", b =>
                {
                    b.HasOne("FBone.Database.Entities.tArea", "Area")
                        .WithMany()
                        .HasForeignKey("AreaId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("FBone.Database.Entities.tPosition", "Position")
                        .WithMany("Users")
                        .HasForeignKey("PositionId")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}
