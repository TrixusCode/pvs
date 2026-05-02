
using PVS.Api.Models;
using BCrypt.Net;
using PVS.Api.Modules.Branches.Enums;
using PVS.Api.Modules.Clients.Enums;

namespace PVS.Api.Data;

public class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        try
        {
            // Only seed if database is empty
            if (context.Users.Any())
            {
                return;
            }

            // Create sample users
            var admin = new User
            {
                Email = "admin@pvs.com",
                FirstName = "Admin",
                LastName = "User",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = "Admin",
                CreatedAt = DateTime.UtcNow
            };

            var agent = new User
            {
                Email = "agent@pvs.com",
                FirstName = "John",
                LastName = "Smith",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Agent@123"),
                Role = "Agent",
                CreatedAt = DateTime.UtcNow
            };

            var user = new User
            {
                Email = "user@pvs.com",
                FirstName = "Jane",
                LastName = "Doe",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"),
                Role = "User",
                CreatedAt = DateTime.UtcNow
            };

            context.Users.AddRange(admin, agent, user);
            await context.SaveChangesAsync();

            // Create sample branches
            var mainBranch = new Branch
            {
                Name = "Main Office",
                Description = "Headquarters and main branch office",
                Address = new Address
                {
                    City = "New York",
                    State = "NY",
                    ZipCode = "10001"
                },
                Phone = "555-0100",
                Email = "main@pvs.com",
                ManagerName = "Admin User",
                Status = BranchStatus.Active,
                ManagerUserId = admin.Id,
                CreatedAt = DateTime.UtcNow
            };

            var downtownBranch = new Branch
            {
                Name = "Downtown Branch",
                Description = "Downtown location for city properties",
                Address = new Address
                {
                    City = "New York",
                    State = "NY",
                    ZipCode = "10002"
                },
                Phone = "555-0200",
                Email = "downtown@pvs.com",
                ManagerName = "John Smith",
                Status = BranchStatus.Active,
                ManagerUserId = agent.Id,
                CreatedAt = DateTime.UtcNow
            };

            context.Branches.AddRange(mainBranch, downtownBranch);
            await context.SaveChangesAsync();

            // Create sample clients if needed
            var client = new Client
            {
                FirstName = "Robert",
                LastName = "Johnson",
                Email = "robert.johnson@email.com",
                Phone = "555-0123",
                Address = new Address(),
                ClientType = ClientType.Buyer,
                Status = ClientStatus.Active,
                UserId = agent.Id,
                CreatedAt = DateTime.UtcNow
            };

            context.Clients.Add(client);
            await context.SaveChangesAsync();

            // Create sample properties
            var property = new Property
            {
                Title = "Beautiful House in Downtown",
                Description = "Modern house with 3 bedrooms and 2 bathrooms",
                Price = 450000,
                Bedrooms = 3,
                Bathrooms = 2,
                SquareFeet = 2500,
                UserId = agent.Id,
                CreatedAt = DateTime.UtcNow
            };

            context.Properties.Add(property);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error seeding database: {ex.Message}");
            throw;
        }
    }
}