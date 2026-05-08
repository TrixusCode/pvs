import { useState, useEffect } from 'react';
import { Container, Row, Col, Button, Table, Badge, Card, Form, Alert, Pagination, ButtonGroup } from 'react-bootstrap';
import { FaPlus, FaEdit, FaTrash, FaSearch, FaCalendarDay, FaClock, FaCheck, FaTimes } from 'react-icons/fa';
import AppointmentsService from './AppointmentsService';
import ClientsService from '../clients/ClientsService';
import PropertiesService from '../properties/PropertiesService';
import FormModal from '../../shared/FormModal';
import './Appointments.css';

export default function Appointments() {
  const [appointments, setAppointments] = useState([]);
  const [clients, setClients] = useState([]);
  const [properties, setProperties] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [showModal, setShowModal] = useState(false);
  const [editingAppointment, setEditingAppointment] = useState(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [submitting, setSubmitting] = useState(false);

  const getPropertyName = (propertyId) => {
    const property = properties.find(item => item.id === Number(propertyId));
    return property ? property.title : 'Unknown property';
  };

  const getClientName = (clientId) => {
    const client = clients.find(item => item.id === Number(clientId));
    return client ? `${client.firstName} ${client.lastName}` : 'Unknown client';
  };

  // Form state
  const [formData, setFormData] = useState({
    appointmentDate: '',
    time: '',
    type: 'Showing',
    status: 'Scheduled',
    propertyId: '',
    clientId: '',
    notes: ''
  });

  useEffect(() => {
    fetchAppointments();
    fetchLookupData();
  }, [page]);

  const fetchLookupData = async () => {
    try {
      const [clientsResponse, propertiesResponse] = await Promise.all([
        ClientsService.getAll(1, 1000),
        PropertiesService.getAll(1, 1000)
      ]);

      setClients(clientsResponse.data || []);
      setProperties(propertiesResponse.data || []);
    } catch (err) {
      console.error('Error loading appointment lookup data:', err);
    }
  };

  const fetchAppointments = async () => {
    setLoading(true);
    setError('');

    try {
      const response = await AppointmentsService.getAll(page, 10);
      setAppointments(response.data || []);
      setTotalPages(response.totalPages || 1);
    } catch (err) {
      setError(err.message || 'Failed to load appointments');
      console.error('Error fetching appointments:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = async (e) => {
    e.preventDefault();
    if (!searchTerm.trim()) {
      setPage(1);
      fetchAppointments();
      return;
    }

    setLoading(true);
    try {
      const response = await AppointmentsService.search(searchTerm);
      setAppointments(response.data || []);
      setTotalPages(1);
      setPage(1);
    } catch (err) {
      setError(err.message || 'Search failed');
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = () => {
    setEditingAppointment(null);
    setFormData({
      appointmentDate: '',
      time: '',
      type: 'Showing',
      status: 'Scheduled',
      propertyId: '',
      clientId: '',
      notes: ''
    });
    setShowModal(true);
  };

  const handleEdit = (appointment) => {
    setEditingAppointment(appointment);
    setFormData({
      appointmentDate: appointment.appointmentDate ? new Date(appointment.appointmentDate).toISOString().split('T')[0] : '',
      time: appointment.time || '',
      type: appointment.type || 'Showing',
      status: appointment.status || 'Scheduled',
      propertyId: appointment.propertyId || '',
      clientId: appointment.clientId || '',
      notes: appointment.notes || ''
    });
    setShowModal(true);
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Are you sure you want to delete this appointment?')) return;

    try {
      await AppointmentsService.delete(id);
      setSuccess('Appointment deleted successfully');
      setAppointments(appointments.filter(a => a.id !== id));
      setTimeout(() => setSuccess(''), 3000);
    } catch (err) {
      setError(err.message || 'Failed to delete appointment');
    }
  };

  const handleStatusChange = async (id, newStatus) => {
    try {
      await AppointmentsService.updateStatus(id, newStatus);
      setSuccess('Appointment status updated successfully');
      fetchAppointments();
      setTimeout(() => setSuccess(''), 3000);
    } catch (err) {
      setError(err.message || 'Failed to update appointment status');
    }
  };

  const handleQuickFilter = async (filter) => {
    setLoading(true);
    setError('');

    try {
      let response;
      if (filter === 'today') {
        response = await AppointmentsService.getToday();
      } else if (filter === 'upcoming') {
        response = await AppointmentsService.getUpcoming(25);
      } else {
        response = await AppointmentsService.getByStatus(filter);
      }

      setAppointments(response.data || []);
      setTotalPages(1);
      setPage(1);
    } catch (err) {
      setError(err.message || 'Failed to filter appointments');
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async () => {
    setError('');
    setSuccess('');
    setSubmitting(true);

    try {
      const data = {
        ...formData,
        propertyId: parseInt(formData.propertyId),
        clientId: parseInt(formData.clientId),
        appointmentDate: formData.appointmentDate ? new Date(formData.appointmentDate).toISOString() : null
      };

      if (editingAppointment) {
        await AppointmentsService.update(editingAppointment.id, data);
        setSuccess('Appointment updated successfully');
        fetchAppointments();
      } else {
        await AppointmentsService.create(data);
        setSuccess('Appointment created successfully');
        fetchAppointments();
      }

      setShowModal(false);
      setTimeout(() => setSuccess(''), 3000);
    } catch (err) {
      setError(err.message || 'Failed to save appointment');
    } finally {
      setSubmitting(false);
    }
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const getStatusBadge = (status) => {
    const variants = {
      'Scheduled': 'primary',
      'Completed': 'success',
      'Cancelled': 'danger',
      'NoShow': 'warning'
    };
    return <Badge bg={variants[status] || 'info'}>{status}</Badge>;
  };

  const getTypeBadge = (type) => {
    const variants = {
      'Showing': 'info',
      'Inspection': 'secondary'
    };
    return <Badge bg={variants[type] || 'secondary'}>{type}</Badge>;
  };

  const formatDateTime = (date, time) => {
    const dateObj = new Date(date);
    return `${dateObj.toLocaleDateString()} at ${time}`;
  };

  return (
    <Container fluid className="pt-4 pb-4">
      {/* Page Header */}
      <div className="page-header d-flex justify-content-between align-items-center mb-4">
        <h1>Appointments</h1>
        <Button variant="primary" onClick={handleCreate}>
          <FaPlus className="me-2" /> Schedule Appointment
        </Button>
      </div>

      {/* Alerts */}
      {error && <Alert variant="danger" onClose={() => setError('')} dismissible>{error}</Alert>}
      {success && <Alert variant="success" onClose={() => setSuccess('')} dismissible>{success}</Alert>}

      {/* Search Bar */}
      <Card className="mb-4">
        <Card.Body>
          <Form onSubmit={handleSearch}>
            <Row>
              <Col md={7}>
                <Form.Group>
                  <Form.Control
                    type="text"
                    placeholder="Search appointments by property, client, or notes..."
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                  />
                </Form.Group>
              </Col>
              <Col md={2}>
                <Button variant="primary" type="submit" className="w-100">
                  <FaSearch className="me-2" /> Search
                </Button>
              </Col>
              <Col md={3}>
                <ButtonGroup className="w-100 appointment-filter-group">
                  <Button variant="outline-secondary" onClick={() => handleQuickFilter('today')}>
                    <FaCalendarDay className="me-1" /> Today
                  </Button>
                  <Button variant="outline-secondary" onClick={() => handleQuickFilter('upcoming')}>
                    <FaClock className="me-1" /> Upcoming
                  </Button>
                </ButtonGroup>
              </Col>
            </Row>
          </Form>
        </Card.Body>
      </Card>

      {/* Appointments Table */}
      <Card>
        <Card.Body>
          {loading ? (
            <div className="text-center py-5">
              <div className="spinner-border" role="status">
                <span className="visually-hidden">Loading...</span>
              </div>
            </div>
          ) : appointments.length === 0 ? (
            <div className="text-center py-5 text-muted">
              <p>No appointments found</p>
            </div>
          ) : (
            <>
              <div className="table-responsive">
                <Table hover>
                  <thead className="table-light">
                    <tr>
                      <th>Date & Time</th>
                      <th>Type</th>
                      <th>Property</th>
                      <th>Client</th>
                      <th>Status</th>
                      <th>Actions</th>
                    </tr>
                  </thead>
                  <tbody>
                    {appointments.map((appointment) => (
                      <tr key={appointment.id}>
                        <td className="fw-bold">
                          {formatDateTime(appointment.appointmentDate, appointment.time)}
                        </td>
                        <td>{getTypeBadge(appointment.type)}</td>
                        <td>{getPropertyName(appointment.propertyId)}</td>
                        <td>{getClientName(appointment.clientId)}</td>
                        <td>{getStatusBadge(appointment.status)}</td>
                        <td>
                          <Button
                            variant="outline-primary"
                            size="sm"
                            className="me-2"
                            onClick={() => handleEdit(appointment)}
                          >
                            <FaEdit /> Edit
                          </Button>
                          {appointment.status === 'Scheduled' && (
                            <>
                              <Button
                                variant="outline-success"
                                size="sm"
                                className="me-2"
                                onClick={() => handleStatusChange(appointment.id, 'Completed')}
                              >
                                <FaCheck /> Complete
                              </Button>
                              <Button
                                variant="outline-warning"
                                size="sm"
                                className="me-2"
                                onClick={() => handleStatusChange(appointment.id, 'Cancelled')}
                              >
                                <FaTimes /> Cancel
                              </Button>
                            </>
                          )}
                          <Form.Select
                            size="sm"
                            className="d-inline-block me-2"
                            style={{ width: 'auto' }}
                            onChange={(e) => {
                              if (e.target.value) {
                                handleStatusChange(appointment.id, e.target.value);
                                e.target.value = '';
                              }
                            }}
                          >
                            <option value="">Change Status</option>
                            <option value="Scheduled">Scheduled</option>
                            <option value="Completed">Completed</option>
                            <option value="Cancelled">Cancelled</option>
                            <option value="NoShow">No Show</option>
                          </Form.Select>
                          <Button
                            variant="outline-danger"
                            size="sm"
                            onClick={() => handleDelete(appointment.id)}
                          >
                            <FaTrash /> Delete
                          </Button>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </Table>
              </div>

              {/* Pagination */}
              {totalPages > 1 && (
                <nav>
                  <Pagination>
                    <Pagination.First
                      onClick={() => setPage(1)}
                      disabled={page === 1}
                    />
                    <Pagination.Prev
                      onClick={() => setPage(Math.max(1, page - 1))}
                      disabled={page === 1}
                    />
                    {[...Array(Math.min(totalPages, 5))].map((_, i) => {
                      const pageNum = page <= 3 ? i + 1 : page - 2 + i;
                      if (pageNum > totalPages) return null;
                      return (
                        <Pagination.Item
                          key={pageNum}
                          active={page === pageNum}
                          onClick={() => setPage(pageNum)}
                        >
                          {pageNum}
                        </Pagination.Item>
                      );
                    })}
                    <Pagination.Next
                      onClick={() => setPage(Math.min(totalPages, page + 1))}
                      disabled={page === totalPages}
                    />
                    <Pagination.Last
                      onClick={() => setPage(totalPages)}
                      disabled={page === totalPages}
                    />
                  </Pagination>
                </nav>
              )}
            </>
          )}
        </Card.Body>
      </Card>

      {/* Modal Form */}
      <FormModal
        show={showModal}
        title="Appointment"
        onClose={() => setShowModal(false)}
        onSubmit={handleSubmit}
        loading={submitting}
        isEditMode={!!editingAppointment}
        size="lg"
      >
        <Form>
          <Row>
            <Col md={6}>
              <Form.Group className="mb-3">
                <Form.Label>Date</Form.Label>
                <Form.Control
                  type="date"
                  name="appointmentDate"
                  value={formData.appointmentDate}
                  onChange={handleInputChange}
                  required
                />
              </Form.Group>
            </Col>
            <Col md={6}>
              <Form.Group className="mb-3">
                <Form.Label>Time</Form.Label>
                <Form.Control
                  type="time"
                  name="time"
                  value={formData.time}
                  onChange={handleInputChange}
                  required
                />
              </Form.Group>
            </Col>
          </Row>

          <Row>
            <Col md={6}>
              <Form.Group className="mb-3">
                <Form.Label>Type</Form.Label>
                <Form.Select
                  name="type"
                  value={formData.type}
                  onChange={handleInputChange}
                >
                  <option value="Showing">Showing</option>
                  <option value="Inspection">Inspection</option>
                </Form.Select>
              </Form.Group>
            </Col>
            <Col md={6}>
              <Form.Group className="mb-3">
                <Form.Label>Status</Form.Label>
                <Form.Select
                  name="status"
                  value={formData.status}
                  onChange={handleInputChange}
                >
                  <option value="Scheduled">Scheduled</option>
                  <option value="Completed">Completed</option>
                  <option value="Cancelled">Cancelled</option>
                  <option value="NoShow">No Show</option>
                </Form.Select>
              </Form.Group>
            </Col>
          </Row>

          <Row>
            <Col md={6}>
              <Form.Group className="mb-3">
                <Form.Label>Property</Form.Label>
                <Form.Select
                  name="propertyId"
                  value={formData.propertyId}
                  onChange={handleInputChange}
                  required
                >
                  <option value="">Select property</option>
                  {properties.map(property => (
                    <option key={property.id} value={property.id}>
                      {property.title}
                    </option>
                  ))}
                </Form.Select>
              </Form.Group>
            </Col>
            <Col md={6}>
              <Form.Group className="mb-3">
                <Form.Label>Client</Form.Label>
                <Form.Select
                  name="clientId"
                  value={formData.clientId}
                  onChange={handleInputChange}
                  required
                >
                  <option value="">Select client</option>
                  {clients.map(client => (
                    <option key={client.id} value={client.id}>
                      {client.firstName} {client.lastName}
                    </option>
                  ))}
                </Form.Select>
              </Form.Group>
            </Col>
          </Row>

          <Form.Group className="mb-3">
            <Form.Label>Notes</Form.Label>
            <Form.Control
              as="textarea"
              rows={3}
              name="notes"
              value={formData.notes}
              onChange={handleInputChange}
              placeholder="Additional notes..."
            />
          </Form.Group>
        </Form>
      </FormModal>
    </Container>
  );
}
