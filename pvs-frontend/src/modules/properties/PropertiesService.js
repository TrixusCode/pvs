import axios from 'axios';

// API base URL - change based on environment
const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5019/api';

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

// ============ PROPERTIES SERVICE ============
class PropertiesService {
  // Get all properties (paginated)
  async getAll(page = 1, pageSize = 10) {
    try {
      const response = await apiClient.get('/properties', {
        params: { page, pageSize }
      });
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Get single property by ID
  async getById(id) {
    try {
      const response = await apiClient.get(`/properties/${id}`);
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Create new property
  async create(propertyData) {
    try {
      const response = await apiClient.post('/properties', propertyData);
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Update existing property
  async update(id, propertyData) {
    try {
      const response = await apiClient.put(`/properties/${id}`, propertyData);
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Upload or replace a property image
  async uploadImage(id, imageFile) {
    try {
      const formData = new FormData();
      formData.append('file', imageFile);

      const response = await apiClient.post(`/properties/${id}/upload-image`, formData, {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      });
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Delete property
  async delete(id) {
    try {
      const response = await apiClient.delete(`/properties/${id}`);
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Search properties
  async search(searchTerm) {
    try {
      const response = await apiClient.get('/properties/search', {
        params: { q: searchTerm }
      });
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Get properties by status
  async getByStatus(status) {
    try {
      const response = await apiClient.get(`/properties/by-status/${status}`);
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Get properties by price range
  async getByPriceRange(minPrice, maxPrice) {
    try {
      const response = await apiClient.get('/properties/price-range', {
        params: { minPrice, maxPrice }
      });
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Get properties by location
  async getByLocation(location) {
    try {
      const response = await apiClient.get('/properties/by-location', {
        params: { location }
      });
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }
}

export default new PropertiesService();
