import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { api } from '@/lib/api'

const roleOptions = [
  { label: 'Company admin', value: 1 },
  { label: 'Manager', value: 2 },
  { label: 'Employee', value: 3 },
  { label: 'Read-only auditor', value: 4 },
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
      <div className="mx-auto grid min-h-screen w-full max-w-7xl gap-6 p-6 lg:grid-cols-[0.95fr_1.05fr] lg:p-10">
        <Card className="flex items-center">
          <CardContent className="w-full space-y-6 p-8">
            <CardHeader className="p-0">
              <Badge>Cont nou</Badge>
              <CardTitle className="text-3xl">Creează un spațiu de lucru</CardTitle>
              <CardDescription>
                Înregistrează tenantul și primul utilizator pentru a putea intra în DocFlow.
              </CardDescription>
            </CardHeader>

            <div className="space-y-3 text-sm text-muted-foreground">
              <p>• Creezi tenantul și utilizatorul inițial dintr-un singur formular.</p>
              <p>• Poți folosi imediat datele pentru login după înregistrare.</p>
              <p>• Rolul ales este trimis direct către backend.</p>
            </div>
          </CardContent>
        </Card>

        <Card className="flex items-center">
          <CardContent className="w-full space-y-6 p-8">
            <CardHeader className="p-0">
              <CardTitle>Înregistrare</CardTitle>
              <CardDescription>Completează datele pentru tenant și utilizator.</CardDescription>
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
                  setErrorMessage(result.error ?? 'Înregistrarea nu a reușit.')
                  return
                }

                navigate('/login', { replace: true })
              }}
            >
              <div className="space-y-2">
                <label className="text-sm font-medium" htmlFor="tenantName">Nume tenant</label>
                <Input id="tenantName" value={tenantName} onChange={(event) => setTenantName(event.target.value)} placeholder="Contabilitate SRL" />
              </div>

              <div className="space-y-2">
                <label className="text-sm font-medium" htmlFor="tenantSlug">Slug tenant</label>
                <Input id="tenantSlug" value={tenantSlug} onChange={(event) => setTenantSlug(event.target.value)} placeholder="contabilitate" />
              </div>

              <div className="space-y-2">
                <label className="text-sm font-medium" htmlFor="fullName">Nume complet</label>
                <Input id="fullName" value={fullName} onChange={(event) => setFullName(event.target.value)} placeholder="Nume Prenume" />
              </div>

              <div className="space-y-2">
                <label className="text-sm font-medium" htmlFor="email">E-mail</label>
                <Input id="email" value={email} onChange={(event) => setEmail(event.target.value)} placeholder="nume@companie.ro" />
              </div>

              <div className="space-y-2">
                <label className="text-sm font-medium" htmlFor="password">Parolă</label>
                <Input id="password" type="password" value={password} onChange={(event) => setPassword(event.target.value)} placeholder="••••••••" />
              </div>

              <div className="space-y-2">
                <label className="text-sm font-medium" htmlFor="role">Rol</label>
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
                {isSubmitting ? 'Se creează contul...' : 'Creează cont'}
              </Button>
            </form>

            <div className="space-y-2">
              <p className="text-sm text-muted-foreground">Ai deja cont?</p>
              <Button type="button" variant="outline" className="w-full" onClick={() => navigate('/login')}>
                Înapoi la login
              </Button>
            </div>
          </CardContent>
        </Card>
      </div>
    </main>
  )
}