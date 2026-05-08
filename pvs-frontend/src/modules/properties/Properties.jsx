import { useState, useEffect } from 'react';
import { useUserRole } from '../../shared/RoleGuard';
import { Container, Row, Col, Button, Table, Badge, Card, Form, Alert, Pagination } from 'react-bootstrap';
import { FaPlus, FaEdit, FaTrash, FaSearch, FaEye, FaImage } from 'react-icons/fa';
import PropertiesService from './PropertiesService';
import BranchesService from '../branches/BranchesService';
import FormModal from '../../shared/FormModal';
import DetailsModal from '../../shared/DetailsModal';
import './Properties.css';

export default function Properties() {
  const apiOrigin = (import.meta.env.VITE_API_URL || 'http://localhost:5019/api').replace(/\/api\/?$/, '');
  const [properties, setProperties] = useState([]);
  const [branches, setBranches] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [showModal, setShowModal] = useState(false);
  const [showDetails, setShowDetails] = useState(false);
  const [selectedProperty, setSelectedProperty] = useState(null);
  const [editingProperty, setEditingProperty] = useState(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [submitting, setSubmitting] = useState(false);
  const [imageFile, setImageFile] = useState(null);
  const [imagePreview, setImagePreview] = useState('');

  const getBranchName = (branchId) => {
    const branch = branches.find(item => item.id === Number(branchId));
    return branch ? branch.name : 'Unassigned';
  };

  const userRole = useUserRole();
  const canManageProperties = ['Admin', 'Manager', 'Agent'].includes(userRole);

  // Form state
  const [formData, setFormData] = useState({
    title: '',
    description: '',
    price: '',
    bedrooms: '',
    bathrooms: '',
    squareFeet: '',
    city: '',
    state: '',
    zipCode: '',
    branchId: '',
    propertyType: 'House',
    status: 'Available'
  });

  useEffect(() => {
    fetchProperties();
    fetchBranches();
  }, [page]);

  const fetchBranches = async () => {
    try {
      const response = await BranchesService.getAll();
      setBranches(response.data || []);
    } catch (err) {
      console.error('Error loading branches:', err);
    }
  };

  const fetchProperties = async () => {
    setLoading(true);
    setError('');

    try {
      const response = await PropertiesService.getAll(page, 10);
      setProperties(response.data || []);
      setTotalPages(response.totalPages || 1);
    } catch (err) {
      setError(err.message || 'Failed to load properties');
      console.error('Error fetching properties:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = async (e) => {
    e.preventDefault();
    if (!searchTerm.trim()) {
      setPage(1);
      fetchProperties();
      return;
    }

    setLoading(true);
    try {
      const response = await PropertiesService.search(searchTerm);
      setProperties(response.data || []);
      setTotalPages(1);
      setPage(1);
    } catch (err) {
      setError(err.message || 'Search failed');
    } finally {
      setLoading(false);
    }
  };

  const getCurrentUserId = () => parseInt(localStorage.getItem('userId')) || 0;

  const handleCreate = () => {
    setEditingProperty(null);
    setImageFile(null);
    setImagePreview('');
    setFormData({
      title: '',
      description: '',
      price: '',
      bedrooms: '',
      bathrooms: '',
      squareFeet: '',
      city: '',
      state: '',
      zipCode: '',
      branchId: '',
      propertyType: 'House',
      status: 'Available'
    });
    setShowModal(true);
  };

  const handleEdit = (property) => {
    setEditingProperty(property);
    setImageFile(null);
    setImagePreview(getImageUrl(property.imagePath));
    setFormData({
      title: property.title || '',
      description: property.description || '',
      price: property.price || '',
      bedrooms: property.bedrooms || '',
      bathrooms: property.bathrooms || '',
      squareFeet: property.squareFeet || '',
      city: property.address?.city || '',
      state: property.address?.state || '',
      zipCode: property.address?.zipCode || '',
      branchId: property.branchId || '',
      propertyType: property.propertyType || 'House',
      status: property.status || 'Available'
    });
    setShowModal(true);
  };

  const handleViewDetails = (property) => {
    setSelectedProperty(property);
    setShowDetails(true);
  };

  const handleCloseDetails = () => {
    setShowDetails(false);
    setSelectedProperty(null);
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Are you sure you want to delete this property?')) return;

    try {
      await PropertiesService.delete(id);
      setSuccess('Property deleted successfully');
      setProperties(properties.filter(p => p.id !== id));
      setTimeout(() => setSuccess(''), 3000);
    } catch (err) {
      setError(err.message || 'Failed to delete property');
    }
  };

  const handleSubmit = async () => {
    setError('');
    setSuccess('');
    setSubmitting(true);

    try {
      const data = {
        title: formData.title,
        description: formData.description,
        branchId: parseInt(formData.branchId, 10) || 0,
        propertyType: formData.propertyType,
        city: formData.city,
        state: formData.state,
        zipCode: formData.zipCode,
        price: parseFloat(formData.price),
        bedrooms: parseInt(formData.bedrooms, 10),
        bathrooms: parseFloat(formData.bathrooms),
        squareFeet: parseFloat(formData.squareFeet)
      };

      let savedPropertyId = editingProperty?.id;

      if (editingProperty) {
        const response = await PropertiesService.update(editingProperty.id, data);
        savedPropertyId = response.data?.id || editingProperty.id;
        setSuccess('Property updated successfully');
      } else {
        const response = await PropertiesService.create(data);
        savedPropertyId = response.data?.id;
        setSuccess('Property created successfully');
      }

      if (imageFile && savedPropertyId) {
        await PropertiesService.uploadImage(savedPropertyId, imageFile);
      }

      fetchProperties();
      setShowModal(false);
      setTimeout(() => setSuccess(''), 3000);
    } catch (err) {
      setError(err.message || 'Failed to save property');
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

  const getImageUrl = (imagePath) => {
    if (!imagePath) return '';
    if (imagePath.startsWith('http')) return imagePath;
    return `${apiOrigin}${imagePath}`;
  };

  const handleImageChange = (e) => {
    const file = e.target.files?.[0];
    setImageFile(file || null);
    setImagePreview(file ? URL.createObjectURL(file) : getImageUrl(editingProperty?.imagePath));
  };

  const getStatusBadge = (status) => {
    const variants = {
      'Available': 'success',
      'Sold': 'danger',
      'Pending': 'warning',
      'Off Market': 'secondary'
    };
    return <Badge bg={variants[status] || 'info'}>{status}</Badge>;
  };

  return (
    <Container fluid className="pt-4 pb-4">
      {/* Page Header */}
      <div className="page-header d-flex justify-content-between align-items-center mb-4">
        <h1>Properties</h1>
        {canManageProperties && (
          <Button variant="primary" onClick={handleCreate}>
            <FaPlus className="me-2" /> Add Property
          </Button>
        )}
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
                    placeholder="Search properties by title or city..."
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

      {/* Properties Table */}
      <Card>
        <Card.Body>
          {loading ? (
            <div className="text-center py-5">
              <div className="spinner-border" role="status">
                <span className="visually-hidden">Loading...</span>
              </div>
            </div>
          ) : properties.length === 0 ? (
            <div className="text-center py-5 text-muted">
              <p>No properties found</p>
            </div>
          ) : (
            <>
              <div className="table-responsive">
                <Table hover>
                  <thead className="table-light">
                    <tr>
                      <th>Photo</th>
                      <th>Title</th>
                      <th>Location</th>
                      <th>Price</th>
                      <th>Beds/Baths</th>
                      <th>Status</th>
                      <th>Actions</th>
                    </tr>
                  </thead>
                  <tbody>
                    {properties.map((property) => (
                      <tr key={property.id}>
                        <td>
                          {property.imagePath ? (
                            <img
                              src={getImageUrl(property.imagePath)}
                              alt={property.title}
                              className="property-table-image"
                            />
                          ) : (
                            <div className="property-image-placeholder">
                              <FaImage />
                            </div>
                          )}
                        </td>
                        <td className="fw-bold">{property.title}</td>
                        <td>{property.address?.city}, {property.address?.state} {property.address?.zipCode}</td>
                        <td className="text-success fw-bold">${property.price?.toLocaleString()}</td>
                        <td>{property.bedrooms} / {property.bathrooms} / {property.squareFeet}</td>
                        <td>{getStatusBadge(property.status)}</td>
                        <td>
                          <Button
                            variant="outline-secondary"
                            size="sm"
                            className="me-2"
                            onClick={() => handleViewDetails(property)}
                          >
                            <FaEye />
                          </Button>
                          {canManageProperties && (
                            <>
                              <Button
                                variant="outline-primary"
                                size="sm"
                                className="me-2"
                                onClick={() => handleEdit(property)}
                              >
                                <FaEdit /> Edit
                              </Button>
                              <Button
                                variant="outline-danger"
                                size="sm"
                                onClick={() => handleDelete(property.id)}
                              >
                                <FaTrash /> Delete
                              </Button>
                            </>
                          )}
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
        title="Property"
        onClose={() => setShowModal(false)}
        onSubmit={handleSubmit}
        loading={submitting}
        isEditMode={!!editingProperty}
        size="lg"
      >
        <Form>
          <Form.Group className="mb-3">
            <Form.Label>Title</Form.Label>
            <Form.Control
              type="text"
              name="title"
              value={formData.title}
              onChange={handleInputChange}
              required
            />
          </Form.Group>

          <Form.Group className="mb-3">
            <Form.Label>Description</Form.Label>
            <Form.Control
              as="textarea"
              rows={3}
              name="description"
              value={formData.description}
              onChange={handleInputChange}
            />
          </Form.Group>

          <Row>
            <Col md={6}>
              <Form.Group className="mb-3">
                <Form.Label>Price</Form.Label>
                <Form.Control
                  type="number"
                  step="0.01"
                  name="price"
                  value={formData.price}
                  onChange={handleInputChange}
                  required
                />
              </Form.Group>
            </Col>
            <Col md={6}>
              <Form.Group className="mb-3">
                <Form.Label>Property Type</Form.Label>
                <Form.Select
                  name="propertyType"
                  value={formData.propertyType}
                  onChange={handleInputChange}
                >
                  <option value="House">House</option>
                  <option value="Apartment">Apartment</option>
                  <option value="Condo">Condo</option>
                  <option value="Townhouse">Townhouse</option>
                  <option value="Land">Land</option>
                </Form.Select>
              </Form.Group>
            </Col>
          </Row>

          <Row>
            <Col md={4}>
              <Form.Group className="mb-3">
                <Form.Label>Bedrooms</Form.Label>
                <Form.Control
                  type="number"
                  name="bedrooms"
                  value={formData.bedrooms}
                  onChange={handleInputChange}
                  required
                />
              </Form.Group>
            </Col>
            <Col md={4}>
              <Form.Group className="mb-3">
                <Form.Label>Bathrooms</Form.Label>
                <Form.Control
                  type="number"
                  step="0.5"
                  name="bathrooms"
                  value={formData.bathrooms}
                  onChange={handleInputChange}
                  required
                />
              </Form.Group>
            </Col>
            <Col md={4}>
              <Form.Group className="mb-3">
                <Form.Label>Square Feet</Form.Label>
                <Form.Control
                  type="number"
                  name="squareFeet"
                  value={formData.squareFeet}
                  onChange={handleInputChange}
                />
              </Form.Group>
            </Col>
          </Row>

          <Row>
            <Col md={6}>
              <Form.Group className="mb-3">
                <Form.Label>Branch</Form.Label>
                <Form.Select
                  name="branchId"
                  value={formData.branchId}
                  onChange={handleInputChange}
                  required
                >
                  <option value="">Select branch</option>
                  {branches.map(branch => (
                    <option key={branch.id} value={branch.id}>{branch.name}</option>
                  ))}
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
                  <option value="Available">Available</option>
                  <option value="Sold">Sold</option>
                  <option value="Pending">Pending</option>
                  <option value="Off Market">Off Market</option>
                </Form.Select>
              </Form.Group>
            </Col>
          </Row>

          <Row>
            <Col md={4}>
              <Form.Group className="mb-3">
                <Form.Label>City</Form.Label>
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
                <Form.Label>State</Form.Label>
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
                <Form.Label>ZIP Code</Form.Label>
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

          <Form.Group className="mb-3">
            <Form.Label>Property Image</Form.Label>
            <div className="property-upload-row">
              <Form.Control
                type="file"
                accept="image/*"
                onChange={handleImageChange}
              />
              {imagePreview && (
                <img
                  src={imagePreview}
                  alt="Property preview"
                  className="property-upload-preview"
                />
              )}
            </div>
          </Form.Group>
        </Form>
      </FormModal>

      <DetailsModal
        show={showDetails}
        title="Property Details"
        onClose={handleCloseDetails}
        footer={null}
        size="lg"
      >
        {selectedProperty ? (
          <div>
            {selectedProperty.imagePath && (
              <img
                src={getImageUrl(selectedProperty.imagePath)}
                alt={selectedProperty.title}
                className="property-details-image"
              />
            )}
            <h5>{selectedProperty.title}</h5>
            <p className="text-muted mb-3">{selectedProperty.description}</p>
            <div className="row">
              <div className="col-md-6 mb-3">
                <strong>Price:</strong> ${selectedProperty.price?.toLocaleString()}
              </div>
              <div className="col-md-6 mb-3">
                <strong>Status:</strong> {getStatusBadge(selectedProperty.status)}
              </div>
              <div className="col-md-6 mb-3">
                <strong>Property Type:</strong> {selectedProperty.propertyType}
              </div>
              <div className="col-md-6 mb-3">
                <strong>Branch:</strong> {getBranchName(selectedProperty.branchId)}
              </div>
              <div className="col-md-6 mb-3">
                <strong>Bedrooms:</strong> {selectedProperty.bedrooms}
              </div>
              <div className="col-md-6 mb-3">
                <strong>Bathrooms:</strong> {selectedProperty.bathrooms}
              </div>
              <div className="col-md-6 mb-3">
                <strong>Square Feet:</strong> {selectedProperty.squareFeet}
              </div>
              <div className="col-md-6 mb-3">
                <strong>Created At:</strong> {new Date(selectedProperty.createdAt).toLocaleDateString()}
              </div>
              <div className="col-md-12 mb-3">
                <strong>Location:</strong> {selectedProperty.address?.city}, {selectedProperty.address?.state} {selectedProperty.address?.zipCode}
              </div>
            </div>
          </div>
        ) : (
          <p>No details available.</p>
        )}
      </DetailsModal>
    </Container>
  );
}
