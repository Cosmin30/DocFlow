import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { api, type DocumentItem } from '@/lib/api'

type CreateDocumentPayload = {
  title: string
  category: string
  department: string
  tagsCsv: string
  confidentialityLevel: number
  expiresAtUtc: string | null
  fileName: string
  storagePath: string
  sizeBytes: number
}

export function DocumentCreatePage() {
  const navigate = useNavigate()
  const [form, setForm] = useState<CreateDocumentPayload>({
    title: '',
    category: '',
    department: '',
    tagsCsv: '',
    confidentialityLevel: 1,
    expiresAtUtc: '',
    fileName: '',
    storagePath: '',
    sizeBytes: 1,
  })
  const [errorMessage, setErrorMessage] = useState<string | null>(null)
  const [saving, setSaving] = useState(false)

  return (
    <div className="space-y-6">
      <div>
        <h3 className="text-2xl font-semibold tracking-tight">Document nou</h3>
        <p className="text-sm text-muted-foreground">Creează un document nou în backend.</p>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Detalii document</CardTitle>
          <CardDescription>Completează câmpurile cerute de API.</CardDescription>
        </CardHeader>
        <CardContent>
          <form
            className="grid gap-4 md:grid-cols-2"
            onSubmit={async (event) => {
              event.preventDefault()
              setErrorMessage(null)
              setSaving(true)

              const response = await api.post<DocumentItem>('/documents', {
                ...form,
                expiresAtUtc: form.expiresAtUtc ? new Date(form.expiresAtUtc).toISOString() : null,
                confidentialityLevel: Number(form.confidentialityLevel),
                sizeBytes: Number(form.sizeBytes),
              })

              setSaving(false)

              if (!response.ok || !response.data) {
                setErrorMessage(response.error ?? 'Documentul nu a putut fi creat.')
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
              ['fileName', 'Nume fișier'],
              ['storagePath', 'Cale stocare'],
            ].map(([key, label]) => (
              <div className="space-y-2" key={key}>
                <label className="text-sm font-medium" htmlFor={key}>
                  {label}
                </label>
                <Input
                  id={key}
                  value={String(form[key as keyof CreateDocumentPayload] ?? '')}
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
              <label className="text-sm font-medium" htmlFor="sizeBytes">Dimensiune bytes</label>
              <Input id="sizeBytes" type="number" min={1} value={form.sizeBytes} onChange={(event) => setForm((current) => ({ ...current, sizeBytes: Number(event.target.value) }))} />
            </div>

            {errorMessage ? <p className="rounded-lg border border-destructive/30 bg-destructive/5 p-3 text-sm text-destructive md:col-span-2">{errorMessage}</p> : null}

            <div className="flex gap-3 md:col-span-2">
              <Button type="submit" disabled={saving}>{saving ? 'Se salvează...' : 'Creează document'}</Button>
              <Button type="button" variant="outline" onClick={() => navigate('/documents')}>Renunță</Button>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  )
}