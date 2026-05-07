import { useState, useEffect } from 'react';
import { Container, Row, Col, Card, Button, Table, Modal, Form, Alert, Badge } from 'react-bootstrap';
import { FaPlus, FaEdit, FaTrash, FaEye, FaBuilding } from 'react-icons/fa';
import BranchesService from './BranchesService';
import './Branches.css';

export default function Branches() {
  const [branches, setBranches] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [showModal, setShowModal] = useState(false);
  const [editingBranch, setEditingBranch] = useState(null);
  const [formData, setFormData] = useState({
    name: '',
    description: '',
    city: '',
    state: '',
    zipCode: '',
    phone: '',
    email: '',
    managerName: '',
    managerUserId: null
  });

  useEffect(() => {
    fetchBranches();
  }, []);

  const fetchBranches = async () => {
    setLoading(true);
    setError('');

    try {
      const response = await BranchesService.getAll();
      setBranches(response.data || []);
    } catch (err) {
      setError(err.message || 'Failed to load branches');
      console.error('Error fetching branches:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleShowModal = (branch = null) => {
    setEditingBranch(branch);
    if (branch) {
      setFormData({
        name: branch.name || '',
        description: branch.description || '',
        city: branch.address?.city || '',
        state: branch.address?.state || '',
        zipCode: branch.address?.zipCode || '',
        phone: branch.phone || '',
        email: branch.email || '',
        managerName: branch.managerName || '',
        managerUserId: branch.managerUserId
      });
    } else {
      setFormData({
        name: '',
        description: '',
        city: '',
        state: '',
        zipCode: '',
        phone: '',
        email: '',
        managerName: '',
        managerUserId: null
      });
    }
    setShowModal(true);
  };

  const handleCloseModal = () => {
    setShowModal(false);
    setEditingBranch(null);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');

    const branchData = {
      name: formData.name,
      description: formData.description,
      address: {
        city: formData.city,
        state: formData.state,
        zipCode: formData.zipCode
      },
      phone: formData.phone,
      email: formData.email,
      managerName: formData.managerName,
      managerUserId: formData.managerUserId
    };

    try {
      if (editingBranch) {
        await BranchesService.update(editingBranch.id, branchData);
      } else {
        await BranchesService.create(branchData);
      }
      await fetchBranches();
      handleCloseModal();
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to save branch');
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Are you sure you want to delete this branch?')) return;

    try {
      await BranchesService.delete(id);
      await fetchBranches();
    } catch (err) {
      setError(err.message || 'Failed to delete branch');
    }
  };

  const handleInputChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value
    });
  };

  if (loading) {
    return (
      <Container className="mt-4">
        <div className="text-center">
          <div className="spinner-border" role="status">
            <span className="visually-hidden">Loading...</span>
          </div>
        </div>
      </Container>
    );
  }

  return (
    <Container className="mt-4">
      <Row className="mb-4">
        <Col>
          <div className="d-flex justify-content-between align-items-center">
            <h2><FaBuilding className="me-2" />Branches</h2>
            <Button variant="primary" onClick={() => handleShowModal()}>
              <FaPlus className="me-2" />Add Branch
            </Button>
          </div>
        </Col>
      </Row>

      {error && <Alert variant="danger">{error}</Alert>}

      <Row>
        <Col>
          <Card>
            <Card.Body>
              <Table responsive hover>
                <thead>
                  <tr>
                    <th>Name</th>
                    <th>Location</th>
                    <th>Manager</th>
                    <th>Status</th>
                    <th>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {branches.map((branch) => (
                    <tr key={branch.id}>
                      <td>
                        <strong>{branch.name}</strong>
                        <br />
                        <small className="text-muted">{branch.description}</small>
                      </td>
                      <td>
                        {branch.address?.city}, {branch.address?.state}
                        <br />
                        <small className="text-muted">{branch.address?.zipCode}</small>
                      </td>
                      <td>{branch.managerName}</td>
                      <td>
                        <Badge bg={branch.status === 'Active' ? 'success' : 'secondary'}>
                          {branch.status}
                        </Badge>
                      </td>
                      <td>
                        <Button
                          variant="outline-primary"
                          size="sm"
                          className="me-2"
                          onClick={() => handleShowModal(branch)}
                        >
                          <FaEdit />
                        </Button>
                        <Button
                          variant="outline-danger"
                          size="sm"
                          onClick={() => handleDelete(branch.id)}
                        >
                          <FaTrash />
                        </Button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </Table>

              {branches.length === 0 && (
                <div className="text-center py-4">
                  <FaBuilding size={48} className="text-muted mb-3" />
                  <p className="text-muted">No branches found. Create your first branch to get started.</p>
                </div>
              )}
            </Card.Body>
          </Card>
        </Col>
      </Row>

      {/* Modal for Create/Edit */}
      <Modal show={showModal} onHide={handleCloseModal} size="lg">
        <Modal.Header closeButton>
          <Modal.Title>
            {editingBranch ? 'Edit Branch' : 'Add New Branch'}
          </Modal.Title>
        </Modal.Header>
        <Form onSubmit={handleSubmit}>
          <Modal.Body>
            <Row>
              <Col md={6}>
                <Form.Group className="mb-3">
                  <Form.Label>Branch Name *</Form.Label>
                  <Form.Control
                    type="text"
                    name="name"
                    value={formData.name}
                    onChange={handleInputChange}
                    required
                  />
                </Form.Group>
              </Col>
              <Col md={6}>
                <Form.Group className="mb-3">
                  <Form.Label>Manager Name</Form.Label>
                  <Form.Control
                    type="text"
                    name="managerName"
                    value={formData.managerName}
                    onChange={handleInputChange}
                  />
                </Form.Group>
              </Col>
            </Row>

            <Form.Group className="mb-3">
              <Form.Label>Description</Form.Label>
              <Form.Control
                as="textarea"
                rows={2}
                name="description"
                value={formData.description}
                onChange={handleInputChange}
              />
            </Form.Group>

            <Row>
              <Col md={4}>
                <Form.Group className="mb-3">
                  <Form.Label>City *</Form.Label>
                  <Form.Control
                    type="text"
                    name="city"
                    value={formData.city}
                    onChange={handleInputChange}
                    required
                  />
                </Form.Group>
              </Col>
              <Col md={4}>
                <Form.Group className="mb-3">
                  <Form.Label>State *</Form.Label>
                  <Form.Control
                    type="text"
                    name="state"
                    value={formData.state}
                    onChange={handleInputChange}
                    required
                  />
                </Form.Group>
              </Col>
              <Col md={4}>
                <Form.Group className="mb-3">
                  <Form.Label>ZIP Code *</Form.Label>
                  <Form.Control
                    type="text"
                    name="zipCode"
                    value={formData.zipCode}
                    onChange={handleInputChange}
                    required
                  />
                </Form.Group>
              </Col>
            </Row>

            <Row>
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
              <Col md={6}>
                <Form.Group className="mb-3">
                  <Form.Label>Email</Form.Label>
                  <Form.Control
                    type="email"
                    name="email"
                    value={formData.email}
                    onChange={handleInputChange}
                  />
                </Form.Group>
              </Col>
            </Row>
          </Modal.Body>
          <Modal.Footer>
            <Button variant="secondary" onClick={handleCloseModal}>
              Cancel
            </Button>
            <Button variant="primary" type="submit">
              {editingBranch ? 'Update Branch' : 'Create Branch'}
            </Button>
          </Modal.Footer>
        </Form>
      </Modal>
    </Container>
  );
}