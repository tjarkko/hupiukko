-- Initial schema for Hupiukko Gym Workout Logging Service
-- Created: Initial version with core workout tracking functionality

-- Users table - stores basic user info (Azure AD will handle authentication)
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    AzureAdObjectId NVARCHAR(255) NOT NULL UNIQUE, -- Azure AD user object ID
    Email NVARCHAR(255) NOT NULL,
    FirstName NVARCHAR(100),
    LastName NVARCHAR(100),
    DisplayName NVARCHAR(200),
    TimeZone NVARCHAR(100) DEFAULT 'UTC',
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    RowVersion ROWVERSION NOT NULL
);

-- Exercise Categories for organization
CREATE TABLE ExerciseCategories (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    SortOrder INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    RowVersion ROWVERSION NOT NULL
);

-- Static library of available exercises
CREATE TABLE Exercises (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX), -- Exercise instructions/guidance
    CategoryId UNIQUEIDENTIFIER NOT NULL,
    MuscleGroups NVARCHAR(500), -- JSON array or comma-separated (e.g., "chest,triceps,shoulders")
    Equipment NVARCHAR(200), -- Required equipment
    DifficultyLevel NVARCHAR(50), -- Beginner, Intermediate, Advanced
    ImageUrl NVARCHAR(500), -- Future: exercise demonstration images
    VideoUrl NVARCHAR(500), -- Future: exercise demonstration videos
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    RowVersion ROWVERSION NOT NULL,
    CONSTRAINT FK_Exercises_Categories FOREIGN KEY (CategoryId) REFERENCES ExerciseCategories(Id)
);

-- User's workout programs (can have multiple, can evolve over time)
CREATE TABLE WorkoutPrograms (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(1000),
    IsActive BIT NOT NULL DEFAULT 1, -- Only one active program per user typically
    StartDate DATETIME2 NOT NULL,
    EndDate DATETIME2, -- NULL means ongoing
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    RowVersion ROWVERSION NOT NULL,
    CONSTRAINT FK_WorkoutPrograms_Users FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- Individual workout sessions
CREATE TABLE WorkoutSessions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    ProgramId UNIQUEIDENTIFIER NOT NULL,
    StartTime DATETIME2 NOT NULL,
    EndTime DATETIME2, -- NULL while workout is in progress
    DurationMinutes AS DATEDIFF(MINUTE, StartTime, EndTime), -- Computed column
    Notes NVARCHAR(1000), -- Overall session notes
    IsCompleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    RowVersion ROWVERSION NOT NULL,
    CONSTRAINT FK_WorkoutSessions_Users FOREIGN KEY (UserId) REFERENCES Users(Id),
    CONSTRAINT FK_WorkoutSessions_Programs FOREIGN KEY (ProgramId) REFERENCES WorkoutPrograms(Id)
);

-- Rename WorkoutDay table to WorkoutDays (plural)
CREATE TABLE WorkoutDays (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    ProgramId UNIQUEIDENTIFIER NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    DayOfWeek INT NULL,
    SortOrder INT NOT NULL,
    Notes NVARCHAR(255) NULL,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NOT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    RowVersion ROWVERSION NOT NULL,
    CONSTRAINT FK_WorkoutDays_WorkoutProgram FOREIGN KEY (ProgramId) REFERENCES WorkoutPrograms(Id)
);

-- Rename ProgramExercise table to ProgramExercises (plural)
CREATE TABLE ProgramExercises (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    WorkoutDayId UNIQUEIDENTIFIER NOT NULL,
    ExerciseId UNIQUEIDENTIFIER NOT NULL,
    SortOrder INT NOT NULL,
    TargetSets INT NOT NULL,
    DefaultRepsMin INT NOT NULL,
    DefaultRepsMax INT NULL,
    DefaultWeight DECIMAL(10,2) NULL,
    DefaultRestTimeSeconds INT NULL,
    Notes NVARCHAR(255) NULL,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NOT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    RowVersion ROWVERSION NOT NULL,
    CONSTRAINT FK_ProgramExercises_WorkoutDay FOREIGN KEY (WorkoutDayId) REFERENCES WorkoutDays(Id),
    CONSTRAINT FK_ProgramExercises_Exercise FOREIGN KEY (ExerciseId) REFERENCES Exercises(Id)
);

CREATE TABLE ProgramExerciseSets (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ProgramExerciseId UNIQUEIDENTIFIER NOT NULL,
    SetNumber INT NOT NULL, -- 1, 2, 3, etc.
    TargetRepsMin INT NOT NULL,
    TargetRepsMax INT, -- NULL if exact reps, otherwise range
    TargetWeight DECIMAL(10,2), -- Can override default weight per set
    RestTimeSeconds INT, -- Can override default rest time per set
    Notes NVARCHAR(500), -- Set-specific instructions (e.g., "to failure", "drop weight by 20%")
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    RowVersion ROWVERSION NOT NULL,
    CONSTRAINT FK_ProgramExerciseSets_ProgramExercises FOREIGN KEY (ProgramExerciseId) REFERENCES ProgramExercises(Id),
    CONSTRAINT UQ_ProgramExerciseSets_Exercise_SetNumber UNIQUE (ProgramExerciseId, SetNumber)
);

