import { Link, useNavigate } from 'react-router-dom';
import { authAPI } from '../../api/Client';
import './Navigation.css';

function Navigation() {
  const navigate = useNavigate();
  const token = localStorage.getItem('authToken');

  const handleLogout = () => {
    authAPI.logout();
    navigate('/login');
  };

  return (
    <nav className="navbar navbar-expand-lg navbar-dark bg-dark">
      <div className="container">
        <Link className="navbar-brand" to="/">
          PVS Real Estate
        </Link>
        <button
          className="navbar-toggler"
          type="button"
          data-bs-toggle="collapse"
          data-bs-target="#navbarNav"
        >
          <span className="navbar-toggler-icon"></span>
        </button>
        <div className="collapse navbar-collapse" id="navbarNav">
          {token && (
            <ul className="navbar-nav ms-auto">
              <li className="nav-item">
                <Link className="nav-link" to="/dashboard">
                  Dashboard
                </Link>
              </li>
              <li className="nav-item">
                <Link className="nav-link" to="/properties">
                  Properties
                </Link>
              </li>
              <li className="nav-item">
                <Link className="nav-link" to="/clients">
                  Clients
                </Link>
              </li>
              <li className="nav-item">
                <Link className="nav-link" to="/appointments">
                  Appointments
                </Link>
              </li>
              <li className="nav-item">
                <Link className="nav-link" to="/offers">
                  Offers
                </Link>
              </li>
              <li className="nav-item">
                <button
                  className="btn btn-danger btn-sm"
                  onClick={handleLogout}
                >
                  Logout
                </button>
              </li>
            </ul>
          )}
        </div>
      </div>
    </nav>
  );
}

export default Navigation;
