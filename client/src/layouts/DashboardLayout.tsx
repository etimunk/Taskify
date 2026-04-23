import { Outlet, Navigate } from 'react-router-dom';
import { SidebarProvider, SidebarTrigger } from '@/components/ui/sidebar';
import { AppSidebar } from '@/components/AppSidebar';
import { useAuthStore } from '@/store/useAuthStore';
import { useLanguage } from '@/hooks/useLanguage';

const DashboardLayout = () => {
  const { isAuthenticated, isTokenExpired } = useAuthStore();
  const { lang } = useLanguage();

  if (!isAuthenticated || isTokenExpired()) {
    return <Navigate to="/login" replace />;
  }

  return (
    <div dir={lang === 'he' ? 'rtl' : 'ltr'}>
      <SidebarProvider>
        <div className="min-h-screen flex w-full">
          <AppSidebar />
          <div className="flex-1 flex flex-col min-w-0">
            <header className="h-12 flex items-center border-b bg-card/50 backdrop-blur-sm sticky top-0 z-10 px-2">
              <SidebarTrigger />
            </header>
            <main className="flex-1 p-4 md:p-6 animate-fade-in">
              <Outlet />
            </main>
          </div>
        </div>
      </SidebarProvider>
    </div>
  );
};

export default DashboardLayout;
