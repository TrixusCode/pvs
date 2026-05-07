import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';

export default function RoleGuard({ children, allowedRoles = [] }) {
  const [userRole, setUserRole] = useState(null);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    const token = localStorage.getItem('authToken');
    const storedRole = localStorage.getItem('userRole');
    
    if (!token) {
      navigate('/login');
      return;
    }

    setUserRole(storedRole || 'Agent');
    setLoading(false);
  }, [navigate]);

  if (loading) {
    return <div className="text-center p-5">Loading...</div>;
  }

  if (allowedRoles.length > 0 && !allowedRoles.includes(userRole)) {
    return (
      <div className="alert alert-danger m-5">
        <h4>Access Denied</h4>
        <p>You don't have permission to access this page.</p>
      </div>
    );
  }

  return children;
}

// Hook to get current user role
export function useUserRole() {
  const [role, setRole] = useState(localStorage.getItem('userRole') || 'Agent');
  
  useEffect(() => {
    const handleStorageChange = () => {
      setRole(localStorage.getItem('userRole') || 'Agent');
    };
    
    window.addEventListener('storage', handleStorageChange);
    return () => window.removeEventListener('storage', handleStorageChange);
  }, []);

  return role;
}
