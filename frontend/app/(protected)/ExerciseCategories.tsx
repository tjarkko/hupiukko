'use client';
import React, { useEffect, useState } from 'react';
import { useGetExercisesCategories } from '../../api/generated/exercises/exercises';

export default function ExerciseCategories() {
  const { data, isLoading, error } = useGetExercisesCategories();

  if (isLoading) return <div className="mt-8">Loading exercise categories...</div>;
  if (error) return <div className="mt-8 text-red-600">Error: {error.message}</div>;

  return (
    <div className="mt-8 w-full max-w-2xl">
      <h2 className="text-xl font-semibold mb-2">Exercise Categories (JSON)</h2>
      <pre className="bg-gray-100 p-4 rounded overflow-x-auto text-xs">
        {JSON.stringify(data, null, 2)}
      </pre>
    </div>
  );
} 