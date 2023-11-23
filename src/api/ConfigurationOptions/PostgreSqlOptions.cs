namespace api_example.ConfigurationOptions
{
    public class PostgreSqlOptions
    {
        public string ConnectionString { get; set; } = default!;
        public bool UseInMemory { get; set; }
        public string? MigrationAssembly { get; set; } = null!;
    }
}
