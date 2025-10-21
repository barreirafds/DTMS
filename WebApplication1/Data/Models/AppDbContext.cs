using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DTMS.Data.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<table> tables { get; set; }

    public virtual DbSet<user> users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<table>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.ToTable("table");

            entity.Property(e => e.id).ValueGeneratedNever();
        });

        modelBuilder.Entity<user>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.password).HasMaxLength(50);
            entity.Property(e => e.role).HasMaxLength(50);
            entity.Property(e => e.user1)
                .HasMaxLength(50)
                .HasColumnName("user");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
