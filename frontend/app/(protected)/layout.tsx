import { getServerSession } from "next-auth";
import { redirect } from "next/navigation";
import { authOptions } from "../api/auth/[...nextauth]/route";
import React from "react";
import Providers from "../Providers";
import MobileLayout from "./MobileLayout";

export default async function ProtectedLayout({ children }: { children: React.ReactNode }) {
  const session = await getServerSession(authOptions);
  if (!session) {
    // Use '/' as fallback since getting the current path in a layout is non-trivial in App Router
    redirect(`/login?callbackUrl=${encodeURIComponent('/')}`);
  }
  return (
    <Providers>
      <MobileLayout>{children}</MobileLayout>
    </Providers>
  );
}

