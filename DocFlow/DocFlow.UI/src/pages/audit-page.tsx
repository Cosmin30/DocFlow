import { Badge } from '@/components/ui/badge'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { useEffect, useState } from 'react'
import { api, type AuditItem } from '@/lib/api'
import { getAccessToken } from '@/lib/auth'

const emptyMessage = 'Nu există înregistrări de audit.'

export function AuditPage() {
  const [auditRows, setAuditRows] = useState<AuditItem[]>([])
  const [loading, setLoading] = useState(true)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)

  useEffect(() => {
    const load = async () => {
      if (!getAccessToken()) {
        setLoading(false)
        setErrorMessage('Autentifică-te pentru a vedea auditul din backend.')
        return
      }

      const result = await api.get<AuditItem[]>('/audit?take=20')
      setLoading(false)

      if (result.ok && result.data) {
        setAuditRows(result.data)
      } else {
        setErrorMessage(result.error ?? 'Nu am putut încărca auditul.')
      }
    }

    void load()
  }, [])

  function severityLabel(action: string) {
    const value = action.toLowerCase()
    if (value.includes('permission') || value.includes('rol')) return 'Ridicat'
    if (value.includes('approval') || value.includes('aprobare')) return 'Mediu'
    return 'Scăzut'
  }

  return (
    <div className="space-y-6">
      <div>
        <h3 className="text-2xl font-semibold tracking-tight">Audit</h3>
        <p className="text-sm text-muted-foreground">Jurnal read-only cu acțiunile importante și schimbările de sistem.</p>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Jurnal audit</CardTitle>
          <CardDescription>Evenimente recente colectate din serviciile workflow.</CardDescription>
        </CardHeader>
        <CardContent>
          {errorMessage ? <p className="mb-4 rounded-lg border border-destructive/30 bg-destructive/5 p-3 text-sm text-destructive">{errorMessage}</p> : null}
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Acțiune</TableHead>
                <TableHead>Entitate</TableHead>
                <TableHead>Utilizator</TableHead>
                <TableHead>Moment</TableHead>
                <TableHead>Severitate</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {loading ? (
                <TableRow>
                  <TableCell colSpan={5} className="py-8 text-center text-muted-foreground">
                    Se încarcă auditul...
                  </TableCell>
                </TableRow>
              ) : auditRows.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={5} className="py-8 text-center text-muted-foreground">
                    {emptyMessage}
                  </TableCell>
                </TableRow>
              ) : (
                auditRows.map((row) => (
                <TableRow key={row.id}>
                  <TableCell className="font-medium">{row.action}</TableCell>
                  <TableCell>{row.entityType}</TableCell>
                  <TableCell>{row.userId ?? 'Anonim'}</TableCell>
                  <TableCell>{row.createdAtUtc}</TableCell>
                  <TableCell>
                    <Badge variant={severityLabel(row.action) === 'Ridicat' ? 'default' : severityLabel(row.action) === 'Mediu' ? 'secondary' : 'outline'}>
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