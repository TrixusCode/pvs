import { useState, useEffect } from 'react';
import { propertiesAPI } from '../../api/Client';
import './Properties.css';

export default function Properties() {
  const [properties, setProperties] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);

  // Load properties on mount and when page changes
  useEffect(() => {
    fetchProperties();
  }, [page]);

  const fetchProperties = async () => {
    setLoading(true);
    setError('');
    
    try {
      const response = await propertiesAPI.getAll(page, 10);
      
      // Assuming API response follows this structure:
      // { data: [...], totalPages: X, currentPage: Y }
      setProperties(response.data.data || response.data);
      setTotalPages(response.data.totalPages || 1);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to load properties');
      console.error('Error fetching properties:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Are you sure you want to delete this property?')) return;

    try {
      await propertiesAPI.delete(id);
      // Refresh the list
      setProperties(properties.filter(p => p.id !== id));
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to delete property');
    }
  };

  if (loading && properties.length === 0) {
    return <div className="properties-container">Loading properties...</div>;
  }

  return (
    <div className="properties-container">
      <h1>Properties</h1>

      {error && <div className="error-message">{error}</div>}

      <div className="properties-grid">
        {properties.length === 0 ? (
          <p>No properties found</p>
        ) : (
          properties.map((property) => (
            <div key={property.id} className="property-card">
              <h3>{property.title || property.name}</h3>
             
              <p className="description">{property.description}</p>
              
              <div className="property-details">
                <span className="price">${property.price}</span>
                <span className="bedrooms">{property.bedrooms} beds</span>
              </div>
              
              <div className="property-actions">
                <button className="edit-btn">Edit</button>
                <button 
                  className="delete-btn"
                  onClick={() => handleDelete(property.id)}
                >
                  Delete
                </button>
              </div>
            </div>
          ))
        )}
      </div>

      {/* Pagination */}
      {totalPages > 1 && (
        <div className="pagination">
          <button
            onClick={() => setPage(Math.max(1, page - 1))}
            disabled={page === 1}
          >
            Previous
          </button>
          <span>Page {page} of {totalPages}</span>
          <button
            onClick={() => setPage(Math.min(totalPages, page + 1))}
            disabled={page === totalPages}
          >
            Next
          </button>
        </div>
      )}
    </div>
  );
}
