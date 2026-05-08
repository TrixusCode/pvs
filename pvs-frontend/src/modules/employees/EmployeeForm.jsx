import React, { useState, useEffect } from 'react';
import  FormModal  from '../../shared/FormModal';

export default function EmployeeForm({ employee, branches, onSubmit, onCancel }) {
  const apiOrigin = (import.meta.env.VITE_API_URL || 'http://localhost:5019/api').replace(/\/api\/?$/, '');
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    phoneNumber: '',
    birthdate: '',
    role: 'Agent',
    branchId: '',
    isActive: true,
    isClient: false,
    address: {
      city: '',
      state: '',
      zipCode: ''
    },
    image: null
  });
  const [imagePreview, setImagePreview] = useState('');
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (employee) {
      setFormData({
        firstName: employee.firstName || '',
        lastName: employee.lastName || '',
        phoneNumber: employee.phoneNumber || '',
        birthdate: employee.birthdate ? new Date(employee.birthdate).toISOString().split('T')[0] : '',
        role: employee.role || 'Agent',
        branchId: employee.branchId || '',
        isActive: employee.isActive ?? true,
        isClient: employee.isClient ?? false,
        address: {
          city: employee.address?.city || '',
          state: employee.address?.state || '',
          zipCode: employee.address?.zipCode || ''
        },
        image: null
      });

      if (employee.imagePath) {
        setImagePreview(`${apiOrigin}${employee.imagePath}`);
      }
    }
  }, [employee]);

  const handleInputChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: type === 'checkbox' ? checked : value
    }));
  };

  const handleAddressChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      address: {
        ...prev.address,
        [name]: value
      }
    }));
  };

  const handleImageChange = (e) => {
    const file = e.target.files[0];
    if (file) {
      setFormData(prev => ({
        ...prev,
        image: file
      }));

      // Create preview
      const reader = new FileReader();
      reader.onload = (e) => setImagePreview(e.target.result);
      reader.readAsDataURL(file);
    }
  };

  const handleSubmit = async () => {
    setLoading(true);

    try {
      await onSubmit(formData);
    } catch (error) {
      console.error('Error submitting form:', error);
    } finally {
      setLoading(false);
    }
  };

  const title = employee ? 'Edit Employee' : 'Add New Employee';

  return (
    <FormModal
      show
      title={title}
      onSubmit={handleSubmit}
      onClose={onCancel}
      loading={loading}
      isEditMode={!!employee}
    >
      <div className="row">
        {/* Image Upload */}
        <div className="col-md-4 mb-3">
          <label className="form-label">Photo</label>
          <div className="text-center">
            <div className="mb-3">
              {imagePreview ? (
                <img
                  src={imagePreview}
                  alt="Employee preview"
                  className="img-thumbnail"
                  style={{ width: '120px', height: '120px', objectFit: 'cover' }}
                />
              ) : (
                <div
                  className="bg-light border rounded d-flex align-items-center justify-content-center"
                  style={{ width: '120px', height: '120px' }}
                >
                  <i className="fas fa-user fa-2x text-muted"></i>
                </div>
              )}
            </div>
            <input
              type="file"
              className="form-control"
              accept="image/*"
              onChange={handleImageChange}
            />
          </div>
        </div>

        {/* Basic Information */}
        <div className="col-md-8">
          <div className="row">
            <div className="col-md-6 mb-3">
              <label htmlFor="firstName" className="form-label">First Name *</label>
              <input
                type="text"
                className="form-control"
                id="firstName"
                name="firstName"
                value={formData.firstName}
                onChange={handleInputChange}
                required
              />
            </div>
            <div className="col-md-6 mb-3">
              <label htmlFor="lastName" className="form-label">Last Name *</label>
              <input
                type="text"
                className="form-control"
                id="lastName"
                name="lastName"
                value={formData.lastName}
                onChange={handleInputChange}
                required
              />
            </div>
          </div>

          <div className="row">
            <div className="col-md-6 mb-3">
              <label htmlFor="phoneNumber" className="form-label">Phone Number *</label>
              <input
                type="tel"
                className="form-control"
                id="phoneNumber"
                name="phoneNumber"
                value={formData.phoneNumber}
                onChange={handleInputChange}
                required
              />
            </div>
            <div className="col-md-6 mb-3">
              <label htmlFor="birthdate" className="form-label">Birth Date</label>
              <input
                type="date"
                className="form-control"
                id="birthdate"
                name="birthdate"
                value={formData.birthdate}
                onChange={handleInputChange}
              />
            </div>
          </div>

          <div className="row">
            <div className="col-md-6 mb-3">
              <label htmlFor="role" className="form-label">Role *</label>
              <select
                className="form-select"
                id="role"
                name="role"
                value={formData.role}
                onChange={handleInputChange}
                required
              >
                <option value="Agent">Agent</option>
                <option value="Manager">Manager</option>
                <option value="Marketer">Marketer</option>
                <option value="Admin">Admin</option>
              </select>
            </div>
            <div className="col-md-6 mb-3">
              <label htmlFor="branchId" className="form-label">Branch *</label>
              <select
                className="form-select"
                id="branchId"
                name="branchId"
                value={formData.branchId}
                onChange={handleInputChange}
                required
              >
                <option value="">Select Branch</option>
                {branches.map(branch => (
                  <option
                    key={branch.id}
                    value={branch.id}
                    disabled={branch.employeeCount >= 5 && branch.id !== formData.branchId}
                  >
                    {branch.name}{branch.employeeCount != null ? ` (${branch.employeeCount}/5)` : ''}
                  </option>
                ))}
              </select>
            </div>
          </div>

          {/* Address */}
          <fieldset className="mb-3">
            <legend>Address</legend>
            <div className="row">
              <div className="col-md-4 mb-3">
                <label htmlFor="city" className="form-label">City</label>
                <input
                  type="text"
                  className="form-control"
                  id="city"
                  name="city"
                  value={formData.address.city}
                  onChange={handleAddressChange}
                />
              </div>
              <div className="col-md-4 mb-3">
                <label htmlFor="state" className="form-label">State</label>
                <input
                  type="text"
                  className="form-control"
                  id="state"
                  name="state"
                  value={formData.address.state}
                  onChange={handleAddressChange}
                />
              </div>
              <div className="col-md-4 mb-3">
                <label htmlFor="zipCode" className="form-label">ZIP Code</label>
                <input
                  type="text"
                  className="form-control"
                  id="zipCode"
                  name="zipCode"
                  value={formData.address.zipCode}
                  onChange={handleAddressChange}
                />
              </div>
            </div>
          </fieldset>

          {/* Status */}
          <div className="row">
            <div className="col-md-6 mb-3">
              <div className="form-check">
                <input
                  className="form-check-input"
                  type="checkbox"
                  id="isActive"
                  name="isActive"
                  checked={formData.isActive}
                  onChange={handleInputChange}
                />
                <label className="form-check-label" htmlFor="isActive">
                  Active Employee
                </label>
              </div>
            </div>
            <div className="col-md-6 mb-3">
              <div className="form-check">
                <input
                  className="form-check-input"
                  type="checkbox"
                  id="isClient"
                  name="isClient"
                  checked={formData.isClient}
                  onChange={handleInputChange}
                />
                <label className="form-check-label" htmlFor="isClient">
                  Also a Client
                </label>
              </div>
            </div>
          </div>
        </div>
      </div>
    </FormModal>
  );
}
