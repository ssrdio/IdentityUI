ALTER TABLE [Audit] ADD [GroupIdentifier] nvarchar(max) NULL;

ALTER TABLE [Audit] ADD [Metadata] nvarchar(max) NULL;

ALTER TABLE [Audit] ADD [SubjectMetadata] nvarchar(max) NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20201029132919_AuditMetadataGroupIdentifier', N'3.1.5');

