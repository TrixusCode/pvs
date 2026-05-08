import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Container, Row, Col, Card, Button, Badge, Alert, ListGroup } from 'react-bootstrap';
import { FaHome, FaUsers, FaCalendar, FaHandshake, FaBell, FaPlus, FaEye, FaCog, FaUserTie } from 'react-icons/fa';
import { useUserRole, useUserInfo } from '../../shared/RoleGuard';
import DashboardService from './DashboardService';
import './Dashboard.css';

export default function Dashboard() {
  const [stats, setStats] = useState({});
  const [recentActivities, setRecentActivities] = useState([]);
  const [notifications, setNotifications] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const navigate = useNavigate();
  const userRole = useUserRole();
  const userInfo = useUserInfo();

  useEffect(() => {
    fetchDashboardData();
  }, []);

  const fetchDashboardData = async () => {
    setLoading(true);
    setError('');

    try {
      const [statsRes, activitiesRes, notificationsRes] = await Promise.all([
        DashboardService.getStatistics(),
        DashboardService.getRecentActivities(),
        DashboardService.getNotifications()
      ]);

      setStats(statsRes);
      setRecentActivities(activitiesRes.data || []);
      setNotifications(notificationsRes.data || []);
    } catch (err) {
      setError(err.message || 'Failed to load dashboard data');
      console.error('Error fetching dashboard data:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleLogout = () => {
    DashboardService.logout();
    navigate('/login');
  };

  const markNotificationAsRead = async (id) => {
    try {
      await DashboardService.markNotificationAsRead(id);
      setNotifications(notifications.map(n =>
        n.id === id ? { ...n, isRead: true } : n
      ));
    } catch (err) {
      console.error('Error marking notification as read:', err);
    }
  };

  const getActivityIcon = (type) => {
    const icons = {
      'property': <FaHome />,
      'client': <FaUsers />,
      'appointment': <FaCalendar />,
      'offer': <FaHandshake />
    };
    return icons[type] || <FaBell />;
  };

  const formatTimeAgo = (timestamp) => {
    const now = new Date();
    const time = new Date(timestamp);
    const diffInHours = Math.floor((now - time) / (1000 * 60 * 60));

    if (diffInHours < 1) return 'Just now';
    if (diffInHours < 24) return `${diffInHours}h ago`;
    const diffInDays = Math.floor(diffInHours / 24);
    return `${diffInDays}d ago`;
  };

  if (loading) {
    return (
      <Container fluid className="pt-4 pb-4">
        <div className="text-center py-5">
          <div className="spinner-border" role="status">
            <span className="visually-hidden">Loading dashboard...</span>
          </div>
        </div>
      </Container>
    );
  }

  return (
    <Container fluid className="pt-4 pb-4">
      {/* Page Header */}
      <div className="page-header d-flex justify-content-between align-items-center mb-4">
        <div>
          <h1>Dashboard</h1>
          <p className="text-muted mb-0">Welcome back, {userInfo?.name || 'User'}!</p>
        </div>
        <Button variant="outline-secondary" onClick={handleLogout}>
          Logout
        </Button>
      </div>

      {/* Error Alert */}
      {error && <Alert variant="danger" onClose={() => setError('')} dismissible>{error}</Alert>}

      {/* Statistics Cards */}
      <Row className="mb-4">
        <Col lg={3} md={6} className="mb-4">
          <Card className="stat-card h-100">
            <Card.Body className="text-center">
              <div className="stat-icon mb-3">
                <FaHome size={32} />
              </div>
              <h2 className="stat-number">{stats.totalProperties || 0}</h2>
              <p className="stat-label mb-0">Total Properties</p>
            </Card.Body>
          </Card>
        </Col>
        <Col lg={3} md={6} className="mb-4">
          <Card className="stat-card h-100">
            <Card.Body className="text-center">
              <div className="stat-icon mb-3">
                <FaUsers size={32} />
              </div>
              <h2 className="stat-number">{stats.totalClients || 0}</h2>
              <p className="stat-label mb-0">Total Clients</p>
            </Card.Body>
          </Card>
        </Col>
        {(userRole === 'Admin' || userRole === 'Manager') && (
          <Col lg={3} md={6} className="mb-4">
            <Card className="stat-card h-100">
              <Card.Body className="text-center">
                <div className="stat-icon mb-3">
                  <FaUserTie size={32} />
                </div>
                <h2 className="stat-number">{stats.totalEmployees || 0}</h2>
                <p className="stat-label mb-0">Total Employees</p>
              </Card.Body>
            </Card>
          </Col>
        )}
        <Col lg={3} md={6} className="mb-4">
          <Card className="stat-card h-100">
            <Card.Body className="text-center">
              <div className="stat-icon mb-3">
                <FaCalendar size={32} />
              </div>
              <h2 className="stat-number">{stats.activeAppointments || 0}</h2>
              <p className="stat-label mb-0">Active Appointments</p>
            </Card.Body>
          </Card>
        </Col>
        <Col lg={3} md={6} className="mb-4">
          <Card className="stat-card h-100">
            <Card.Body className="text-center">
              <div className="stat-icon mb-3">
                <FaHandshake size={32} />
              </div>
              <h2 className="stat-number">{stats.pendingOffers || 0}</h2>
              <p className="stat-label mb-0">Pending Offers</p>
            </Card.Body>
          </Card>
        </Col>
      </Row>

      <Row>
        {/* User Profile & Quick Actions */}
        <Col lg={4} className="mb-4">
          <Card className="h-100">
            <Card.Header>
              <h5 className="mb-0">My Profile</h5>
            </Card.Header>
            <Card.Body>
              <div className="profile-details">
                <p className="mb-2"><strong>Name:</strong> {userInfo?.firstName} {userInfo?.lastName}</p>
                <p className="mb-2"><strong>Email:</strong> {userInfo?.email}</p>
                <p className="mb-0"><strong>Role:</strong> <Badge bg="primary">{userRole}</Badge></p>
              </div>
            </Card.Body>
          </Card>

          <Card className="mt-4">
            <Card.Header>
              <h5 className="mb-0">Quick Actions</h5>
            </Card.Header>
            <Card.Body>
              <div className="d-grid gap-2">
                {(userRole === 'Admin' || userRole === 'Manager' || userRole === 'Agent') && (
                  <Button
                    variant="primary"
                    onClick={() => navigate('/properties')}
                    className="d-flex align-items-center justify-content-start"
                  >
                    <FaPlus className="me-2" />
                    Add Property
                  </Button>
                )}
                {(userRole === 'Admin' || userRole === 'Manager' || userRole === 'Agent') && (
                  <Button
                    variant="outline-primary"
                    onClick={() => navigate('/clients')}
                    className="d-flex align-items-center justify-content-start"
                  >
                    <FaUsers className="me-2" />
                    Manage Clients
                  </Button>
                )}
                {(userRole === 'Admin' || userRole === 'Manager' || userRole === 'Agent') && (
                  <Button
                    variant="outline-primary"
                    onClick={() => navigate('/appointments')}
                    className="d-flex align-items-center justify-content-start"
                  >
                    <FaCalendar className="me-2" />
                    Schedule Appointment
                  </Button>
                )}
                {(userRole === 'Admin' || userRole === 'Manager' || userRole === 'Agent') && (
                  <Button
                    variant="outline-primary"
                    onClick={() => navigate('/offers')}
                    className="d-flex align-items-center justify-content-start"
                  >
                    <FaHandshake className="me-2" />
                    Review Offers
                  </Button>
                )}
                {(userRole === 'Admin' || userRole === 'Manager') && (
                  <Button
                    variant="outline-success"
                    onClick={() => navigate('/employees')}
                    className="d-flex align-items-center justify-content-start"
                  >
                    <FaUserTie className="me-2" />
                    Manage Employees
                  </Button>
                )}
                {userRole === 'Admin' && (
                  <Button
                    variant="outline-warning"
                    onClick={() => navigate('/branches')}
                    className="d-flex align-items-center justify-content-start"
                  >
                    <FaCog className="me-2" />
                    Manage Branches
                  </Button>
                )}
              </div>
            </Card.Body>
          </Card>
        </Col>

        {/* Recent Activities & Notifications */}
        <Col lg={8}>
          <Row>
            <Col md={6} className="mb-4">
              <Card className="h-100">
                <Card.Header>
                  <h5 className="mb-0">Recent Activities</h5>
                </Card.Header>
                <Card.Body className="p-0">
                  <ListGroup variant="flush">
                    {recentActivities.length === 0 ? (
                      <ListGroup.Item className="text-center py-4 text-muted">
                        No recent activities
                      </ListGroup.Item>
                    ) : (
                      recentActivities.slice(0, 5).map((activity, index) => (
                        <ListGroup.Item key={index} className="d-flex align-items-start">
                          <div className="activity-icon me-3 mt-1">
                            {getActivityIcon(activity.type)}
                          </div>
                          <div className="flex-grow-1">
                            <p className="mb-1 small">{activity.description}</p>
                            <small className="text-muted">{formatTimeAgo(activity.timestamp)}</small>
                          </div>
                        </ListGroup.Item>
                      ))
                    )}
                  </ListGroup>
                </Card.Body>
              </Card>
            </Col>

            <Col md={6} className="mb-4">
              <Card className="h-100">
                <Card.Header>
                  <h5 className="mb-0">Notifications</h5>
                </Card.Header>
                <Card.Body className="p-0">
                  <ListGroup variant="flush">
                    {notifications.length === 0 ? (
                      <ListGroup.Item className="text-center py-4 text-muted">
                        No notifications
                      </ListGroup.Item>
                    ) : (
                      notifications.slice(0, 5).map((notification) => (
                        <ListGroup.Item
                          key={notification.id}
                          className={`d-flex align-items-start ${!notification.isRead ? 'bg-light' : ''}`}
                          style={{ cursor: !notification.isRead ? 'pointer' : 'default' }}
                          onClick={() => !notification.isRead && markNotificationAsRead(notification.id)}
                        >
                          <div className="flex-grow-1">
                            <p className="mb-1 small">{notification.message}</p>
                            <small className="text-muted">{formatTimeAgo(notification.createdAt)}</small>
                          </div>
                          {!notification.isRead && (
                            <Badge bg="primary" pill className="ms-2">New</Badge>
                          )}
                        </ListGroup.Item>
                      ))
                    )}
                  </ListGroup>
                </Card.Body>
              </Card>
            </Col>
          </Row>
        </Col>
      </Row>
    </Container>
  );
}
