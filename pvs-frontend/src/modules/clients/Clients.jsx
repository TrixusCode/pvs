import { useState, useEffect } from 'react';
import { Container, Row, Col, Button, Table, Badge, Card, Form, Alert, Pagination } from 'react-bootstrap';
import { FaPlus, FaEdit, FaTrash, FaSearch } from 'react-icons/fa';
import ClientsService from './ClientsService';
import FormModal from '../../shared/FormModal';
import './Clients.css';

export default function Clients() {
  const [clients, setClients] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [showModal, setShowModal] = useState(false);
  const [editingClient, setEditingClient] = useState(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [submitting, setSubmitting] = useState(false);

  // Form state
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    email: '',
    phone: '',
    clientType: 'Buyer',
    status: 'Active'
  });

  useEffect(() => {
    fetchClients();
  }, [page]);

  const fetchClients = async () => {
    setLoading(true);
    setError('');

    try {
      const response = await ClientsService.getAll(page, 10);
      setClients(response.data || []);
      setTotalPages(response.totalPages || 1);
    } catch (err) {
      setError(err.message || 'Failed to load clients');
      console.error('Error fetching clients:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = async (e) => {
    e.preventDefault();
    if (!searchTerm.trim()) {
      setPage(1);
      fetchClients();
      return;
    }

    setLoading(true);
    try {
      const response = await ClientsService.search(searchTerm);
      setClients(response.data || []);
      setTotalPages(1);
      setPage(1);
    } catch (err) {
      setError(err.message || 'Search failed');
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = () => {
    setEditingClient(null);
    setFormData({
      firstName: '',
      lastName: '',
      email: '',
      phone: '',
      clientType: 'Buyer',
      status: 'Active'
    });
    setShowModal(true);
  };

  const handleEdit = (client) => {
    setEditingClient(client);
    setFormData({
      firstName: client.firstName || '',
      lastName: client.lastName || '',
      email: client.email || '',
      phone: client.phone || '',
      clientType: client.clientType || 'Buyer',
      status: client.status || 'Active'
    });
    setShowModal(true);
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Are you sure you want to delete this client?')) return;

    try {
      await ClientsService.delete(id);
      setSuccess('Client deleted successfully');
      setClients(clients.filter(c => c.id !== id));
      setTimeout(() => setSuccess(''), 3000);
    } catch (err) {
      setError(err.message || 'Failed to delete client');
    }
  };

  const handleSubmit = async () => {
    setError('');
    setSuccess('');
    setSubmitting(true);

    try {
      if (editingClient) {
        await ClientsService.update(editingClient.id, formData);
        setSuccess('Client updated successfully');
        fetchClients();
      } else {
        await ClientsService.create(formData);
        setSuccess('Client created successfully');
        fetchClients();
      }

      setShowModal(false);
      setTimeout(() => setSuccess(''), 3000);
    } catch (err) {
      setError(err.message || 'Failed to save client');
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
      'Active': 'success',
      'Prospect': 'warning',
      'Inactive': 'secondary'
    };
    return <Badge bg={variants[status] || 'info'}>{status}</Badge>;
  };

  const getClientTypeBadge = (clientType) => {
    const variants = {
      'Buyer': 'primary',
      'Seller': 'info',
      'Both': 'success'
    };
    return <Badge bg={variants[clientType] || 'secondary'}>{clientType}</Badge>;
  };

  return (
    <Container fluid className="pt-4 pb-4">
      {/* Page Header */}
      <div className="page-header d-flex justify-content-between align-items-center mb-4">
        <h1>Clients</h1>
        <Button variant="primary" onClick={handleCreate}>
          <FaPlus className="me-2" /> Add Client
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
              <Col md={9}>
                <Form.Group>
                  <Form.Control
                    type="text"
                    placeholder="Search clients by name, email, or phone..."
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                  />
                </Form.Group>
              </Col>
              <Col md={3}>
                <Button variant="primary" type="submit" className="w-100">
                  <FaSearch className="me-2" /> Search
                </Button>
              </Col>
            </Row>
          </Form>
        </Card.Body>
      </Card>

      {/* Clients Table */}
      <Card>
        <Card.Body>
          {loading ? (
            <div className="text-center py-5">
              <div className="spinner-border" role="status">
                <span className="visually-hidden">Loading...</span>
              </div>
            </div>
          ) : clients.length === 0 ? (
            <div className="text-center py-5 text-muted">
              <p>No clients found</p>
            </div>
          ) : (
            <>
              <div className="table-responsive">
                <Table hover>
                  <thead className="table-light">
                    <tr>
                      <th>Name</th>
                      <th>Email</th>
                      <th>Phone</th>
                      <th>Type</th>
                      <th>Status</th>
                      <th>Actions</th>
                    </tr>
                  </thead>
                  <tbody>
                    {clients.map((client) => (
                      <tr key={client.id}>
                        <td className="fw-bold">{client.firstName} {client.lastName}</td>
                        <td>{client.email}</td>
                        <td>{client.phone}</td>
                        <td>{getClientTypeBadge(client.clientType)}</td>
                        <td>{getStatusBadge(client.status)}</td>
                        <td>
                          <Button
                            variant="outline-primary"
                            size="sm"
                            className="me-2"
                            onClick={() => handleEdit(client)}
                          >
                            <FaEdit /> Edit
                          </Button>
                          <Button
                            variant="outline-danger"
                            size="sm"
                            onClick={() => handleDelete(client.id)}
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
        title="Client"
        onClose={() => setShowModal(false)}
        onSubmit={handleSubmit}
        loading={submitting}
        isEditMode={!!editingClient}
      >
        <Form>
          <Row>
            <Col md={6}>
              <Form.Group className="mb-3">
                <Form.Label>First Name</Form.Label>
                <Form.Control
                  type="text"
                  name="firstName"
                  value={formData.firstName}
                  onChange={handleInputChange}
                  required
                />
              </Form.Group>
            </Col>
            <Col md={6}>
              <Form.Group className="mb-3">
                <Form.Label>Last Name</Form.Label>
                <Form.Control
                  type="text"
                  name="lastName"
                  value={formData.lastName}
                  onChange={handleInputChange}
                  required
                />
              </Form.Group>
            </Col>
          </Row>

          <Row>
            <Col md={6}>
              <Form.Group className="mb-3">
                <Form.Label>Email</Form.Label>
                <Form.Control
                  type="email"
                  name="email"
                  value={formData.email}
                  onChange={handleInputChange}
                  required
                />
              </Form.Group>
            </Col>
            <Col md={6}>
              <Form.Group className="mb-3">
                <Form.Label>Phone</Form.Label>
                <Form.Control
                  type="tel"
                  name="phone"
                  value={formData.phone}
                  onChange={handleInputChange}
                />
              </Form.Group>
            </Col>
          </Row>

          <Row>
            <Col md={6}>
              <Form.Group className="mb-3">
                <Form.Label>Client Type</Form.Label>
                <Form.Select
                  name="clientType"
                  value={formData.clientType}
                  onChange={handleInputChange}
                >
                  <option value="Buyer">Buyer</option>
                  <option value="Seller">Seller</option>
                  <option value="Both">Both</option>
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
                  <option value="Active">Active</option>
                  <option value="Prospect">Prospect</option>
                  <option value="Inactive">Inactive</option>
                </Form.Select>
              </Form.Group>
            </Col>
          </Row>
        </Form>
      </FormModal>
    </Container>
  );
}
