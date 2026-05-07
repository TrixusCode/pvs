import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import Login from './Login';
import AuthService from './AuthService';
import { vi } from 'vitest';

const mockNavigate = vi.fn();

vi.mock('./AuthService', () => ({
  default: {
    login: vi.fn(),
    getCurrentUser: vi.fn()
  }
}));

vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual('react-router-dom');
  return {
    ...actual,
    useNavigate: () => mockNavigate
  };
});

describe('Login component', () => {
  beforeEach(() => {
    vi.clearAllMocks();
    localStorage.clear();
  });

  it('renders email and password fields', () => {
    render(
      <MemoryRouter>
        <Login />
      </MemoryRouter>
    );

    expect(screen.getByLabelText(/Email/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/Password/i)).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /login/i })).toBeInTheDocument();
  });

  it('redirects to dashboard after successful login', async () => {
    AuthService.login.mockResolvedValue({ data: 'token-123' });
    AuthService.getCurrentUser.mockResolvedValue({ data: { id: 1, firstName: 'Jane', lastName: 'Doe', role: 'Agent' } });

    render(
      <MemoryRouter>
        <Login />
      </MemoryRouter>
    );

    fireEvent.change(screen.getByLabelText(/Email/i), { target: { value: 'jane@example.com' } });
    fireEvent.change(screen.getByLabelText(/Password/i), { target: { value: 'password' } });
    fireEvent.click(screen.getByRole('button', { name: /login/i }));

    await waitFor(() => expect(localStorage.getItem('authToken')).toBe('token-123'));
    expect(mockNavigate).toHaveBeenCalledWith('/dashboard');
  });

  it('shows an error when login fails', async () => {
    AuthService.login.mockRejectedValue({ response: { data: { message: 'Login failed' } } });

    render(
      <MemoryRouter>
        <Login />
      </MemoryRouter>
    );

    fireEvent.change(screen.getByLabelText(/Email/i), { target: { value: 'jane@example.com' } });
    fireEvent.change(screen.getByLabelText(/Password/i), { target: { value: 'wrongpass' } });
    fireEvent.click(screen.getByRole('button', { name: /login/i }));

    await waitFor(() => expect(screen.getByText('Login failed')).toBeInTheDocument());
  });
});
