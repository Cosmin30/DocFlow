import { getAccessToken, getRefreshToken, setSessionTokens } from '@/lib/auth'

export type ApiResult<T> = {
  ok: boolean
  status: number
  data: T | null
  error?: string
}

type ProblemDetails = {
  title?: string
  detail?: string
  message?: string
  errors?: Record<string, string[] | string>
}

let refreshPromise: Promise<boolean> | null = null

async function refreshAccessToken(): Promise<boolean> {
  const refreshToken = getRefreshToken()
  if (!refreshToken) return false

  try {
    const refreshed = await fetch('/auth/refresh', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ refreshToken }),
    })

    if (!refreshed.ok) return false

    const refreshedText = await refreshed.text()
    const parsed = parseResponseText<LoginResponse>(refreshedText)

    if (parsed.data) {
      setSessionTokens(parsed.data.accessToken, parsed.data.refreshToken)
      return true
    }

    return false
  } catch {
    return false
  }
}

function parseResponseText<T>(text: string): { data: T | null; raw: unknown | null } {
  if (!text.trim()) {
    return { data: null, raw: null }
  }

  try {
    const parsed = JSON.parse(text)
    return { data: parsed as T, raw: parsed }
  } catch {
    return { data: null, raw: text }
  }
}

function extractErrorMessage(raw: unknown, fallback: string): string {
  if (typeof raw === 'string') {
    return raw || fallback
  }

  if (!raw || typeof raw !== 'object') {
    return fallback
  }

  const problem = raw as ProblemDetails
  if (problem.detail) return problem.detail
  if (problem.title) return problem.title
  if (problem.message) return problem.message

  if (problem.errors && typeof problem.errors === 'object') {
    const firstError = Object.values(problem.errors).flat().find((value) => Boolean(String(value).trim()))
    if (firstError) return String(firstError)
  }

  return fallback
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
    const response = await fetch(path, { ...init, headers })
    const text = await response.text()
    const parsed = parseResponseText<T>(text)

    if (response.status === 401 && path !== '/auth/login' && path !== '/auth/refresh') {
      if (!refreshPromise) {
        refreshPromise = refreshAccessToken().finally(() => {
          refreshPromise = null
        })
      }

      const refreshed = await refreshPromise

      if (refreshed) {
        const newToken = getAccessToken()
        if (newToken) {
          const retryHeaders = new Headers(init.headers)
          retryHeaders.set('Authorization', `Bearer ${newToken}`)
          if (!retryHeaders.has('Content-Type') && init.body) {
            retryHeaders.set('Content-Type', 'application/json')
          }

          const retryResponse = await fetch(path, { ...init, headers: retryHeaders })
          const retryText = await retryResponse.text()
          const retryParsed = parseResponseText<T>(retryText)

          if (retryResponse.ok) {
            return { ok: true, status: retryResponse.status, data: retryParsed.data }
          }

          return {
            ok: false,
            status: retryResponse.status,
            data: retryParsed.data,
            error: extractErrorMessage(retryParsed.raw, retryResponse.statusText || 'Server error.'),
          }
        }
      }

      return {
        ok: false,
        status: 401,
        data: null,
        error: 'Session expired. Please sign in again.',
      }
    }

    if (!response.ok) {
      return {
        ok: false,
        status: response.status,
        data: parsed.data,
        error: extractErrorMessage(parsed.raw, response.statusText || 'Server error.'),
      }
    }

    return { ok: true, status: response.status, data: parsed.data }
  } catch (error) {
    return {
      ok: false,
      status: 0,
      data: null,
      error: error instanceof Error ? error.message : 'Unable to reach the server.',
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
  put<T>(path: string, body?: unknown) {
    return request<T>(path, {
      method: 'PUT',
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

export type DocumentVersionItem = {
  id: string
  documentId: string
  versionNumber: number
  fileName: string
  storagePath: string
  sizeBytes: number
  uploadedByUserId: string
  createdAtUtc: string
}
