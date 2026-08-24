import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Card, CardContent } from '@/components/ui/card'
import { Separator } from '@/components/ui/separator'
import { TabsList, TabsTrigger } from '@/components/ui/tabs'
import { useEffect, useState } from 'react'
import { api, type ApprovalItem } from '@/lib/api'
import { approvalStatusLabel, formatDateTime } from '@/lib/utils'

export function ApprovalsPage() {
  const [approvals, setApprovals] = useState<ApprovalItem[]>([])
  const [loading, setLoading] = useState(true)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)
  const [pendingActionId, setPendingActionId] = useState<string | null>(null)
  const [activeTab, setActiveTab] = useState('pending')

  useEffect(() => {
    const load = async () => {
      const result = await api.get<ApprovalItem[]>('/approvals/pending')
      setLoading(false)

      if (result.ok && result.data) {
        setApprovals(result.data)
      } else {
        setErrorMessage(result.error ?? 'Failed to load approvals.')
      }
    }

    void load()
  }, [])

  const handleDecision = async (approvalId: string, approve: boolean) => {
    setPendingActionId(approvalId)
    setErrorMessage(null)

    const result = await api.post<ApprovalItem>(`/approvals/${approvalId}/decision`, {
      approve,
      comment: approve ? 'Approved.' : 'Rejected.',
    })

    setPendingActionId(null)

    if (!result.ok || !result.data) {
      setErrorMessage(result.error ?? 'Failed to save decision.')
      return
    }

    setApprovals((current) => current.filter((item) => item.id !== approvalId))
  }

  return (
    <div className="space-y-6">
      <div>
        <h3 className="text-2xl font-semibold tracking-tight">Approvals</h3>
        <p className="text-sm text-muted-foreground">Review and manage pending approval requests.</p>
      </div>

      <Card>
        <CardContent className="pt-6 space-y-4">
          <TabsList>
            <TabsTrigger value="pending" active={activeTab === 'pending'} onClick={() => setActiveTab('pending')}>Pending ({approvals.length})</TabsTrigger>
            <TabsTrigger value="resolved" active={activeTab === 'resolved'} onClick={() => setActiveTab('resolved')}>Resolved</TabsTrigger>
          </TabsList>

          {activeTab === 'pending' && (
            <>
              {errorMessage ? <p className="rounded-lg border border-destructive/30 bg-destructive/5 p-3 text-sm text-destructive">{errorMessage}</p> : null}
              <div className="space-y-4">
                {loading ? (
                  <p className="text-sm text-muted-foreground">Loading approvals...</p>
                ) : approvals.length === 0 ? (
                  <p className="text-sm text-muted-foreground">No pending approvals.</p>
                ) : (
                  approvals.map((item) => (
                    <div key={item.id} className="rounded-lg border p-4">
                      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
                        <div>
                          <p className="font-medium">Document {item.documentId.slice(0, 8)}...</p>
                          <p className="text-sm text-muted-foreground">Requested by {item.requestedByUserId.slice(0, 8)}... · {formatDateTime(item.createdAtUtc)}</p>
                        </div>
                        <Badge variant={String(item.status) === '1' ? 'outline' : String(item.status) === '2' ? 'secondary' : 'default'}>
                          {approvalStatusLabel(item.status)}
                        </Badge>
                      </div>
                      <Separator className="my-4" />
                      <div className="flex flex-col gap-3 sm:flex-row">
                        <Button variant="secondary" disabled={pendingActionId === item.id} onClick={() => void handleDecision(item.id, true)}>
                          {pendingActionId === item.id ? 'Saving...' : 'Approve'}
                        </Button>
                        <Button variant="outline" disabled={pendingActionId === item.id} onClick={() => void handleDecision(item.id, false)}>
                          {pendingActionId === item.id ? 'Saving...' : 'Request changes'}
                        </Button>
                      </div>
                    </div>
                  ))
                )}
              </div>
            </>
          )}

          {activeTab === 'resolved' && (
            <p className="text-sm text-muted-foreground">Resolved approvals will appear here.</p>
          )}
        </CardContent>
      </Card>
    </div>
  )
}
