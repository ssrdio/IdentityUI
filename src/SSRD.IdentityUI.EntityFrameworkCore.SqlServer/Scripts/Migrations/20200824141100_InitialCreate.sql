IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;



CREATE TABLE [Emails] (
    [Id] bigint NOT NULL IDENTITY,
    [Subject] nvarchar(max) NOT NULL,
    [Body] nvarchar(max) NOT NULL,
    [Type] int NOT NULL,
    [_CreatedDate] datetimeoffset NULL,
    [_ModifiedDate] datetimeoffset NULL,
    CONSTRAINT [PK_Emails] PRIMARY KEY ([Id])
);



CREATE TABLE [Groups] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [_CreatedDate] datetimeoffset NULL,
    [_ModifiedDate] datetimeoffset NULL,
    CONSTRAINT [PK_Groups] PRIMARY KEY ([Id])
);



CREATE TABLE [Permissions] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NULL,
    [_CreatedDate] datetimeoffset NULL,
    [_ModifiedDate] datetimeoffset NULL,
    CONSTRAINT [PK_Permissions] PRIMARY KEY ([Id])
);



CREATE TABLE [Roles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [_CreatedDate] datetimeoffset NULL,
    [_ModifiedDate] datetimeoffset NULL,
    [Description] nvarchar(256) NULL,
    [Type] int NOT NULL DEFAULT 1,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
);



CREATE TABLE [Users] (
    [Id] nvarchar(450) NOT NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    [_CreatedDate] datetimeoffset NULL,
    [_ModifiedDate] datetimeoffset NULL,
    [FirstName] nvarchar(256) NULL,
    [LastName] nvarchar(256) NULL,
    [Enabled] bit NOT NULL DEFAULT CAST(0 AS bit),
    [TwoFactor] int NOT NULL DEFAULT 0,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);



CREATE TABLE [GroupAttributes] (
    [Id] bigint NOT NULL IDENTITY,
    [Key] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    [GroupId] nvarchar(450) NOT NULL,
    [_CreatedDate] datetimeoffset NULL,
    [_ModifiedDate] datetimeoffset NULL,
    CONSTRAINT [PK_GroupAttributes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_GroupAttributes_Groups_GroupId] FOREIGN KEY ([GroupId]) REFERENCES [Groups] ([Id]) ON DELETE CASCADE
);



CREATE TABLE [Invite] (
    [Id] nvarchar(450) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [Token] nvarchar(max) NOT NULL,
    [Status] int NOT NULL,
    [RoleId] nvarchar(450) NULL,
    [GroupId] nvarchar(450) NULL,
    [GroupRoleId] nvarchar(450) NULL,
    [ExpiresAt] datetimeoffset NOT NULL,
    [_CreatedDate] datetimeoffset NULL,
    [_ModifiedDate] datetimeoffset NULL,
    CONSTRAINT [PK_Invite] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Invite_Groups_GroupId] FOREIGN KEY ([GroupId]) REFERENCES [Groups] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Invite_Roles_GroupRoleId] FOREIGN KEY ([GroupRoleId]) REFERENCES [Roles] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Invite_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE NO ACTION
);



CREATE TABLE [PermissionRole] (
    [Id] bigint NOT NULL IDENTITY,
    [PermissionId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    [_CreatedDate] datetimeoffset NULL,
    [_ModifiedDate] datetimeoffset NULL,
    CONSTRAINT [PK_PermissionRole] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PermissionRole_Permissions_PermissionId] FOREIGN KEY ([PermissionId]) REFERENCES [Permissions] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PermissionRole_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE
);



