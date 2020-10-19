1. Add Migration 
  - pmc `Add-Migration {MIGRATION_NAME} -StartUpProject IdentityUI.Dev -Project SSRD.IdentityUI.EntityFrameworkCore.Postgre -OutputDir Migrations`
  - dotnet cli `dotnet ef migrations add {MIGRATION_NAME} --startup-project IdentityUI.Dev --project SSRD.IdentityUI.EntityFrameworkCore.Postgre --output-dir Migrations`
2. Script Migration
  - pmc `Script-Migration -From {LAST_MIGRATION_NAME} -Output SSRD.IdentityUI.EntityFrameworkCore.Postgre/Scripts/Migrations/{GENERATED_MIGRATION_NAME}.sql`
  - dotnet cli `dotnet ef migrations script {LAST_MIGRATION_NAME} --output SSRD.IdentityUI.EntityFrameworkCore.Postgre/Scripts/Migrations/{GENERATED_MIGRATION_NAME}.sql --startup-project IdentityUI.Dev --project SSRD.IdentityUI.EntityFrameworkCore.Postgre`
3. Add Update class to `SSRD.IdentityUI.EntityFrameworkCore.Postgre/Updates`
- Update class name format `Update_{GENERATED_MIGRATION_NAME}`
- Update class must implement `PostgreUpdate`
- In Update class you need to override `MigrationId` with {GENERATED_MIGRATION_NAME}
4. Add your Update class to `PostgreUpdateList._updates`