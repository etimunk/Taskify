export type UserRole = 'worker' | 'manager' | 'headmanager';

export interface User {
  id: number;
  tz: string;
  name: string;
  email: string;
  password?: string;
  role: string;
  level: UserRole;
  tasks?: Task[];
}

export interface UserDTO {
  id: number;
  tz: string;
  name: string;
  email: string;
  role: string;
  level: UserRole;
}

export interface Project {
  id: number;
  name: string;
  description: string;
  startDate: string;
  dueDate: string;
  status: string;
  managerId: number;
  managerName?: string;
  managerEmail?: string;
  totalTasks?: number;
  completedTasks?: number;
  inProgressTasks?: number;
  pendingTasks?: number;
  completionPercentage?: number;
  tasks?: Task[];
}

export interface Task {
  id: number;
  name: string;
  description: string;
  type: string; // pending | in-progress | done
  priority: number;
  projectId: number;
  userId: number;
  userName?: string;
  projectName?: string;
  projectDueDate?: string;
  projectManagerName?: string;
  projectManagerEmail?: string;
}

export interface LoginRequest {
  email: string;
  name?: string;
  password?: string;
}

export interface LoginResponse {
  token: string;
}

export interface JwtPayload {
  email: string;
  role: UserRole;
  exp: number;
  [key: string]: unknown;
}

export interface ProjectPostModel {
  name: string;
  description: string;
  startDate: string;
  dueDate: string;
  status: string;
  managerId: number;
}

export interface TaskPostModel {
  id?: number;
  name: string;
  description: string;
  type: string;
  priority: number;
  projectId: number;
  userId: number;
}

export interface UserPostModel {
  id?: number;
  tz: string;
  name: string;
  email: string;
  password: string;
}
