"use client";
import { Box, Button, Grid, Typography, Paper } from "@mui/material";
import React, { useState } from "react";
import { useSession } from "next-auth/react";

const days = ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"];

// TODO: Replace with API data
const mockExercises = {
  Mon: { name: "Push Day", planned: true },
  Tue: { name: "Pull Day", planned: true },
  Wed: { name: "Legs", planned: true },
  Thu: { name: "Rest", planned: false },
  Fri: { name: "Push Day", planned: true },
  Sat: { name: "Pull Day", planned: true },
  Sun: { name: "Rest", planned: false },
};

export default function TrainingPage() {
  const [selectedDay, setSelectedDay] = useState("Mon");
  const exercise = mockExercises[selectedDay];
  const { data: session } = useSession();
  const userName = session?.user?.name || "User";

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
          <Grid key={day} size={2}>
            <Paper
              onClick={() => setSelectedDay(day)}
              sx={{
                p: 1,
                textAlign: "center",
                cursor: "pointer",
                bgcolor: selectedDay === day ? "primary.main" : "grey.100",
                color: selectedDay === day ? "primary.contrastText" : "text.primary",
                fontWeight: selectedDay === day ? 600 : 400,
              }}
              elevation={selectedDay === day ? 4 : 1}
            >
              {day}
            </Paper>
          </Grid>
        ))}
      </Grid>
      <Box textAlign="center" mt={4}>
        {exercise && exercise.planned ? (
          <>
            <Typography variant="subtitle1" mb={2}>
              {exercise.name}
            </Typography>
            <Button variant="contained" color="primary">
              Start Exercise
            </Button>
          </>
        ) : (
          <Typography color="text.secondary">No exercise planned for this day.</Typography>
        )}
      </Box>
    </Box>
  );
} 