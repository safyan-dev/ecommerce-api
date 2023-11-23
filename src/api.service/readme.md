#### Migration Scripts

```bash
dotnet ef migrations add InitialMigration -o Shared\Data\Migrations -c ApiDbContext
dotnet ef database update -c ApiDbContext

dotnet ef migrations bundle -o Shared\Data\Migrations -c ApiDbContext
```
