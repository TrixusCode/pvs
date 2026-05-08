import React from 'react';
import { FaEdit } from 'react-icons/fa';
import DetailsModal from '../../shared/DetailsModal';

export default function EmployeeDetails({ employee, onClose, onEdit }) {
  const apiOrigin = (import.meta.env.VITE_API_URL || 'http://localhost:5019/api').replace(/\/api\/?$/, '');

  if (!employee) return null;

  return (
    <DetailsModal
      show
      title="Employee Details"
      onClose={onClose}
      size="lg"
      footer={(
        <div className="d-flex justify-content-end gap-2">
          {onEdit && (
            <button type="button" className="btn btn-primary" onClick={onEdit}>
              <FaEdit className="me-2" />Edit Employee
            </button>
          )}
        </div>
      )}
    >
      <div className="row">
        {/* Employee Photo */}
        <div className="col-md-4 text-center mb-4">
          {employee.imagePath ? (
            <img
              src={`${apiOrigin}${employee.imagePath}`}
              alt={`${employee.firstName} ${employee.lastName}`}
              className="img-fluid rounded-circle mb-3"
              style={{ width: '150px', height: '150px', objectFit: 'cover' }}
              onError={(e) => {
                e.target.src = '/default-avatar.png';
              }}
            />
          ) : (
            <div
              className="bg-light border rounded-circle d-flex align-items-center justify-content-center mx-auto mb-3"
              style={{ width: '150px', height: '150px' }}
            >
              <span className="display-4 text-muted">
                {employee.firstName?.charAt(0)}{employee.lastName?.charAt(0)}
              </span>
            </div>
          )}
          <h4>{employee.firstName} {employee.lastName}</h4>
          <span className={`badge fs-6 bg-${employee.role === 'Admin' ? 'danger' :
            employee.role === 'Manager' ? 'warning' :
            employee.role === 'Agent' ? 'success' : 'info'}`}>
            {employee.role}
          </span>
        </div>

        {/* Employee Information */}
        <div className="col-md-8">
          <div className="row">
            <div className="col-md-6">
              <h5>Personal Information</h5>
              <table className="table table-sm">
                <tbody>
                  <tr>
                    <td className="fw-bold">Full Name:</td>
                    <td>{employee.firstName} {employee.lastName}</td>
                  </tr>
                  <tr>
                    <td className="fw-bold">Phone:</td>
                    <td>{employee.phoneNumber}</td>
                  </tr>
                  <tr>
                    <td className="fw-bold">Birth Date:</td>
                    <td>{employee.birthdate ? new Date(employee.birthdate).toLocaleDateString() : 'N/A'}</td>
                  </tr>
                  <tr>
                    <td className="fw-bold">Status:</td>
                    <td>
                      <span className={`badge ${employee.isActive ? 'bg-success' : 'bg-secondary'}`}>
                        {employee.isActive ? 'Active' : 'Inactive'}
                      </span>
                    </td>
                  </tr>
                  <tr>
                    <td className="fw-bold">Client:</td>
                    <td>
                      <span className={`badge ${employee.isClient ? 'bg-info' : 'bg-light text-muted'}`}>
                        {employee.isClient ? 'Yes' : 'No'}
                      </span>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>

            <div className="col-md-6">
              <h5>Work Information</h5>
              <table className="table table-sm">
                <tbody>
                  <tr>
                    <td className="fw-bold">Role:</td>
                    <td>{employee.role}</td>
                  </tr>
                  <tr>
                    <td className="fw-bold">Branch:</td>
                    <td>{employee.branchName || 'N/A'}</td>
                  </tr>
                  <tr>
                    <td className="fw-bold">Branch Phone:</td>
                    <td>{employee.branchPhone || 'N/A'}</td>
                  </tr>
                  <tr>
                    <td className="fw-bold">Branch Email:</td>
                    <td>{employee.branchEmail || 'N/A'}</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>

          {/* Address Information */}
          <div className="mt-4">
            <h5>Address</h5>
            {employee.address ? (
              <div className="card">
                <div className="card-body">
                  <div className="row">
                    <div className="col-md-4">
                      <strong>City:</strong> {employee.address.city || 'N/A'}
                    </div>
                    <div className="col-md-4">
                      <strong>State:</strong> {employee.address.state || 'N/A'}
                    </div>
                    <div className="col-md-4">
                      <strong>ZIP Code:</strong> {employee.address.zipCode || 'N/A'}
                    </div>
                  </div>
                </div>
              </div>
            ) : (
              <p className="text-muted">No address information available</p>
            )}
          </div>

          {/* Employment Details */}
          <div className="mt-4">
            <h5>Employment Details</h5>
            <div className="row">
              <div className="col-md-6">
                <strong>Branch:</strong> {employee.branchName || 'N/A'}
              </div>
            </div>
            <div className="row mt-2">
              <div className="col-md-6">
                <strong>Created:</strong> {new Date(employee.createdAt).toLocaleDateString()}
              </div>
              <div className="col-md-6">
                <strong>Last Modified:</strong> {employee.modifiedAt ? new Date(employee.modifiedAt).toLocaleDateString() : 'Never'}
              </div>
            </div>
          </div>
        </div>
      </div>

    </DetailsModal>
  );
}
