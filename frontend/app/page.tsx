import React from 'react';
import Button from '@mui/material/Button';
import AuthShowcase from './AuthShowcase';

export default function HomePage() {
  return (
    <main className="flex min-h-screen flex-col items-center justify-center bg-gray-50">
      <h1 className="text-4xl font-bold mb-6">Welcome to Hupiukko</h1>
      <Button variant="contained" color="primary">
        MUI Button
      </Button>
      <p className="mt-4 text-gray-600">Tailwind and MUI are both working!</p>
      <AuthShowcase />
    </main>
  );
} 