﻿// <auto-generated />
using System;
using KeyGenerator.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace KeyGenerator.Migrations
{
    [DbContext(typeof(KeyGeneratorDBContext))]
    partial class KeyGeneratorDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("KeyGenerator.Models.Answers", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Answer")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("varchar(10)");

                    b.Property<string>("CatchNumber")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("PageNumber")
                        .HasColumnType("int");

                    b.Property<int>("PaperID")
                        .HasColumnType("int");

                    b.Property<int>("ProgID")
                        .HasColumnType("int");

                    b.Property<int>("QuestionNumber")
                        .HasColumnType("int");

                    b.Property<int>("SerialNumber")
                        .HasColumnType("int");

                    b.Property<int>("SetID")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("AnswersKeys");
                });

            modelBuilder.Entity("KeyGenerator.Models.CType", b =>
                {
                    b.Property<int>("TypeID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("TypeID"));

                    b.Property<string>("TypeName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("TypeID");

                    b.ToTable("Types");
                });

            modelBuilder.Entity("KeyGenerator.Models.Course", b =>
                {
                    b.Property<int>("CourseID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("CourseID"));

                    b.Property<string>("CourseName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("CourseID");

                    b.ToTable("Courses");
                });

            modelBuilder.Entity("KeyGenerator.Models.ErrorLog", b =>
                {
                    b.Property<int>("ErrorID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("ErrorID"));

                    b.Property<string>("Error")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("LoggedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Message")
                        .HasColumnType("longtext");

                    b.Property<string>("OccuranceSpace")
                        .HasColumnType("longtext");

                    b.HasKey("ErrorID");

                    b.ToTable("ErrorLogs");
                });

            modelBuilder.Entity("KeyGenerator.Models.EventLog", b =>
                {
                    b.Property<int>("EventID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("EventID"));

                    b.Property<string>("Category")
                        .HasColumnType("longtext");

                    b.Property<string>("Event")
                        .HasColumnType("longtext");

                    b.Property<int>("EventTriggeredBy")
                        .HasColumnType("int");

                    b.Property<DateTime>("LoggedAT")
                        .HasColumnType("datetime(6)");

                    b.HasKey("EventID");

                    b.ToTable("EventLogs");
                });

            modelBuilder.Entity("KeyGenerator.Models.ExamType", b =>
                {
                    b.Property<int>("ExamTypeID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("ExamTypeID"));

                    b.Property<string>("ExamTypeName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("ExamTypeID");

                    b.ToTable("ExamTypes");
                });

            modelBuilder.Entity("KeyGenerator.Models.Group", b =>
                {
                    b.Property<int>("GroupID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("GroupID"));

                    b.Property<string>("GroupName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("status")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("GroupID");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("KeyGenerator.Models.Module", b =>
                {
                    b.Property<int>("ModuleID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("ModuleID"));

                    b.Property<string>("ModuleName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("ModuleID");

                    b.ToTable("Modules");

                    b.HasData(
                        new
                        {
                            ModuleID = 1,
                            ModuleName = "Users"
                        },
                        new
                        {
                            ModuleID = 2,
                            ModuleName = "Key Generator"
                        },
                        new
                        {
                            ModuleID = 3,
                            ModuleName = "Masters"
                        });
                });

            modelBuilder.Entity("KeyGenerator.Models.Paper", b =>
                {
                    b.Property<int>("PaperID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("PaperID"));

                    b.Property<int?>("BookletSize")
                        .HasColumnType("int");

                    b.Property<string>("CatchNumber")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int?>("CourseID")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("CreatedByID")
                        .HasColumnType("int");

                    b.Property<DateTime>("ExamDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("ExamType")
                        .HasColumnType("longtext");

                    b.Property<bool>("KeyGenerated")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("MasterUploaded")
                        .HasColumnType("tinyint(1)");

                    b.Property<int?>("NumberofQuestion")
                        .HasColumnType("int");

                    b.Property<string>("PaperCode")
                        .HasColumnType("longtext");

                    b.Property<string>("PaperName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("PaperNumber")
                        .HasColumnType("longtext");

                    b.Property<int>("ProgrammeID")
                        .HasColumnType("int");

                    b.Property<int?>("SubjectID")
                        .HasColumnType("int");

                    b.HasKey("PaperID");

                    b.ToTable("Papers");
                });

            modelBuilder.Entity("KeyGenerator.Models.Permission", b =>
                {
                    b.Property<int>("PermissionID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("PermissionID"));

                    b.Property<bool>("Can_Add")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("Can_Delete")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("Can_Update")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("Can_View")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("ModuleID")
                        .HasColumnType("int");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("PermissionID");

                    b.HasIndex("UserID", "ModuleID")
                        .IsUnique();

                    b.ToTable("Permissions");

                    b.HasData(
                        new
                        {
                            PermissionID = 1,
                            Can_Add = true,
                            Can_Delete = true,
                            Can_Update = true,
                            Can_View = true,
                            ModuleID = 1,
                            UserID = 1
                        },
                        new
                        {
                            PermissionID = 2,
                            Can_Add = true,
                            Can_Delete = true,
                            Can_Update = true,
                            Can_View = true,
                            ModuleID = 2,
                            UserID = 1
                        },
                        new
                        {
                            PermissionID = 3,
                            Can_Add = true,
                            Can_Delete = true,
                            Can_Update = true,
                            Can_View = true,
                            ModuleID = 3,
                            UserID = 1
                        });
                });

            modelBuilder.Entity("KeyGenerator.Models.ProgConfig", b =>
                {
                    b.Property<int>("ProgConfigID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("ProgConfigID"));

                    b.Property<int>("BookletSize")
                        .HasColumnType("int");

                    b.Property<int>("NumberofJumblingSteps")
                        .HasColumnType("int");

                    b.Property<int>("NumberofQuestions")
                        .HasColumnType("int");

                    b.Property<int>("ProgID")
                        .HasColumnType("int");

                    b.Property<string>("SetOrder")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("SetofStepsID")
                        .HasColumnType("int");

                    b.Property<int>("Sets")
                        .HasColumnType("int");

                    b.HasKey("ProgConfigID");

                    b.ToTable("ProgConfigs");
                });

            modelBuilder.Entity("KeyGenerator.Models.Programme", b =>
                {
                    b.Property<int>("ProgrammeID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("ProgrammeID"));

                    b.Property<int>("GroupID")
                        .HasColumnType("int");

                    b.Property<string>("ProgrammeName")
                        .HasColumnType("longtext");

                    b.Property<int>("SessionID")
                        .HasColumnType("int");

                    b.Property<int>("TypeID")
                        .HasColumnType("int");

                    b.HasKey("ProgrammeID");

                    b.ToTable("Programmes");
                });

            modelBuilder.Entity("KeyGenerator.Models.Session", b =>
                {
                    b.Property<int>("SessionID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("SessionID"));

                    b.Property<string>("SessionName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("SessionID");

                    b.ToTable("Sessions");

                    b.HasData(
                        new
                        {
                            SessionID = 1,
                            SessionName = "2024-25"
                        });
                });

            modelBuilder.Entity("KeyGenerator.Models.SetofStep", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("ID"));

                    b.Property<int>("ProgConfigID")
                        .HasColumnType("int");

                    b.Property<int>("StepID")
                        .HasColumnType("int");

                    b.Property<string>("steps")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("ID");

                    b.ToTable("SetofSteps");
                });

            modelBuilder.Entity("KeyGenerator.Models.Subject", b =>
                {
                    b.Property<int>("SubjectID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("SubjectID"));

                    b.Property<string>("SubjectName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("SubjectID");

                    b.ToTable("Subjects");
                });

            modelBuilder.Entity("KeyGenerator.Models.User", b =>
                {
                    b.Property<int>("UserID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("UserID"));

                    b.Property<string>("Designation")
                        .HasColumnType("longtext");

                    b.Property<string>("EmailAddress")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("LastName")
                        .HasColumnType("longtext");

                    b.Property<string>("MiddleName")
                        .HasColumnType("longtext");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("longtext");

                    b.Property<string>("ProfilePicturePath")
                        .HasColumnType("longtext");

                    b.Property<bool>("Status")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("UserID");

                    b.HasIndex("EmailAddress")
                        .IsUnique();

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            UserID = 1,
                            Designation = "SeniorDev",
                            EmailAddress = "kay.g.3599@gmail.com",
                            FirstName = "Kartikey",
                            LastName = "Gupta",
                            MiddleName = "",
                            PhoneNumber = "7459076207",
                            ProfilePicturePath = "",
                            Status = true
                        });
                });

            modelBuilder.Entity("KeyGenerator.Models.UserAuth", b =>
                {
                    b.Property<int>("UserAuthID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("UserAuthID"));

                    b.Property<bool>("AutoGenPass")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("UserAuthID");

                    b.ToTable("UserAuthentication");

                    b.HasData(
                        new
                        {
                            UserAuthID = 1,
                            AutoGenPass = false,
                            Password = "2232fffae0886cc010ea81b6e5154c28b55bdc10138d9bee30ee38945e0c33f5",
                            UserID = 1
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
