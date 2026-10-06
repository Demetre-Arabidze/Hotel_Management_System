using HMS.Domain.Entities;
using HMS.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
    
namespace HMS.Infrastructure.Data
{
    public class ApplicationDbContext
        : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Hotel> Hotels => Set<Hotel>();
        public DbSet<Manager> Managers => Set<Manager>();
        public DbSet<Room> Rooms => Set<Room>();
        public DbSet<Guest> Guests => Set<Guest>();
        public DbSet<Reservation> Reservations => Set<Reservation>();
        public DbSet<ReservationRoom> ReservationRooms => Set<ReservationRoom>();
        public DbSet<Review> Reviews => Set<Review>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            ConfigureHotel(builder);
            ConfigureManager(builder);
            ConfigureRoom(builder);
            ConfigureGuest(builder);
            ConfigureReservation(builder);
            ConfigureReservationRoom(builder);
            ConfigureReview(builder);

            builder.SeedData();
        }


        #region Configuration
        private static void ConfigureHotel(ModelBuilder builder)
        {
            builder.Entity<Hotel>(entity =>
            {
                entity.ToTable("Hotels", table =>
                    table.HasCheckConstraint(
                        "CK_Hotels_Rating",
                        "[Rating] >= 0.0 AND [Rating] <= 5"));

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.Name)
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(x => x.Rating)
                    .HasPrecision(2, 1)
                    .IsRequired();

                entity.Property(x => x.Country)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.City)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.Address)
                    .HasMaxLength(250)
                    .IsRequired();
            });
        }

        private static void ConfigureManager(ModelBuilder builder)
        {
            builder.Entity<Manager>(entity =>
            {
                entity.ToTable("Managers");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.FirstName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.LastName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.PersonalNumber)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(x => x.Email)
                    .HasMaxLength(256)
                    .IsRequired();

                entity.Property(x => x.PhoneNumber)
                    .HasMaxLength(30)
                    .IsRequired();

                entity.HasIndex(x => x.PersonalNumber)
                    .IsUnique();

                entity.HasIndex(x => x.Email)
                    .IsUnique();

                entity.HasOne(x => x.Hotel)
                    .WithMany(x => x.Managers)
                    .HasForeignKey(x => x.HotelId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<ApplicationUser>()
                    .WithOne()
                    .HasForeignKey<Manager>(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigureRoom(ModelBuilder builder)
        {
            builder.Entity<Room>(entity =>
            {
                entity.ToTable("Rooms", table =>
                    table.HasCheckConstraint(
                        "CK_Rooms_Price",
                        "[Price] > 0"));

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.Name)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.Price)
                    .HasPrecision(18, 2)
                    .IsRequired();

                entity.HasOne(x => x.Hotel)
                    .WithMany(x => x.Rooms)
                    .HasForeignKey(x => x.HotelId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigureGuest(ModelBuilder builder)
        {
            builder.Entity<Guest>(entity =>
            {
                entity.ToTable("Guests");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.FirstName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.LastName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.PersonalNumber)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(x => x.PhoneNumber)
                    .HasMaxLength(30)
                    .IsRequired();

                entity.HasIndex(x => x.PersonalNumber)
                    .IsUnique();

                entity.HasIndex(x => x.PhoneNumber)
                    .IsUnique();

                entity.HasOne<ApplicationUser>()
                    .WithOne()
                    .HasForeignKey<Guest>(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigureReservation(ModelBuilder builder)
        {
            builder.Entity<Reservation>(entity =>
            {
                entity.ToTable("Reservations", table =>
                    table.HasCheckConstraint(
                        "CK_Reservations_Dates",
                        "[CheckOutDate] > [CheckInDate]"));

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.CheckInDate)
                    .HasColumnType("date")
                    .IsRequired();

                entity.Property(x => x.CheckOutDate)
                    .HasColumnType("date")
                    .IsRequired();

                entity.HasOne(x => x.Guest)
                    .WithMany(x => x.Reservations)
                    .HasForeignKey(x => x.GuestId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigureReservationRoom(ModelBuilder builder)
        {
            builder.Entity<ReservationRoom>(entity =>
            {
                entity.ToTable("ReservationRooms");

                entity.HasKey(x => new { x.ReservationId, x.RoomId });

                entity.HasOne(x => x.Reservation)
                    .WithMany(x => x.ReservationRooms)
                    .HasForeignKey(x => x.ReservationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.Room)
                    .WithMany(x => x.ReservationRooms)
                    .HasForeignKey(x => x.RoomId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigureReview(ModelBuilder builder)
        {
            builder.Entity<Review>(builder =>
            {
                builder.HasKey(r => r.Id);
                builder.Property(r => r.Rating).IsRequired();
                builder.Property(r => r.Comment).HasMaxLength(1000);

                builder.HasOne(r => r.Hotel)
                       .WithMany(h => h.Reviews)
                       .HasForeignKey(r => r.HotelId)
                       .OnDelete(DeleteBehavior.Cascade);
            });
        }
        #endregion
    }
}
