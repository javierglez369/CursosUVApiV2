BEGIN TRANSACTION;
DECLARE @var nvarchar(max);
SELECT @var = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Estudiantes]') AND [c].[name] = N'UserId');
IF @var IS NOT NULL EXEC(N'ALTER TABLE [Estudiantes] DROP CONSTRAINT ' + @var + ';');
ALTER TABLE [Estudiantes] ALTER COLUMN [UserId] nvarchar(450) NULL;

CREATE TABLE [ApplicationUser] (
    [Id] nvarchar(450) NOT NULL,
    [NombreCompleto] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UserName] nvarchar(max) NULL,
    [NormalizedUserName] nvarchar(max) NULL,
    [Email] nvarchar(max) NULL,
    [NormalizedEmail] nvarchar(max) NULL,
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
    CONSTRAINT [PK_ApplicationUser] PRIMARY KEY ([Id])
);

CREATE TABLE [RefreshTokens] (
    [Id] int NOT NULL IDENTITY,
    [Token] nvarchar(512) NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    [ExpiresAt] datetime2 NOT NULL,
    [IsRevoked] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_RefreshTokens] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RefreshTokens_ApplicationUser_UserId] FOREIGN KEY ([UserId]) REFERENCES [ApplicationUser] ([Id]) ON DELETE CASCADE
);

CREATE UNIQUE INDEX [IX_Estudiantes_UserId] ON [Estudiantes] ([UserId]) WHERE [UserId] IS NOT NULL;

CREATE UNIQUE INDEX [IX_RefreshTokens_Token] ON [RefreshTokens] ([Token]);

CREATE INDEX [IX_RefreshTokens_UserId] ON [RefreshTokens] ([UserId]);

ALTER TABLE [Estudiantes] ADD CONSTRAINT [FK_Estudiantes_ApplicationUser_UserId] FOREIGN KEY ([UserId]) REFERENCES [ApplicationUser] ([Id]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260413165945_IdentityRefreshToken', N'10.0.5');

COMMIT;
GO

