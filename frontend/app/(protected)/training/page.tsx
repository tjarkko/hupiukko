"use client";
import { Box, Button, Grid, Typography, Paper } from "@mui/material";
import React, { useState } from "react";
import { useSession } from "next-auth/react";
import { useGetWorkoutPrograms } from "../../../api/generated/workout/workout";
import { ProgramExerciseDto, WorkoutProgramDto, WorkoutDayDto, ExerciseDto } from "../../../api/generated/hupiukkoAPI.schemas";
import { useGetExercises } from "../../../api/generated/exercises/exercises";


const days = [
  { key: "Monday", label: "Mon" },
  { key: "Tuesday", label: "Tue" },
  { key: "Wednesday", label: "Wed" },
  { key: "Thursday", label: "Thu" },
  { key: "Friday", label: "Fri" },
  { key: "Saturday", label: "Sat" },
  { key: "Sunday", label: "Sun" },
];

export default function TrainingPage() {
  const { data: session } = useSession();
  const userName = session?.user?.name || "User";

  // Fetch workout programs
  const { data, isLoading, error } = useGetWorkoutPrograms();
  const programs: WorkoutProgramDto[] = data?.data || [];
  // Use the first (active) program for now
  const program = programs.find((p) => p.isActive) || programs[0];

  // Fetch all exercises for name lookup
  const { data: exercisesData } = useGetExercises();
  const exercises: ExerciseDto[] = exercisesData?.data || [];
  const exerciseMap = Object.fromEntries(
    exercises.map((ex) => [ex.id, ex.name])
  );

  // Get workout days from the program
  const workoutDays: WorkoutDayDto[] = program?.workoutDays || [];

  // Helper: Map day key to workoutDay
  const workoutDayMap = Object.fromEntries(
    workoutDays.map((d) => [d.dayOfWeek || d.name, d])
  );

  // Selected day logic: allow selecting any weekday
  const [selectedDayKey, setSelectedDayKey] = useState<string>(days[0].key);
  const selectedWorkoutDay = workoutDayMap[selectedDayKey];
  const selectedExercises =
    selectedWorkoutDay?.programExercises?.sort((a, b) => (a.sortOrder ?? 0) - (b.sortOrder ?? 0)) || [];

  return (
    <Box p={2}>
      <Typography variant="h5" mb={2} align="center">
        Hi {userName}!
      </Typography>
      <Typography variant="h6" mb={2} align="center">
        Training Program
      </Typography>
      <Grid container spacing={1} justifyContent="center" mb={2}>
        {days.map((day) => {
          const workoutDay = workoutDayMap[day.key];
          const hasExercises =
            workoutDay && workoutDay.programExercises && workoutDay.programExercises.length > 0;
          return (
            <Grid key={day.key} >
              <Paper
                onClick={() => setSelectedDayKey(day.key)}
                sx={{
                  p: 1,
                  textAlign: "center",
                  cursor: "pointer",
                  bgcolor: selectedDayKey === day.key ? "primary.main" : "grey.100",
                  color: selectedDayKey === day.key ? "primary.contrastText" : "text.primary",
                  fontWeight: selectedDayKey === day.key ? 600 : 400,
                  border: hasExercises ? "2px solid #1976d2" : undefined,
                  position: "relative",
                  minWidth: 48,
                }}
                elevation={selectedDayKey === day.key ? 4 : 1}
              >
                <span style={{ fontWeight: hasExercises ? 700 : 400 }}>
                  {day.label}
                </span>
                {hasExercises && (
                  <span
                    style={{
                      display: "block",
                      width: 8,
                      height: 8,
                      borderRadius: "50%",
                      background: selectedDayKey === day.key ? "#fff" : "#1976d2",
                      position: "absolute",
                      bottom: 4,
                      left: "50%",
                      transform: "translateX(-50%)",
                    }}
                  />
                )}
              </Paper>
            </Grid>
          );
        })}
      </Grid>
      <Box textAlign="center" mt={4}>
        {isLoading ? (
          <Typography>Loading...</Typography>
        ) : error ? (
          <Typography color="error">Failed to load program.</Typography>
        ) : selectedWorkoutDay && selectedExercises.length > 0 ? (
          selectedExercises.map((exercise) => (
            <Box key={exercise.id} mb={2}>
              <Typography variant="subtitle1" mb={1}>
                {exercise.exerciseId && exerciseMap[exercise.exerciseId]
                  ? exerciseMap[exercise.exerciseId]
                  : exercise.exerciseId}
              </Typography>
              <Button variant="contained" color="primary">
                Start Exercise
              </Button>
            </Box>
          ))
        ) : (
          <Typography color="text.secondary">No exercise planned for this day.</Typography>
        )}
      </Box>
    </Box>
  );
} 