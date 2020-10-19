CREATE TABLE [Audit] (
    [Id] bigint NOT NULL IDENTITY,
    [ActionType] int NOT NULL,
    [ObjectIdentifier] nvarchar(max) NULL,
    [ObjectType] nvarchar(max) NULL,
    [ObjectMetadata] nvarchar(max) NULL,
    [SubjectType] int NOT NULL,
    [SubjectIdentifier] nvarchar(max) NULL,
    [Host] nvarchar(max) NULL,
    [RemoteIp] nvarchar(max) NULL,
    [ResourceName] nvarchar(max) NULL,
    [UserAgent] nvarchar(max) NULL,
    [TraceIdentifier] nvarchar(max) NULL,
    [AppVersion] nvarchar(max) NULL,
    [Created] datetime2 NOT NULL,
    CONSTRAINT [PK_Audit] PRIMARY KEY ([Id])
);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20201012131856_AddAudit', N'3.1.5');

