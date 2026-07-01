const ACCESS_TOKEN_KEY = 'docflow_access_token'
const REFRESH_TOKEN_KEY = 'docflow_refresh_token'

export function getAccessToken() {
  return localStorage.getItem(ACCESS_TOKEN_KEY)
}

export function getRefreshToken() {
  return localStorage.getItem(REFRESH_TOKEN_KEY)
}

export function setSessionTokens(accessToken: string, refreshToken?: string) {
  localStorage.setItem(ACCESS_TOKEN_KEY, accessToken)

  if (refreshToken) {
    localStorage.setItem(REFRESH_TOKEN_KEY, refreshToken)
  }
}

export function clearSessionTokens() {
  localStorage.removeItem(ACCESS_TOKEN_KEY)
  localStorage.removeItem(REFRESH_TOKEN_KEY)
}

export async function logout() {
  try {
    const token = getAccessToken()
    if (token) {
      await fetch('/auth/logout-all', {
        method: 'POST',
        headers: {
          Authorization: `Bearer ${token}`,
        },
      })
    }
  } finally {
    clearSessionTokens()
  }
}
