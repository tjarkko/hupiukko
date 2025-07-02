import { NextRequest, NextResponse } from 'next/server';
import { getServerSession } from 'next-auth';
import { authOptions } from '../../auth/authOptions';

const BACKEND_API_URL = process.env.BACKEND_API_URL || 'https://localhost:7151';

export async function GET(req: NextRequest, { params }: { params: Promise<{ path: string[] }> }) {
  const { path } = await params;
  const session = await getServerSession(authOptions);
  const accessToken = session?.accessToken;

  // Reconstruct the backend URL
  const backendPath = path.join('/');
  const url = `${BACKEND_API_URL}/${backendPath}${req.nextUrl.search}`;

  const res = await fetch(url, {
    headers: {
      ...(accessToken ? { Authorization: `Bearer ${accessToken}` } : {}),
      'Content-Type': 'application/json',
    },
  });

  const data = await res.json();
  return NextResponse.json(data, { status: res.status });
} 