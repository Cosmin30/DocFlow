import { useEffect, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Separator } from '@/components/ui/separator'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { api, type ApprovalItem, type DocumentItem, type DocumentVersionItem } from '@/lib/api'
import { confidentialityLabel, formatBytes, formatDateTime } from '@/lib/utils'

export function DocumentDetailPage() {
  const navigate = useNavigate()
  const { documentId } = useParams()
  const [documentItem, setDocumentItem] = useState<DocumentItem | null>(null)
  const [versions, setVersions] = useState<DocumentVersionItem[]>([])
  const [loading, setLoading] = useState(true)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)
  const [approvalTargetUserId, setApprovalTargetUserId] = useState('')
  const [approvalComment, setApprovalComment] = useState('')
  const [approvalSaving, setApprovalSaving] = useState(false)
  const [approvalMessage, setApprovalMessage] = useState<string | null>(null)

  useEffect(() => {
    const load = async () => {
      if (!documentId) {
        setLoading(false)
        setErrorMessage('Missing document identifier.')
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
        setErrorMessage(documentsResult.error ?? 'Failed to load document.')
      } else if (documentId && !documentsResult.data?.some((item) => item.id === documentId)) {
        setErrorMessage('Document not found or you do not have access.')
      }
    }

    void load()
  }, [documentId])

  const createApproval = async () => {
    if (!documentId || !approvalTargetUserId.trim()) {
      setApprovalMessage('Please enter the assignee user ID.')
      return
    }

    setApprovalSaving(true)
    setApprovalMessage(null)

    const result = await api.post<ApprovalItem>('/approvals', {
      documentId,
      assignedToUserId: approvalTargetUserId.trim(),
      comment: approvalComment.trim() || null,
    })

    setApprovalSaving(false)

    if (!result.ok || !result.data) {
      setApprovalMessage(result.error ?? 'Failed to create approval.')
      return
    }

    setApprovalComment('')
    setApprovalMessage('Approval request created successfully.')
  }

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h3 className="text-2xl font-semibold tracking-tight">Document details</h3>
        </div>
        <div className="flex gap-2">
          <Button variant="outline" onClick={() => navigate('/documents')}>Back to list</Button>
          {documentId ? <Button onClick={() => navigate(`/documents/${documentId}/edit`)}>Edit</Button> : null}
        </div>
      </div>

      {errorMessage ? <p className="rounded-lg border border-destructive/30 bg-destructive/5 p-3 text-sm text-destructive">{errorMessage}</p> : null}

      <div className="grid gap-6 lg:grid-cols-[1fr_0.9fr]">
        <Card>
          <CardHeader>
            <CardTitle>{documentItem ? documentItem.title : 'Document not available'}</CardTitle>
          </CardHeader>
          <CardContent className="space-y-3 text-sm">
            {loading ? (
              <p className="text-muted-foreground">Loading...</p>
            ) : documentItem ? (
              <>
                <div className="flex items-center justify-between">
                  <span className="text-muted-foreground">Category</span>
                  <span>{documentItem.category} · {documentItem.department}</span>
                </div>
                <Separator />
                <div className="flex items-center justify-between">
                  <span className="text-muted-foreground">Classification</span>
                  <Badge variant="outline">{confidentialityLabel(documentItem.confidentialityLevel)}</Badge>
                </div>
                <Separator />
                <div className="flex items-center justify-between">
                  <span className="text-muted-foreground">Tags</span>
                  <span>{documentItem.tagsCsv || '—'}</span>
                </div>
                <Separator />
                <div className="flex items-center justify-between">
                  <span className="text-muted-foreground">Expires</span>
                  <span>{formatDateTime(documentItem.expiresAtUtc)}</span>
                </div>
                <Separator />
                <div className="flex items-center justify-between">
                  <span className="text-muted-foreground">Current version</span>
                  <Badge>{documentItem.currentVersionNumber}</Badge>
                </div>
                <Separator />
                <div className="flex items-center justify-between">
                  <span className="text-muted-foreground">Created</span>
                  <span>{formatDateTime(documentItem.createdAtUtc)}</span>
                </div>
              </>
            ) : (
              <p className="text-muted-foreground">No data available.</p>
            )}
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Versions</CardTitle>
          </CardHeader>
          <CardContent>
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Version</TableHead>
                  <TableHead>File</TableHead>
                  <TableHead>Size</TableHead>
                  <TableHead>Date</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {versions.length === 0 ? (
                  <TableRow>
                    <TableCell colSpan={4} className="py-8 text-center text-muted-foreground">
                      No versions yet.
                    </TableCell>
                  </TableRow>
                ) : (
                  versions.map((version) => (
                    <TableRow key={version.id}>
                      <TableCell>v{version.versionNumber}</TableCell>
                      <TableCell>{version.fileName}</TableCell>
                      <TableCell>{formatBytes(version.sizeBytes)}</TableCell>
                      <TableCell className="text-muted-foreground">{formatDateTime(version.createdAtUtc)}</TableCell>
                    </TableRow>
                  ))
                )}
              </TableBody>
            </Table>
          </CardContent>
        </Card>

        <Card className="lg:col-span-2">
          <CardHeader>
            <CardTitle>Create approval</CardTitle>
          </CardHeader>
          <CardContent className="grid gap-4 md:grid-cols-2">
            <div className="space-y-2">
              <label className="text-sm font-medium" htmlFor="assignedToUserId">Assignee user ID</label>
              <Input id="assignedToUserId" value={approvalTargetUserId} onChange={(event) => setApprovalTargetUserId(event.target.value)} placeholder="Enter user UUID" />
            </div>
            <div className="space-y-2 md:col-span-2">
              <label className="text-sm font-medium" htmlFor="approvalComment">Comment</label>
              <Input id="approvalComment" value={approvalComment} onChange={(event) => setApprovalComment(event.target.value)} placeholder="Optional comment" />
            </div>
            {approvalMessage ? <p className="rounded-lg border border-border bg-muted/40 p-3 text-sm text-muted-foreground md:col-span-2">{approvalMessage}</p> : null}
            <div className="flex gap-3 md:col-span-2">
              <Button type="button" onClick={() => void createApproval()} disabled={approvalSaving || !documentId || !approvalTargetUserId.trim()}>
                {approvalSaving ? 'Creating...' : 'Create approval'}
              </Button>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  )
}
