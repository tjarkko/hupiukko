import { getServerSession } from 'next-auth';
import { redirect } from 'next/navigation';
import { authOptions } from '../api/auth/[...nextauth]/route';
import LoginButton from './LoginButton';

export const azureLogoutUrl = `https://login.microsoftonline.com/common/oauth2/v2.0/logout?post_logout_redirect_uri=${encodeURIComponent('http://localhost:3000/login')}`;

export default async function LoginPage({ searchParams }: { searchParams: { callbackUrl?: string } }) {
  const session = await getServerSession(authOptions);
  if (session) {
    redirect('/');
  }
  const callbackUrl = searchParams?.callbackUrl || '/';

  return (
    <main className="flex min-h-screen flex-col items-center justify-center bg-gray-50">
      <h1 className="text-3xl font-bold mb-4">Sign in to Hupiukko</h1>
      <LoginButton callbackUrl={callbackUrl} />
    </main>
  );
} 