CREATE TABLE [RoleAssignments] (
    [Id] bigint NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [CanAssigneRoleId] nvarchar(450) NOT NULL,
    [_CreatedDate] datetimeoffset NULL,
    [_ModifiedDate] datetimeoffset NULL,
    CONSTRAINT [PK_RoleAssignments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RoleAssignments_Roles_CanAssigneRoleId] FOREIGN KEY ([CanAssigneRoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_RoleAssignments_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE NO ACTION
);



CREATE TABLE [RoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    [_CreatedDate] datetimeoffset NULL,
    [_ModifiedDate] datetimeoffset NULL,
    CONSTRAINT [PK_RoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RoleClaims_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE
);



CREATE TABLE [GroupUsers] (
    [Id] bigint NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [GroupId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NULL,
    [_CreatedDate] datetimeoffset NULL,
    [_ModifiedDate] datetimeoffset NULL,
    CONSTRAINT [PK_GroupUsers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_GroupUsers_Groups_GroupId] FOREIGN KEY ([GroupId]) REFERENCES [Groups] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_GroupUsers_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE SET NULL,
    CONSTRAINT [FK_GroupUsers_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);



CREATE TABLE [Sessions] (
    [Id] bigint NOT NULL IDENTITY,
    [_CreatedDate] datetimeoffset NULL,
    [_ModifiedDate] datetimeoffset NULL,
    [_DeletedDate] datetimeoffset NULL,
    [Ip] nvarchar(max) NULL,
    [Code] nvarchar(max) NULL,
    [LastAccess] datetimeoffset NOT NULL,
    [EndType] int NULL,
    [UserId] nvarchar(450) NULL,
    CONSTRAINT [PK_Sessions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Sessions_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);



CREATE TABLE [UserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    [_CreatedDate] datetimeoffset NULL,
    [_ModifiedDate] datetimeoffset NULL,
    CONSTRAINT [PK_UserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserClaims_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);



CREATE TABLE [UserImage] (
    [Id] bigint NOT NULL IDENTITY,
    [_CreatedDate] datetimeoffset NULL,
    [_ModifiedDate] datetimeoffset NULL,
    [BlobImage] varbinary(max) NOT NULL,
    [FileName] nvarchar(250) NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_UserImage] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserImage_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);



CREATE TABLE [UserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    [_CreatedDate] datetimeoffset NULL,
    [_ModifiedDate] datetimeoffset NULL,
    CONSTRAINT [PK_UserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_UserLogins_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);



CREATE TABLE [UserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    [_CreatedDate] datetimeoffset NULL,
    [_ModifiedDate] datetimeoffset NULL,
    CONSTRAINT [PK_UserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_UserRoles_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserRoles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);



CREATE TABLE [UserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    [_CreatedDate] datetimeoffset NULL,
    [_ModifiedDate] datetimeoffset NULL,
    CONSTRAINT [PK_UserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_UserTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);



CREATE INDEX [IX_GroupAttributes_GroupId] ON [GroupAttributes] ([GroupId]);



CREATE INDEX [IX_GroupAttributes_Key] ON [GroupAttributes] ([Key]);



CREATE INDEX [IX_GroupUsers_GroupId] ON [GroupUsers] ([GroupId]);



CREATE INDEX [IX_GroupUsers_RoleId] ON [GroupUsers] ([RoleId]);



CREATE INDEX [IX_GroupUsers_UserId] ON [GroupUsers] ([UserId]);



CREATE INDEX [IX_Invite_GroupId] ON [Invite] ([GroupId]);



CREATE INDEX [IX_Invite_GroupRoleId] ON [Invite] ([GroupRoleId]);



CREATE INDEX [IX_Invite_RoleId] ON [Invite] ([RoleId]);



CREATE INDEX [IX_PermissionRole_PermissionId] ON [PermissionRole] ([PermissionId]);



CREATE INDEX [IX_PermissionRole_RoleId] ON [PermissionRole] ([RoleId]);



CREATE INDEX [IX_RoleAssignments_CanAssigneRoleId] ON [RoleAssignments] ([CanAssigneRoleId]);



CREATE INDEX [IX_RoleAssignments_RoleId] ON [RoleAssignments] ([RoleId]);



CREATE INDEX [IX_RoleClaims_RoleId] ON [RoleClaims] ([RoleId]);



CREATE UNIQUE INDEX [RoleNameIndex] ON [Roles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;



CREATE INDEX [IX_Sessions_UserId] ON [Sessions] ([UserId]);



CREATE INDEX [IX_UserClaims_UserId] ON [UserClaims] ([UserId]);



CREATE UNIQUE INDEX [IX_UserImage_UserId] ON [UserImage] ([UserId]);



CREATE INDEX [IX_UserLogins_UserId] ON [UserLogins] ([UserId]);



CREATE INDEX [IX_UserRoles_RoleId] ON [UserRoles] ([RoleId]);



CREATE INDEX [EmailIndex] ON [Users] ([NormalizedEmail]);



CREATE UNIQUE INDEX [UserNameIndex] ON [Users] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;



INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20200824141100_InitialCreate', N'3.1.5');



