﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using matcrm.data.Context;

namespace matcrm.data.Migrations
{
    [DbContext(typeof(OneClappContext))]
    [Migration("20220127132058_Add_Theme_Tables")]
    partial class Add_Theme_Tables
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityByDefaultColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("matcrm.data.Models.Tables.EmailTemplate", b =>
                {
                    b.Property<long>("EmailTemplateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .UseIdentityByDefaultColumn();

                    b.Property<DateTime?>("CreatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("TemplateCode")
                        .HasColumnType("varchar(5)");

                    b.Property<string>("TemplateHtml")
                        .HasColumnType("text");

                    b.Property<string>("TemplateName")
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("EmailTemplateId");

                    b.ToTable("EmailTemplate");
                });

            modelBuilder.Entity("matcrm.data.Models.Tables.ErrorLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<DateTime?>("CreatedTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("InnerException")
                        .HasColumnType("text");

                    b.Property<string>("Message")
                        .HasColumnType("text");

                    b.Property<string>("Source")
                        .HasColumnType("text");

                    b.Property<string>("StackTrace")
                        .HasColumnType("text");

                    b.Property<string>("TargetSite")
                        .HasColumnType("text");

                    b.Property<int?>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("ErrorLog");
                });

            modelBuilder.Entity("matcrm.data.Models.Tables.Language", b =>
                {
                    b.Property<int>("LanguageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<DateTime?>("CreatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("LanguageCode")
                        .HasColumnType("varchar(20)");

                    b.Property<string>("LanguageName")
                        .HasColumnType("varchar(150)");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("LanguageId");

                    b.ToTable("Language");
                });

            modelBuilder.Entity("matcrm.data.Models.Tables.OneClappLatestTheme", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .UseIdentityByDefaultColumn();

                    b.Property<string>("Accent")
                        .HasColumnType("varchar(500)");

                    b.Property<long?>("CreatedBy")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("CreatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(500)");

                    b.Property<string>("Primary")
                        .HasColumnType("varchar(500)");

                    b.Property<long?>("UpdatedBy")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Warn")
                        .HasColumnType("varchar(500)");

                    b.HasKey("Id");

                    b.ToTable("OneClappLatestTheme");
                });

            modelBuilder.Entity("matcrm.data.Models.Tables.OneClappLatestThemeConfig", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .UseIdentityByDefaultColumn();

                    b.Property<long?>("CreatedBy")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("CreatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<long?>("OneClappLatestThemeId")
                        .HasColumnType("bigint");

                    b.Property<long?>("OneClappLatestThemeLayoutId")
                        .HasColumnType("bigint");

                    b.Property<long?>("OneClappLatestThemeSchemeId")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("OneClappLatestThemeId");

                    b.HasIndex("OneClappLatestThemeLayoutId");

                    b.HasIndex("OneClappLatestThemeSchemeId");

                    b.ToTable("OneClappLatestThemeConfig");
                });

            modelBuilder.Entity("matcrm.data.Models.Tables.OneClappLatestThemeLayout", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .UseIdentityByDefaultColumn();

                    b.Property<long?>("CreatedBy")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("CreatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(500)");

                    b.Property<string>("TemplateHtml")
                        .HasColumnType("text");

                    b.Property<long?>("UpdatedBy")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.ToTable("OneClappLatestThemeLayout");
                });

