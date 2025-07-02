import { getServerSession } from 'next-auth';
import { redirect } from 'next/navigation';
import { authOptions } from '../api/auth/authOptions';
import LoginButton from './LoginButton';

export default async function LoginPage({ searchParams }: { searchParams: Promise<{ callbackUrl?: string }> }) {
  const session = await getServerSession(authOptions);
  if (session) {
    redirect('/');
  }
  const { callbackUrl = '/' } = await searchParams;
  

  return (
    <main className="flex min-h-screen flex-col items-center justify-center bg-gray-50">
      <h1 className="text-3xl font-bold mb-4">Sign in to Hupiukko</h1>
      <LoginButton callbackUrl={callbackUrl} />
    </main>
  );
} 