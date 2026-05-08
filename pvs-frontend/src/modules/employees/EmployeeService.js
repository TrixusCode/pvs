import axios from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5019/api';

// Create axios instance for employees
const employeeApi = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor to add JWT token
employeeApi.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('authToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Response interceptor to handle errors
employeeApi.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('authToken');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

class EmployeeService {
  // Get all employees with pagination and filters
  async getAll(page = 1, pageSize = 10, search = '', branchId = '', role = '') {
    try {
      const params = new URLSearchParams({
        page: page.toString(),
        pageSize: pageSize.toString(),
      });

      if (search) params.append('search', search);
      if (branchId) params.append('branchId', branchId);
      if (role) params.append('role', role);

      const response = await employeeApi.get(`/employees?${params}`);
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Get employee by ID
  async getById(id) {
    try {
      const response = await employeeApi.get(`/employees/${id}`);
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Create new employee
  async create(employeeData) {
    try {
      const response = await employeeApi.post('/employees', employeeData);
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Update employee
  async update(id, employeeData) {
    try {
      const response = await employeeApi.put(`/employees/${id}`, employeeData);
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  async uploadImage(id, imageFile) {
    try {
      const formData = new FormData();
      formData.append('file', imageFile);

      const response = await employeeApi.post(`/employees/${id}/upload-image`, formData, {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      });
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Delete employee
  async delete(id) {
    try {
      const response = await employeeApi.delete(`/employees/${id}`);
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Get branches for dropdown
  async getBranches() {
    try {
      const response = await employeeApi.get('/branches');
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Get employee statistics
  async getStatistics() {
    try {
      const response = await employeeApi.get('/employees/statistics');
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }
}

export default new EmployeeService();
