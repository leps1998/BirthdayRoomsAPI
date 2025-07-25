﻿using BirthdayRoomsBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace BirthdayRoomsBackend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Room> Rooms { get; set; } = null!;
        public DbSet<Booking> Bookings { get; set; } = null!;
    }
}
