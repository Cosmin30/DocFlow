import { useEffect, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
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
        setErrorMessage('Lipsește identificatorul documentului.')
        return
      }

      const result = await api.get<DocumentItem[]>('/documents')
      setLoading(false)

      if (!result.ok || !result.data) {
        setErrorMessage(result.error ?? 'Nu am putut încărca documentul.')
        return
      }

      const item = result.data.find((current) => current.id === documentId) ?? null
      if (!item) {
        setErrorMessage('Documentul nu există sau nu ai acces la el.')
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
          <h3 className="text-2xl font-semibold tracking-tight">Editează document</h3>
          <p className="text-sm text-muted-foreground">Actualizează metadatele sau atașează o nouă versiune.</p>
        </div>
        <Button variant="outline" onClick={() => navigate(documentId ? `/documents/${documentId}` : '/documents')}>Înapoi</Button>
      </div>

      {errorMessage ? <p className="rounded-lg border border-destructive/30 bg-destructive/5 p-3 text-sm text-destructive">{errorMessage}</p> : null}

      <Card>
        <CardHeader>
          <CardTitle>Formular de editare</CardTitle>
          <CardDescription>Câmpurile lăsate goale nu schimbă valoarea curentă, iar fișierul nou este opțional.</CardDescription>
        </CardHeader>
        <CardContent>
          {loading ? (
            <p className="text-sm text-muted-foreground">Se încarcă documentul...</p>
          ) : (
            <form
              className="grid gap-4 md:grid-cols-2"
              onSubmit={async (event) => {
                event.preventDefault()
                if (!documentId) {
                  setErrorMessage('Lipsește identificatorul documentului.')
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
                  setErrorMessage(response.error ?? 'Documentul nu a putut fi actualizat.')
                  return
                }

                navigate(`/documents/${response.data.id}`)
              }}
            >
              {[
                ['title', 'Titlu'],
                ['category', 'Categorie'],
                ['department', 'Departament'],
                ['tagsCsv', 'Etichete (CSV)'],
                ['newFileName', 'Nume fișier nou'],
                ['newStoragePath', 'Cale stocare nouă'],
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
                <label className="text-sm font-medium" htmlFor="confidentialityLevel">Clasificare</label>
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
                <label className="text-sm font-medium" htmlFor="expiresAtUtc">Expirare</label>
                <Input
                  id="expiresAtUtc"
                  type="datetime-local"
                  value={form.expiresAtUtc ?? ''}
                  onChange={(event) => setForm((current) => ({ ...current, expiresAtUtc: event.target.value }))}
                />
              </div>

              <div className="space-y-2">
                <label className="text-sm font-medium" htmlFor="newSizeBytes">Dimensiune nouă bytes</label>
                <Input
                  id="newSizeBytes"
                  type="number"
                  min={1}
                  value={form.newSizeBytes}
                  onChange={(event) => setForm((current) => ({ ...current, newSizeBytes: event.target.value }))}
                />
              </div>

              <div className="flex gap-3 md:col-span-2">
                <Button type="submit" disabled={saving}>{saving ? 'Se salvează...' : 'Salvează schimbările'}</Button>
                <Button type="button" variant="outline" onClick={() => navigate(documentId ? `/documents/${documentId}` : '/documents')}>Renunță</Button>
              </div>
            </form>
          )}
        </CardContent>
      </Card>
    </div>
  )
}