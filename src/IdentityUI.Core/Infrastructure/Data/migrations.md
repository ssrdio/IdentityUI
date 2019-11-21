1. Add Migration `Add-Migration {MIGRATION_NAME} -Project SSRD.IdentityUI.Core -OutputDir Infrastructure\Data\Migrations`
2. Script Migration `Script-Migration -From {LAST_MIGRATION_NAME} -Output IdentityUI.Core\Infrastructure\Data\Scripts\Migrations\{GENERATED_MIGRATION_NAME}.sql`
3. Add Update class to `IdentityUI.Core\Infrastructure\Data\ReleaseManagment\Updates`
- Update class name format `Update_{VERSION_NUMBER}_{MIGRATION_NAME}`
- Update class must implement `AbstractUpdate`
4. Add your Update class to `ReleaseManagement.AllUpdates`