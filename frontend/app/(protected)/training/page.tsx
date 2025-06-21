"use client";
import { Box, Button, Grid, Typography, Paper, Chip, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Divider } from "@mui/material";
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
          <>
            {selectedExercises.map((exercise) => {
              const ex = exercises.find((e) => e.id === exercise.exerciseId);
              return (
                <Paper key={exercise.id} sx={{ mb: 2, p: 2, textAlign: "left" }} elevation={2}>
                  <Typography variant="h6" gutterBottom>
                    {ex?.name || exercise.exerciseId}
                  </Typography>
                  {ex?.description && (
                    <Typography variant="body2" color="text.secondary" gutterBottom>
                      {ex.description}
                    </Typography>
                  )}
                  <Box sx={{ display: "flex", gap: 1, flexWrap: "wrap", mb: 1 }}>
                    {ex?.muscleGroups && <Chip label={ex.muscleGroups} size="small" />}
                    {ex?.equipment && <Chip label={ex.equipment} size="small" />}
                    {ex?.difficultyLevel && <Chip label={ex.difficultyLevel} size="small" />}
                  </Box>
                  <Divider sx={{ my: 1 }} />
                  {exercise.programExerciseSets && exercise.programExerciseSets.length > 0 ? (
                    <TableContainer>
                      <Table size="small">
                        <TableHead>
                          <TableRow>
                            <TableCell>Set</TableCell>
                            <TableCell>Reps</TableCell>
                            <TableCell>Weight</TableCell>
                            <TableCell>Rest (s)</TableCell>
                            <TableCell>Notes</TableCell>
                          </TableRow>
                        </TableHead>
                        <TableBody>
                          {exercise.programExerciseSets.map((set) => (
                            <TableRow key={set.id || set.setNumber}>
                              <TableCell>{set.setNumber}</TableCell>
                              <TableCell>{set.targetRepsMin}{set.targetRepsMax ? `-${set.targetRepsMax}` : ""}</TableCell>
                              <TableCell>{set.targetWeight ?? "-"}</TableCell>
                              <TableCell>{set.restTimeSeconds ?? "-"}</TableCell>
                              <TableCell>{set.notes ?? ""}</TableCell>
                            </TableRow>
                          ))}
                        </TableBody>
                      </Table>
                    </TableContainer>
                  ) : (
                    <Box sx={{ mb: 1 }}>
                      <Typography variant="body2">
                        Sets: <b>{exercise.targetSets ?? "-"}</b>, Reps: <b>{exercise.defaultRepsMin}{exercise.defaultRepsMax ? `-${exercise.defaultRepsMax}` : ""}</b>
                        {exercise.defaultWeight !== undefined && exercise.defaultWeight !== null && (
                          <> , Weight: <b>{exercise.defaultWeight}</b></>
                        )}
                        {exercise.defaultRestTimeSeconds !== undefined && exercise.defaultRestTimeSeconds !== null && (
                          <> , Rest: <b>{exercise.defaultRestTimeSeconds}s</b></>
                        )}
                      </Typography>
                    </Box>
                  )}
                  {exercise.notes && (
                    <Typography variant="body2" color="text.secondary" mt={1}>
                      <i>{exercise.notes}</i>
                    </Typography>
                  )}
                </Paper>
              );
            })}
            <Button variant="contained" color="primary" sx={{ mt: 2 }}>
              Start Training Session
            </Button>
          </>
        ) : (
          <Typography color="text.secondary">No exercise planned for this day.</Typography>
        )}
      </Box>
    </Box>
  );
} 