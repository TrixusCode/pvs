import axios from 'axios';

// API base URL - change based on environment
const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';

// Create axios instance
const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor to add JWT token
apiClient.interceptors.request.use(
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
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('authToken');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

// ============ BRANCHES SERVICE ============
class BranchesService {
  // Get all branches
  async getAll() {
    try {
      const response = await apiClient.get('/branches');
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Get branch by ID
  async getById(id) {
    try {
      const response = await apiClient.get(`/branches/${id}`);
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Create new branch
  async create(branchData) {
    try {
      const response = await apiClient.post('/branches', branchData);
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Update branch
  async update(id, branchData) {
    try {
      const response = await apiClient.put(`/branches/${id}`, branchData);
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Delete branch
  async delete(id) {
    try {
      const response = await apiClient.delete(`/branches/${id}`);
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Get branch staff
  async getBranchStaff(branchId) {
    try {
      const response = await apiClient.get(`/branches/${branchId}/staff`);
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }
}

export default new BranchesService();