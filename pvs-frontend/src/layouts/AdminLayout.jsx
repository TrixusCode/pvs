import { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { Container, Navbar, Nav, Offcanvas, Button, Dropdown } from 'react-bootstrap';
import { FaBars, FaHome, FaBuilding, FaUsers, FaCalendar, FaHandshake, FaSignOutAlt, FaBell, FaMapMarker, FaUserCog } from 'react-icons/fa';
import './AdminLayout.css';

export default function AdminLayout({ children, userRole = 'Agent' }) {
  const [showSidebar, setShowSidebar] = useState(true);
  const navigate = useNavigate();
  const location = useLocation();

  const getMenuItems = () => {
    const allItems = [
      { label: 'Dashboard', icon: <FaHome />, path: '/dashboard' },
      { label: 'Properties', icon: <FaBuilding />, path: '/properties' },
      { label: 'Clients', icon: <FaUsers />, path: '/clients' },
      { label: 'Appointments', icon: <FaCalendar />, path: '/appointments' },
      { label: 'Offers', icon: <FaHandshake />, path: '/offers' },
      { label: 'Branches', icon: <FaMapMarker />, path: '/branches' },
      { label: 'Users', icon: <FaUserCog />, path: '/users' },
    ];

    // Filter items based on user role
    if (userRole === 'Client_Buyer' || userRole === 'Client_Seller') {
      return allItems.filter(item => 
        ['Dashboard', 'Properties', 'Appointments', 'Offers'].includes(item.label)
      );
    } else if (userRole === 'Agent') {
      return allItems.filter(item => 
        !['Branches', 'Users'].includes(item.label)
      );
    } else if (userRole === 'Manager') {
      return allItems.filter(item => 
        item.label !== 'Users'
      );
    } else if (userRole === 'Admin') {
      return allItems;
    }
    return allItems;
  };

  const handleLogout = () => {
    localStorage.removeItem('authToken');
    navigate('/login');
  };

  const isActive = (path) => location.pathname === path;

  return (
    <div className="admin-layout">
      {/* Top Navigation Bar */}
      <Navbar bg="dark" expand="lg" sticky="top" className="admin-navbar">
        <Container fluid>
          <Button 
            variant="link" 
            className="text-light sidebar-toggle"
            onClick={() => setShowSidebar(!showSidebar)}
          >
            <FaBars size={24} />
          </Button>
          <Navbar.Brand href="/dashboard" className="text-light fw-bold">
            PVS
          </Navbar.Brand>
          <Nav className="ms-auto">
            <Nav.Link href="#" className="text-light position-relative">
              <FaBell size={20} />
              <span className="badge bg-danger position-absolute top-0 start-100 translate-middle">3</span>
            </Nav.Link>
            <Dropdown align="end" className="ms-3">
              <Dropdown.Toggle variant="link" className="text-light text-decoration-none">
                {userRole}
              </Dropdown.Toggle>
              <Dropdown.Menu>
                <Dropdown.Item onClick={handleLogout}>
                  <FaSignOutAlt className="me-2" /> Logout
                </Dropdown.Item>
              </Dropdown.Menu>
            </Dropdown>
          </Nav>
        </Container>
      </Navbar>

      <div className="admin-container">
        {/* Sidebar */}
        <div className={`admin-sidebar ${showSidebar ? 'show' : 'hide'}`}>
          <nav className="sidebar-nav">
            {getMenuItems().map((item) => (
              <a
                key={item.path}
                href={item.path}
                className={`sidebar-item ${isActive(item.path) ? 'active' : ''}`}
                onClick={(e) => {
                  e.preventDefault();
                  navigate(item.path);
                }}
              >
                <span className="sidebar-icon">{item.icon}</span>
                <span className="sidebar-label">{item.label}</span>
              </a>
            ))}
          </nav>
        </div>

        {/* Main Content */}
        <main className="admin-main">
          <div className="admin-content">
            {children}
          </div>
        </main>
      </div>
    </div>
  );
}
