ALTER TABLE [Audit] ADD [Metadata] nvarchar(max) NULL;

ALTER TABLE [Audit] ADD [SubjectMetadata] nvarchar(max) NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20201020221321_AddAuditMetadata', N'3.1.5');
