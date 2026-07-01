import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Separator } from '@/components/ui/separator'
import { Activity, FileText, Workflow, Users } from 'lucide-react'
import { useEffect, useMemo, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { api, type ApprovalItem, type AuditItem, type DocumentItem } from '@/lib/api'
import { getAccessToken } from '@/lib/auth'

const emptyMessage = 'Nu există date încă în baza de date.'

export function DashboardPage() {
  const navigate = useNavigate()
  const [documents, setDocuments] = useState<DocumentItem[]>([])
  const [approvals, setApprovals] = useState<ApprovalItem[]>([])
  const [auditLogs, setAuditLogs] = useState<AuditItem[]>([])
  const [loading, setLoading] = useState(true)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)

  useEffect(() => {
    const load = async () => {
      if (!getAccessToken()) {
        setLoading(false)
        setErrorMessage('Trebuie să te autentifici pentru a încărca datele din backend.')
        return
      }

      const [documentsResult, approvalsResult, auditResult] = await Promise.all([
        api.get<DocumentItem[]>('/documents'),
        api.get<ApprovalItem[]>('/approvals/pending'),
        api.get<AuditItem[]>('/audit?take=20'),
      ])

      setLoading(false)

      if (documentsResult.ok && documentsResult.data) {
        setDocuments(documentsResult.data)
      }

      if (approvalsResult.ok && approvalsResult.data) {
        setApprovals(approvalsResult.data)
      }

      if (auditResult.ok && auditResult.data) {
        setAuditLogs(auditResult.data)
      }

      const firstError = [documentsResult, approvalsResult, auditResult].find((result) => !result.ok)
      if (firstError) {
        setErrorMessage(firstError.error ?? 'Nu am putut încărca datele din backend.')
      }
    }

    void load()
  }, [])

  const summary = useMemo(
    () => [
      { label: 'Documente', value: documents.length.toString(), icon: FileText },
      { label: 'Aprobări', value: approvals.length.toString(), icon: Workflow },
      { label: 'Utilizatori activi', value: '1', icon: Users },
      { label: 'Înregistrări audit', value: auditLogs.length.toString(), icon: Activity },
    ],
    [documents.length, approvals.length, auditLogs.length],
  )

  const latestDocument = documents[0]
  const latestApproval = approvals[0]
  const latestAudit = auditLogs[0]

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h3 className="text-2xl font-semibold tracking-tight">Panou principal</h3>
          <p className="text-sm text-muted-foreground">Prezentare rapidă a datelor venite din backend.</p>
        </div>
        <Button variant="secondary" onClick={() => navigate('/documents/new')}>Creează document</Button>
      </div>

      <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
        {summary.map((item) => {
          const Icon = item.icon
          return (
            <Card key={item.label}>
              <CardHeader className="pb-3">
                <CardDescription>{item.label}</CardDescription>
                <CardTitle className="flex items-center justify-between text-3xl">
                  {item.value}
                  <Icon className="h-5 w-5 text-muted-foreground" />
                </CardTitle>
              </CardHeader>
            </Card>
          )
        })}
      </div>

      {errorMessage ? <p className="rounded-lg border border-destructive/30 bg-destructive/5 p-3 text-sm text-destructive">{errorMessage}</p> : null}

      <div className="grid gap-6 lg:grid-cols-[1.3fr_0.7fr]">
        <Card>
          <CardHeader>
            <CardTitle>Ultimele documente</CardTitle>
            <CardDescription>Date din backend sau stare goală dacă baza nu are înregistrări.</CardDescription>
          </CardHeader>
          <CardContent className="space-y-4 text-sm">
            {loading ? (
              <p className="text-muted-foreground">Se încarcă datele...</p>
            ) : latestDocument ? (
              <>
                <div className="flex items-center justify-between gap-4">
                  <div>
                    <p className="font-medium">{latestDocument.title}</p>
                    <p className="text-muted-foreground">{latestDocument.category} · {latestDocument.department}</p>
                  </div>
                  <Badge variant="outline">Versiunea {latestDocument.currentVersionNumber}</Badge>
                </div>
                <Separator />
                <div className="flex items-center justify-between gap-4">
                  <div>
                    <p className="font-medium">{latestApproval ? `Aprobare pentru documentul ${latestApproval.documentId.slice(0, 8)}...` : 'Nu există aprobări încă'}</p>
                    <p className="text-muted-foreground">{latestApproval ? latestApproval.createdAtUtc : emptyMessage}</p>
                  </div>
                  <Badge variant="secondary">{latestApproval ? String(latestApproval.status) : 'Fără date'}</Badge>
                </div>
                <Separator />
                <div className="flex items-center justify-between gap-4">
                  <div>
                    <p className="font-medium">{latestAudit ? latestAudit.action : 'Nu există înregistrări de audit'}</p>
                    <p className="text-muted-foreground">{latestAudit ? latestAudit.createdAtUtc : emptyMessage}</p>
                  </div>
                  <Badge variant="outline">Audit</Badge>
                </div>
              </>
            ) : (
              <p className="text-muted-foreground">{emptyMessage}</p>
            )}
          </CardContent>
        </Card>

        <div className="space-y-6">
          <Card>
            <CardHeader>
              <CardTitle>Acțiuni următoare</CardTitle>
              <CardDescription>Ce poți face imediat în funcție de datele încărcate.</CardDescription>
            </CardHeader>
            <CardContent className="space-y-3 text-sm">
              <p>• Verifică aprobările urgente</p>
              <p>• Încarcă documentele lipsă</p>
              <p>• Rezolvă observațiile de audit</p>
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle>Starea serviciilor</CardTitle>
              <CardDescription>Indicatori simpli pentru backend-ul din development.</CardDescription>
            </CardHeader>
            <CardContent className="space-y-3 text-sm">
              <div className="flex items-center justify-between">
                <span>Gateway</span>
                <Badge variant="outline">Disponibil</Badge>
              </div>
              <Separator />
              <div className="flex items-center justify-between">
                <span>Auth</span>
                <Badge variant="outline">Disponibil</Badge>
              </div>
              <Separator />
              <div className="flex items-center justify-between">
                <span>Database</span>
                <Badge>Poate fi goală</Badge>
              </div>
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  )
}