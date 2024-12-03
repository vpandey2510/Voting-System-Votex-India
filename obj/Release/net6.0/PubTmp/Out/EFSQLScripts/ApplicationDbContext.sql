IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114053441_Initial_1')
BEGIN
    CREATE TABLE [Areas] (
        [Id] int NOT NULL IDENTITY,
        [AreaName] nvarchar(100) NOT NULL,
        CONSTRAINT [PK_Areas] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114053441_Initial_1')
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114053441_Initial_1')
BEGIN
    CREATE TABLE [AspNetUsers] (
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
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114053441_Initial_1')
BEGIN
    CREATE TABLE [Elections] (
        [ElectionID] int NOT NULL IDENTITY,
        [Name] nvarchar(100) NOT NULL,
        [StartDate] datetime2 NOT NULL,
        [EndDate] datetime2 NOT NULL,
        [Status] nvarchar(10) NOT NULL,
        [Description] nvarchar(500) NOT NULL,
        CONSTRAINT [PK_Elections] PRIMARY KEY ([ElectionID])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114053441_Initial_1')
BEGIN
    CREATE TABLE [Parties] (
        [PartyID] int NOT NULL IDENTITY,
        [Name] nvarchar(100) NOT NULL,
        CONSTRAINT [PK_Parties] PRIMARY KEY ([PartyID])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114053441_Initial_1')
BEGIN
    CREATE TABLE [Voters] (
        [VoterID] int NOT NULL IDENTITY,
        [Name] nvarchar(100) NOT NULL,
        [Age] int NOT NULL,
        [Gender] nvarchar(10) NOT NULL,
        [Username] nvarchar(100) NOT NULL,
        [Area] nvarchar(100) NOT NULL,
        [Eligible] bit NOT NULL,
        CONSTRAINT [PK_Voters] PRIMARY KEY ([VoterID])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114053441_Initial_1')
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114053441_Initial_1')
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114053441_Initial_1')
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(128) NOT NULL,
        [ProviderKey] nvarchar(128) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114053441_Initial_1')
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114053441_Initial_1')
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(128) NOT NULL,
        [Name] nvarchar(128) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114053441_Initial_1')
BEGIN
    CREATE TABLE [Candidates] (
        [CandidateID] int NOT NULL IDENTITY,
        [Name] nvarchar(100) NOT NULL,
        [Username] nvarchar(max) NOT NULL,
        [Area] nvarchar(100) NOT NULL,
        [ElectionID] int NOT NULL,
        [PartyID] int NOT NULL,
        [Verified] bit NOT NULL,
        [Position] nvarchar(100) NOT NULL,
        [AreaId] int NULL,
        CONSTRAINT [PK_Candidates] PRIMARY KEY ([CandidateID]),
        CONSTRAINT [FK_Candidates_Areas_AreaId] FOREIGN KEY ([AreaId]) REFERENCES [Areas] ([Id]),
        CONSTRAINT [FK_Candidates_Elections_ElectionID] FOREIGN KEY ([ElectionID]) REFERENCES [Elections] ([ElectionID]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Candidates_Parties_PartyID] FOREIGN KEY ([PartyID]) REFERENCES [Parties] ([PartyID]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114053441_Initial_1')
BEGIN
    CREATE TABLE [Votes] (
        [VoteId] int NOT NULL IDENTITY,
        [VoterHashID] nvarchar(450) NOT NULL,
        [CandidateID] int NOT NULL,
        [ElectionID] int NOT NULL,
        CONSTRAINT [PK_Votes] PRIMARY KEY ([VoteId]),
        CONSTRAINT [FK_Votes_Candidates_CandidateID] FOREIGN KEY ([CandidateID]) REFERENCES [Candidates] ([CandidateID]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Votes_Elections_ElectionID] FOREIGN KEY ([ElectionID]) REFERENCES [Elections] ([ElectionID]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114053441_Initial_1')
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114053441_Initial_1')
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114053441_Initial_1')
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114053441_Initial_1')
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114053441_Initial_1')
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114053441_Initial_1')
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114053441_Initial_1')
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114053441_Initial_1')
BEGIN
    CREATE INDEX [IX_Candidates_AreaId] ON [Candidates] ([AreaId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114053441_Initial_1')
BEGIN
    CREATE INDEX [IX_Candidates_ElectionID] ON [Candidates] ([ElectionID]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114053441_Initial_1')
BEGIN
    CREATE INDEX [IX_Candidates_PartyID] ON [Candidates] ([PartyID]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114053441_Initial_1')
BEGIN
    CREATE UNIQUE INDEX [IX_Voters_Username] ON [Voters] ([Username]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114053441_Initial_1')
BEGIN
    CREATE INDEX [IX_Votes_CandidateID] ON [Votes] ([CandidateID]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114053441_Initial_1')
BEGIN
    CREATE INDEX [IX_Votes_ElectionID] ON [Votes] ([ElectionID]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114053441_Initial_1')
BEGIN
    CREATE UNIQUE INDEX [IX_Votes_VoterHashID_ElectionID] ON [Votes] ([VoterHashID], [ElectionID]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114053441_Initial_1')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241114053441_Initial_1', N'6.0.35');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114060434_Initial_2')
BEGIN
    ALTER TABLE [Candidates] DROP CONSTRAINT [FK_Candidates_Areas_AreaId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114060434_Initial_2')
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Candidates]') AND [c].[name] = N'Area');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Candidates] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [Candidates] DROP COLUMN [Area];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114060434_Initial_2')
BEGIN
    EXEC sp_rename N'[Candidates].[AreaId]', N'AreaID', N'COLUMN';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114060434_Initial_2')
BEGIN
    EXEC sp_rename N'[Candidates].[IX_Candidates_AreaId]', N'IX_Candidates_AreaID', N'INDEX';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114060434_Initial_2')
BEGIN
    DROP INDEX [IX_Candidates_AreaID] ON [Candidates];
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Candidates]') AND [c].[name] = N'AreaID');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Candidates] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [Candidates] ALTER COLUMN [AreaID] int NOT NULL;
    ALTER TABLE [Candidates] ADD DEFAULT 0 FOR [AreaID];
    CREATE INDEX [IX_Candidates_AreaID] ON [Candidates] ([AreaID]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114060434_Initial_2')
BEGIN
    ALTER TABLE [Candidates] ADD CONSTRAINT [FK_Candidates_Areas_AreaID] FOREIGN KEY ([AreaID]) REFERENCES [Areas] ([Id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114060434_Initial_2')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241114060434_Initial_2', N'6.0.35');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114062335_Initial_3')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241114062335_Initial_3', N'6.0.35');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114062648_UpdateCandidate')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241114062648_UpdateCandidate', N'6.0.35');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114065305_UpdateCandidate_Area')
BEGIN
    EXEC sp_rename N'[Areas].[AreaName]', N'Name', N'COLUMN';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114065305_UpdateCandidate_Area')
BEGIN
    EXEC sp_rename N'[Areas].[Id]', N'AreaID', N'COLUMN';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114065305_UpdateCandidate_Area')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241114065305_UpdateCandidate_Area', N'6.0.35');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114065808_UpdateCandidate_Model')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241114065808_UpdateCandidate_Model', N'6.0.35');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114083037_UpdateVoter_Model')
BEGIN
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Voters]') AND [c].[name] = N'Area');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Voters] DROP CONSTRAINT [' + @var2 + '];');
    ALTER TABLE [Voters] DROP COLUMN [Area];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114083037_UpdateVoter_Model')
BEGIN
    ALTER TABLE [Voters] ADD [AreaID] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114083037_UpdateVoter_Model')
BEGIN
    CREATE INDEX [IX_Voters_AreaID] ON [Voters] ([AreaID]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114083037_UpdateVoter_Model')
BEGIN
    ALTER TABLE [Voters] ADD CONSTRAINT [FK_Voters_Areas_AreaID] FOREIGN KEY ([AreaID]) REFERENCES [Areas] ([AreaID]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114083037_UpdateVoter_Model')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241114083037_UpdateVoter_Model', N'6.0.35');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114092225_UpdateVote_Model')
BEGIN
    DROP INDEX [IX_Votes_VoterHashID_ElectionID] ON [Votes];
    DECLARE @var3 sysname;
    SELECT @var3 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Votes]') AND [c].[name] = N'VoterHashID');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Votes] DROP CONSTRAINT [' + @var3 + '];');
    ALTER TABLE [Votes] ALTER COLUMN [VoterHashID] int NOT NULL;
    CREATE UNIQUE INDEX [IX_Votes_VoterHashID_ElectionID] ON [Votes] ([VoterHashID], [ElectionID]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114092225_UpdateVote_Model')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241114092225_UpdateVote_Model', N'6.0.35');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114092854_UpdateVote_Model_1')
BEGIN
    ALTER TABLE [Votes] ADD [AreaID] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114092854_UpdateVote_Model_1')
BEGIN
    CREATE INDEX [IX_Votes_AreaID] ON [Votes] ([AreaID]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114092854_UpdateVote_Model_1')
BEGIN
    ALTER TABLE [Votes] ADD CONSTRAINT [FK_Votes_Areas_AreaID] FOREIGN KEY ([AreaID]) REFERENCES [Areas] ([AreaID]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114092854_UpdateVote_Model_1')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241114092854_UpdateVote_Model_1', N'6.0.35');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114093148_UpdateVote_Model_new')
BEGIN
    ALTER TABLE [Votes] DROP CONSTRAINT [FK_Votes_Areas_AreaID];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114093148_UpdateVote_Model_new')
BEGIN
    DROP INDEX [IX_Votes_AreaID] ON [Votes];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114093148_UpdateVote_Model_new')
BEGIN
    DECLARE @var4 sysname;
    SELECT @var4 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Votes]') AND [c].[name] = N'AreaID');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [Votes] DROP CONSTRAINT [' + @var4 + '];');
    ALTER TABLE [Votes] DROP COLUMN [AreaID];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114093148_UpdateVote_Model_new')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241114093148_UpdateVote_Model_new', N'6.0.35');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241116074109_UpdatePartyModel')
BEGIN
    ALTER TABLE [Parties] ADD [FlagImagePath] nvarchar(255) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241116074109_UpdatePartyModel')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241116074109_UpdatePartyModel', N'6.0.35');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241117033018_UpdateImages_model')
BEGIN
    ALTER TABLE [Voters] ADD [VoterImagePath] nvarchar(255) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241117033018_UpdateImages_model')
BEGIN
    ALTER TABLE [Elections] ADD [ElectionImagePath] nvarchar(255) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241117033018_UpdateImages_model')
BEGIN
    ALTER TABLE [Candidates] ADD [CandidateImagePath] nvarchar(255) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241117033018_UpdateImages_model')
BEGIN
    ALTER TABLE [Areas] ADD [AreaImagePath] nvarchar(255) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241117033018_UpdateImages_model')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241117033018_UpdateImages_model', N'6.0.35');
END;
GO

COMMIT;
GO

