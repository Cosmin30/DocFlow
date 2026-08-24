import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Separator } from '@/components/ui/separator'
import { Activity, FileText, Workflow } from 'lucide-react'
import { useEffect, useMemo, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { api, type ApprovalItem, type AuditItem, type DocumentItem } from '@/lib/api'
import { formatDateTime } from '@/lib/utils'

export function DashboardPage() {
  const navigate = useNavigate()
  const [documents, setDocuments] = useState<DocumentItem[]>([])
  const [approvals, setApprovals] = useState<ApprovalItem[]>([])
  const [auditLogs, setAuditLogs] = useState<AuditItem[]>([])
  const [loading, setLoading] = useState(true)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)

  useEffect(() => {
    const load = async () => {
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
        setErrorMessage(firstError.error ?? 'Failed to load data.')
      }
    }

    void load()
  }, [])

  const summary = useMemo(
    () => [
      { label: 'Documents', value: documents.length.toString(), icon: FileText },
      { label: 'Pending approvals', value: approvals.length.toString(), icon: Workflow },
      { label: 'Audit entries', value: auditLogs.length.toString(), icon: Activity },
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
          <h3 className="text-2xl font-semibold tracking-tight">Dashboard</h3>
          <p className="text-sm text-muted-foreground">Overview of your workspace.</p>
        </div>
        <Button variant="secondary" onClick={() => navigate('/documents/new')}>New document</Button>
      </div>

      <div className="grid gap-4 md:grid-cols-3">
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

      <Card>
        <CardHeader>
          <CardTitle>Recent activity</CardTitle>
        </CardHeader>
        <CardContent className="space-y-4 text-sm">
          {loading ? (
            <p className="text-muted-foreground">Loading...</p>
          ) : !latestDocument && !latestApproval && !latestAudit ? (
            <p className="text-muted-foreground">No data yet. Create a document or register an approval to get started.</p>
          ) : (
            <>
              {latestDocument && (
                <>
                  <div className="flex items-center justify-between gap-4">
                    <div>
                      <p className="font-medium">{latestDocument.title}</p>
                      <p className="text-muted-foreground">{latestDocument.category} · {latestDocument.department}</p>
                    </div>
                    <Badge variant="outline">v{latestDocument.currentVersionNumber}</Badge>
                  </div>
                  <Separator />
                </>
              )}
              {latestApproval && (
                <>
                  <div className="flex items-center justify-between gap-4">
                    <div>
                      <p className="font-medium">Approval for document {latestApproval.documentId.slice(0, 8)}...</p>
                      <p className="text-muted-foreground">{formatDateTime(latestApproval.createdAtUtc)}</p>
                    </div>
                    <Badge variant="secondary">{String(latestApproval.status)}</Badge>
                  </div>
                  <Separator />
                </>
              )}
              {latestAudit && (
                <div className="flex items-center justify-between gap-4">
                  <div>
                    <p className="font-medium">{latestAudit.action}</p>
                    <p className="text-muted-foreground">{formatDateTime(latestAudit.createdAtUtc)}</p>
                  </div>
                  <Badge variant="outline">{latestAudit.entityType}</Badge>
                </div>
              )}
            </>
          )}
        </CardContent>
      </Card>
    </div>
  )
}
