import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Separator } from '@/components/ui/separator'
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs'
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
        setErrorMessage(result.error ?? 'Nu am putut încărca aprobările.')
      }
    }

    void load()
  }, [])

  const handleDecision = async (approvalId: string, approve: boolean) => {
    setPendingActionId(approvalId)
    setErrorMessage(null)

    const result = await api.post<ApprovalItem>(`/approvals/${approvalId}/decision`, {
      approve,
      comment: approve ? 'Aprobat din UI.' : 'Respins din UI.',
    })

    setPendingActionId(null)

    if (!result.ok || !result.data) {
      setErrorMessage(result.error ?? 'Nu am putut salva decizia.')
      return
    }

    setApprovals((current) => current.filter((item) => item.id !== approvalId))
  }

  const filteredApprovals = approvals

  return (
    <div className="space-y-6">
      <div>
        <h3 className="text-2xl font-semibold tracking-tight">Aprobări</h3>
        <p className="text-sm text-muted-foreground">Urmărește fluxul de aprobare și starea fiecărei cereri.</p>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Coada de aprobare</CardTitle>
          <CardDescription>Locul unde vezi cererile în așteptare și progresul lor.</CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <Tabs value={activeTab} onValueChange={setActiveTab}>
            <TabsList>
              <TabsTrigger value="pending">În așteptare ({filteredApprovals.length})</TabsTrigger>
              <TabsTrigger value="resolved">Finalizate</TabsTrigger>
            </TabsList>
            <TabsContent value="pending">
              {errorMessage ? <p className="rounded-lg border border-destructive/30 bg-destructive/5 p-3 text-sm text-destructive">{errorMessage}</p> : null}
              <div className="space-y-4">
                {loading ? (
                  <p className="text-sm text-muted-foreground">Se încarcă aprobările...</p>
                ) : filteredApprovals.length === 0 ? (
                  <p className="text-sm text-muted-foreground">Nu există aprobări în așteptare.</p>
                ) : (
                  filteredApprovals.map((item) => (
                    <div key={item.id} className="rounded-lg border p-4">
                      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
                        <div>
                          <p className="font-medium">Document {item.documentId.slice(0, 8)}...</p>
                          <p className="text-sm text-muted-foreground">Cerut de {item.requestedByUserId.slice(0, 8)}... · {formatDateTime(item.createdAtUtc)}</p>
                        </div>
                        <Badge variant={String(item.status) === '1' ? 'outline' : String(item.status) === '2' ? 'secondary' : 'default'}>
                          {approvalStatusLabel(item.status)}
                        </Badge>
                      </div>
                      <Separator className="my-4" />
                      <div className="flex flex-col gap-3 sm:flex-row">
                        <Button variant="secondary" disabled={pendingActionId === item.id} onClick={() => void handleDecision(item.id, true)}>
                          {pendingActionId === item.id ? 'Se salvează...' : 'Aprobă'}
                        </Button>
                        <Button variant="outline" disabled={pendingActionId === item.id} onClick={() => void handleDecision(item.id, false)}>
                          {pendingActionId === item.id ? 'Se salvează...' : 'Cere modificări'}
                        </Button>
                      </div>
                    </div>
                  ))
                )}
              </div>
            </TabsContent>
            <TabsContent value="resolved">
              <p className="text-sm text-muted-foreground">Aprobările finalizate vor apărea aici.</p>
            </TabsContent>
          </Tabs>
        </CardContent>
      </Card>
    </div>
  )
}
