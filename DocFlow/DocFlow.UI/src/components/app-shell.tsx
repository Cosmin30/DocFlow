import { NavLink, Outlet, useNavigate } from 'react-router-dom'
import { Button } from '@/components/ui/button'
import { Separator } from '@/components/ui/separator'
import { cn } from '@/lib/utils'
import { FileText, LayoutDashboard, LogOut, ScrollText, ShieldCheck, Workflow } from 'lucide-react'
import { logout } from '@/lib/auth'

const navigationItems = [
  { to: '/dashboard', label: 'Dashboard', icon: LayoutDashboard },
  { to: '/documents', label: 'Documents', icon: FileText },
  { to: '/approvals', label: 'Approvals', icon: Workflow },
  { to: '/audit', label: 'Audit', icon: ScrollText },
]

export function AppShell() {
  const navigate = useNavigate()

  return (
    <div className="min-h-screen bg-background text-foreground">
      <div className="mx-auto grid min-h-screen w-full max-w-7xl gap-6 p-4 lg:grid-cols-[260px_1fr] lg:p-6">
        <aside className="flex flex-col gap-4 rounded-2xl border bg-card p-4 shadow-sm">
          <div className="space-y-1">
            <div className="flex items-center gap-2 text-sm font-semibold">
              <ShieldCheck className="h-4 w-4" />
              DocFlow
            </div>
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

          <Button
            variant="secondary"
            className="mt-auto justify-start gap-2"
            onClick={async () => {
              await logout()
              navigate('/login')
            }}
          >
            <LogOut className="h-4 w-4" />
            <span>Sign out</span>
          </Button>
        </aside>

        <section className="flex flex-col gap-6">
          <div className="flex-1 rounded-2xl border bg-card p-4 shadow-sm lg:p-6">
            <Outlet />
          </div>
        </section>
      </div>
    </div>
  )
}
