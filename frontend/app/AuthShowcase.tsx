'use client';
import React from 'react';
import Button from '@mui/material/Button';
import { signIn, signOut, useSession } from 'next-auth/react';

export default function AuthShowcase() {
  const { data: session } = useSession();

  if (session) {
    return (
      <div className="mt-8 flex flex-col items-center">
        <div className="mb-2">Signed in as {session.user?.email}</div>
        <Button variant="outlined" color="secondary" onClick={() => signOut()}>
          Sign out
        </Button>
      </div>
    );
  }
  return (
    <div className="mt-8 flex flex-col items-center">
      <Button variant="contained" color="primary" onClick={() => signIn('azure-ad')}>
        Sign in with Azure AD
      </Button>
    </div>
  );
} 