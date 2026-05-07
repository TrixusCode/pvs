import { useState, useEffect } from 'react';
import { Container, Row, Col, Button, Table, Badge, Card, Form, Alert, Pagination } from 'react-bootstrap';
import { FaPlus, FaEdit, FaTrash, FaSearch, FaCheck, FaTimes, FaUndo } from 'react-icons/fa';
import OffersService from './OffersService';
import FormModal from '../../shared/FormModal';
import './Offers.css';

export default function Offers() {
  const [offers, setOffers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [showModal, setShowModal] = useState(false);
  const [editingOffer, setEditingOffer] = useState(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [submitting, setSubmitting] = useState(false);

  // Form state
  const [formData, setFormData] = useState({
    propertyId: '',
    clientId: '',
    offeredPrice: '',
    offerType: 'FullPrice',
    expirationDate: '',
    conditions: ''
  });

  useEffect(() => {
    fetchOffers();
  }, [page]);

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

  const handleCreate = () => {
    setEditingOffer(null);
    setFormData({
      propertyId: '',
      clientId: '',
      offeredPrice: '',
      offerType: 'FullPrice',
      expirationDate: '',
      conditions: ''
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
      conditions: offer.conditions || ''
    });
    setShowModal(true);
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
        ...formData,
        propertyId: parseInt(formData.propertyId),
        clientId: parseInt(formData.clientId),
        offeredPrice: parseFloat(formData.offeredPrice)
      };

      if (editingOffer) {
        await OffersService.update(editingOffer.id, data);
        setSuccess('Offer updated successfully');
        fetchOffers();
      } else {
        await OffersService.create(data);
        setSuccess('Offer created successfully');
        fetchOffers();
      }

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

  const formatExpirationDate = (date) => {
    if (!date) return 'No expiration';
    return new Date(date).toLocaleDateString();
  };

  return (
    <Container fluid className="pt-4 pb-4">
      {/* Page Header */}
      <div className="page-header d-flex justify-content-between align-items-center mb-4">
        <h1>Offers</h1>
        <Button variant="primary" onClick={handleCreate}>
          <FaPlus className="me-2" /> Create Offer
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
                    placeholder="Search offers by property ID, client ID, or offer amount..."
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
                      <th>Property ID</th>
                      <th>Client ID</th>
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
                        <td>{offer.propertyId}</td>
                        <td>{offer.clientId}</td>
                        <td>{getStatusBadge(offer.status)}</td>
                        <td>{formatExpirationDate(offer.expirationDate)}</td>
                        <td>
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
                <Form.Label>Property ID</Form.Label>
                <Form.Control
                  type="number"
                  name="propertyId"
                  value={formData.propertyId}
                  onChange={handleInputChange}
                  placeholder="Enter property ID"
                  required
                />
              </Form.Group>
            </Col>
            <Col md={6}>
              <Form.Group className="mb-3">
                <Form.Label>Client ID</Form.Label>
                <Form.Control
                  type="number"
                  name="clientId"
                  value={formData.clientId}
                  onChange={handleInputChange}
                  placeholder="Enter client ID"
                  required
                />
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
                  <option value="FullPrice">Full Price</option>
                  <option value="Contingent">Contingent</option>
                  <option value="AsIs">As-Is</option>
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

          <Form.Group className="mb-3">
            <Form.Label>Conditions</Form.Label>
            <Form.Control
              as="textarea"
              rows={3}
              name="conditions"
              value={formData.conditions}
              onChange={handleInputChange}
              placeholder="Special conditions or contingencies..."
            />
          </Form.Group>
        </Form>
      </FormModal>
    </Container>
  );
}
