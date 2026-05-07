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

// ============ OFFERS SERVICE ============
class OffersService {
  // Get all offers (paginated)
  async getAll(page = 1, pageSize = 10) {
    try {
      const response = await apiClient.get('/offers', {
        params: { page, pageSize }
      });
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Get single offer by ID
  async getById(id) {
    try {
      const response = await apiClient.get(`/offers/${id}`);
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Search offers
  async search(searchTerm) {
    try {
      const response = await apiClient.get('/offers/search', {
        params: { q: searchTerm }
      });
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Create new offer
  async create(offerData) {
    try {
      const response = await apiClient.post('/offers', offerData);
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Update existing offer
  async update(id, offerData) {
    try {
      const response = await apiClient.put(`/offers/${id}`, offerData);
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Delete offer
  async delete(id) {
    try {
      const response = await apiClient.delete(`/offers/${id}`);
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Get offers for a property
  async getByPropertyId(propertyId) {
    try {
      const response = await apiClient.get(`/offers/property/${propertyId}`);
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Get offers from a client
  async getByClientId(clientId) {
    try {
      const response = await apiClient.get(`/offers/client/${clientId}`);
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Get offers by status
  async getByStatus(status) {
    try {
      const response = await apiClient.get(`/offers/by-status/${status}`);
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Accept an offer
  async accept(id) {
    try {
      const response = await apiClient.post(`/offers/${id}/accept`);
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Reject an offer
  async reject(id, reason = '') {
    try {
      const response = await apiClient.post(`/offers/${id}/reject`, { reason });
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Withdraw an offer
  async withdraw(id, reason = '') {
    try {
      const response = await apiClient.post(`/offers/${id}/withdraw`, { reason });
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Get offers within price range
  async getByPriceRange(minPrice, maxPrice) {
    try {
      const response = await apiClient.get('/offers/price-range', {
        params: { minPrice, maxPrice }
      });
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Get pending offers
  async getPending() {
    try {
      const response = await apiClient.get('/offers/pending');
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Get accepted offers
  async getAccepted() {
    try {
      const response = await apiClient.get('/offers/accepted');
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Counter an offer
  async counter(id, newAmount, message = '') {
    try {
      const response = await apiClient.post(`/offers/${id}/counter`, {
        newAmount,
        message
      });
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }
}

export default new OffersService();