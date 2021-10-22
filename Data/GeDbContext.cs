using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace GopherExchange.Data
{
    public partial class GeDbContext : DbContext
    {
        public GeDbContext()
        {
        }

        public GeDbContext(DbContextOptions<GeDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Contain> Contains { get; set; }
        public virtual DbSet<File> Files { get; set; }
        public virtual DbSet<Listing> Listings { get; set; }
        public virtual DbSet<Report> Reports { get; set; }
        public virtual DbSet<Wishlist> Wishlists { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Name=DBConnection");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "C");

            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => e.Userid)
                    .HasName("account_pkey");

                entity.ToTable("account");

                entity.Property(e => e.Userid).HasColumnName("userid");

                entity.Property(e => e.Accounttype).HasColumnName("accounttype");

                entity.Property(e => e.Goucheremail)
                    .IsRequired()
                    .HasColumnName("goucheremail");

                entity.Property(e => e.Hashedpassword)
                    .IsRequired()
                    .HasColumnName("hashedpassword");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("username");
            });

            modelBuilder.Entity<Contain>(entity =>
            {
                entity.HasKey(e => new { e.Listingid, e.Wishlistid });

                entity.ToTable("contains");

                entity.Property(e => e.Listingid)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("listingid");

                entity.Property(e => e.Wishlistid)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("wishlistid");

                entity.HasOne(d => d.Listing)
                    .WithMany()
                    .HasForeignKey(d => d.Listingid)
                    .HasConstraintName("contains_listingid_fkey");

                entity.HasOne(d => d.Wishlist)
                    .WithMany()
                    .HasForeignKey(d => d.Wishlistid)
                    .HasConstraintName("contains_wishlistid_fkey");
            });

            modelBuilder.Entity<File>(entity =>
            {
                entity.HasKey(e => new { e.Userid, e.Reportid });

                entity.ToTable("files");

                entity.Property(e => e.Reportid)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("reportid");

                entity.Property(e => e.Userid)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("userid");

                entity.HasOne(d => d.Report)
                    .WithMany()
                    .HasForeignKey(d => d.Reportid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("files_reportid_fkey");

                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.Userid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("files_userid_fkey");
            });

            modelBuilder.Entity<Listing>(entity =>
            {
                entity.ToTable("listing");

                entity.Property(e => e.Listingid).HasColumnName("listingid");

                entity.Property(e => e.Date)
                            .HasColumnType("timestamp with time zone")
                            .HasColumnName("date");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Title).HasColumnName("title");

                entity.Property(e => e.Typeid).HasColumnName("typeid");

                entity.Property(e => e.Userid)
                            .ValueGeneratedOnAdd()
                            .HasColumnName("userid");

                entity.HasOne(d => d.User)
                            .WithMany(p => p.Listings)
                            .HasForeignKey(d => d.Userid)
                            .HasConstraintName("listing_userid_fkey");
            });

            modelBuilder.Entity<Report>(entity =>
            {
                entity.ToTable("report");

                entity.Property(e => e.Reportid).HasColumnName("reportid");

                entity.Property(e => e.Action).HasColumnName("action");

                entity.Property(e => e.Actiondate)
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("actiondate");

                entity.Property(e => e.Adminid)
                                .ValueGeneratedOnAdd()
                                .HasColumnName("adminid");

                entity.Property(e => e.Description)
                                .IsRequired()
                                .HasColumnName("description");

                entity.Property(e => e.Incidentdate)
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("incidentdate");

                entity.Property(e => e.Incidentid).HasColumnName("incidentid");

                entity.Property(e => e.Listingid)
                                .ValueGeneratedOnAdd()
                                .HasColumnName("listingid");

                entity.HasOne(d => d.Listing)
                                .WithMany(p => p.Reports)
                                .HasForeignKey(d => d.Listingid)
                                .OnDelete(DeleteBehavior.ClientSetNull)
                                .HasConstraintName("report_listingid_fkey");
            });

            modelBuilder.Entity<Wishlist>(entity =>
            {
                entity.ToTable("wishlist");

                entity.Property(e => e.Wishlistid).HasColumnName("wishlistid");

                entity.Property(e => e.Userid)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("userid");

                entity.Property(e => e.Title)
                    .HasColumnName("title");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Wishlists)
                    .HasForeignKey(d => d.Userid)
                    .HasConstraintName("wishlist_userid_fkey");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
