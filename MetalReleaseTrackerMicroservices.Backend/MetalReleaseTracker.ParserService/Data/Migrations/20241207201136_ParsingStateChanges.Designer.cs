﻿// <auto-generated />
using System;
using MetalReleaseTracker.ParserService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MetalReleaseTracker.ParserService.Migrations
{
    [DbContext(typeof(ParserServiceDbContext))]
    [Migration("20241207201136_ParsingStateChanges")]
    partial class ParsingStateChanges
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("MetalReleaseTracker.ParserService.Data.Entities.AlbumParsedEventEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("EventPayload")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("ParsingStateId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ParsingStateId");

                    b.ToTable("AlbumParsedEvents");
                });

            modelBuilder.Entity("MetalReleaseTracker.ParserService.Data.Entities.ParsingStateEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("DistributorCode")
                        .HasColumnType("integer");

                    b.Property<DateTime>("LastUpdatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("NextPageToProcess")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ParsingStatus")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("ParsingStates");
                });

            modelBuilder.Entity("MetalReleaseTracker.ParserService.Data.Entities.AlbumParsedEventEntity", b =>
                {
                    b.HasOne("MetalReleaseTracker.ParserService.Data.Entities.ParsingStateEntity", "ParsingState")
                        .WithMany()
                        .HasForeignKey("ParsingStateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ParsingState");
                });
#pragma warning restore 612, 618
        }
    }
}
