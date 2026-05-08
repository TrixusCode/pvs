import { useState, useEffect } from 'react';
import { useUserRole } from '../../shared/RoleGuard';
import { Container, Row, Col, Button, Table, Badge, Card, Form, Alert, Pagination } from 'react-bootstrap';
import { FaPlus, FaEdit, FaTrash, FaSearch, FaCheck, FaTimes, FaUndo, FaEye } from 'react-icons/fa';
import OffersService from './OffersService';
import ClientsService from '../clients/ClientsService';
import PropertiesService from '../properties/PropertiesService';
import FormModal from '../../shared/FormModal';
import DetailsModal from '../../shared/DetailsModal';
import './Offers.css';

export default function Offers() {
  const [offers, setOffers] = useState([]);
  const [clients, setClients] = useState([]);
  const [properties, setProperties] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [showModal, setShowModal] = useState(false);
  const [showDetails, setShowDetails] = useState(false);
  const [selectedOffer, setSelectedOffer] = useState(null);
  const [editingOffer, setEditingOffer] = useState(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [submitting, setSubmitting] = useState(false);

  const offerTypeOptions = [
    { value: 'FullPrice', label: 'Full Price' },
    { value: 'Contingent', label: 'Contingent' },
    { value: 'AsIs', label: 'As-Is' }
  ];

  const contingencyOptions = [
    { value: 'None', label: 'None' },
    { value: 'HomeInspection', label: 'Home Inspection' },
    { value: 'Appraisal', label: 'Appraisal' },
    { value: 'Financing', label: 'Financing' }
  ];

  const userRole = useUserRole();
  const canManageOffers = ['Admin', 'Manager', 'Agent'].includes(userRole);
  const canCreateOffers = ['Admin', 'Manager', 'Agent', 'Client_Buyer', 'Client_Seller'].includes(userRole);

  // Form state
  const [formData, setFormData] = useState({
    propertyId: '',
    clientId: '',
    offeredPrice: '',
    offerType: 'FullPrice',
    expirationDate: '',
    agentNotes: '',
    downPaymentPercent: '',
    closingDaysRequested: '',
    contingencies: 'None'
  });

  useEffect(() => {
    fetchOffers();
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
      console.error('Error loading offer lookup data:', err);
    }
  };

  const getPropertyName = (propertyId) => {
    const property = properties.find(item => item.id === Number(propertyId));
    return property ? property.title : 'Unknown property';
  };

  const getClientName = (clientId) => {
    const client = clients.find(item => item.id === Number(clientId));
    return client ? `${client.firstName} ${client.lastName}` : 'Unknown client';
  };

  const fetchOffers = async () => {
    setLoading(true);
    setError('');

    try {
      const response = await OffersService.getAll(page, 10);
      setOffers(response.data || []);
      setTotalPages(response.totalPages || 1);
    } catch (err) {
      setError(err.message || 'Failed to load offers');
      console.error('Error fetching offers:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = async (e) => {
    e.preventDefault();
    if (!searchTerm.trim()) {
      setPage(1);
      fetchOffers();
      return;
    }

    setLoading(true);
    try {
      const response = await OffersService.search(searchTerm);
      setOffers(response.data || []);
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
    setEditingOffer(null);
    setFormData({
      propertyId: '',
      clientId: '',
      offeredPrice: '',
      offerType: 'FullPrice',
      expirationDate: '',
      agentNotes: '',
      downPaymentPercent: '',
      closingDaysRequested: '',
      contingencies: 'None'
    });
    setShowModal(true);
  };

  const handleEdit = (offer) => {
    setEditingOffer(offer);
    setFormData({
      propertyId: offer.propertyId || '',
      clientId: offer.clientId || '',
      offeredPrice: offer.offeredPrice || '',
      offerType: offer.offerType || 'FullPrice',
      expirationDate: offer.expirationDate ? new Date(offer.expirationDate).toISOString().split('T')[0] : '',
      agentNotes: offer.agentNotes || '',
      downPaymentPercent: offer.downPaymentPercent || '',
      closingDaysRequested: offer.closingDaysRequested || '',
      contingencies: offer.contingencies || 'None'
    });
    setShowModal(true);
  };

  const handleViewDetails = (offer) => {
    setSelectedOffer(offer);
    setShowDetails(true);
  };

  const handleCloseDetails = () => {
    setShowDetails(false);
    setSelectedOffer(null);
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Are you sure you want to delete this offer?')) return;

    try {
      await OffersService.delete(id);
      setSuccess('Offer deleted successfully');
      setOffers(offers.filter(o => o.id !== id));
      setTimeout(() => setSuccess(''), 3000);
    } catch (err) {
      setError(err.message || 'Failed to delete offer');
    }
  };

  const handleStatusChange = async (id, action) => {
    try {
      if (action === 'accept') {
        await OffersService.accept(id);
        setSuccess('Offer accepted successfully');
      } else if (action === 'reject') {
        await OffersService.reject(id);
        setSuccess('Offer rejected successfully');
      } else if (action === 'withdraw') {
        await OffersService.withdraw(id);
        setSuccess('Offer withdrawn successfully');
      }

      fetchOffers();
      setTimeout(() => setSuccess(''), 3000);
    } catch (err) {
      setError(err.message || 'Failed to update offer status');
    }
  };

  const handleSubmit = async () => {
    setError('');
    setSuccess('');
    setSubmitting(true);

    try {
      const data = {
        propertyId: parseInt(formData.propertyId, 10),
        clientId: parseInt(formData.clientId, 10),
        offeredPrice: parseFloat(formData.offeredPrice),
        offerType: formData.offerType,
        expirationDate: formData.expirationDate ? new Date(formData.expirationDate).toISOString() : null,
        agentNotes: formData.agentNotes,
        downPaymentPercent: formData.downPaymentPercent ? parseFloat(formData.downPaymentPercent) : null,
        closingDaysRequested: formData.closingDaysRequested ? parseInt(formData.closingDaysRequested, 10) : null,
        contingencies: formData.contingencies || 'None'
      };

      if (editingOffer) {
        await OffersService.update(editingOffer.id, data);
        setSuccess('Offer updated successfully');
      } else {
        await OffersService.create(data);
        setSuccess('Offer created successfully');
      }

      fetchOffers();
      setShowModal(false);
      setTimeout(() => setSuccess(''), 3000);
    } catch (err) {
      setError(err.message || 'Failed to save offer');
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
      'Pending': 'warning',
      'Accepted': 'success',
      'Rejected': 'danger',
      'Withdrawn': 'secondary',
      'Expired': 'secondary'
    };
    return <Badge bg={variants[status] || 'info'}>{status}</Badge>;
  };

  const getOfferTypeBadge = (offerType) => {
    const variants = {
      'FullPrice': 'primary',
      'Contingent': 'warning',
      'AsIs': 'secondary'
    };
    const labels = {
      'FullPrice': 'Full Price',
      'Contingent': 'Contingent',
      'AsIs': 'As-Is'
    };
    return <Badge bg={variants[offerType] || 'secondary'}>{labels[offerType] || offerType}</Badge>;
  };

  const getContingencyLabel = (contingency) => (
    contingencyOptions.find(option => option.value === contingency)?.label || contingency || 'None'
  );

  const formatExpirationDate = (date) => {
    if (!date) return 'No expiration';
    return new Date(date).toLocaleDateString();
  };

  return (
    <Container fluid className="pt-4 pb-4">
      {/* Page Header */}
      <div className="page-header d-flex justify-content-between align-items-center mb-4">
        <h1>Offers</h1>
        {canCreateOffers && (
          <Button variant="primary" onClick={handleCreate}>
            <FaPlus className="me-2" /> Create Offer
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
                    placeholder="Search offers by property, client, or offer amount..."
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

      {/* Offers Table */}
      <Card>
        <Card.Body>
          {loading ? (
            <div className="text-center py-5">
              <div className="spinner-border" role="status">
                <span className="visually-hidden">Loading...</span>
              </div>
            </div>
          ) : offers.length === 0 ? (
            <div className="text-center py-5 text-muted">
              <p>No offers found</p>
            </div>
          ) : (
            <>
              <div className="table-responsive">
                <Table hover>
                  <thead className="table-light">
                    <tr>
                      <th>Offer Amount</th>
                      <th>Type</th>
                      <th>Property</th>
                      <th>Client</th>
                      <th>Status</th>
                      <th>Expiration</th>
                      <th>Actions</th>
                    </tr>
                  </thead>
                  <tbody>
                    {offers.map((offer) => (
                      <tr key={offer.id}>
                        <td className="fw-bold text-success">${offer.offeredPrice?.toLocaleString()}</td>
                        <td>{getOfferTypeBadge(offer.offerType)}</td>
                        <td>{getPropertyName(offer.propertyId)}</td>
                        <td>{getClientName(offer.clientId)}</td>
                        <td>{getStatusBadge(offer.status)}</td>
                        <td>{formatExpirationDate(offer.expirationDate)}</td>
                        <td>
                          <Button
                            variant="outline-secondary"
                            size="sm"
                            className="me-1"
                            onClick={() => handleViewDetails(offer)}
                          >
                            <FaEye />
                          </Button>
                          {canManageOffers && (
                            <>
                              <Button
                                variant="outline-primary"
                                size="sm"
                                className="me-1"
                                onClick={() => handleEdit(offer)}
                              >
                                <FaEdit /> Edit
                              </Button>
                              {offer.status === 'Pending' && (
                                <>
                                  <Button
                                    variant="outline-success"
                                    size="sm"
                                    className="me-1"
                                    onClick={() => handleStatusChange(offer.id, 'accept')}
                                  >
                                    <FaCheck /> Accept
                                  </Button>
                                  <Button
                                    variant="outline-danger"
                                    size="sm"
                                    className="me-1"
                                    onClick={() => handleStatusChange(offer.id, 'reject')}
                                  >
                                    <FaTimes /> Reject
                                  </Button>
                                </>
                              )}
                              <Button
                                variant="outline-danger"
                                size="sm"
                                onClick={() => handleDelete(offer.id)}
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
        title="Offer"
        onClose={() => setShowModal(false)}
        onSubmit={handleSubmit}
        loading={submitting}
        isEditMode={!!editingOffer}
        size="lg"
      >
        <Form>
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
                    <option key={property.id} value={property.id}>{property.title}</option>
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

          <Row>
            <Col md={6}>
              <Form.Group className="mb-3">
                <Form.Label>Offered Price</Form.Label>
                <Form.Control
                  type="number"
                  step="0.01"
                  name="offeredPrice"
                  value={formData.offeredPrice}
                  onChange={handleInputChange}
                  placeholder="Enter offer amount"
                  required
                />
              </Form.Group>
            </Col>
            <Col md={6}>
              <Form.Group className="mb-3">
                <Form.Label>Offer Type</Form.Label>
                <Form.Select
                  name="offerType"
                  value={formData.offerType}
                  onChange={handleInputChange}
                >
                  {offerTypeOptions.map(option => (
                    <option key={option.value} value={option.value}>{option.label}</option>
                  ))}
                </Form.Select>
              </Form.Group>
            </Col>
          </Row>

          <Form.Group className="mb-3">
            <Form.Label>Expiration Date</Form.Label>
            <Form.Control
              type="date"
              name="expirationDate"
              value={formData.expirationDate}
              onChange={handleInputChange}
            />
          </Form.Group>

          <Row>
            <Col md={4}>
              <Form.Group className="mb-3">
                <Form.Label>Down Payment %</Form.Label>
                <Form.Control
                  type="number"
                  step="0.01"
                  name="downPaymentPercent"
                  value={formData.downPaymentPercent}
                  onChange={handleInputChange}
                  placeholder="e.g. 20"
                />
              </Form.Group>
            </Col>
            <Col md={4}>
              <Form.Group className="mb-3">
                <Form.Label>Closing Days</Form.Label>
                <Form.Control
                  type="number"
                  name="closingDaysRequested"
                  value={formData.closingDaysRequested}
                  onChange={handleInputChange}
                  placeholder="e.g. 30"
                />
              </Form.Group>
            </Col>
            <Col md={4}>
              <Form.Group className="mb-3">
                <Form.Label>Contingencies</Form.Label>
                <Form.Select
                  name="contingencies"
                  value={formData.contingencies}
                  onChange={handleInputChange}
                >
                  {contingencyOptions.map(option => (
                    <option key={option.value} value={option.value}>{option.label}</option>
                  ))}
                </Form.Select>
              </Form.Group>
            </Col>
          </Row>

          <Form.Group className="mb-3">
            <Form.Label>Agent Notes</Form.Label>
            <Form.Control
              as="textarea"
              rows={3}
              name="agentNotes"
              value={formData.agentNotes}
              onChange={handleInputChange}
              placeholder="Notes from the agent..."
            />
          </Form.Group>
        </Form>
      </FormModal>

      <DetailsModal
        show={showDetails}
        title="Offer Details"
        onClose={handleCloseDetails}
        footer={null}
      >
        {selectedOffer ? (
          <div>
            <h5>{getPropertyName(selectedOffer.propertyId)}</h5>
            <p className="text-muted mb-3">Client: {getClientName(selectedOffer.clientId)}</p>
            <div className="row">
              <div className="col-md-6 mb-3">
                <strong>Offer Amount:</strong> ${selectedOffer.offeredPrice?.toLocaleString()}
              </div>
              <div className="col-md-6 mb-3">
                <strong>Status:</strong> {getStatusBadge(selectedOffer.status)}
              </div>
              <div className="col-md-6 mb-3">
                <strong>Offer Type:</strong> {selectedOffer.offerType}
              </div>
              <div className="col-md-6 mb-3">
                <strong>Expiration:</strong> {formatExpirationDate(selectedOffer.expirationDate)}
              </div>
              <div className="col-md-12 mb-3">
                <strong>Agent Notes:</strong> {selectedOffer.agentNotes || 'None'}
              </div>
              <div className="col-md-4 mb-3">
                <strong>Down Payment %:</strong> {selectedOffer.downPaymentPercent ?? 'N/A'}
              </div>
              <div className="col-md-4 mb-3">
                <strong>Closing Days:</strong> {selectedOffer.closingDaysRequested ?? 'N/A'}
              </div>
              <div className="col-md-4 mb-3">
                <strong>Contingencies:</strong> {getContingencyLabel(selectedOffer.contingencies)}
              </div>
              <div className="col-md-12 mb-3">
                <strong>Created At:</strong> {selectedOffer.createdAt ? new Date(selectedOffer.createdAt).toLocaleDateString() : 'N/A'}
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
