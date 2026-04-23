import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { BrowserRouter, Route, Routes, Navigate } from "react-router-dom";
import { Toaster as Sonner } from "@/components/ui/sonner";
import { Toaster } from "@/components/ui/toaster";
import { TooltipProvider } from "@/components/ui/tooltip";

import Login from "./pages/Login";
import DashboardLayout from "./layouts/DashboardLayout";
import HeadManagerDashboard from "./pages/HeadManagerDashboard";
import ManagerDashboard from "./pages/ManagerDashboard";
import WorkerDashboard from "./pages/WorkerDashboard";
import UsersManagement from "./pages/UsersManagement";
import ProjectsPage from "./pages/ProjectsPage";
import TasksPage from "./pages/TasksPage";
import Index from "./pages/Index";
import NotFound from "./pages/NotFound";
import ProtectedRoute from "./components/ProtectedRoute";
import ProfilePage from "./pages/ProfilePage";

const queryClient = new QueryClient();

const App = () => (
  <QueryClientProvider client={queryClient}>
    <TooltipProvider>
      <Toaster />
      <Sonner />
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<Index />} />
          <Route path="/login" element={<Login />} />

          {/* Head Manager Routes */}
          <Route element={<DashboardLayout />}>
            <Route path="/dashboard" element={<ProtectedRoute allowedRoles={['headmanager']}><HeadManagerDashboard /></ProtectedRoute>} />
            <Route path="/users" element={<ProtectedRoute allowedRoles={['headmanager']}><UsersManagement /></ProtectedRoute>} />
            <Route path="/projects" element={<ProtectedRoute allowedRoles={['headmanager', 'manager']}><ProjectsPage /></ProtectedRoute>} />
            <Route path="/tasks" element={<ProtectedRoute allowedRoles={['headmanager', 'manager']}><TasksPage /></ProtectedRoute>} />
          </Route>

          {/* Manager Routes */}
          <Route element={<DashboardLayout />}>
            <Route path="/manager" element={<ProtectedRoute allowedRoles={['manager']}><ManagerDashboard /></ProtectedRoute>} />
            <Route path="/manager/projects" element={<ProtectedRoute allowedRoles={['manager']}><ProjectsPage /></ProtectedRoute>} />
            <Route path="/manager/tasks" element={<ProtectedRoute allowedRoles={['manager']}><TasksPage /></ProtectedRoute>} />
          </Route>

          {/* Worker Routes */}
          <Route element={<DashboardLayout />}>
            <Route path="/worker" element={<ProtectedRoute allowedRoles={['worker']}><WorkerDashboard /></ProtectedRoute>} />
            <Route path="/profile" element={<ProtectedRoute><ProfilePage /></ProtectedRoute>} />
          </Route>

          <Route path="*" element={<NotFound />} />
        </Routes>
      </BrowserRouter>
    </TooltipProvider>
  </QueryClientProvider>
);

export default App;
