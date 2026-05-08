import React, { useState, useEffect } from 'react';
import { useUserRole } from '../../shared/RoleGuard';
import { FaEdit, FaEye, FaPlus, FaTrash, FaUsers } from 'react-icons/fa';
import EmployeeService from './EmployeeService';
import EmployeeForm from './EmployeeForm';
import EmployeeDetails from './EmployeeDetails';
import './Employees.css';

export default function Employees() {
  const apiOrigin = (import.meta.env.VITE_API_URL || 'http://localhost:5019/api').replace(/\/api\/?$/, '');
  const [employees, setEmployees] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [showForm, setShowForm] = useState(false);
  const [showDetails, setShowDetails] = useState(false);
  const [selectedEmployee, setSelectedEmployee] = useState(null);
  const [editingEmployee, setEditingEmployee] = useState(null);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(10);
  const [totalCount, setTotalCount] = useState(0);
  const [searchTerm, setSearchTerm] = useState('');
  const [branchFilter, setBranchFilter] = useState('');
  const [roleFilter, setRoleFilter] = useState('');
  const [branches, setBranches] = useState([]);

  const userRole = useUserRole();
  const canCreate = ['Admin', 'Manager'].includes(userRole);
  const canEdit = ['Admin', 'Manager'].includes(userRole);
  const canDelete = userRole === 'Admin';

  useEffect(() => {
    loadEmployees();
    loadBranches();
  }, [page, searchTerm, branchFilter, roleFilter]);

  const handleSearchChange = (value) => {
    setSearchTerm(value);
    setPage(1);
  };

  const handleBranchFilterChange = (value) => {
    setBranchFilter(value);
    setPage(1);
  };

  const handleRoleFilterChange = (value) => {
    setRoleFilter(value);
    setPage(1);
  };

  const loadEmployees = async () => {
    try {
      setLoading(true);
      const response = await EmployeeService.getAll(page, pageSize, searchTerm, branchFilter, roleFilter);
      setEmployees(response.data || []);
      setTotalCount(response.totalCount || 0);
    } catch (err) {
      setError('Failed to load employees');
      console.error('Error loading employees:', err);
    } finally {
      setLoading(false);
    }
  };

  const loadBranches = async () => {
    try {
      const response = await EmployeeService.getBranches();
      setBranches(response.data || []);
    } catch (err) {
      console.error('Error loading branches:', err);
    }
  };

  const handleCreate = () => {
    setEditingEmployee(null);
    setShowForm(true);
  };

  const handleEdit = (employee) => {
    setEditingEmployee(employee);
    setShowForm(true);
  };

  const handleViewDetails = (employee) => {
    setSelectedEmployee(employee);
    setShowDetails(true);
  };

  const handleDelete = async (employeeId) => {
    if (!window.confirm('Are you sure you want to delete this employee?')) return;

    try {
      await EmployeeService.delete(employeeId);
      loadEmployees();
    } catch (err) {
      setError('Failed to delete employee');
      console.error('Error deleting employee:', err);
    }
  };

  const getCurrentUserId = () => parseInt(localStorage.getItem('userId'), 10) || 0;

  const handleFormSubmit = async (formData) => {
    try {
      const { image, ...payload } = formData;
      if (!editingEmployee) {
        payload.userId = getCurrentUserId();
      }

      let savedEmployeeId = editingEmployee?.id;
      if (editingEmployee) {
        const response = await EmployeeService.update(editingEmployee.id, payload);
        savedEmployeeId = response.data?.id || editingEmployee.id;
      } else {
        const response = await EmployeeService.create(payload);
        savedEmployeeId = response.data?.id;
      }

      if (image && savedEmployeeId) {
        await EmployeeService.uploadImage(savedEmployeeId, image);
      }
      setShowForm(false);
      loadEmployees();
    } catch (err) {
      setError('Failed to save employee');
      console.error('Error saving employee:', err);
    }
  };

  const handleFormCancel = () => {
    setShowForm(false);
    setEditingEmployee(null);
  };

  const handleDetailsClose = () => {
    setShowDetails(false);
    setSelectedEmployee(null);
  };

  const totalPages = Math.ceil(totalCount / pageSize);

  if (loading && employees.length === 0) {
    return <div className="text-center p-5">Loading employees...</div>;
  }

  return (
    <div className="employees-container">
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h2>Employee Management</h2>
        {canCreate && (
          <button className="btn btn-primary" onClick={handleCreate}>
            <FaPlus className="me-2" />Add Employee
          </button>
        )}
      </div>

      {error && (
        <div className="alert alert-danger alert-dismissible fade show" role="alert">
          {error}
          <button type="button" className="btn-close" onClick={() => setError('')}></button>
        </div>
      )}

      {/* Filters */}
      <div className="row mb-3">
        <div className="col-md-4">
          <input
            type="text"
            className="form-control"
            placeholder="Search employees..."
            value={searchTerm}
            onChange={(e) => handleSearchChange(e.target.value)}
          />
        </div>
        <div className="col-md-3">
          <select
            className="form-select"
            value={branchFilter}
            onChange={(e) => handleBranchFilterChange(e.target.value)}
          >
            <option value="">All Branches</option>
            {branches.map(branch => (
              <option key={branch.id} value={branch.id}>{branch.name}</option>
            ))}
          </select>
        </div>
        <div className="col-md-3">
          <select
            className="form-select"
            value={roleFilter}
            onChange={(e) => handleRoleFilterChange(e.target.value)}
          >
            <option value="">All Roles</option>
            <option value="Admin">Admin</option>
            <option value="Manager">Manager</option>
            <option value="Agent">Agent</option>
            <option value="Marketer">Marketer</option>
          </select>
        </div>
      </div>

      {/* Employee Table */}
      <div className="table-responsive">
        <table className="table table-striped table-hover">
          <thead className="table-dark">
            <tr>
              <th>Photo</th>
              <th>Name</th>
              <th>Role</th>
              <th>Branch</th>
              <th>Phone</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {employees.map(employee => (
              <tr key={employee.id}>
                <td>
                  {employee.imagePath ? (
                    <img
                      src={`${apiOrigin}${employee.imagePath}`}
                      alt={employee.firstName}
                      className="employee-photo"
                      onError={(e) => {
                        e.target.src = '/default-avatar.png';
                      }}
                    />
                  ) : (
                    <div className="employee-photo-placeholder">
                      {employee.firstName?.charAt(0)}{employee.lastName?.charAt(0)}
                    </div>
                  )}
                </td>
                <td>{employee.firstName} {employee.lastName}</td>
                <td>
                  <span className={`badge bg-${employee.role === 'Admin' ? 'danger' :
                    employee.role === 'Manager' ? 'warning' :
                    employee.role === 'Agent' ? 'success' : 'info'}`}>
                    {employee.role}
                  </span>
                </td>
                <td>{employee.branchName || 'N/A'}</td>
                <td>{employee.phoneNumber}</td>
                <td>
                  <span className={`badge ${employee.isActive ? 'bg-success' : 'bg-secondary'}`}>
                    {employee.isActive ? 'Active' : 'Inactive'}
                  </span>
                </td>
                <td>
                  <div className="btn-group employee-action-group" role="group" aria-label={`${employee.firstName} ${employee.lastName} actions`}>
                    <button
                      className="btn btn-sm btn-outline-primary"
                      onClick={() => handleViewDetails(employee)}
                      title="View Details"
                    >
                      <FaEye />
                    </button>
                    {canEdit && (
                      <button
                        className="btn btn-sm btn-outline-secondary"
                        onClick={() => handleEdit(employee)}
                        title="Edit"
                      >
                        <FaEdit />
                      </button>
                    )}
                    {canDelete && (
                      <button
                        className="btn btn-sm btn-outline-danger"
                        onClick={() => handleDelete(employee.id)}
                        title="Delete"
                      >
                        <FaTrash />
                      </button>
                    )}
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {employees.length === 0 && !loading && (
        <div className="text-center py-5">
          <FaUsers className="text-muted mb-3" size={48} />
          <h4>No employees found</h4>
          <p className="text-muted">
            {searchTerm || branchFilter || roleFilter ? 'Try adjusting your filters.' : 'Start by adding your first employee.'}
          </p>
        </div>
      )}

      {/* Pagination */}
      {totalPages > 1 && (
        <nav aria-label="Employee pagination" className="mt-4">
          <ul className="pagination justify-content-center">
            <li className={`page-item ${page === 1 ? 'disabled' : ''}`}>
              <button className="page-link" onClick={() => setPage(page - 1)}>Previous</button>
            </li>
            {Array.from({ length: totalPages }, (_, i) => i + 1).map(pageNum => (
              <li key={pageNum} className={`page-item ${page === pageNum ? 'active' : ''}`}>
                <button className="page-link" onClick={() => setPage(pageNum)}>{pageNum}</button>
              </li>
            ))}
            <li className={`page-item ${page === totalPages ? 'disabled' : ''}`}>
              <button className="page-link" onClick={() => setPage(page + 1)}>Next</button>
            </li>
          </ul>
        </nav>
      )}

      {/* Modals */}
      {showForm && (
        <EmployeeForm
          employee={editingEmployee}
          branches={branches}
          onSubmit={handleFormSubmit}
          onCancel={handleFormCancel}
        />
      )}

      {showDetails && selectedEmployee && (
        <EmployeeDetails
          employee={selectedEmployee}
          onClose={handleDetailsClose}
          onEdit={canEdit ? () => { handleDetailsClose(); handleEdit(selectedEmployee); } : null}
        />
      )}
    </div>
  );
}
