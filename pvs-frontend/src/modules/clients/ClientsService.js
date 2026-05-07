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

// ============ CLIENTS SERVICE ============
class ClientsService {
  // Get all clients (paginated)
  async getAll(page = 1, pageSize = 10) {
    try {
      const response = await apiClient.get('/clients', {
        params: { page, pageSize }
      });
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Get single client by ID
  async getById(id) {
    try {
      const response = await apiClient.get(`/clients/${id}`);
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Create new client
  async create(clientData) {
    try {
      const response = await apiClient.post('/clients', clientData);
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Update existing client
  async update(id, clientData) {
    try {
      const response = await apiClient.put(`/clients/${id}`, clientData);
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Delete client
  async delete(id) {
    try {
      const response = await apiClient.delete(`/clients/${id}`);
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Search clients
  async search(searchTerm) {
    try {
      const response = await apiClient.get('/clients/search', {
        params: { q: searchTerm }
      });
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Get clients by type (Buyer, Seller, Both)
  async getByType(type) {
    try {
      const response = await apiClient.get(`/clients/by-type/${type}`);
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Get clients by agent
  async getByAgent(agentId) {
    try {
      const response = await apiClient.get(`/clients/by-agent/${agentId}`);
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Get client statistics
  async getStatistics() {
    try {
      const response = await apiClient.get('/clients/statistics');
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }
}

export default new ClientsService();