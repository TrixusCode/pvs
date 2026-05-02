import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { authAPI, propertiesAPI } from '../../api/Client';
import '../../App.css';

function Dashboard() {
  const [user, setUser] = useState(null);
  const [properties, setProperties] = useState([]);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    fetchUserAndProperties();
  }, []);

  const fetchUserAndProperties = async () => {
    try {
      const userRes = await authAPI.me();
      setUser(userRes.data.data);
      
      const propsRes = await propertiesAPI.getAll(1, 5);
      setProperties(propsRes.data.data);
    } catch (error) {
      console.error('Error fetching data:', error);
      navigate('/login');
    } finally {
      setLoading(false);
    }
  };

  const handleLogout = () => {
    authAPI.logout();
    navigate('/login');
  };

  if (loading) return <div className="container mt-5"><p>Loading...</p></div>;

  return (
    <div className="container mt-5">
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h1>Dashboard</h1>
        <div>
          <span className="me-3">Welcome, {user?.firstName}!</span>
          <button className="btn btn-danger btn-sm" onClick={handleLogout}>Logout</button>
        </div>
      </div>

      <div className="row mb-4">
        <div className="col-md-6">
          <div className="card">
            <div className="card-body">
              <h5 className="card-title">My Information</h5>
              <p><strong>Email:</strong> {user?.email}</p>
              <p><strong>Role:</strong> {user?.role}</p>
              <p><strong>Name:</strong> {user?.firstName} {user?.lastName}</p>
            </div>
          </div>
        </div>
      </div>

      <div className="mb-4">
        <h3>Recent Properties</h3>
        <div className="row">
          {properties.length > 0 ? (
            properties.map(prop => (
              <div key={prop.id} className="col-md-4 mb-3">
                <div className="card">
                  <div className="card-body">
                    <h5 className="card-title">{prop.title}</h5>
                    <p className="card-text">{prop.description}</p>
                    <p><strong>Price:</strong> ${prop.price?.toLocaleString()}</p>
                    <p><strong>Bedrooms:</strong> {prop.bedrooms} | <strong>Bathrooms:</strong> {prop.bathrooms}</p>
                  </div>
                </div>
              </div>
            ))
          ) : (
            <p>No properties yet.</p>
          )}
        </div>
      </div>
    </div>
  );
}

export default Dashboard;
