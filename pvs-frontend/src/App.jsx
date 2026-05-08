import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { useUserRole } from './shared/RoleGuard';
import RoleGuard from './shared/RoleGuard';
import AdminLayout from './layouts/AdminLayout';
import Login from './modules/auth/Login';
import Register from './modules/auth/Register';
import Properties from './modules/properties/Properties';
import Dashboard from './modules/dashboard/Dashboard';
import Clients from './modules/clients/Clients';
import Appointments from './modules/appointments/Appointments';
import Offers from './modules/offers/Offers';
import Branches from './modules/branches/Branches';
import Users from './modules/users/Users';
import Employees from './modules/employees/Employees';
import ProtectedRoute from './modules/shared/ProtectedRoute';
import 'bootstrap/dist/css/bootstrap.min.css';
import './App.css';

function AppContent() {
  const userRole = useUserRole();

  return (
    <Routes>
      <Route path="/login" element={<Login />} />
      <Route path="/register" element={<Register />} />
      <Route
        path="/dashboard"
        element={
          <ProtectedRoute>
            <RoleGuard>
              <AdminLayout userRole={userRole}>
                <Dashboard />
              </AdminLayout>
            </RoleGuard>
          </ProtectedRoute>
        }
      />
      <Route
        path="/properties"
        element={
          <ProtectedRoute>
            <RoleGuard>
              <AdminLayout userRole={userRole}>
                <Properties />
              </AdminLayout>
            </RoleGuard>
          </ProtectedRoute>
        }
      />
      <Route
        path="/clients"
        element={
          <ProtectedRoute>
            <RoleGuard allowedRoles={['Admin', 'Manager', 'Agent']}>
              <AdminLayout userRole={userRole}>
                <Clients />
              </AdminLayout>
            </RoleGuard>
          </ProtectedRoute>
        }
      />
      <Route
        path="/appointments"
        element={
          <ProtectedRoute>
            <RoleGuard>
              <AdminLayout userRole={userRole}>
                <Appointments />
              </AdminLayout>
            </RoleGuard>
          </ProtectedRoute>
        }
      />
      <Route
        path="/offers"
        element={
          <ProtectedRoute>
            <RoleGuard>
              <AdminLayout userRole={userRole}>
                <Offers />
              </AdminLayout>
            </RoleGuard>
          </ProtectedRoute>
        }
      />
      <Route
        path="/branches"
        element={
          <ProtectedRoute>
            <RoleGuard allowedRoles={['Admin', 'Manager']}>
              <AdminLayout userRole={userRole}>
                <Branches />
              </AdminLayout>
            </RoleGuard>
          </ProtectedRoute>
        }
      />
      <Route
        path="/users"
        element={
          <ProtectedRoute>
            <RoleGuard allowedRoles={['Admin']}>
              <AdminLayout userRole={userRole}>
                <Users />
              </AdminLayout>
            </RoleGuard>
          </ProtectedRoute>
        }
      />
      <Route
        path="/employees"
        element={
          <ProtectedRoute>
            <RoleGuard allowedRoles={['Admin', 'Manager']}>
              <AdminLayout userRole={userRole}>
                <Employees />
              </AdminLayout>
            </RoleGuard>
          </ProtectedRoute>
        }
      />
      <Route path="/" element={<Navigate to="/dashboard" />} />
    </Routes>
  );
}

function App() {
  return (
    <Router>
      <AppContent />
    </Router>
  );
}

export default App;
