import { useEffect, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { Button } from '@/components/ui/button'
import { Card, CardContent } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { api, type DocumentItem } from '@/lib/api'

type UpdateDocumentPayload = {
  title: string
  category: string
  department: string
  tagsCsv: string
  confidentialityLevel: number
  expiresAtUtc: string | null
  newFileName: string
  newStoragePath: string
  newSizeBytes: string
}

export function DocumentEditPage() {
  const navigate = useNavigate()
  const { documentId } = useParams()
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)
  const [form, setForm] = useState<UpdateDocumentPayload>({
    title: '',
    category: '',
    department: '',
    tagsCsv: '',
    confidentialityLevel: 1,
    expiresAtUtc: '',
    newFileName: '',
    newStoragePath: '',
    newSizeBytes: '',
  })

  useEffect(() => {
    const load = async () => {
      if (!documentId) {
        setLoading(false)
        setErrorMessage('Missing document identifier.')
        return
      }

      const result = await api.get<DocumentItem[]>('/documents')
      setLoading(false)

      if (!result.ok || !result.data) {
        setErrorMessage(result.error ?? 'Failed to load document.')
        return
      }

      const item = result.data.find((current) => current.id === documentId) ?? null
      if (!item) {
        setErrorMessage('Document not found or you do not have access.')
        return
      }

      setForm({
        title: item.title,
        category: item.category,
        department: item.department,
        tagsCsv: item.tagsCsv,
        confidentialityLevel: Number(item.confidentialityLevel),
        expiresAtUtc: item.expiresAtUtc ? item.expiresAtUtc.slice(0, 16) : '',
        newFileName: '',
        newStoragePath: '',
        newSizeBytes: '',
      })
    }

    void load()
  }, [documentId])

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h3 className="text-2xl font-semibold tracking-tight">Edit document</h3>
          <p className="text-sm text-muted-foreground">Update metadata or upload a new version.</p>
        </div>
        <Button variant="outline" onClick={() => navigate(documentId ? `/documents/${documentId}` : '/documents')}>Back</Button>
      </div>

      {errorMessage ? <p className="rounded-lg border border-destructive/30 bg-destructive/5 p-3 text-sm text-destructive">{errorMessage}</p> : null}

      <Card>
        <CardContent className="pt-6">
          {loading ? (
            <p className="text-sm text-muted-foreground">Loading...</p>
          ) : (
            <form
              className="grid gap-4 md:grid-cols-2"
              onSubmit={async (event) => {
                event.preventDefault()
                if (!documentId) {
                  setErrorMessage('Missing document identifier.')
                  return
                }

                setSaving(true)
                setErrorMessage(null)

                const response = await api.put<DocumentItem>(`/documents/${documentId}`, {
                  title: form.title || null,
                  category: form.category || null,
                  department: form.department || null,
                  tagsCsv: form.tagsCsv || null,
                  confidentialityLevel: Number(form.confidentialityLevel),
                  expiresAtUtc: form.expiresAtUtc ? new Date(form.expiresAtUtc).toISOString() : null,
                  newFileName: form.newFileName || null,
                  newStoragePath: form.newStoragePath || null,
                  newSizeBytes: form.newSizeBytes ? Number(form.newSizeBytes) : null,
                })

                setSaving(false)

                if (!response.ok || !response.data) {
                  setErrorMessage(response.error ?? 'Failed to update document.')
                  return
                }

                navigate(`/documents/${response.data.id}`)
              }}
            >
              {[
                ['title', 'Title'],
                ['category', 'Category'],
                ['department', 'Department'],
                ['tagsCsv', 'Tags (CSV)'],
                ['newFileName', 'New file name'],
                ['newStoragePath', 'New storage path'],
              ].map(([key, label]) => (
                <div className="space-y-2" key={key}>
                  <label className="text-sm font-medium" htmlFor={key}>{label}</label>
                  <Input
                    id={key}
                    value={String(form[key as keyof UpdateDocumentPayload] ?? '')}
                    onChange={(event) => setForm((current) => ({ ...current, [key]: event.target.value }))}
                  />
                </div>
              ))}

              <div className="space-y-2">
                <label className="text-sm font-medium" htmlFor="confidentialityLevel">Classification</label>
                <Input
                  id="confidentialityLevel"
                  type="number"
                  min={0}
                  max={3}
                  value={form.confidentialityLevel}
                  onChange={(event) => setForm((current) => ({ ...current, confidentialityLevel: Number(event.target.value) }))}
                />
              </div>

              <div className="space-y-2">
                <label className="text-sm font-medium" htmlFor="expiresAtUtc">Expires</label>
                <Input
                  id="expiresAtUtc"
                  type="datetime-local"
                  value={form.expiresAtUtc ?? ''}
                  onChange={(event) => setForm((current) => ({ ...current, expiresAtUtc: event.target.value }))}
                />
              </div>

              <div className="space-y-2">
                <label className="text-sm font-medium" htmlFor="newSizeBytes">New size (bytes)</label>
                <Input
                  id="newSizeBytes"
                  type="number"
                  min={1}
                  value={form.newSizeBytes}
                  onChange={(event) => setForm((current) => ({ ...current, newSizeBytes: event.target.value }))}
                />
              </div>

              <div className="flex gap-3 md:col-span-2">
                <Button type="submit" disabled={saving}>{saving ? 'Saving...' : 'Save changes'}</Button>
                <Button type="button" variant="outline" onClick={() => navigate(documentId ? `/documents/${documentId}` : '/documents')}>Cancel</Button>
              </div>
            </form>
          )}
        </CardContent>
      </Card>
    </div>
  )
}
