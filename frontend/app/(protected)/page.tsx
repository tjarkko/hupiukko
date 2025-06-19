// app/(protected)/page.tsx
"use client";
import { useEffect } from "react";
import { useRouter } from "next/navigation";

export default function ProtectedRedirect() {
  const router = useRouter();
  useEffect(() => {
    router.replace("/training");
  }, [router]);
  return null;
}