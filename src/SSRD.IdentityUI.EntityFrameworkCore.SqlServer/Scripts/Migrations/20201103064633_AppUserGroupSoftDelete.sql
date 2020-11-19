ALTER TABLE [Users] ADD [_DeletedDate] datetimeoffset NULL;

ALTER TABLE [Groups] ADD [_DeletedDate] datetimeoffset NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20201103064633_AppUserGroupSoftDelete', N'3.1.5');

