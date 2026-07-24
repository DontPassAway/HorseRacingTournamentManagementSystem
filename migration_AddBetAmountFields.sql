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

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519014733_InitialCreate'
)
BEGIN
    CREATE TABLE [Tournaments] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(200) NOT NULL,
        [Description] nvarchar(2000) NULL,
        [Location] nvarchar(200) NOT NULL,
        [StartDate] datetime2 NOT NULL,
        [EndDate] datetime2 NOT NULL,
        [RegistrationDeadline] datetime2 NOT NULL,
        [Status] int NOT NULL,
        [MaxParticipants] int NOT NULL,
        [Rules] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Tournaments] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519014733_InitialCreate'
)
BEGIN
    CREATE TABLE [Users] (
        [Id] int NOT NULL IDENTITY,
        [Username] nvarchar(50) NOT NULL,
        [Email] nvarchar(150) NOT NULL,
        [PasswordHash] nvarchar(max) NOT NULL,
        [FullName] nvarchar(100) NOT NULL,
        [PhoneNumber] nvarchar(20) NULL,
        [AvatarUrl] nvarchar(max) NULL,
        [Role] int NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519014733_InitialCreate'
)
BEGIN
    CREATE TABLE [Prizes] (
        [Id] int NOT NULL IDENTITY,
        [TournamentId] int NOT NULL,
        [Position] int NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        [Description] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Prizes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Prizes_Tournaments_TournamentId] FOREIGN KEY ([TournamentId]) REFERENCES [Tournaments] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519014733_InitialCreate'
)
BEGIN
    CREATE TABLE [Races] (
        [Id] int NOT NULL IDENTITY,
        [TournamentId] int NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Description] nvarchar(max) NULL,
        [RoundNumber] int NOT NULL,
        [Distance] decimal(10,2) NOT NULL,
        [ScheduledAt] datetime2 NOT NULL,
        [StartedAt] datetime2 NULL,
        [FinishedAt] datetime2 NULL,
        [Status] int NOT NULL,
        [MaxParticipants] int NOT NULL,
        [TrackCondition] nvarchar(max) NULL,
        [WeatherCondition] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Races] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Races_Tournaments_TournamentId] FOREIGN KEY ([TournamentId]) REFERENCES [Tournaments] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519014733_InitialCreate'
)
BEGIN
    CREATE TABLE [HorseOwners] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [LicenseNumber] nvarchar(max) NULL,
        [Address] nvarchar(max) NULL,
        [DateOfBirth] datetime2 NULL,
        [RegisteredAt] datetime2 NOT NULL,
        CONSTRAINT [PK_HorseOwners] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HorseOwners_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519014733_InitialCreate'
)
BEGIN
    CREATE TABLE [JockeyProfiles] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [LicenseNumber] nvarchar(50) NULL,
        [Weight] decimal(8,2) NOT NULL,
        [ExperienceYears] int NOT NULL,
        [TotalRaces] int NOT NULL,
        [TotalWins] int NOT NULL,
        [Nationality] nvarchar(100) NULL,
        [RegisteredAt] datetime2 NOT NULL,
        CONSTRAINT [PK_JockeyProfiles] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_JockeyProfiles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519014733_InitialCreate'
)
BEGIN
    CREATE TABLE [RaceAssignments] (
        [Id] int NOT NULL IDENTITY,
        [RaceId] int NOT NULL,
        [RefereeUserId] int NOT NULL,
        [Notes] nvarchar(max) NULL,
        [AssignedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_RaceAssignments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RaceAssignments_Races_RaceId] FOREIGN KEY ([RaceId]) REFERENCES [Races] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_RaceAssignments_Users_RefereeUserId] FOREIGN KEY ([RefereeUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519014733_InitialCreate'
)
BEGIN
    CREATE TABLE [Horses] (
        [Id] int NOT NULL IDENTITY,
        [HorseOwnerId] int NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [Breed] nvarchar(100) NOT NULL,
        [Age] int NOT NULL,
        [Color] nvarchar(50) NOT NULL,
        [Weight] decimal(8,2) NOT NULL,
        [Status] int NOT NULL,
        [MedicalHistory] nvarchar(max) NULL,
        [ImageUrl] nvarchar(max) NULL,
        [TotalRaces] int NOT NULL,
        [TotalWins] int NOT NULL,
        [RegisteredAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Horses] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Horses_HorseOwners_HorseOwnerId] FOREIGN KEY ([HorseOwnerId]) REFERENCES [HorseOwners] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519014733_InitialCreate'
)
BEGIN
    CREATE TABLE [Bets] (
        [Id] int NOT NULL IDENTITY,
        [SpectatorUserId] int NOT NULL,
        [RaceId] int NOT NULL,
        [PredictedHorseId] int NOT NULL,
        [PredictedPosition] int NOT NULL,
        [Status] int NOT NULL,
        [Notes] nvarchar(max) NULL,
        [PlacedAt] datetime2 NOT NULL,
        [ResolvedAt] datetime2 NULL,
        CONSTRAINT [PK_Bets] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Bets_Horses_PredictedHorseId] FOREIGN KEY ([PredictedHorseId]) REFERENCES [Horses] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Bets_Races_RaceId] FOREIGN KEY ([RaceId]) REFERENCES [Races] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Bets_Users_SpectatorUserId] FOREIGN KEY ([SpectatorUserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519014733_InitialCreate'
)
BEGIN
    CREATE TABLE [JockeyInvitations] (
        [Id] int NOT NULL IDENTITY,
        [HorseId] int NOT NULL,
        [HorseOwnerId] int NOT NULL,
        [JockeyId] int NOT NULL,
        [RaceId] int NULL,
        [Status] int NOT NULL,
        [Message] nvarchar(500) NULL,
        [ResponseMessage] nvarchar(500) NULL,
        [InvitedAt] datetime2 NOT NULL,
        [RespondedAt] datetime2 NULL,
        CONSTRAINT [PK_JockeyInvitations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_JockeyInvitations_HorseOwners_HorseOwnerId] FOREIGN KEY ([HorseOwnerId]) REFERENCES [HorseOwners] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_JockeyInvitations_Horses_HorseId] FOREIGN KEY ([HorseId]) REFERENCES [Horses] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_JockeyInvitations_JockeyProfiles_JockeyId] FOREIGN KEY ([JockeyId]) REFERENCES [JockeyProfiles] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_JockeyInvitations_Races_RaceId] FOREIGN KEY ([RaceId]) REFERENCES [Races] ([Id]) ON DELETE SET NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519014733_InitialCreate'
)
BEGIN
    CREATE TABLE [Registrations] (
        [Id] int NOT NULL IDENTITY,
        [RaceId] int NOT NULL,
        [HorseId] int NOT NULL,
        [HorseOwnerId] int NOT NULL,
        [JockeyId] int NULL,
        [LaneNumber] int NOT NULL,
        [Status] int NOT NULL,
        [RejectionReason] nvarchar(max) NULL,
        [JockeyConfirmed] bit NOT NULL,
        [OwnerConfirmed] bit NOT NULL,
        [RegisteredAt] datetime2 NOT NULL,
        [ApprovedAt] datetime2 NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Registrations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Registrations_HorseOwners_HorseOwnerId] FOREIGN KEY ([HorseOwnerId]) REFERENCES [HorseOwners] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Registrations_Horses_HorseId] FOREIGN KEY ([HorseId]) REFERENCES [Horses] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Registrations_JockeyProfiles_JockeyId] FOREIGN KEY ([JockeyId]) REFERENCES [JockeyProfiles] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_Registrations_Races_RaceId] FOREIGN KEY ([RaceId]) REFERENCES [Races] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519014733_InitialCreate'
)
BEGIN
    CREATE TABLE [RaceResults] (
        [Id] int NOT NULL IDENTITY,
        [RaceId] int NOT NULL,
        [RegistrationId] int NOT NULL,
        [Position] int NOT NULL,
        [FinishTime] time NULL,
        [Disqualified] bit NOT NULL,
        [DisqualificationReason] nvarchar(max) NULL,
        [PrizeMoney] decimal(18,2) NULL,
        [IsConfirmed] bit NOT NULL,
        [ConfirmedByUserId] int NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_RaceResults] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RaceResults_Races_RaceId] FOREIGN KEY ([RaceId]) REFERENCES [Races] ([Id]),
        CONSTRAINT [FK_RaceResults_Registrations_RegistrationId] FOREIGN KEY ([RegistrationId]) REFERENCES [Registrations] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_RaceResults_Users_ConfirmedByUserId] FOREIGN KEY ([ConfirmedByUserId]) REFERENCES [Users] ([Id]) ON DELETE SET NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519014733_InitialCreate'
)
BEGIN
    CREATE TABLE [RefereeReports] (
        [Id] int NOT NULL IDENTITY,
        [RaceId] int NOT NULL,
        [RefereeUserId] int NOT NULL,
        [Content] nvarchar(max) NOT NULL,
        [HasViolation] bit NOT NULL,
        [ViolationType] int NULL,
        [ViolationDescription] nvarchar(1000) NULL,
        [ViolatingRegistrationId] int NULL,
        [IsFinalized] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_RefereeReports] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RefereeReports_Races_RaceId] FOREIGN KEY ([RaceId]) REFERENCES [Races] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_RefereeReports_Registrations_ViolatingRegistrationId] FOREIGN KEY ([ViolatingRegistrationId]) REFERENCES [Registrations] ([Id]),
        CONSTRAINT [FK_RefereeReports_Users_RefereeUserId] FOREIGN KEY ([RefereeUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519014733_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Bets_PredictedHorseId] ON [Bets] ([PredictedHorseId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519014733_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Bets_RaceId] ON [Bets] ([RaceId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519014733_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Bets_SpectatorUserId] ON [Bets] ([SpectatorUserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519014733_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_HorseOwners_UserId] ON [HorseOwners] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519014733_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Horses_HorseOwnerId] ON [Horses] ([HorseOwnerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519014733_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_JockeyInvitations_HorseId] ON [JockeyInvitations] ([HorseId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519014733_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_JockeyInvitations_HorseOwnerId] ON [JockeyInvitations] ([HorseOwnerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519014733_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_JockeyInvitations_JockeyId] ON [JockeyInvitations] ([JockeyId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519014733_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_JockeyInvitations_RaceId] ON [JockeyInvitations] ([RaceId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519014733_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_JockeyProfiles_UserId] ON [JockeyProfiles] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519014733_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Prizes_TournamentId] ON [Prizes] ([TournamentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519014733_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_RaceAssignments_RaceId_RefereeUserId] ON [RaceAssignments] ([RaceId], [RefereeUserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519014733_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_RaceAssignments_RefereeUserId] ON [RaceAssignments] ([RefereeUserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519014733_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_RaceResults_ConfirmedByUserId] ON [RaceResults] ([ConfirmedByUserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519014733_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_RaceResults_RaceId] ON [RaceResults] ([RaceId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519014733_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_RaceResults_RegistrationId] ON [RaceResults] ([RegistrationId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519014733_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Races_TournamentId] ON [Races] ([TournamentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519014733_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_RefereeReports_RaceId] ON [RefereeReports] ([RaceId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519014733_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_RefereeReports_RefereeUserId] ON [RefereeReports] ([RefereeUserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519014733_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_RefereeReports_ViolatingRegistrationId] ON [RefereeReports] ([ViolatingRegistrationId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519014733_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Registrations_HorseId] ON [Registrations] ([HorseId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519014733_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Registrations_HorseOwnerId] ON [Registrations] ([HorseOwnerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519014733_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Registrations_JockeyId] ON [Registrations] ([JockeyId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519014733_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Registrations_RaceId] ON [Registrations] ([RaceId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519014733_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519014733_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Users_Username] ON [Users] ([Username]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519014733_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260519014733_InitialCreate', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724030224_AddBetAmountFields'
)
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Bets]') AND [c].[name] = N'OddsMultiplier');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Bets] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [Bets] ALTER COLUMN [OddsMultiplier] decimal(18,4) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724030224_AddBetAmountFields'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260724030224_AddBetAmountFields', N'8.0.0');
END;
GO

COMMIT;
GO

