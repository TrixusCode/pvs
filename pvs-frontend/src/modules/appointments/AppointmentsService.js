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

// ============ APPOINTMENTS SERVICE ============
class AppointmentsService {
  // Get all appointments (paginated)
  async getAll(page = 1, pageSize = 10) {
    try {
      const response = await apiClient.get('/appointments', {
        params: { page, pageSize }
      });
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Search appointments
  async search(query) {
    try {
      const response = await apiClient.get('/appointments/search', {
        params: { query }
      });
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Create new appointment
  async create(appointmentData) {
    try {
      const response = await apiClient.post('/appointments', appointmentData);
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Update existing appointment
  async update(id, appointmentData) {
    try {
      const response = await apiClient.put(`/appointments/${id}`, appointmentData);
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Delete appointment
  async delete(id) {
    try {
      const response = await apiClient.delete(`/appointments/${id}`);
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Get appointments for a property
  async getByPropertyId(propertyId) {
    try {
      const response = await apiClient.get(`/appointments/property/${propertyId}`);
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Get appointments for a client
  async getByClientId(clientId) {
    try {
      const response = await apiClient.get(`/appointments/client/${clientId}`);
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Get appointments by status
  async getByStatus(status) {
    try {
      const response = await apiClient.get(`/appointments/by-status/${status}`);
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Get appointments within date range
  async getByDateRange(startDate, endDate) {
    try {
      const response = await apiClient.get('/appointments/by-date', {
        params: { startDate, endDate }
      });
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Get upcoming appointments
  async getUpcoming(limit = 10) {
    try {
      const response = await apiClient.get('/appointments/upcoming', {
        params: { limit }
      });
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Get today's appointments
  async getToday() {
    try {
      const response = await apiClient.get('/appointments/today');
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Confirm appointment
  async confirm(id) {
    try {
      const response = await apiClient.post(`/appointments/${id}/confirm`);
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Update appointment status
  async updateStatus(id, status) {
    try {
      const response = await apiClient.patch(`/appointments/${id}/status`, { status });
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }
}

export default new AppointmentsService();