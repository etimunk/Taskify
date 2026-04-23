import { Navigate } from 'react-router-dom';
import { useAuthStore } from '@/store/useAuthStore';

const Index = () => {
  const { isAuthenticated, role } = useAuthStore();

  if (!isAuthenticated) return <Navigate to="/login" replace />;
  if (role === 'headmanager') return <Navigate to="/dashboard" replace />;
  if (role === 'manager') return <Navigate to="/manager" replace />;
  return <Navigate to="/worker" replace />;
};

export default Index;
