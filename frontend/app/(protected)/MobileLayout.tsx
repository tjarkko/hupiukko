"use client";
import { usePathname, useRouter } from "next/navigation";
import { BottomNavigation, BottomNavigationAction, CssBaseline, Paper } from "@mui/material";
import FitnessCenterIcon from "@mui/icons-material/FitnessCenter";
import SettingsIcon from "@mui/icons-material/Settings";
import React from "react";

const navItems = [
  { label: "Training", value: "/training", icon: <FitnessCenterIcon /> },
  { label: "Settings", value: "/settings", icon: <SettingsIcon /> },
];

export default function MobileLayout({ children }: { children: React.ReactNode }) {
  const pathname = usePathname();
  const router = useRouter();
  const current = pathname;

  return (
    <>
      <CssBaseline />
      <main style={{ paddingBottom: 56 }}>{children}</main>
      <Paper sx={{ position: "fixed", bottom: 0, left: 0, right: 0 }} elevation={3}>
        <BottomNavigation
          showLabels
          value={navItems.findIndex((item) => current.startsWith(item.value))}
          onChange={(_, newValue) => router.push(navItems[newValue].value)}
        >
          {navItems.map((item) => (
            <BottomNavigationAction key={item.value} label={item.label} icon={item.icon} />
          ))}
        </BottomNavigation>
      </Paper>
    </>
  );
} 