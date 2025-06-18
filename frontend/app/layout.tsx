import React from 'react';
import '../globals.css';
import type { ReactNode } from 'react';
import Providers from './Providers';

export const metadata = {
  title: 'Hupiukko',
  description: 'Gym workout logging app',
};

export default function RootLayout({ children }: { children: ReactNode }) {
  return (
    <html lang="en">
      <body>
        <Providers>{children}</Providers>
      </body>
    </html>
  );
} 