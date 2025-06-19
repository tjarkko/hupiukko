'use client';
import Button, { ButtonProps } from '@mui/material/Button';
import React from 'react';

export default function MUIButton(props: ButtonProps) {
  return <Button {...props}>{props.children}</Button>;
} 