import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { offersAPI } from '../../api/Client';
import '../../App.css';

function Offers() {
  const [offers, setOffers] = useState([]);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    fetchOffers();
  }, []);

  const fetchOffers = async () => {
    try {
      const res = await offersAPI.getAll(1, 20);
      setOffers(res.data.data);
    } catch (error) {
      console.error('Error fetching offers:', error);
      if (error.response?.status === 401) navigate('/login');
    } finally {
      setLoading(false);
    }
  };

  const handleStatusChange = async (id, status) => {
    try {
      if (status === 'Accepted') {
        await offersAPI.accept(id);
      } else if (status === 'Rejected') {
        await offersAPI.reject(id);
      } else if (status === 'Withdrawn') {
        await offersAPI.withdraw(id);
      }
      fetchOffers();
    } catch (error) {
      console.error('Error updating offer:', error);
      alert('Error updating offer');
    }
  };

  const handleDelete = async (id) => {
    if (window.confirm('Are you sure?')) {
      try {
        await offersAPI.delete(id);
        fetchOffers();
      } catch (error) {
        console.error('Error deleting offer:', error);
        alert('Error deleting offer');
      }
    }
  };

  if (loading) return <div className="container mt-5"><p>Loading...</p></div>;

  return (
    <div className="container mt-5">
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h1>Offers</h1>
      </div>

      <table className="table table-striped">
        <thead>
          <tr>
            <th>Property ID</th>
            <th>Client ID</th>
            <th>Offered Price</th>
            <th>Status</th>
            <th>Type</th>
            <th>Expiration</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {offers.length > 0 ? (
            offers.map(offer => (
              <tr key={offer.id}>
                <td>{offer.propertyId}</td>
                <td>{offer.clientId}</td>
                <td>${offer.offeredPrice?.toLocaleString()}</td>
                <td>{offer.status}</td>
                <td>{offer.offerType}</td>
                <td>{offer.expirationDate ? new Date(offer.expirationDate).toLocaleDateString() : 'N/A'}</td>
                <td>
                  <button
                    className="btn btn-sm btn-success me-2"
                    onClick={() => handleStatusChange(offer.id, 'Accepted')}
                    disabled={offer.status !== 'Pending'}
                  >
                    Accept
                  </button>
                  <button
                    className="btn btn-sm btn-warning me-2"
                    onClick={() => handleStatusChange(offer.id, 'Rejected')}
                    disabled={offer.status !== 'Pending'}
                  >
                    Reject
                  </button>
                  <button
                    className="btn btn-sm btn-danger"
                    onClick={() => handleDelete(offer.id)}
                  >
                    Delete
                  </button>
                </td>
              </tr>
            ))
          ) : (
            <tr>
              <td colSpan="7">No offers found.</td>
            </tr>
          )}
        </tbody>
      </table>
    </div>
  );
}

export default Offers;
