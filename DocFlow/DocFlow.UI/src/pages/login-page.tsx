import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { ShieldCheck } from 'lucide-react'
import { api, type LoginResponse } from '@/lib/api'
import { setSessionTokens } from '@/lib/auth'

export function LoginPage() {
  const navigate = useNavigate()
  const [tenantSlug, setTenantSlug] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [device, setDevice] = useState('Browser local')
  const [errorMessage, setErrorMessage] = useState<string | null>(null)
  const [isSubmitting, setIsSubmitting] = useState(false)

  return (
    <main className="min-h-screen bg-background text-foreground">
      <div className="mx-auto grid min-h-screen w-full max-w-7xl gap-6 p-6 lg:grid-cols-[1.05fr_0.95fr] lg:p-10">
        <section className="flex items-center rounded-3xl border bg-card p-8 shadow-sm">
          <div className="max-w-xl space-y-6">
            <div className="flex items-center gap-2 text-sm text-muted-foreground">
              <ShieldCheck className="h-4 w-4" />
              DocFlow.UI
            </div>
            <div className="space-y-3">
              <Badge>Acces securizat</Badge>
              <h1 className="text-4xl font-semibold tracking-tight lg:text-5xl">Autentifică-te în DocFlow</h1>
              <p className="text-sm text-muted-foreground">
                Acces la dashboard, documente, aprobări și audit dintr-un singur loc.
              </p>
            </div>

            <div className="grid gap-3 sm:grid-cols-3">
              <Card>
                <CardHeader className="pb-3">
                  <CardDescription>Autentificare</CardDescription>
                  <CardTitle className="text-2xl">JWT</CardTitle>
                </CardHeader>
              </Card>
              <Card>
                <CardHeader className="pb-3">
                  <CardDescription>Acces</CardDescription>
                  <CardTitle className="text-2xl">Pe roluri</CardTitle>
                </CardHeader>
              </Card>
              <Card>
                <CardHeader className="pb-3">
                  <CardDescription>Trasabilitate</CardDescription>
                  <CardTitle className="text-2xl">Pregătit pentru audit</CardTitle>
                </CardHeader>
              </Card>
            </div>
          </div>
        </section>

        <Card className="flex items-center">
          <CardContent className="w-full space-y-6 p-8">
            <CardHeader className="p-0">
              <CardTitle>Bun venit</CardTitle>
              <CardDescription>Autentifică-te cu datele din backend. Dacă nu există utilizatori, vei vedea mesajul de eroare.</CardDescription>
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
                  device,
                })

                setIsSubmitting(false)

                if (!result.ok || !result.data) {
                  setErrorMessage(result.error ?? 'Autentificarea nu a reușit.')
                  return
                }

                setSessionTokens(result.data.accessToken, result.data.refreshToken)
                navigate('/dashboard')
              }}
            >
              <div className="space-y-2">
                <label className="text-sm font-medium" htmlFor="tenantSlug">
                  Tenant / spațiu de lucru
                </label>
                <Input id="tenantSlug" value={tenantSlug} onChange={(event) => setTenantSlug(event.target.value)} placeholder="contabilitate" />
              </div>

              <div className="space-y-2">
                <label className="text-sm font-medium" htmlFor="email">
                  E-mail
                </label>
                <Input id="email" value={email} onChange={(event) => setEmail(event.target.value)} placeholder="nume@companie.ro" />
              </div>

              <div className="space-y-2">
                <label className="text-sm font-medium" htmlFor="password">
                  Parolă
                </label>
                <Input
                  id="password"
                  type="password"
                  value={password}
                  onChange={(event) => setPassword(event.target.value)}
                  placeholder="••••••••"
                />
              </div>

              <div className="space-y-2">
                <label className="text-sm font-medium" htmlFor="device">
                  Dispozitiv
                </label>
                <Input id="device" value={device} onChange={(event) => setDevice(event.target.value)} placeholder="Browser local" />
              </div>

              {errorMessage ? <p className="rounded-lg border border-destructive/30 bg-destructive/5 p-3 text-sm text-destructive">{errorMessage}</p> : null}

              <Button className="w-full" type="submit" disabled={isSubmitting}>
                {isSubmitting ? 'Se verifică datele...' : 'Intră în aplicație'}
              </Button>
            </form>

            <p className="text-sm text-muted-foreground">
              Dacă baza de date este goală, autentificarea va eșua, iar paginile protejate vor afișa stări goale în română.
            </p>
          </CardContent>
        </Card>
      </div>
    </main>
  )
}