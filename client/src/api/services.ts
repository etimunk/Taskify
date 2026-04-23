import apiClient from './client';
import type { LoginRequest, LoginResponse, User, UserDTO, Project, Task, ProjectPostModel, TaskPostModel, UserPostModel } from '@/types';

// Auth
export const authApi = {
  login: (data: LoginRequest) => apiClient.post<LoginResponse>('/auth', data),
};

// Users
export const usersApi = {
  getAll: () => apiClient.get<UserDTO[]>('/User'),
  getWorkers: () => apiClient.get<UserDTO[]>('/User/workers'),
  getById: (id: number) => apiClient.get<UserDTO>(`/User/${id}`),
  getWorkersByManager: (managerId: number) => apiClient.get<UserDTO[]>(`/User/manager/${managerId}/workers`),
  create: (data: UserPostModel) => apiClient.post<UserDTO>('/User', data),
  update: (id: number, data: Partial<UserDTO>) => apiClient.put<UserDTO>(`/User/${id}`, data),
  delete: (id: number) => apiClient.delete(`/User/${id}`),
};

// Projects
export const projectsApi = {
  getAll: () => apiClient.get<Project[]>('/Project'),
  getByManager: (managerId: number) => apiClient.get<Project[]>(`/Project/manager/${managerId}`),
  getById: (id: number) => apiClient.get<Project>(`/Project/${id}`),
  create: (data: ProjectPostModel) => apiClient.post<Project>('/Project', data),
  update: (id: number, data: Partial<ProjectPostModel>) => apiClient.put<Project>(`/Project/${id}`, data),
  delete: (id: number) => apiClient.delete(`/Project/${id}`),
};

// Tasks
export const tasksApi = {
  getAll: () => apiClient.get<Task[]>('/Task'),
  getByProject: (projectId: number) => apiClient.get<Task[]>(`/Task/project/${projectId}`),
  getByManager: (managerId: number) => apiClient.get<Task[]>(`/Task/manager/${managerId}`),
  getByWorker: (userId: number) => apiClient.get<Task[]>(`/Task/worker/${userId}`),
  getById: (id: number) => apiClient.get<Task>(`/Task/${id}`),
  create: (data: TaskPostModel) => apiClient.post<Task>('/Task', data),
  update: (id: number, data: TaskPostModel) => apiClient.put<Task>(`/Task/${id}`, data),
  delete: (id: number) => apiClient.delete(`/Task/${id}`),
};
