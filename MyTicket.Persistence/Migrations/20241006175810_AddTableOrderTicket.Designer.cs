﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyTicket.Persistence;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyTicket.Persistence.Migrations
{
    [DbContext(typeof(MyTicketDbContext))]
    [Migration("20241006175810_AddTableOrderTicket")]
    partial class AddTableOrderTicket
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("MyTicket.Domain.Entities.Event", b =>
                {
                    b.Property<Guid>("EventId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("AvailableTickets")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("CreatedAtServer")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<string>("CreatedByFullName")
                        .HasColumnType("text");

                    b.Property<string>("CreatedByName")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("LastUpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("LastUpdatedAtServer")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("LastUpdatedBy")
                        .HasColumnType("text");

                    b.Property<string>("LastUpdatedByFullName")
                        .HasColumnType("text");

                    b.Property<string>("LastUpdatedByName")
                        .HasColumnType("text");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("EventId");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("MyTicket.Domain.Entities.OrderTicket", b =>
                {
                    b.Property<Guid>("OrderTicketId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("CreatedAtServer")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<string>("CreatedByFullName")
                        .HasColumnType("text");

                    b.Property<string>("CreatedByName")
                        .HasColumnType("text");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("EventId")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("LastUpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("LastUpdatedAtServer")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("LastUpdatedBy")
                        .HasColumnType("text");

                    b.Property<string>("LastUpdatedByFullName")
                        .HasColumnType("text");

                    b.Property<string>("LastUpdatedByName")
                        .HasColumnType("text");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("OrderTicketId");

                    b.HasIndex("EventId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("OrderTickets");
                });

            modelBuilder.Entity("MyTicket.Domain.Entities.User", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("Age")
                        .HasColumnType("integer");

                    b.Property<DateTime>("BirthDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("CreatedAtServer")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<string>("CreatedByFullName")
                        .HasColumnType("text");

                    b.Property<string>("CreatedByName")
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("LastUpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("LastUpdatedAtServer")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("LastUpdatedBy")
                        .HasColumnType("text");

                    b.Property<string>("LastUpdatedByFullName")
                        .HasColumnType("text");

                    b.Property<string>("LastUpdatedByName")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("MyTicket.Domain.Entities.UserPassword", b =>
                {
                    b.Property<Guid>("UserPasswordId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("CreatedAtServer")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<string>("CreatedByFullName")
                        .HasColumnType("text");

                    b.Property<string>("CreatedByName")
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("LastUpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("LastUpdatedAtServer")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("LastUpdatedBy")
                        .HasColumnType("text");

                    b.Property<string>("LastUpdatedByFullName")
                        .HasColumnType("text");

                    b.Property<string>("LastUpdatedByName")
                        .HasColumnType("text");

                    b.Property<DateTime>("NotActiveUntil")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<byte[]>("Salt")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("UserPasswordId");

                    b.HasIndex("UserId");

                    b.ToTable("UserPasswords");
                });

            modelBuilder.Entity("MyTicket.Domain.Entities.UserRole", b =>
                {
                    b.Property<Guid>("UserRoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("CreatedAtServer")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<string>("CreatedByFullName")
                        .HasColumnType("text");

                    b.Property<string>("CreatedByName")
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("LastUpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("LastUpdatedAtServer")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("LastUpdatedBy")
                        .HasColumnType("text");

                    b.Property<string>("LastUpdatedByFullName")
                        .HasColumnType("text");

                    b.Property<string>("LastUpdatedByName")
                        .HasColumnType("text");

                    b.Property<DateTime>("NotActiveUntil")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("UserRoleId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("MyTicket.Domain.Entities.OrderTicket", b =>
                {
                    b.HasOne("MyTicket.Domain.Entities.Event", "Event")
                        .WithMany()
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MyTicket.Domain.Entities.User", "User")
                        .WithOne("orderTicket")
                        .HasForeignKey("MyTicket.Domain.Entities.OrderTicket", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Event");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MyTicket.Domain.Entities.UserPassword", b =>
                {
                    b.HasOne("MyTicket.Domain.Entities.User", "User")
                        .WithMany("UserPasswords")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("MyTicket.Domain.Entities.UserRole", b =>
                {
                    b.HasOne("MyTicket.Domain.Entities.User", "User")
                        .WithOne("UserRoles")
                        .HasForeignKey("MyTicket.Domain.Entities.UserRole", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("MyTicket.Domain.Entities.User", b =>
                {
                    b.Navigation("UserPasswords");

                    b.Navigation("UserRoles");

                    b.Navigation("orderTicket");
                });
#pragma warning restore 612, 618
        }
    }
}
