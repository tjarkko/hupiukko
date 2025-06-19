import { getServerSession } from 'next-auth';
import { authOptions } from '../api/auth/[...nextauth]/route';
import AuthShowcase from './AuthShowcase';
import MUIButton from './MUIButton';

export default async function HomePage() {
  const session = await getServerSession(authOptions);

  return (
    <main className="flex min-h-screen flex-col items-center justify-center bg-gray-50">
      <h1 className="text-4xl font-bold mb-6">Welcome to Hupiukko</h1>
      <MUIButton variant="contained" color="primary">
        MUI Button
      </MUIButton>
      <p className="mt-4 text-gray-600">Tailwind and MUI are both working!</p>
      <AuthShowcase session={session} />
    </main>
  );
} 