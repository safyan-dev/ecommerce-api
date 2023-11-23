namespace api.building.Persistence
{
    public interface IDataSeeder
    {
        Task SeedAllAsync();
        int Order { get; }
    }
}
