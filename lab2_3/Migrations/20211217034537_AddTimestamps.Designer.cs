﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Trs.DataManager;

#nullable disable

namespace Trs.Migrations
{
    [DbContext(typeof(TrsDbContext))]
    [Migration("20211217034537_AddTimestamps")]
    partial class AddTimestamps
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.1");

            modelBuilder.Entity("Trs.Models.DomainModels.AcceptedTime", b =>
                {
                    b.Property<int>("ReportId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ProjectCode")
                        .HasColumnType("TEXT");

                    b.Property<int>("Time")
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("BLOB");

                    b.HasKey("ReportId", "ProjectCode");

                    b.HasIndex("ProjectCode");

                    b.ToTable("AcceptedTime");
                });

            modelBuilder.Entity("Trs.Models.DomainModels.Category", b =>
                {
                    b.Property<string>("ProjectCode")
                        .HasColumnType("TEXT");

                    b.Property<string>("Code")
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("BLOB");

                    b.HasKey("ProjectCode", "Code");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("Trs.Models.DomainModels.Project", b =>
                {
                    b.Property<string>("Code")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Active")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Budget")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ManagerId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("BLOB");

                    b.HasKey("Code");

                    b.HasIndex("ManagerId");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("Trs.Models.DomainModels.Report", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Frozen")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Month")
                        .HasColumnType("TEXT");

                    b.Property<int>("OwnerId")
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("BLOB");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId", "Month")
                        .IsUnique();

                    b.ToTable("Reports");
                });

            modelBuilder.Entity("Trs.Models.DomainModels.ReportEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CategoryCode")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ProjectCode")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("ReportId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Time")
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("BLOB");

                    b.HasKey("Id");

                    b.HasIndex("ReportId");

                    b.HasIndex("ProjectCode", "CategoryCode");

                    b.ToTable("ReportEntries");
                });

            modelBuilder.Entity("Trs.Models.DomainModels.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("BLOB");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Trs.Models.DomainModels.AcceptedTime", b =>
                {
                    b.HasOne("Trs.Models.DomainModels.Project", "Project")
                        .WithMany("AcceptedTime")
                        .HasForeignKey("ProjectCode")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Trs.Models.DomainModels.Report", "Report")
                        .WithMany("AcceptedTime")
                        .HasForeignKey("ReportId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");

                    b.Navigation("Report");
                });

            modelBuilder.Entity("Trs.Models.DomainModels.Category", b =>
                {
                    b.HasOne("Trs.Models.DomainModels.Project", "Project")
                        .WithMany("Categories")
                        .HasForeignKey("ProjectCode")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");
                });

            modelBuilder.Entity("Trs.Models.DomainModels.Project", b =>
                {
                    b.HasOne("Trs.Models.DomainModels.User", "Manager")
                        .WithMany("Projects")
                        .HasForeignKey("ManagerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Manager");
                });

            modelBuilder.Entity("Trs.Models.DomainModels.Report", b =>
                {
                    b.HasOne("Trs.Models.DomainModels.User", "Owner")
                        .WithMany("Reports")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("Trs.Models.DomainModels.ReportEntry", b =>
                {
                    b.HasOne("Trs.Models.DomainModels.Project", "Project")
                        .WithMany("ReportEntries")
                        .HasForeignKey("ProjectCode")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Trs.Models.DomainModels.Report", "Report")
                        .WithMany("ReportEntries")
                        .HasForeignKey("ReportId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Trs.Models.DomainModels.Category", "Category")
                        .WithMany()
                        .HasForeignKey("ProjectCode", "CategoryCode");

                    b.Navigation("Category");

                    b.Navigation("Project");

                    b.Navigation("Report");
                });

            modelBuilder.Entity("Trs.Models.DomainModels.Project", b =>
                {
                    b.Navigation("AcceptedTime");

                    b.Navigation("Categories");

                    b.Navigation("ReportEntries");
                });

            modelBuilder.Entity("Trs.Models.DomainModels.Report", b =>
                {
                    b.Navigation("AcceptedTime");

                    b.Navigation("ReportEntries");
                });

            modelBuilder.Entity("Trs.Models.DomainModels.User", b =>
                {
                    b.Navigation("Projects");

                    b.Navigation("Reports");
                });
#pragma warning restore 612, 618
        }
    }
}