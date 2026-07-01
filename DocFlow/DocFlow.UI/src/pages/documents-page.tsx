import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { api, type DocumentItem } from '@/lib/api'
import { getAccessToken } from '@/lib/auth'
import { Search, Upload } from 'lucide-react'

const emptyMessage = 'Nu există documente în baza de date.'

function confidentialityLabel(value: number | string) {
  const normalized = String(value)
  if (normalized === '0' || normalized.toLowerCase() === 'public') return 'Public'
  if (normalized === '1' || normalized.toLowerCase() === 'internal') return 'Intern'
  if (normalized === '2' || normalized.toLowerCase() === 'confidential') return 'Confidențial'
  return 'Strict'
}

export function DocumentsPage() {
  const navigate = useNavigate()
  const [documents, setDocuments] = useState<DocumentItem[]>([])
  const [loading, setLoading] = useState(true)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)

  useEffect(() => {
    const load = async () => {
      if (!getAccessToken()) {
        setLoading(false)
        setErrorMessage('Autentifică-te pentru a vedea documentele din backend.')
        return
      }

      const result = await api.get<DocumentItem[]>('/documents')
      setLoading(false)

      if (result.ok && result.data) {
        setDocuments(result.data)
      } else {
        setErrorMessage(result.error ?? 'Nu am putut încărca documentele.')
      }
    }

    void load()
  }, [])

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h3 className="text-2xl font-semibold tracking-tight">Documente</h3>
          <p className="text-sm text-muted-foreground">Răsfoiește, caută și încarcă documente din backend.</p>
        </div>
        <Button className="gap-2" onClick={() => navigate('/documents/new')}>
          <Upload className="h-4 w-4" />
          Încarcă document
        </Button>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Bibliotecă</CardTitle>
          <CardDescription>Filtrează după titlu, categorie sau departament.</CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="relative max-w-md">
            <Search className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
            <Input className="pl-9" placeholder="Caută documente..." />
          </div>

          {errorMessage ? <p className="rounded-lg border border-destructive/30 bg-destructive/5 p-3 text-sm text-destructive">{errorMessage}</p> : null}

          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Titlu</TableHead>
                <TableHead>Categorie</TableHead>
                <TableHead>Departament</TableHead>
                <TableHead>Clasificare</TableHead>
                <TableHead>Versiune</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {loading ? (
                <TableRow>
                  <TableCell colSpan={5} className="py-8 text-center text-muted-foreground">
                    Se încarcă documentele...
                  </TableCell>
                </TableRow>
              ) : documents.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={5} className="py-8 text-center text-muted-foreground">
                    {emptyMessage}
                  </TableCell>
                </TableRow>
              ) : (
                documents.map((document) => (
                  <TableRow key={document.id} className="cursor-pointer" onClick={() => navigate(`/documents/${document.id}`)}>
                    <TableCell className="font-medium">{document.title}</TableCell>
                    <TableCell>{document.category}</TableCell>
                    <TableCell>{document.department}</TableCell>
                    <TableCell>
                      <Badge variant="outline">{confidentialityLabel(document.confidentialityLevel)}</Badge>
                    </TableCell>
                    <TableCell>
                      <Badge variant="secondary">v{document.currentVersionNumber}</Badge>
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