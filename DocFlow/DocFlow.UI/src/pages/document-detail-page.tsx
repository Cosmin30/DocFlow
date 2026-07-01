import { useEffect, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Separator } from '@/components/ui/separator'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { api, type ApprovalItem, type DocumentItem, type DocumentVersionItem } from '@/lib/api'

const seededManagerUserId = '33333333-3333-3333-3333-333333333333'

function confidentialityLabel(value: number | string) {
  const normalized = String(value)
  if (normalized === '0' || normalized.toLowerCase() === 'public') return 'Public'
  if (normalized === '1' || normalized.toLowerCase() === 'internal') return 'Intern'
  if (normalized === '2' || normalized.toLowerCase() === 'confidential') return 'Confidențial'
  return 'Strict'
}

export function DocumentDetailPage() {
  const navigate = useNavigate()
  const { documentId } = useParams()
  const [documentItem, setDocumentItem] = useState<DocumentItem | null>(null)
  const [versions, setVersions] = useState<DocumentVersionItem[]>([])
  const [loading, setLoading] = useState(true)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)
  const [approvalTargetUserId, setApprovalTargetUserId] = useState(seededManagerUserId)
  const [approvalComment, setApprovalComment] = useState('')
  const [approvalSaving, setApprovalSaving] = useState(false)
  const [approvalMessage, setApprovalMessage] = useState<string | null>(null)

  useEffect(() => {
    const load = async () => {
      if (!documentId) {
        setLoading(false)
        setErrorMessage('Lipsește identificatorul documentului.')
        return
      }

      const documentsResult = await api.get<DocumentItem[]>('/documents')
      if (documentsResult.ok && documentsResult.data) {
        setDocumentItem(documentsResult.data.find((item) => item.id === documentId) ?? null)
      }

      const versionsResult = await api.get<DocumentVersionItem[]>(`/documents/${documentId}/versions`)
      setLoading(false)

      if (versionsResult.ok && versionsResult.data) {
        setVersions(versionsResult.data)
      }

      if (!documentsResult.ok) {
        setErrorMessage(documentsResult.error ?? 'Nu am putut încărca documentul.')
      } else if (documentId && !documentsResult.data?.some((item) => item.id === documentId)) {
        setErrorMessage('Documentul nu există sau nu ai acces la el.')
      }
    }

    void load()
  }, [documentId])

  const createApproval = async () => {
    if (!documentId) {
      setApprovalMessage('Lipsește identificatorul documentului.')
      return
    }

    setApprovalSaving(true)
    setApprovalMessage(null)

    const result = await api.post<ApprovalItem>('/approvals', {
      documentId,
      assignedToUserId: approvalTargetUserId,
      comment: approvalComment.trim() || null,
    })

    setApprovalSaving(false)

    if (!result.ok || !result.data) {
      setApprovalMessage(result.error ?? 'Nu am putut crea aprobarea.')
      return
    }

    setApprovalComment('')
    setApprovalMessage('Cererea de aprobare a fost creată.')
  }

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h3 className="text-2xl font-semibold tracking-tight">Detalii document</h3>
          <p className="text-sm text-muted-foreground">Informații complete și versiuni pentru documentul selectat.</p>
        </div>
        <div className="flex gap-2">
          <Button variant="outline" onClick={() => navigate('/documents')}>Înapoi la listă</Button>
          {documentId ? <Button onClick={() => navigate(`/documents/${documentId}/edit`)}>Editează</Button> : null}
        </div>
      </div>

      {errorMessage ? <p className="rounded-lg border border-destructive/30 bg-destructive/5 p-3 text-sm text-destructive">{errorMessage}</p> : null}

      <div className="grid gap-6 lg:grid-cols-[1fr_0.9fr]">
        <Card>
          <CardHeader>
            <CardTitle>{documentItem ? documentItem.title : 'Document indisponibil'}</CardTitle>
            <CardDescription>
              {documentItem ? `${documentItem.category} · ${documentItem.department}` : 'Nu există date pentru acest document.'}
            </CardDescription>
          </CardHeader>
          <CardContent className="space-y-3 text-sm">
            {loading ? (
              <p className="text-muted-foreground">Se încarcă detaliile...</p>
            ) : documentItem ? (
              <>
                <div className="flex items-center justify-between">
                  <span>Clasificare</span>
                  <Badge variant="outline">{confidentialityLabel(documentItem.confidentialityLevel)}</Badge>
                </div>
                <Separator />
                <div className="flex items-center justify-between">
                  <span>Etichete</span>
                  <span className="text-muted-foreground">{documentItem.tagsCsv || 'Fără etichete'}</span>
                </div>
                <Separator />
                <div className="flex items-center justify-between">
                  <span>Expirare</span>
                  <span className="text-muted-foreground">{documentItem.expiresAtUtc ?? 'Nu expiră'}</span>
                </div>
                <Separator />
                <div className="flex items-center justify-between">
                  <span>Versiune curentă</span>
                  <Badge>{documentItem.currentVersionNumber}</Badge>
                </div>
              </>
            ) : (
              <p className="text-muted-foreground">Nu există date suficiente pentru acest document.</p>
            )}
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Versiuni</CardTitle>
            <CardDescription>Istoricul fișierelor încărcate pentru document.</CardDescription>
          </CardHeader>
          <CardContent>
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Versiune</TableHead>
                  <TableHead>Fișier</TableHead>
                  <TableHead>Dimensiune</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {versions.length === 0 ? (
                  <TableRow>
                    <TableCell colSpan={3} className="py-8 text-center text-muted-foreground">
                      Nu există versiuni încă.
                    </TableCell>
                  </TableRow>
                ) : (
                  versions.map((version) => (
                    <TableRow key={version.id}>
                      <TableCell>v{version.versionNumber}</TableCell>
                      <TableCell>{version.fileName}</TableCell>
                      <TableCell>{version.sizeBytes.toLocaleString('ro-RO')} bytes</TableCell>
                    </TableRow>
                  ))
                )}
              </TableBody>
            </Table>
          </CardContent>
        </Card>

        <Card className="lg:col-span-2">
          <CardHeader>
            <CardTitle>Creează aprobare</CardTitle>
            <CardDescription>Trimite documentul către un utilizator pentru aprobare din backend.</CardDescription>
          </CardHeader>
          <CardContent className="grid gap-4 md:grid-cols-2">
            <div className="space-y-2">
              <label className="text-sm font-medium" htmlFor="assignedToUserId">Atribuit către user ID</label>
              <Input id="assignedToUserId" value={approvalTargetUserId} onChange={(event) => setApprovalTargetUserId(event.target.value)} />
            </div>
            <div className="space-y-2 md:col-span-2">
              <label className="text-sm font-medium" htmlFor="approvalComment">Comentariu</label>
              <Input id="approvalComment" value={approvalComment} onChange={(event) => setApprovalComment(event.target.value)} placeholder="De ce are nevoie de aprobare" />
            </div>
            {approvalMessage ? <p className="rounded-lg border border-border bg-muted/40 p-3 text-sm text-muted-foreground md:col-span-2">{approvalMessage}</p> : null}
            <div className="flex gap-3 md:col-span-2">
              <Button type="button" onClick={() => void createApproval()} disabled={approvalSaving || !documentId}>
                {approvalSaving ? 'Se creează...' : 'Creează aprobare'}
              </Button>
              <Button type="button" variant="outline" onClick={() => setApprovalTargetUserId(seededManagerUserId)}>
                Reset target
              </Button>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  )
}