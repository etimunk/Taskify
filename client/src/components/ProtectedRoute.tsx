import { Navigate } from 'react-router-dom';
import { useAuthStore } from '@/store/useAuthStore';
import type { UserRole } from '@/types';

interface ProtectedRouteProps {
  children: React.ReactNode;
  allowedRoles?: UserRole[];
}

const ProtectedRoute = ({ children, allowedRoles }: ProtectedRouteProps) => {
  const { isAuthenticated, isTokenExpired, role } = useAuthStore();

  if (!isAuthenticated || isTokenExpired()) {
    return <Navigate to="/login" replace />;
  }

  if (allowedRoles && (!role || !allowedRoles.includes(role))) {
    if (role === 'headmanager') return <Navigate to="/dashboard" replace />;
    if (role === 'manager') return <Navigate to="/manager" replace />;
    return <Navigate to="/worker" replace />;
  }

  return <>{children}</>;
};

export default ProtectedRoute;
