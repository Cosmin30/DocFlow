import { clsx, type ClassValue } from 'clsx'
import { twMerge } from 'tailwind-merge'

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs))
}

export function formatDate(iso: string | null | undefined): string {
  if (!iso) return '—'
  try {
    return new Date(iso).toLocaleDateString('ro-RO', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
    })
  } catch {
    return iso
  }
}

export function formatDateTime(iso: string | null | undefined): string {
  if (!iso) return '—'
  try {
    return new Date(iso).toLocaleString('ro-RO', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    })
  } catch {
    return iso
  }
}

export function confidentialityLabel(value: number | string): string {
  const normalized = String(value)
  if (normalized === '0' || normalized.toLowerCase() === 'public') return 'Public'
  if (normalized === '1' || normalized.toLowerCase() === 'internal') return 'Intern'
  if (normalized === '2' || normalized.toLowerCase() === 'confidential') return 'Confidențial'
  if (normalized === '3' || normalized.toLowerCase() === 'strict') return 'Strict'
  return normalized
}

export function approvalStatusLabel(value: number | string): string {
  const normalized = String(value)
  if (normalized === '0' || normalized.toLowerCase() === 'pending') return 'În așteptare'
  if (normalized === '1' || normalized.toLowerCase() === 'approved') return 'Aprobat'
  if (normalized === '2' || normalized.toLowerCase() === 'rejected') return 'Respins'
  return normalized
}

export function formatBytes(bytes: number): string {
  if (bytes === 0) return '0 B'
  const k = 1024
  const sizes = ['B', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return parseFloat((bytes / Math.pow(k, i)).toFixed(1)) + ' ' + sizes[i]
}
