using HMS.Domain.Entities;
using HMS.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HMS.Infrastructure.Data
{
    public static class DataSeeder
    {
        public static void SeedData(this ModelBuilder modelBuilder)
        {
            SeedRoles(modelBuilder);
            SeedUsers(modelBuilder);
            SeedUserRoles(modelBuilder);
            SeedProfiles(modelBuilder);
            SeedHotelsAndRooms(modelBuilder);
            SeedReservations(modelBuilder);
        }

        private static class SeedIds
        {
            // Roles
            public static readonly Guid AdminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            public static readonly Guid ManagerRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");
            public static readonly Guid GuestRoleId = Guid.Parse("33333333-3333-3333-3333-333333333333");

            // Users
            public static readonly Guid AdminUserId = Guid.Parse("a0000000-0000-0000-0000-000000000000"); // Added Admin User ID
            public static readonly Guid ManagerUserId = Guid.Parse("a0000000-0000-0000-0000-000000000001");
            public static readonly Guid GuestUserId = Guid.Parse("a0000000-0000-0000-0000-000000000002");

            // Profiles
            public static readonly Guid ManagerProfileId = Guid.Parse("b0000000-0000-0000-0000-000000000001");
            public static readonly Guid GuestProfileId = Guid.Parse("b0000000-0000-0000-0000-000000000002");

            // Hotels & Rooms
            public static readonly Guid GrandPlazaHotelId = Guid.Parse("c0000000-0000-0000-0000-000000000001");
            public static readonly Guid Room101Id = Guid.Parse("d0000000-0000-0000-0000-000000000001");
            public static readonly Guid Room102Id = Guid.Parse("d0000000-0000-0000-0000-000000000002");

            // Reservations
            public static readonly Guid SampleReservationId = Guid.Parse("e0000000-0000-0000-0000-000000000001");
        }

        private static void SeedRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityRole<Guid>>().HasData(
                new IdentityRole<Guid>
                {
                    Id = SeedIds.AdminRoleId,
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = "11111111-1111-1111-1111-111111111111"
                },
                new IdentityRole<Guid>
                {
                    Id = SeedIds.ManagerRoleId,
                    Name = "Manager",
                    NormalizedName = "MANAGER",
                    ConcurrencyStamp = "22222222-2222-2222-2222-222222222222"
                },
                new IdentityRole<Guid>
                {
                    Id = SeedIds.GuestRoleId,
                    Name = "Guest",
                    NormalizedName = "GUEST",
                    ConcurrencyStamp = "33333333-3333-3333-3333-333333333333"
                }
            );
        }

        private static void SeedUsers(ModelBuilder modelBuilder)
        {
            // Valid ASP.NET Core Identity V3 Hashes
            // Admin123! (Reusing manager's hash for simplicity in testing - password will be Manager123!)
            const string adminPasswordHash = "AQAAAAEAACcQAAAAEAEBAQEBAQEBAQEBAQEBAQFgQSgkAKOAlM4nL+ruW1lW8cX7LQFBLSFMhGfkRpgdtA==";
            // Manager123!
            const string managerPasswordHash = "AQAAAAEAACcQAAAAEAEBAQEBAQEBAQEBAQEBAQFgQSgkAKOAlM4nL+ruW1lW8cX7LQFBLSFMhGfkRpgdtA==";
            // Guest123!
            const string guestPasswordHash = "AQAAAAEAACcQAAAAEAICAgICAgICAgICAgICAgL2rbbBkaJpmUNkK8uDDH5I10HuF26q4gmKLVeIu9jHBg==";

            var adminUser = new ApplicationUser
            {
                Id = SeedIds.AdminUserId,
                UserName = "admin@hms.com",
                NormalizedUserName = "ADMIN@HMS.COM",
                Email = "admin@hms.com",
                NormalizedEmail = "ADMIN@HMS.COM",
                EmailConfirmed = true,
                SecurityStamp = "00000000-0000-0000-0000-000000000000",
                ConcurrencyStamp = "00000000-0000-0000-0000-000000000000",
                PasswordHash = adminPasswordHash
            };

            var managerUser = new ApplicationUser
            {
                Id = SeedIds.ManagerUserId,
                UserName = "manager@hms.com",
                NormalizedUserName = "MANAGER@HMS.COM",
                Email = "manager@hms.com",
                NormalizedEmail = "MANAGER@HMS.COM",
                EmailConfirmed = true,
                SecurityStamp = "00000000-0000-0000-0000-000000000001",
                ConcurrencyStamp = "00000000-0000-0000-0000-000000000001",
                PasswordHash = managerPasswordHash
            };

            var guestUser = new ApplicationUser
            {
                Id = SeedIds.GuestUserId,
                UserName = "guest@hms.com",
                NormalizedUserName = "GUEST@HMS.COM",
                Email = "guest@hms.com",
                NormalizedEmail = "GUEST@HMS.COM",
                EmailConfirmed = true,
                SecurityStamp = "00000000-0000-0000-0000-000000000002",
                ConcurrencyStamp = "00000000-0000-0000-0000-000000000002",
                PasswordHash = guestPasswordHash
            };

            modelBuilder.Entity<ApplicationUser>().HasData(adminUser, managerUser, guestUser);
        }

        private static void SeedUserRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(
                new IdentityUserRole<Guid> // Added Admin Role Mapping
                {
                    UserId = SeedIds.AdminUserId,
                    RoleId = SeedIds.AdminRoleId
                },
                new IdentityUserRole<Guid>
                {
                    UserId = SeedIds.ManagerUserId,
                    RoleId = SeedIds.ManagerRoleId
                },
                new IdentityUserRole<Guid>
                {
                    UserId = SeedIds.GuestUserId,
                    RoleId = SeedIds.GuestRoleId
                }
            );
        }

        private static void SeedProfiles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Manager>().HasData(
                new Manager
                {
                    Id = SeedIds.ManagerProfileId,
                    UserId = SeedIds.ManagerUserId,
                    HotelId = SeedIds.GrandPlazaHotelId,
                    FirstName = "Alex",
                    LastName = "Manager",
                    Email = "manager@hms.com",
                    PersonalNumber = "01001022222",
                    PhoneNumber = "+995555987654"
                }
            );

            modelBuilder.Entity<Guest>().HasData(
                new Guest
                {
                    Id = SeedIds.GuestProfileId,
                    UserId = SeedIds.GuestUserId,
                    FirstName = "John",
                    LastName = "Doe",
                    PhoneNumber = "+995555123456",
                    PersonalNumber = "01001011111"
                }
            );
        }

        private static void SeedHotelsAndRooms(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Hotel>().HasData(
                new Hotel
                {
                    Id = SeedIds.GrandPlazaHotelId,
                    Name = "Grand Plaza Hotel",
                    Rating = 4.8m,
                    Country = "Georgia",
                    City = "Tbilisi",
                    Address = "Rustaveli Avenue 12"
                }
            );

            modelBuilder.Entity<Room>().HasData(
                new Room
                {
                    Id = SeedIds.Room101Id,
                    Name = "Deluxe Suite 101",
                    Price = 150.00m,
                    HotelId = SeedIds.GrandPlazaHotelId
                },
                new Room
                {
                    Id = SeedIds.Room102Id,
                    Name = "Standard Room 102",
                    Price = 85.00m,
                    HotelId = SeedIds.GrandPlazaHotelId
                }
            );
        }

        private static void SeedReservations(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Reservation>().HasData(
                new Reservation
                {
                    Id = SeedIds.SampleReservationId,
                    GuestId = SeedIds.GuestProfileId,
                    CheckInDate = new DateOnly(2026, 10, 1),
                    CheckOutDate = new DateOnly(2026, 10, 5)
                }
            );

            modelBuilder.Entity<ReservationRoom>().HasData(
                new ReservationRoom
                {
                    ReservationId = SeedIds.SampleReservationId,
                    RoomId = SeedIds.Room101Id
                }
            );
        }
    }
}