import { Navigate, Route, Routes } from 'react-router-dom'
import { AppShell } from '@/components/app-shell'
import { DashboardPage } from '@/pages/dashboard-page'
import { LoginPage } from '@/pages/login-page'
import { RegisterPage } from '@/pages/register-page'
import { DocumentsPage } from '@/pages/documents-page'
import { ApprovalsPage } from '@/pages/approvals-page'
import { AuditPage } from '@/pages/audit-page'
import { DocumentCreatePage } from '@/pages/document-create-page'
import { DocumentDetailPage } from '@/pages/document-detail-page'
import { DocumentEditPage } from './pages/document-edit-page'

function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />
      <Route element={<AppShell />}>
        <Route index element={<Navigate to="/dashboard" replace />} />
        <Route path="/dashboard" element={<DashboardPage />} />
        <Route path="/documents" element={<DocumentsPage />} />
        <Route path="/documents/new" element={<DocumentCreatePage />} />
        <Route path="/documents/:documentId" element={<DocumentDetailPage />} />
        <Route path="/documents/:documentId/edit" element={<DocumentEditPage />} />
        <Route path="/approvals" element={<ApprovalsPage />} />
        <Route path="/audit" element={<AuditPage />} />
      </Route>
      <Route path="*" element={<Navigate to="/dashboard" replace />} />
    </Routes>
  )
}

export default App