            modelBuilder.Entity("matcrm.data.Models.Tables.OneClappLatestThemeScheme", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .UseIdentityByDefaultColumn();

                    b.Property<long?>("CreatedBy")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("CreatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(500)");

                    b.Property<string>("TemplateHtml")
                        .HasColumnType("text");

                    b.Property<long?>("UpdatedBy")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.ToTable("OneClappLatestThemeScheme");
                });

            modelBuilder.Entity("matcrm.data.Models.Tables.Role", b =>
                {
                    b.Property<int>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<long?>("CreatedBy")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("CreatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<long?>("DeletedBy")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("RoleName")
                        .HasColumnType("varchar(150)");

                    b.Property<long?>("TenantId")
                        .HasColumnType("bigint");

                    b.Property<long?>("UpdatedBy")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("RoleId");

                    b.ToTable("Role");
                });

            modelBuilder.Entity("matcrm.data.Models.Tables.Tenant", b =>
                {
                    b.Property<int>("TenantId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<int?>("BlockedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("BlockedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("CreatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsBlocked")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("TenantName")
                        .HasColumnType("text");

                    b.Property<string>("Token")
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .HasColumnType("text");

                    b.HasKey("TenantId");

                    b.ToTable("Tenants");
                });

            modelBuilder.Entity("matcrm.data.Models.Tables.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<string>("Address")
                        .HasColumnType("text");

                    b.Property<int?>("BlockedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("BlockedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("CreatedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("CreatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("DOB")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("DeletedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("DialCode")
                        .HasColumnType("integer");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<bool>("IsBlocked")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsEmailVerified")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("LastLoggedIn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<long?>("OneClappLatestThemeConfigId")
                        .HasColumnType("bigint");

                    b.Property<byte[]>("PasswordHash")
                        .HasColumnType("bytea");

                    b.Property<byte[]>("PasswordSalt")
                        .HasColumnType("bytea");

                    b.Property<string>("PhoneNo")
                        .HasColumnType("text");

                    b.Property<string>("ProfileImage")
                        .HasColumnType("varchar(1000)");

                    b.Property<int?>("RoleId")
                        .HasColumnType("integer");

                    b.Property<string>("TempGuid")
                        .HasColumnType("varchar(150)");

                    b.Property<int?>("TenantId")
                        .HasColumnType("integer");

                    b.Property<int?>("UpdatedBy")
                        .HasColumnType("integer");

                    b.Property<string>("UserName")
                        .HasColumnType("text");

                    b.Property<DateTime?>("VerifiedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("WeClappToken")
                        .HasColumnType("text");

                    b.Property<int?>("WeClappUserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("OneClappLatestThemeConfigId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("matcrm.data.Models.Tables.VerificationCode", b =>
                {
                    b.Property<long>("VerificationCodeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .UseIdentityByDefaultColumn();

                    b.Property<string>("Code")
                        .HasColumnType("varchar(10)");

                    b.Property<DateTime?>("CreatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<DateTime?>("ExpiredOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsExpired")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsUsed")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("UserId")
                        .HasColumnType("integer");

                    b.Property<string>("VerificationFor")
                        .HasColumnType("varchar(50)");

                    b.HasKey("VerificationCodeId");

                    b.ToTable("VerificationCode");
                });

            modelBuilder.Entity("matcrm.data.Models.Tables.OneClappLatestThemeConfig", b =>
                {
                    b.HasOne("matcrm.data.Models.Tables.OneClappLatestTheme", "OneClappLatestTheme")
                        .WithMany()
                        .HasForeignKey("OneClappLatestThemeId");

                    b.HasOne("matcrm.data.Models.Tables.OneClappLatestThemeLayout", "OneClappLatestThemeLayout")
                        .WithMany()
                        .HasForeignKey("OneClappLatestThemeLayoutId");

                    b.HasOne("matcrm.data.Models.Tables.OneClappLatestThemeScheme", "OneClappLatestThemeScheme")
                        .WithMany()
                        .HasForeignKey("OneClappLatestThemeSchemeId");

                    b.Navigation("OneClappLatestTheme");

                    b.Navigation("OneClappLatestThemeLayout");

                    b.Navigation("OneClappLatestThemeScheme");
                });

            modelBuilder.Entity("matcrm.data.Models.Tables.User", b =>
                {
                    b.HasOne("matcrm.data.Models.Tables.OneClappLatestThemeConfig", "OneClappLatestThemeConfig")
                        .WithMany()
                        .HasForeignKey("OneClappLatestThemeConfigId");

                    b.Navigation("OneClappLatestThemeConfig");
                });
#pragma warning restore 612, 618
        }
    }
}