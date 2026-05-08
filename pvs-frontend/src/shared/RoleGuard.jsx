import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';

// Utility function to decode JWT token
function decodeJWT(token) {
  try {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(atob(base64).split('').map(function(c) {
      return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
    }).join(''));

    return JSON.parse(jsonPayload);
  } catch (error) {
    console.error('Error decoding JWT:', error);
    return null;
  }
}

export default function RoleGuard({ children, allowedRoles = [] }) {
  const [userRole, setUserRole] = useState(null);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    const token = localStorage.getItem('authToken');
    
    if (!token) {
      navigate('/login');
      return;
    }

    // Decode JWT token to get role
    const decoded = decodeJWT(token);
    if (decoded && decoded.role) {
      setUserRole(decoded.role);
      // Also store in localStorage for backward compatibility
      localStorage.setItem('userRole', decoded.role);
      localStorage.setItem('userId', decoded.nameid);
      localStorage.setItem('userEmail', decoded.email);
    } else {
      // Fallback to localStorage if JWT parsing fails
      const storedRole = localStorage.getItem('userRole');
      setUserRole(storedRole || 'Agent');
    }
    
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
        <p className="text-muted">Your role: {userRole}</p>
      </div>
    );
  }

  return children;
}

// Hook to get current user role
export function useUserRole() {
  const [role, setRole] = useState(() => {
    const token = localStorage.getItem('authToken');
    if (token) {
      const decoded = decodeJWT(token);
      return decoded?.role || localStorage.getItem('userRole') || 'Agent';
    }
    return localStorage.getItem('userRole') || 'Agent';
  });
  
  useEffect(() => {
    const handleStorageChange = () => {
      const token = localStorage.getItem('authToken');
      if (token) {
        const decoded = decodeJWT(token);
        setRole(decoded?.role || localStorage.getItem('userRole') || 'Agent');
      } else {
        setRole(localStorage.getItem('userRole') || 'Agent');
      }
    };
    
    window.addEventListener('storage', handleStorageChange);
    return () => window.removeEventListener('storage', handleStorageChange);
  }, []);

  return role;
}

// Hook to get current user info
export function useUserInfo() {
  const [userInfo, setUserInfo] = useState(() => {
    const token = localStorage.getItem('authToken');
    if (token) {
      const decoded = decodeJWT(token);
      return {
        id: decoded?.nameid,
        email: decoded?.email,
        role: decoded?.role,
        name: decoded?.unique_name
      };
    }
    return null;
  });

  return userInfo;
}
