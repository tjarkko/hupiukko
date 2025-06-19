'use client';
import React from 'react';
import MUIButton from './MUIButton';
import { signOut, useSession } from 'next-auth/react';
import { azureLogoutUrl } from '../login/page';
import type { Session } from 'next-auth';

export default function AuthShowcase({ session: serverSession }: { session?: Session | null }) {
  // If session is provided as a prop (SSR), use it; otherwise, fallback to useSession
  const { data: clientSession, status } = useSession();
  const session = serverSession ?? clientSession;

  if (!serverSession && status === 'loading') {
    return null;
  }

  if (session) {
    const handleSignOut = async () => {
      await signOut({ redirect: false });
      window.location.href = azureLogoutUrl;
    };
    return (
      <div className="mt-8 flex flex-col items-center">
        <div className="mb-2">Signed in as {session.user?.name}</div>
        <MUIButton variant="outlined" color="secondary" onClick={handleSignOut}>
          Sign out
        </MUIButton>
      </div>
    );
  }

  return null;
} 