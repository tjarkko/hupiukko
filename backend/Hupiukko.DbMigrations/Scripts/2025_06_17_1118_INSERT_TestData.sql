-- Test data for Hupiukko Gym App
-- Insert exercise categories, exercises, users, and sample workout programs

-- Insert Exercise Categories
INSERT INTO ExerciseCategories (Id, Name, Description, SortOrder, CreatedAt, UpdatedAt, IsDeleted)
VALUES 
    (NEWID(), 'Upper Body', 'Exercises targeting upper body muscles', 1, GETUTCDATE(), GETUTCDATE(), 0),
    (NEWID(), 'Lower Body', 'Exercises targeting lower body muscles', 2, GETUTCDATE(), GETUTCDATE(), 0),
    (NEWID(), 'Core', 'Exercises targeting core muscles', 3, GETUTCDATE(), GETUTCDATE(), 0),
    (NEWID(), 'Cardio', 'Cardiovascular exercises', 4, GETUTCDATE(), GETUTCDATE(), 0);

-- Get category IDs for exercises
DECLARE @UpperBodyId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM ExerciseCategories WHERE Name = 'Upper Body');
DECLARE @LowerBodyId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM ExerciseCategories WHERE Name = 'Lower Body');
DECLARE @CoreId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM ExerciseCategories WHERE Name = 'Core');

-- Insert Exercises
INSERT INTO Exercises (Id, Name, Description, CategoryId, MuscleGroups, Equipment, DifficultyLevel, IsActive, CreatedAt, UpdatedAt, IsDeleted)
VALUES 
    -- Upper Body
    (NEWID(), 'Push-ups', 'Classic bodyweight exercise for chest, shoulders, and triceps', @UpperBodyId, 'chest,shoulders,triceps', 'bodyweight', 'Beginner', 1, GETUTCDATE(), GETUTCDATE(), 0),
    (NEWID(), 'Pull-ups', 'Bodyweight exercise for back and biceps', @UpperBodyId, 'back,biceps', 'pull-up bar', 'Intermediate', 1, GETUTCDATE(), GETUTCDATE(), 0),
    (NEWID(), 'Bench Press', 'Compound exercise for chest, shoulders, and triceps', @UpperBodyId, 'chest,shoulders,triceps', 'barbell,bench', 'Intermediate', 1, GETUTCDATE(), GETUTCDATE(), 0),
    (NEWID(), 'Dumbbell Rows', 'Back exercise using dumbbells', @UpperBodyId, 'back,biceps', 'dumbbells,bench', 'Beginner', 1, GETUTCDATE(), GETUTCDATE(), 0),
    
    -- Lower Body
    (NEWID(), 'Squats', 'Fundamental lower body exercise', @LowerBodyId, 'quadriceps,glutes,hamstrings', 'bodyweight', 'Beginner', 1, GETUTCDATE(), GETUTCDATE(), 0),
    (NEWID(), 'Deadlifts', 'Compound exercise for posterior chain', @LowerBodyId, 'hamstrings,glutes,back', 'barbell', 'Advanced', 1, GETUTCDATE(), GETUTCDATE(), 0),
    (NEWID(), 'Lunges', 'Unilateral leg exercise', @LowerBodyId, 'quadriceps,glutes,hamstrings', 'bodyweight', 'Beginner', 1, GETUTCDATE(), GETUTCDATE(), 0),
    (NEWID(), 'Leg Press', 'Machine-based leg exercise', @LowerBodyId, 'quadriceps,glutes', 'leg press machine', 'Beginner', 1, GETUTCDATE(), GETUTCDATE(), 0),
    
    -- Core
    (NEWID(), 'Plank', 'Isometric core exercise', @CoreId, 'core,shoulders', 'bodyweight', 'Beginner', 1, GETUTCDATE(), GETUTCDATE(), 0),
    (NEWID(), 'Crunches', 'Basic abdominal exercise', @CoreId, 'abs', 'bodyweight', 'Beginner', 1, GETUTCDATE(), GETUTCDATE(), 0);

