import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { api } from '@/lib/api'

const roleOptions = [
  { label: 'Admin', value: 1 },
  { label: 'Manager', value: 2 },
  { label: 'Employee', value: 3 },
  { label: 'Auditor (read-only)', value: 4 },
] as const

export function RegisterPage() {
  const navigate = useNavigate()
  const [tenantName, setTenantName] = useState('')
  const [tenantSlug, setTenantSlug] = useState('')
  const [fullName, setFullName] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [role, setRole] = useState<number>(3)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)
  const [isSubmitting, setIsSubmitting] = useState(false)

  return (
    <main className="min-h-screen bg-background text-foreground">
      <div className="mx-auto flex min-h-screen w-full max-w-md flex-col items-center justify-center p-6">
        <div className="w-full space-y-8">
          <div className="space-y-2 text-center">
            <h1 className="text-2xl font-semibold tracking-tight">Create an account</h1>
            <p className="text-sm text-muted-foreground">Set up your tenant and first user to get started.</p>
          </div>

          <Card>
            <CardContent className="p-6">
              <CardHeader className="p-0 mb-6">
                <CardTitle className="text-lg">Registration</CardTitle>
              </CardHeader>

              <form
                className="space-y-4"
                onSubmit={async (event) => {
                  event.preventDefault()
                  setErrorMessage(null)
                  setIsSubmitting(true)

                  const result = await api.post<{ message: string }>('/auth/register', {
                    tenantName,
                    tenantSlug,
                    fullName,
                    email,
                    password,
                    role,
                  })

                  setIsSubmitting(false)

                  if (!result.ok) {
                    setErrorMessage(result.error ?? 'Registration failed.')
                    return
                  }

                  navigate('/login', { replace: true })
                }}
              >
                <div className="space-y-2">
                  <label className="text-sm font-medium" htmlFor="tenantName">Tenant name</label>
                  <Input id="tenantName" value={tenantName} onChange={(event) => setTenantName(event.target.value)} placeholder="Acme Corp" />
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium" htmlFor="tenantSlug">Tenant slug</label>
                  <Input id="tenantSlug" value={tenantSlug} onChange={(event) => setTenantSlug(event.target.value)} placeholder="acme" />
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium" htmlFor="fullName">Full name</label>
                  <Input id="fullName" value={fullName} onChange={(event) => setFullName(event.target.value)} placeholder="John Doe" />
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium" htmlFor="email">Email</label>
                  <Input id="email" type="email" value={email} onChange={(event) => setEmail(event.target.value)} placeholder="you@company.com" />
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium" htmlFor="password">Password</label>
                  <Input id="password" type="password" value={password} onChange={(event) => setPassword(event.target.value)} placeholder="At least 8 characters" />
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium" htmlFor="role">Role</label>
                  <select
                    id="role"
                    className="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background"
                    value={role}
                    onChange={(event) => setRole(Number(event.target.value))}
                  >
                    {roleOptions.map((item) => (
                      <option key={item.value} value={item.value}>
                        {item.label}
                      </option>
                    ))}
                  </select>
                </div>

                {errorMessage ? <p className="rounded-lg border border-destructive/30 bg-destructive/5 p-3 text-sm text-destructive">{errorMessage}</p> : null}

                <Button className="w-full" type="submit" disabled={isSubmitting}>
                  {isSubmitting ? 'Creating account...' : 'Create account'}
                </Button>
              </form>

              <div className="mt-4 text-center">
                <p className="text-sm text-muted-foreground">
                  Already have an account?{' '}
                  <button type="button" className="font-medium text-foreground underline underline-offset-4 hover:text-muted-foreground" onClick={() => navigate('/login')}>
                    Sign in
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
