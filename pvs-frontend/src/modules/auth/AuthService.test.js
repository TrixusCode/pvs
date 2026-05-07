import AuthService from './AuthService';

describe('AuthService', () => {
  beforeEach(() => {
    localStorage.clear();
  });

  it('removes authToken on logout', async () => {
    localStorage.setItem('authToken', 'abc123');

    await AuthService.logout();

    expect(localStorage.getItem('authToken')).toBeNull();
  });

  it('returns authenticated when token exists', () => {
    localStorage.setItem('authToken', 'valid-token');

    expect(AuthService.isAuthenticated()).toBe(true);
  });

  it('returns not authenticated when token is missing', () => {
    expect(AuthService.isAuthenticated()).toBe(false);
  });

  it('returns the stored token from localStorage', () => {
    localStorage.setItem('authToken', 'my-token');

    expect(AuthService.getToken()).toBe('my-token');
  });
});
