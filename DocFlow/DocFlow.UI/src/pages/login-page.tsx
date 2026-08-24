import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { ShieldCheck } from 'lucide-react'
import { api, type LoginResponse } from '@/lib/api'
import { setSessionTokens } from '@/lib/auth'

export function LoginPage() {
  const navigate = useNavigate()
  const [tenantSlug, setTenantSlug] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [errorMessage, setErrorMessage] = useState<string | null>(null)
  const [isSubmitting, setIsSubmitting] = useState(false)

  return (
    <main className="min-h-screen bg-background text-foreground">
      <div className="mx-auto flex min-h-screen w-full max-w-md flex-col items-center justify-center p-6">
        <div className="w-full space-y-8">
          <div className="space-y-2 text-center">
            <div className="flex items-center justify-center gap-2 text-sm text-muted-foreground">
              <ShieldCheck className="h-5 w-5" />
              <span className="font-medium">DocFlow</span>
            </div>
            <h1 className="text-2xl font-semibold tracking-tight">Sign in</h1>
            <p className="text-sm text-muted-foreground">Enter your credentials to access the platform.</p>
          </div>

          <Card>
            <CardContent className="p-6">
              <CardHeader className="p-0 mb-6">
                <CardTitle className="text-lg">Welcome back</CardTitle>
              </CardHeader>

              <form
                className="space-y-4"
                onSubmit={async (event) => {
                  event.preventDefault()
                  setErrorMessage(null)
                  setIsSubmitting(true)

                  const result = await api.post<LoginResponse>('/auth/login', {
                    tenantSlug,
                    email,
                    password,
                    device: 'Web',
                  })

                  setIsSubmitting(false)

                  if (!result.ok || !result.data) {
                    setErrorMessage(result.error ?? 'Invalid credentials.')
                    return
                  }

                  setSessionTokens(result.data.accessToken, result.data.refreshToken)
                  navigate('/dashboard')
                }}
              >
                <div className="space-y-2">
                  <label className="text-sm font-medium" htmlFor="tenantSlug">
                    Tenant
                  </label>
                  <Input id="tenantSlug" value={tenantSlug} onChange={(event) => setTenantSlug(event.target.value)} placeholder="e.g. acme" />
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium" htmlFor="email">
                    Email
                  </label>
                  <Input id="email" type="email" value={email} onChange={(event) => setEmail(event.target.value)} placeholder="you@company.com" />
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium" htmlFor="password">
                    Password
                  </label>
                  <Input
                    id="password"
                    type="password"
                    value={password}
                    onChange={(event) => setPassword(event.target.value)}
                    placeholder="Enter your password"
                  />
                </div>

                {errorMessage ? <p className="rounded-lg border border-destructive/30 bg-destructive/5 p-3 text-sm text-destructive">{errorMessage}</p> : null}

                <Button className="w-full" type="submit" disabled={isSubmitting}>
                  {isSubmitting ? 'Signing in...' : 'Sign in'}
                </Button>
              </form>

              <div className="mt-4 text-center">
                <p className="text-sm text-muted-foreground">
                  Don't have an account?{' '}
                  <button type="button" className="font-medium text-foreground underline underline-offset-4 hover:text-muted-foreground" onClick={() => navigate('/register')}>
                    Create one
                  </button>
                </p>
              </div>
            </CardContent>
          </Card>
        </div>
      </div>
    </main>
  )
}
