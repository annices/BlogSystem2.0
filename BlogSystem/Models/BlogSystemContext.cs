using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BlogSystem.Models
{
    public partial class BlogSystemContext : DbContext
    {
        public BlogSystemContext()
        {
        }

        public BlogSystemContext(DbContextOptions<BlogSystemContext> options)
            : base(options)
        {
        }

        public virtual DbSet<BsCategory> BsCategories { get; set; }
        public virtual DbSet<BsComment> BsComments { get; set; }
        public virtual DbSet<BsEntry> BsEntries { get; set; }
        public virtual DbSet<BsEntryCategory> BsEntryCategories { get; set; }
        public virtual DbSet<BsUser> BsUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BsCategory>(entity =>
            {
                entity.ToTable("BS_Categories");

                entity.HasIndex(e => e.Category)
                    .HasName("UQ__BS_Categ__4BB73C324FA012C0")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Category)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<BsComment>(entity =>
            {
                entity.ToTable("BS_Comments");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Comment)
                    .IsRequired()
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.EntryId).HasColumnName("EntryID");

                entity.Property(e => e.Name)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Reply)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.Website)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Entry)
                    .WithMany(p => p.BsComments)
                    .HasForeignKey(d => d.EntryId)
                    .HasConstraintName("FK__BS_Commen__Entry__1DE57479");
            });

            modelBuilder.Entity<BsEntry>(entity =>
            {
                entity.ToTable("BS_Entries");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Entry)
                    .IsRequired()
                    .HasMaxLength(8000)
                    .IsUnicode(false);

                entity.Property(e => e.IsPublished).HasColumnName("IsPublished");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.BsEntries)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__BS_Entrie__UserI__173876EA");
            });

            modelBuilder.Entity<BsEntryCategory>(entity =>
            {
                entity.ToTable("BS_EntryCategories");

                entity.HasIndex(e => new { e.EntryId, e.CategoryId })
                    .HasName("Unique_EntryCategory")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.EntryId).HasColumnName("EntryID");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.BsEntryCategories)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__BS_EntryC__Categ__1B0907CE");

                entity.HasOne(d => d.Entry)
                    .WithMany(p => p.BsEntryCategories)
                    .HasForeignKey(d => d.EntryId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__BS_EntryC__Entry__1A14E395");
            });

            modelBuilder.Entity<BsUser>(entity =>
            {
                entity.ToTable("BS_Users");

                entity.HasIndex(e => e.Email)
                    .HasName("UQ__BS_Users__A9D10534AAC52413")
                    .IsUnique();

                entity.HasIndex(e => e.Username)
                    .HasName("UQ__BS_Users__536C85E4A787C76A")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Firstname)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Lastname)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