-- Insert Test Users
INSERT INTO Users (Id, AzureAdObjectId, Email, FirstName, LastName, DisplayName, TimeZone, CreatedAt, UpdatedAt, IsDeleted)
VALUES 
    (NEWID(), 'test-user-1', 'john.doe@example.com', 'John', 'Doe', 'John Doe', 'UTC', GETUTCDATE(), GETUTCDATE(), 0),
    (NEWID(), 'test-user-2', 'jane.smith@example.com', 'Jane', 'Smith', 'Jane Smith', 'UTC', GETUTCDATE(), GETUTCDATE(), 0);

-- Get user and exercise IDs for sample program
DECLARE @UserId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Users WHERE Email = 'john.doe@example.com');
DECLARE @PushUpId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Exercises WHERE Name = 'Push-ups');
DECLARE @SquatId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Exercises WHERE Name = 'Squats');
DECLARE @PlankId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Exercises WHERE Name = 'Plank');

-- Insert Sample Workout Program
DECLARE @ProgramId UNIQUEIDENTIFIER = NEWID();
INSERT INTO WorkoutPrograms (Id, UserId, Name, Description, IsActive, StartDate, CreatedAt, UpdatedAt, IsDeleted)
VALUES 
    (@ProgramId, @UserId, 'Beginner Full Body', 'A simple full-body workout for beginners', 1, GETUTCDATE(), GETUTCDATE(), GETUTCDATE(), 0);

-- Insert WorkoutDays
DECLARE @Day1Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Day2Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Day3Id UNIQUEIDENTIFIER = NEWID();
INSERT INTO WorkoutDays (Id, ProgramId, Name, DayOfWeek, SortOrder, Notes, CreatedAt, UpdatedAt, IsDeleted, RowVersion)
VALUES
    (@Day1Id, @ProgramId, 'Day 1 - Push-ups', 1, 1, 'Push focus', GETUTCDATE(), GETUTCDATE(), 0, DEFAULT),
    (@Day2Id, @ProgramId, 'Day 2 - Squats', 3, 2, 'Leg focus', GETUTCDATE(), GETUTCDATE(), 0, DEFAULT),
    (@Day3Id, @ProgramId, 'Day 3 - Plank', 0, 3, 'Core focus', GETUTCDATE(), GETUTCDATE(), 0, DEFAULT);

-- Insert Program Exercises (now reference WorkoutDayId)
DECLARE @ProgramExercise1 UNIQUEIDENTIFIER = NEWID();
DECLARE @ProgramExercise2 UNIQUEIDENTIFIER = NEWID();
DECLARE @ProgramExercise3 UNIQUEIDENTIFIER = NEWID();

INSERT INTO ProgramExercises (Id, WorkoutDayId, ExerciseId, SortOrder, TargetSets, DefaultRepsMin, DefaultRepsMax, DefaultWeight, DefaultRestTimeSeconds, Notes, CreatedAt, UpdatedAt, IsDeleted, RowVersion)
VALUES 
    (@ProgramExercise1, @Day1Id, @PushUpId, 1, 3, 8, 12, NULL, 60, NULL, GETUTCDATE(), GETUTCDATE(), 0, DEFAULT),
    (@ProgramExercise2, @Day2Id, @SquatId, 1, 3, 10, 15, NULL, 90, NULL, GETUTCDATE(), GETUTCDATE(), 0, DEFAULT),
    (@ProgramExercise3, @Day3Id, @PlankId, 1, 3, 30, 60, NULL, 60, NULL, GETUTCDATE(), GETUTCDATE(), 0, DEFAULT);

-- Insert Program Exercise Sets (example of pyramid sets for push-ups)
INSERT INTO ProgramExerciseSets (Id, ProgramExerciseId, SetNumber, TargetRepsMin, TargetRepsMax, TargetWeight, RestTimeSeconds, Notes, CreatedAt, UpdatedAt, IsDeleted)
VALUES 
    (NEWID(), @ProgramExercise1, 1, 12, 15, NULL, 60, 'Warm-up set', GETUTCDATE(), GETUTCDATE(), 0),
    (NEWID(), @ProgramExercise1, 2, 8, 10, NULL, 60, 'Working set', GETUTCDATE(), GETUTCDATE(), 0),
    (NEWID(), @ProgramExercise1, 3, 6, 8, NULL, 60, 'Challenging set', GETUTCDATE(), GETUTCDATE(), 0); 