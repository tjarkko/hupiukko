'use client';
import { signIn } from 'next-auth/react';
import Button from '@mui/material/Button';

export default function LoginButton({ callbackUrl }: { callbackUrl: string }) {
  return (
    <Button
      variant="contained"
      color="primary"
      onClick={() => signIn('azure-ad', { callbackUrl })}
    >
      Sign in with Azure AD
    </Button>
  );
} 