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

// ============ DASHBOARD SERVICE ============
class DashboardService {
  // Get dashboard statistics
  async getStatistics() {
    try {
      const response = await apiClient.get('/dashboard/statistics');
      return response.data.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Get recent activities
  async getRecentActivities(limit = 10) {
    try {
      const response = await apiClient.get('/dashboard/activities', {
        params: { limit }
      });
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Get notifications
  async getNotifications() {
    try {
      const response = await apiClient.get('/dashboard/notifications');
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Mark notification as read
  async markNotificationAsRead(id) {
    try {
      const response = await apiClient.post(`/dashboard/notifications/${id}/read`);
      return response.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Get current user info
  async getCurrentUser() {
    try {
      const response = await apiClient.get('/auth/me');
      return response.data.data;
    } catch (error) {
      throw error.response?.data || error;
    }
  }

  // Logout
  async logout() {
    try {
      localStorage.removeItem('authToken');
      return true;
    } catch (error) {
      throw error;
    }
  }
}

export default new DashboardService();