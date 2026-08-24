import { Badge } from '@/components/ui/badge'
import { Card, CardContent } from '@/components/ui/card'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { useEffect, useState } from 'react'
import { api, type AuditItem } from '@/lib/api'
import { formatDateTime } from '@/lib/utils'

function severityLabel(action: string) {
  const value = action.toLowerCase()
  if (value.includes('permission') || value.includes('role')) return 'High'
  if (value.includes('approval') || value.includes('aprobare')) return 'Medium'
  return 'Low'
}

export function AuditPage() {
  const [auditRows, setAuditRows] = useState<AuditItem[]>([])
  const [loading, setLoading] = useState(true)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)

  useEffect(() => {
    const load = async () => {
      const result = await api.get<AuditItem[]>('/audit?take=20')
      setLoading(false)

      if (result.ok && result.data) {
        setAuditRows(result.data)
      } else {
        setErrorMessage(result.error ?? 'Failed to load audit logs.')
      }
    }

    void load()
  }, [])

  return (
    <div className="space-y-6">
      <div>
        <h3 className="text-2xl font-semibold tracking-tight">Audit</h3>
        <p className="text-sm text-muted-foreground">Read-only log of important actions and system changes.</p>
      </div>

      <Card>
        <CardContent className="pt-6">
          {errorMessage ? <p className="mb-4 rounded-lg border border-destructive/30 bg-destructive/5 p-3 text-sm text-destructive">{errorMessage}</p> : null}
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Action</TableHead>
                <TableHead>Entity</TableHead>
                <TableHead>User</TableHead>
                <TableHead>Timestamp</TableHead>
                <TableHead>Severity</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {loading ? (
                <TableRow>
                  <TableCell colSpan={5} className="py-8 text-center text-muted-foreground">
                    Loading audit logs...
                  </TableCell>
                </TableRow>
              ) : auditRows.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={5} className="py-8 text-center text-muted-foreground">
                    No audit entries found.
                  </TableCell>
                </TableRow>
              ) : (
                auditRows.map((row) => (
                <TableRow key={row.id}>
                  <TableCell className="font-medium">{row.action}</TableCell>
                  <TableCell>{row.entityType}</TableCell>
                  <TableCell>{row.userId ? row.userId.slice(0, 8) + '...' : '—'}</TableCell>
                  <TableCell className="text-muted-foreground">{formatDateTime(row.createdAtUtc)}</TableCell>
                  <TableCell>
                    <Badge variant={severityLabel(row.action) === 'High' ? 'default' : severityLabel(row.action) === 'Medium' ? 'secondary' : 'outline'}>
                      {severityLabel(row.action)}
                    </Badge>
                  </TableCell>
                </TableRow>
                ))
              )}
            </TableBody>
          </Table>
        </CardContent>
      </Card>
    </div>
  )
}