CREATE TABLE WorkoutSession (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL,
    ProgramId UNIQUEIDENTIFIER NOT NULL,
    WorkoutDayId UNIQUEIDENTIFIER NULL,
    StartTime DATETIME2 NOT NULL,
    EndTime DATETIME2 NULL,
    Notes NVARCHAR(255) NULL,
    IsCompleted BIT NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NOT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    RowVersion ROWVERSION NOT NULL,
    CONSTRAINT FK_WorkoutSession_User FOREIGN KEY (UserId) REFERENCES Users(Id),
    CONSTRAINT FK_WorkoutSession_Program FOREIGN KEY (ProgramId) REFERENCES WorkoutPrograms(Id),
    CONSTRAINT FK_WorkoutSession_WorkoutDay FOREIGN KEY (WorkoutDayId) REFERENCES WorkoutDays(Id)
);

CREATE TABLE WorkoutExercises (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    SessionId UNIQUEIDENTIFIER NOT NULL,
    ProgramExerciseId UNIQUEIDENTIFIER NOT NULL, -- Links to the program exercise
    SortOrder INT NOT NULL DEFAULT 0,
    IsCompleted BIT NOT NULL DEFAULT 0,
    Notes NVARCHAR(500), -- Exercise-specific notes for this session
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    RowVersion ROWVERSION NOT NULL,
    CONSTRAINT FK_WorkoutExercises_Sessions FOREIGN KEY (SessionId) REFERENCES WorkoutSessions(Id),
    CONSTRAINT FK_WorkoutExercises_ProgramExercises FOREIGN KEY (ProgramExerciseId) REFERENCES ProgramExercises(Id)
);

CREATE TABLE WorkoutSets (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    WorkoutExerciseId UNIQUEIDENTIFIER NOT NULL,
    ProgramExerciseSetId UNIQUEIDENTIFIER, -- Links to planned set (NULL for extra sets)
    SetNumber INT NOT NULL, -- 1, 2, 3, etc.
    Reps INT NOT NULL,
    Weight DECIMAL(10,2), -- NULL for bodyweight
    RestTimeSeconds INT, -- Actual rest time taken
    IsCompleted BIT NOT NULL DEFAULT 0,
    Notes NVARCHAR(300), -- Set-specific notes (e.g., "felt easy", "struggled")
    CompletedAt DATETIME2, -- When this set was marked complete
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    RowVersion ROWVERSION NOT NULL,
    CONSTRAINT FK_WorkoutSets_WorkoutExercises FOREIGN KEY (WorkoutExerciseId) REFERENCES WorkoutExercises(Id),
    CONSTRAINT FK_WorkoutSets_ProgramExerciseSets FOREIGN KEY (ProgramExerciseSetId) REFERENCES ProgramExerciseSets(Id)
);

-- Add any necessary indexes for the new tables
CREATE NONCLUSTERED INDEX IX_ProgramExercises_WorkoutDayId_SortOrder ON ProgramExercises(WorkoutDayId, SortOrder);
CREATE NONCLUSTERED INDEX IX_ProgramExerciseSets_ProgramExerciseId_SetNumber ON ProgramExerciseSets(ProgramExerciseId, SetNumber);

-- Create indexes for better performance
CREATE NONCLUSTERED INDEX IX_Users_AzureAdObjectId ON Users(AzureAdObjectId);
CREATE NONCLUSTERED INDEX IX_Users_Email ON Users(Email);
CREATE NONCLUSTERED INDEX IX_WorkoutPrograms_UserId_IsActive ON WorkoutPrograms(UserId, IsActive);
CREATE NONCLUSTERED INDEX IX_WorkoutSessions_UserId_StartTime ON WorkoutSessions(UserId, StartTime DESC);
CREATE NONCLUSTERED INDEX IX_WorkoutSets_WorkoutExerciseId_SetNumber ON WorkoutSets(WorkoutExerciseId, SetNumber);

-- Program suggestions table (restored)
CREATE TABLE ProgramSuggestions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    CurrentProgramId UNIQUEIDENTIFIER NOT NULL,
    SuggestionType NVARCHAR(100) NOT NULL, -- 'WEIGHT_INCREASE', 'REP_INCREASE', 'EXERCISE_SUBSTITUTION', etc.
    Description NVARCHAR(1000) NOT NULL, -- Human-readable explanation
    SuggestionData NVARCHAR(MAX), -- JSON with specific changes
    Reasoning NVARCHAR(MAX), -- AI's reasoning for the suggestion
    Status NVARCHAR(50) NOT NULL DEFAULT 'PENDING', -- PENDING, ACCEPTED, REJECTED
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    RowVersion ROWVERSION NOT NULL,
    CONSTRAINT FK_ProgramSuggestions_Users FOREIGN KEY (UserId) REFERENCES Users(Id),
    CONSTRAINT FK_ProgramSuggestions_Programs FOREIGN KEY (CurrentProgramId) REFERENCES WorkoutPrograms(Id)
);