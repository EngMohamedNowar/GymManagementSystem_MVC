using GymManagement.DbContexts;
using GymManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text.Json;

namespace GymManagementSystem.DAL.DataSeeding
{
    public static class GymDataSeeding
    {
        public static async Task SeedAsync(
            GymDbContext context,
            ILogger logger,
            string seedFolderPath,
            CancellationToken ct = default)
        {
            try
            {
                if (!await ((IQueryable<Plan>)context.Plans).AnyAsync(ct))
                {
                    var plans = await LoadDataFromJsonFile<Plan>(seedFolderPath, "plans.json", ct);

                    await context.Plans.AddRangeAsync(plans, ct);
                    await context.SaveChangesAsync(ct);

                    logger.LogInformation("Seeded {Count} Plans Successfully.", plans.Count);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        public static async Task<List<T>> LoadDataFromJsonFile<T>(string folderPath, string fileName, CancellationToken ct = default)
        {
            var filePath = Path.Combine(folderPath, fileName);

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Seed data file not found: {filePath}");

            var json = await File.ReadAllTextAsync(filePath, ct);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<List<T>>(json, options) ?? new List<T>();
        }
    }
}