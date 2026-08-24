import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Card, CardContent } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { api, type DocumentItem } from '@/lib/api'
import { confidentialityLabel, formatDate } from '@/lib/utils'
import { Search, Upload } from 'lucide-react'

export function DocumentsPage() {
  const navigate = useNavigate()
  const [documents, setDocuments] = useState<DocumentItem[]>([])
  const [loading, setLoading] = useState(true)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)
  const [searchQuery, setSearchQuery] = useState('')

  useEffect(() => {
    const load = async () => {
      const result = await api.get<DocumentItem[]>('/documents')
      setLoading(false)

      if (result.ok && result.data) {
        setDocuments(result.data)
      } else {
        setErrorMessage(result.error ?? 'Failed to load documents.')
      }
    }

    void load()
  }, [])

  const filteredDocuments = documents.filter((doc) => {
    if (!searchQuery.trim()) return true
    const q = searchQuery.toLowerCase()
    return (
      doc.title.toLowerCase().includes(q) ||
      doc.category.toLowerCase().includes(q) ||
      doc.department.toLowerCase().includes(q) ||
      doc.tagsCsv.toLowerCase().includes(q)
    )
  })

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h3 className="text-2xl font-semibold tracking-tight">Documents</h3>
          <p className="text-sm text-muted-foreground">Browse and manage your documents.</p>
        </div>
        <Button className="gap-2" onClick={() => navigate('/documents/new')}>
          <Upload className="h-4 w-4" />
          Upload document
        </Button>
      </div>

      <Card>
        <CardContent className="pt-6 space-y-4">
          <div className="relative max-w-md">
            <Search className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
            <Input
              className="pl-9"
              placeholder="Search documents..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
            />
          </div>

          {errorMessage ? <p className="rounded-lg border border-destructive/30 bg-destructive/5 p-3 text-sm text-destructive">{errorMessage}</p> : null}

          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Title</TableHead>
                <TableHead>Category</TableHead>
                <TableHead>Department</TableHead>
                <TableHead>Classification</TableHead>
                <TableHead>Version</TableHead>
                <TableHead>Date</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {loading ? (
                <TableRow>
                  <TableCell colSpan={6} className="py-8 text-center text-muted-foreground">
                    Loading documents...
                  </TableCell>
                </TableRow>
              ) : filteredDocuments.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={6} className="py-8 text-center text-muted-foreground">
                    {searchQuery ? 'No documents match your search.' : 'No documents found.'}
                  </TableCell>
                </TableRow>
              ) : (
                filteredDocuments.map((document) => (
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
                    <TableCell className="text-muted-foreground">{formatDate(document.createdAtUtc)}</TableCell>
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
