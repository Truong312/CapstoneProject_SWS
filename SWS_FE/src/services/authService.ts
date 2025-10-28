import apiClient from '@/lib/api'

export interface RegisterRequest {
  name?: string
  email: string
  password: string
  confirmPassword: string
  phone?: string
  address?: string
  role?: number  // 0 = User, 1 = Admin
}

export interface LoginRequest {
  email: string
  password: string
}

export interface LoginResponse {
  message: string
  token: string
  user?: {
    userId: number
    name: string
    email: string
    phone?: string
    address?: string
    role: number
    createdAt?: string
  }
}

export interface User {
  userId: number
  name: string
  email: string
  phone?: string
  address?: string
  role: number
  createdAt?: string
}

class AuthService {
  /**
   * Register a new user
   */
  async register(data: RegisterRequest): Promise<{ message: string }> {
    const response = await apiClient.post('/User/Register', {
      ...data,
      role: data.role ?? 0  // Default to User role (0)
    })
    return response.data
  }

  /**
   * Login with email and password
   */
  async login(data: LoginRequest): Promise<LoginResponse> {
    const response = await apiClient.post('/User/Login', data)
    return response.data
  }

  /**
   * Logout current user
   */
  async logout(): Promise<{ message: string }> {
    const response = await apiClient.post('/User/Logout')
    return response.data
  }

  /**
   * Find user by property
   */
  async findUser(property: 'name' | 'email' | 'phone', value: string): Promise<User[]> {
    const response = await apiClient.get(`/User/find`, {
      params: { property, value }
    })
    return response.data
  }
}

export default new AuthService()
