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
      navigate("/login");
    }
    return Promise.reject(error);
  }
);

// ============ AUTH ENDPOINTS ============
export const authAPI = {
  login: (email, password) =>
    apiClient.post('/auth/login', { email, password }),
  
  register: (email, password, firstName, lastName, role = 'Agent') =>
    apiClient.post('/auth/register', { email, password, firstName, lastName, role }),
  
  logout: () => {
    localStorage.removeItem('authToken');
    return Promise.resolve();
  },
  
  me: () => apiClient.get('/auth/me'),
  
  refreshToken: () => apiClient.post('/auth/refresh-token'),
  
  changePassword: (currentPassword, newPassword) =>
    apiClient.post('/auth/change-password', { currentPassword, newPassword }),
};

// ============ PROPERTIES ENDPOINTS ============
export const propertiesAPI = {
  // Get all properties (paginated)
  getAll: (page = 1, pageSize = 10) =>
    apiClient.get('/properties', { params: { page, pageSize } }),
  
  // Get single property
  getById: (id) => apiClient.get(`/properties/${id}`),
  
  // Create property
  create: (data) => apiClient.post('/properties', data),
  
  // Update property
  update: (id, data) => apiClient.put(`/properties/${id}`, data),
  
  // Delete property
  delete: (id) => apiClient.delete(`/properties/${id}`),
  
  // Search properties
  search: (searchTerm) =>
    apiClient.get('/properties/search', { params: { q: searchTerm } }),
};

// ============ CLIENTS ENDPOINTS ============
export const clientsAPI = {
  // Get all clients (paginated)
  getAll: (page = 1, pageSize = 10) =>
    apiClient.get('/clients', { params: { page, pageSize } }),
  
  // Get single client
  getById: (id) => apiClient.get(`/clients/${id}`),
  
  // Create client
  create: (data) => apiClient.post('/clients', data),
  
  // Update client
  update: (id, data) => apiClient.put(`/clients/${id}`, data),
  
  // Delete client
  delete: (id) => apiClient.delete(`/clients/${id}`),
  
  // Search clients
  search: (q) => apiClient.get('/clients/search', { params: { q } }),
  
  // Get clients by type (Buyer, Seller, Both)
  getByType: (type) => apiClient.get(`/clients/by-type/${type}`),
};

// ============ APPOINTMENTS ENDPOINTS ============
export const appointmentsAPI = {
  // Get all appointments (paginated)
  getAll: (page = 1, pageSize = 10) =>
    apiClient.get('/appointments', { params: { page, pageSize } }),
  
  // Get single appointment
  getById: (id) => apiClient.get(`/appointments/${id}`),
  
  // Create appointment
  create: (data) => apiClient.post('/appointments', data),
  
  // Update appointment
  update: (id, data) => apiClient.put(`/appointments/${id}`, data),
  
  // Delete appointment
  delete: (id) => apiClient.delete(`/appointments/${id}`),
  
  // Get appointments for a property
  getByPropertyId: (propertyId) =>
    apiClient.get(`/appointments/property/${propertyId}`),
  
  // Get appointments for a client
  getByClientId: (clientId) =>
    apiClient.get(`/appointments/client/${clientId}`),
  
  // Get appointments by status
  getByStatus: (status) => apiClient.get(`/appointments/by-status/${status}`),
  
  // Get appointments within date range
  getByDateRange: (startDate, endDate) =>
    apiClient.get('/appointments/by-date', { params: { startDate, endDate } }),
};

// ============ OFFERS ENDPOINTS ============
export const offersAPI = {
  // Get all offers (paginated)
  getAll: (page = 1, pageSize = 10) =>
    apiClient.get('/offers', { params: { page, pageSize } }),
  
  // Get single offer
  getById: (id) => apiClient.get(`/offers/${id}`),
  
  // Create offer
  create: (data) => apiClient.post('/offers', data),
  
  // Update offer
  update: (id, data) => apiClient.put(`/offers/${id}`, data),
  
  // Delete offer
  delete: (id) => apiClient.delete(`/offers/${id}`),
  
  // Get offers for a property
  getByPropertyId: (propertyId) =>
    apiClient.get(`/offers/property/${propertyId}`),
  
  // Get offers from a client
  getByClientId: (clientId) =>
    apiClient.get(`/offers/client/${clientId}`),
  
  // Get offers by status
  getByStatus: (status) => apiClient.get(`/offers/by-status/${status}`),
  
  // Accept an offer
  accept: (id) => apiClient.post(`/offers/${id}/accept`),
  
  // Reject an offer
  reject: (id) => apiClient.post(`/offers/${id}/reject`),
  
  // Withdraw an offer
  withdraw: (id) => apiClient.post(`/offers/${id}/withdraw`),
  
  // Get offers within price range
  getByPriceRange: (minPrice, maxPrice) =>
    apiClient.get('/offers/price-range', { params: { minPrice, maxPrice } }),
};

export default apiClient;
