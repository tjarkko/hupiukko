'use client';
import React, { useEffect, useState } from 'react';

export default function ExerciseCategories() {
  const [data, setData] = useState<any>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetch('/api/proxy/Exercises/categories')
      .then(res => res.json())
      .then(json => {
        setData(json);
        setLoading(false);
      })
      .catch(err => {
        setError(err.message);
        setLoading(false);
      });
  }, []);

  if (loading) return <div className="mt-8">Loading exercise categories...</div>;
  if (error) return <div className="mt-8 text-red-600">Error: {error}</div>;

  return (
    <div className="mt-8 w-full max-w-2xl">
      <h2 className="text-xl font-semibold mb-2">Exercise Categories (JSON)</h2>
      <pre className="bg-gray-100 p-4 rounded overflow-x-auto text-xs">
        {JSON.stringify(data, null, 2)}
      </pre>
    </div>
  );
} 