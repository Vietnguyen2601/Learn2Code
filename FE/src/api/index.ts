import apiClient from './client'

export interface GetOptions {
  params?: Record<string, any>
}

export interface PostOptions {
  data?: Record<string, any>
}

export const api = {
  get: (url: string, options?: GetOptions) => {
    return apiClient.get(url, options)
  },
  post: (url: string, data?: Record<string, any>) => {
    return apiClient.post(url, data)
  },
  put: (url: string, data?: Record<string, any>) => {
    return apiClient.put(url, data)
  },
  patch: (url: string, data?: Record<string, any>) => {
    return apiClient.patch(url, data)
  },
  delete: (url: string) => {
    return apiClient.delete(url)
  },
}

export default api
