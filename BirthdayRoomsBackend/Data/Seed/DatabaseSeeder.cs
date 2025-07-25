using BirthdayRoomsBackend.Models;

namespace BirthdayRoomsBackend.Data.Seed
{
    public static class DatabaseSeeder
    {
        public static void SeedRooms(AppDbContext context)
        {
            if (!context.Rooms.Any())
            {
                context.Rooms.AddRange(
                    new Room { Name = "Room A"},
                    new Room { Name = "Room B"},
                    new Room { Name = "Room C"}
                );
                
                context.SaveChanges();
            }
        }
    }
}
