CREATE TABLE [UserAttributes] (
    [Id] bigint NOT NULL IDENTITY,
    [Key] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    [UserId] nvarchar(450) NULL,
    [_CreatedDate] datetimeoffset NULL,
    [_ModifiedDate] datetimeoffset NULL,
    CONSTRAINT [PK_UserAttributes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserAttributes_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_UserAttributes_Key] ON [UserAttributes] ([Key]);

CREATE INDEX [IX_UserAttributes_UserId] ON [UserAttributes] ([UserId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20200901071938_UserAttributes', N'3.1.5');