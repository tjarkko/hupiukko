"use client";
import { Box, Typography, Button } from "@mui/material";
import React from "react";
import { signOut, useSession } from "next-auth/react";
import { azureLogoutUrl } from "../../login/page";

export default function SettingsPage() {
  const { data: session } = useSession();
  const userName = session?.user?.name || "User";

  const handleSignOut = async () => {
    await signOut({ redirect: false });
    window.location.href = azureLogoutUrl;
  };

  return (
    <Box p={2}>
      <Typography variant="h6" mb={2} align="center">
        Settings
      </Typography>
      <Typography variant="body1">User: {userName}</Typography>
      {/* TODO: Add more settings here */}
      <Button variant="outlined" color="secondary" onClick={handleSignOut} sx={{ mt: 3 }}>
        Sign out
      </Button>
    </Box>
  );
} 