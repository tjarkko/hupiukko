"use client";
import { Box, Button, Grid, Typography, Paper } from "@mui/material";
import React, { useState } from "react";
import { useSession } from "next-auth/react";
import { useGetWorkoutPrograms } from "../../../api/generated/workout/workout";
import { ProgramExerciseDto, WorkoutProgramDto } from "../../../api/generated/hupiukkoAPI.schemas";


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
  const [selectedDay, setSelectedDay] = useState("Monday");
  const { data: session } = useSession();
  const userName = session?.user?.name || "User";

  // Fetch workout programs
  const { data, isLoading, error } = useGetWorkoutPrograms();
  const programs: WorkoutProgramDto[] = data?.data || [];
  // Use the first (active) program for now
  const program = programs.find((p) => p.isActive) || programs[0];

  // Map exercises by day
  const exercisesByDay: Record<string, ProgramExerciseDto[]> = {};
  if (program?.programExercises) {
    for (const ex of program.programExercises) {
      if (ex.dayOfWeek) {
        if (!exercisesByDay[ex.dayOfWeek]) exercisesByDay[ex.dayOfWeek] = [];
        exercisesByDay[ex.dayOfWeek].push(ex);
      }
    }
  }

  const selectedExercises = exercisesByDay[selectedDay] || [];

  return (
    <Box p={2}>
      <Typography variant="h5" mb={2} align="center">
        Hi {userName}!
      </Typography>
      <Typography variant="h6" mb={2} align="center">
        Training Program
      </Typography>
      <Grid container spacing={1} justifyContent="center" mb={2}>
        {days.map((day) => (
          <Grid key={day.key} size={2}>
            <Paper
              onClick={() => setSelectedDay(day.key)}
              sx={{
                p: 1,
                textAlign: "center",
                cursor: "pointer",
                bgcolor: selectedDay === day.key ? "primary.main" : "grey.100",
                color: selectedDay === day.key ? "primary.contrastText" : "text.primary",
                fontWeight: selectedDay === day.key ? 600 : 400,
              }}
              elevation={selectedDay === day.key ? 4 : 1}
            >
              {day.label}
            </Paper>
          </Grid>
        ))}
      </Grid>
      <Box textAlign="center" mt={4}>
        {isLoading ? (
          <Typography>Loading...</Typography>
        ) : error ? (
          <Typography color="error">Failed to load program.</Typography>
        ) : selectedExercises.length > 0 ? (
          selectedExercises.map((exercise) => (
            <Box key={exercise.id} mb={2}>
              <Typography variant="subtitle1" mb={1}>
                {exercise.exerciseId} {/* Replace with exercise name if available */}
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