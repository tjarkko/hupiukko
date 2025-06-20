'use client';
import { SessionProvider, signOut } from 'next-auth/react';
import React from 'react';
import { QueryClient, QueryClientProvider, QueryCache, MutationCache } from '@tanstack/react-query';

export default function Providers({ children }: { children: React.ReactNode }) {
  const [queryClient] = React.useState(() => new QueryClient({
    queryCache: new QueryCache({
      onError: (error: any) => {
        if (error?.response?.status === 401 || error?.response?.status === 403) {
          signOut();
        }
      }
    }),
    mutationCache: new MutationCache({
      onError: (error: any) => {
        if (error?.response?.status === 401 || error?.response?.status === 403) {
          signOut();
        }
      }
    })
  }));
  return (
    <SessionProvider>
      <QueryClientProvider client={queryClient}>
        {children}
      </QueryClientProvider>
    </SessionProvider>
  );
} 