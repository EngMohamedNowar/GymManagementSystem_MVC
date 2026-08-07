using GymManagement.DbContexts;
using GymManagement.Models;
using Microsoft.Extensions.Logging;
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
                if (!context.Plans.Any())
                {
                    var plans = LoadDataFromJsonFile<Plan>(seedFolderPath, "plans.json");

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

        public static List<T> LoadDataFromJsonFile<T>(string folderPath, string fileName)
        {
            var filePath = Path.Combine(folderPath, fileName);

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Seed data file not found: {filePath}");

            var json = File.ReadAllText(filePath);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<List<T>>(json, options) ?? new List<T>();
        }
    }
}