import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { appointmentsAPI } from '../../api/Client';
import '../../App.css';

function Appointments() {
  const [appointments, setAppointments] = useState([]);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    fetchAppointments();
  }, []);

  const fetchAppointments = async () => {
    try {
      const res = await appointmentsAPI.getAll(1, 20);
      setAppointments(res.data.data);
    } catch (error) {
      console.error('Error fetching appointments:', error);
      if (error.response?.status === 401) navigate('/login');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id) => {
    if (window.confirm('Are you sure?')) {
      try {
        await appointmentsAPI.delete(id);
        fetchAppointments();
      } catch (error) {
        console.error('Error deleting appointment:', error);
        alert('Error deleting appointment');
      }
    }
  };

  if (loading) return <div className="container mt-5"><p>Loading...</p></div>;

  return (
    <div className="container mt-5">
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h1>Appointments</h1>
      </div>

      <table className="table table-striped">
        <thead>
          <tr>
            <th>Date</th>
            <th>Time</th>
            <th>Type</th>
            <th>Status</th>
            <th>Property ID</th>
            <th>Client ID</th>
            <th>Notes</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {appointments.length > 0 ? (
            appointments.map(apt => (
              <tr key={apt.id}>
                <td>{new Date(apt.appointmentDate).toLocaleDateString()}</td>
                <td>{apt.time}</td>
                <td>{apt.type}</td>
                <td>{apt.status}</td>
                <td>{apt.propertyId}</td>
                <td>{apt.clientId}</td>
                <td>{apt.notes}</td>
                <td>
                  <button
                    className="btn btn-sm btn-danger"
                    onClick={() => handleDelete(apt.id)}
                  >
                    Delete
                  </button>
                </td>
              </tr>
            ))
          ) : (
            <tr>
              <td colSpan="8">No appointments found.</td>
            </tr>
          )}
        </tbody>
      </table>
    </div>
  );
}

export default Appointments;
