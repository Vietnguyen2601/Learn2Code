import api from './index'

export const userService = {
  getProfile: () => api.get('/users/profile'),
  updateProfile: (data: Record<string, any>) => api.put('/users/profile', data),
  logout: () => api.post('/auth/logout'),
}

export const authService = {
  login: (email: string, password: string) =>
    api.post('/auth/login', { email, password }),
  register: (data: Record<string, any>) =>
    api.post('/auth/register', data),
  refreshToken: () => api.post('/auth/refresh'),
}

export const postService = {
  getPosts: (params?: Record<string, any>) =>
    api.get('/posts', { params }),
  getPost: (id: string) => api.get(`/posts/${id}`),
  createPost: (data: Record<string, any>) =>
    api.post('/posts', data),
  updatePost: (id: string, data: Record<string, any>) =>
    api.put(`/posts/${id}`, data),
  deletePost: (id: string) => api.delete(`/posts/${id}`),
}
