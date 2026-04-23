import { create } from 'zustand';
import { jwtDecode } from 'jwt-decode';
import type { UserRole } from '@/types';

interface DecodedToken {
  email: string;
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role': UserRole;
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'?: string;
  exp: number;
}

interface AuthState {
  token: string | null;
  email: string | null;
  role: UserRole | null;
  userId: number | null;
  isAuthenticated: boolean;
  setToken: (token: string) => void;
  setUserId: (id: number) => void;
  logout: () => void;
  isTokenExpired: () => boolean;
}

const parseToken = (token: string | null) => {
  if (!token) return { email: null, role: null, userId: null };
  try {
    const decoded = jwtDecode<DecodedToken>(token);
    const email = decoded.email || (decoded as any)['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] as string;
    const role = (decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || (decoded as any).role) as UserRole;
    const userIdClaim =
      decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] ||
      (decoded as any).nameid ||
      (decoded as any).sub;
    const userId = userIdClaim ? Number(userIdClaim) : null;
    return { email, role, userId: Number.isNaN(userId) ? null : userId };
  } catch {
    return { email: null, role: null, userId: null };
  }
};

const storedToken = localStorage.getItem('taskify_token');
const parsed = parseToken(storedToken);

export const useAuthStore = create<AuthState>((set, get) => ({
  token: storedToken,
  email: parsed.email,
  role: parsed.role,
  userId: parsed.userId ?? (localStorage.getItem('taskify_userId') ? Number(localStorage.getItem('taskify_userId')) : null),
  isAuthenticated: !!storedToken && !!parsed.role,

  setToken: (token: string) => {
    localStorage.setItem('taskify_token', token);
    const { email, role, userId } = parseToken(token);
    if (userId !== null) {
      localStorage.setItem('taskify_userId', String(userId));
    } else {
      localStorage.removeItem('taskify_userId');
    }
    set({ token, email, role, userId, isAuthenticated: !!role });
  },

  setUserId: (id: number) => {
    localStorage.setItem('taskify_userId', String(id));
    set({ userId: id });
  },

  logout: () => {
    localStorage.removeItem('taskify_token');
    localStorage.removeItem('taskify_userId');
    set({ token: null, email: null, role: null, userId: null, isAuthenticated: false });
  },

  isTokenExpired: () => {
    const { token } = get();
    if (!token) return true;
    try {
      const decoded = jwtDecode<DecodedToken>(token);
      return decoded.exp * 1000 < Date.now();
    } catch {
      return true;
    }
  },
}));
