import { getAccessToken } from '@/lib/auth'

export type ApiResult<T> = {
  ok: boolean
  status: number
  data: T | null
  error?: string
}

async function request<T>(path: string, init: RequestInit = {}): Promise<ApiResult<T>> {
  const headers = new Headers(init.headers)

  if (!headers.has('Content-Type') && init.body) {
    headers.set('Content-Type', 'application/json')
  }

  const token = getAccessToken()
  if (token) {
    headers.set('Authorization', `Bearer ${token}`)
  }

  try {
    const response = await fetch(path, {
      ...init,
      headers,
    })

    const text = await response.text()
    const data = text ? (JSON.parse(text) as T) : null

    if (!response.ok) {
      return {
        ok: false,
        status: response.status,
        data,
        error: (data && typeof data === 'object' && 'message' in data ? String((data as { message?: string }).message) : null) || response.statusText || 'A apărut o eroare la server.',
      }
    }

    return { ok: true, status: response.status, data }
  } catch (error) {
    return {
      ok: false,
      status: 0,
      data: null,
      error: error instanceof Error ? error.message : 'Nu am putut contacta serverul.',
    }
  }
}

export const api = {
  request,
  get<T>(path: string) {
    return request<T>(path)
  },
  post<T>(path: string, body?: unknown) {
    return request<T>(path, {
      method: 'POST',
      body: body === undefined ? undefined : JSON.stringify(body),
    })
  },
}

export type LoginResponse = {
  accessToken: string
  refreshToken: string
  expiresAtUtc: string
  userId: string
  tenantId: string
  role: string
}

export type DocumentItem = {
  id: string
  tenantId: string
  ownerUserId: string
  title: string
  category: string
  department: string
  tagsCsv: string
  confidentialityLevel: number | string
  expiresAtUtc: string | null
  currentVersionNumber: number
  createdAtUtc: string
}

export type ApprovalItem = {
  id: string
  tenantId: string
  documentId: string
  requestedByUserId: string
  assignedToUserId: string
  status: number | string
  comment: string | null
  createdAtUtc: string
  resolvedAtUtc: string | null
}

export type AuditItem = {
  id: string
  tenantId: string
  userId: string | null
  action: string
  entityType: string
  entityId: string
  metadataJson: string | null
  ipAddress: string
  device: string
  createdAtUtc: string
}
