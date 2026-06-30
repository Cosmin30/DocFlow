import { NavLink, Outlet } from 'react-router-dom'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Card, CardContent } from '@/components/ui/card'
import { Separator } from '@/components/ui/separator'
import { cn } from '@/lib/utils'
import { Activity, FileText, LayoutDashboard, LogIn, ScrollText, ShieldCheck, Workflow } from 'lucide-react'
import { clearSessionTokens } from '@/lib/auth'

const navigationItems = [
  { to: '/dashboard', label: 'Panou', icon: LayoutDashboard },
  { to: '/documents', label: 'Documente', icon: FileText },
  { to: '/approvals', label: 'Aprobări', icon: Workflow },
  { to: '/audit', label: 'Audit', icon: ScrollText },
]

export function AppShell() {
  return (
    <div className="min-h-screen bg-background text-foreground">
      <div className="mx-auto grid min-h-screen w-full max-w-7xl gap-6 p-4 lg:grid-cols-[280px_1fr] lg:p-6">
        <aside className="flex flex-col gap-4 rounded-2xl border bg-card p-4 shadow-sm">
          <div className="space-y-2">
            <div className="flex items-center gap-2 text-sm font-medium text-muted-foreground">
              <ShieldCheck className="h-4 w-4" />
              DocFlow.UI
            </div>
            <h1 className="text-2xl font-semibold tracking-tight">Suita de lucru</h1>
            <p className="text-sm text-muted-foreground">Documente, aprobări și audit într-un singur loc.</p>
          </div>

          <Separator />

          <nav className="flex flex-col gap-1">
            {navigationItems.map((item) => {
              const Icon = item.icon
              return (
                <NavLink
                  key={item.to}
                  to={item.to}
                  className={({ isActive }) =>
                    cn(
                      'flex items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium transition-colors hover:bg-muted',
                      isActive && 'bg-foreground text-background hover:bg-foreground/90',
                    )
                  }
                >
                  <Icon className="h-4 w-4" />
                  {item.label}
                </NavLink>
              )
            })}
          </nav>

          <Separator />

          <Card>
            <CardContent className="space-y-3 p-4">
              <div className="flex items-center justify-between text-sm">
                <span className="text-muted-foreground">System</span>
                <Badge variant="outline">Disponibil</Badge>
              </div>
              <div className="flex items-center justify-between text-sm">
                <span className="text-muted-foreground">Cereri/min</span>
                <span className="font-medium">128/min</span>
              </div>
              <div className="flex items-center justify-between text-sm">
                <span className="text-muted-foreground">Alerte</span>
                <Badge>2 active</Badge>
              </div>
            </CardContent>
          </Card>

          <Button
            variant="secondary"
            className="mt-auto justify-start gap-2"
            onClick={() => {
              clearSessionTokens()
              window.location.href = '/login'
            }}
          >
            <LogIn className="h-4 w-4" />
            <span>Conectare</span>
          </Button>
        </aside>

        <section className="flex flex-col gap-6">
          <header className="flex flex-col gap-3 rounded-2xl border bg-card p-4 shadow-sm sm:flex-row sm:items-center sm:justify-between">
            <div>
              <p className="text-sm text-muted-foreground">Frontend DocFlow</p>
              <h2 className="text-xl font-semibold tracking-tight">Vedere operațională</h2>
            </div>
            <div className="flex items-center gap-2 text-sm text-muted-foreground">
              <Activity className="h-4 w-4" />
              Date în timp real
            </div>
          </header>

          <div className="flex-1 rounded-2xl border bg-card p-4 shadow-sm lg:p-6">
            <Outlet />
          </div>
        </section>
      </div>
    </div>
  )
}