﻿// <auto-generated />
using Heroes.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace hero_api_test.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250307181221_AddingTeamsTable")]
    partial class AddingTeamsTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Heroes.Models.Hability", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("HeroId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("VillainId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("HeroId");

                    b.HasIndex("VillainId");

                    b.ToTable("Habilities");
                });

            modelBuilder.Entity("Heroes.Models.Hero", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Hero_name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Power")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("TeamId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("Hero_name")
                        .IsUnique();

                    b.HasIndex("TeamId");

                    b.ToTable("Heroes");
                });

            modelBuilder.Entity("Heroes.Models.Team", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("TeamName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("TeamName")
                        .IsUnique();

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("Heroes.Models.Villain", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("TeamId")
                        .HasColumnType("integer");

                    b.Property<string>("Villain_Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("TeamId");

                    b.HasIndex("Villain_Name")
                        .IsUnique();

                    b.ToTable("Villains");
                });

            modelBuilder.Entity("Heroes.Models.Hability", b =>
                {
                    b.HasOne("Heroes.Models.Hero", "Hero")
                        .WithMany("Habilities")
                        .HasForeignKey("HeroId");

                    b.HasOne("Heroes.Models.Villain", "Villain")
                        .WithMany("Habilities")
                        .HasForeignKey("VillainId");

                    b.Navigation("Hero");

                    b.Navigation("Villain");
                });

            modelBuilder.Entity("Heroes.Models.Hero", b =>
                {
                    b.HasOne("Heroes.Models.Team", "Team")
                        .WithMany()
                        .HasForeignKey("TeamId");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("Heroes.Models.Villain", b =>
                {
                    b.HasOne("Heroes.Models.Team", "Team")
                        .WithMany()
                        .HasForeignKey("TeamId");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("Heroes.Models.Hero", b =>
                {
                    b.Navigation("Habilities");
                });

            modelBuilder.Entity("Heroes.Models.Villain", b =>
                {
                    b.Navigation("Habilities");
                });
#pragma warning restore 612, 618
        }
    }
}
