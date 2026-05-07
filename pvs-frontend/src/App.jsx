import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { useUserRole } from './shared/RoleGuard';
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
            <AdminLayout userRole={userRole}>
              <Dashboard />
            </AdminLayout>
          </ProtectedRoute>
        }
      />
      <Route
        path="/properties"
        element={
          <ProtectedRoute>
            <AdminLayout userRole={userRole}>
              <Properties />
            </AdminLayout>
          </ProtectedRoute>
        }
      />
      <Route
        path="/clients"
        element={
          <ProtectedRoute>
            <AdminLayout userRole={userRole}>
              <Clients />
            </AdminLayout>
          </ProtectedRoute>
        }
      />
      <Route
        path="/appointments"
        element={
          <ProtectedRoute>
            <AdminLayout userRole={userRole}>
              <Appointments />
            </AdminLayout>
          </ProtectedRoute>
        }
      />
      <Route
        path="/offers"
        element={
          <ProtectedRoute>
            <AdminLayout userRole={userRole}>
              <Offers />
            </AdminLayout>
          </ProtectedRoute>
        }
      />
      <Route
        path="/branches"
        element={
          <ProtectedRoute>
            <AdminLayout userRole={userRole}>
              <Branches />
            </AdminLayout>
          </ProtectedRoute>
        }
      />
      <Route
        path="/users"
        element={
          <ProtectedRoute>
            <AdminLayout userRole={userRole}>
              <Users />
            </AdminLayout>
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
