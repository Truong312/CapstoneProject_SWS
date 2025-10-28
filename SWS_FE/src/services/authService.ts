import apiClient from '@/lib/api'

export interface RegisterRequest {
  name?: string
  fullName?: string
  email: string
  password: string
  confirmPassword?: string
  phone?: string
  address?: string
  role?: number  // 0 = User, 1 = Admin
}

export interface LoginRequest {
  email: string
  password: string
}

export interface LoginResponse {
  isSuccess?: boolean
  message: string
  token?: string
  data?: {
    userId: number
    fullName?: string
    name?: string
    email: string
    phone?: string
    address?: string
    role: number
    token?: string
  }
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
    const response = await apiClient.post('/warehouse/auth/register', {
      fullName: data.name || data.fullName,  // Map name to FullName
      email: data.email,
      password: data.password,
      phone: data.phone,
      address: data.address,
      role: data.role ?? 0  // Default to User role (0)
    })
    return response.data
  }

  /**
   * Login with email and password
   */
  async login(data: LoginRequest): Promise<LoginResponse> {
    const response = await apiClient.post('/warehouse/auth/login', data)
    // Handle both response formats
    if (response.data.isSuccess && response.data.data) {
      return {
        ...response.data,
        token: response.data.data.token,
        user: {
          userId: response.data.data.userId,
          name: response.data.data.fullName || response.data.data.name,
          email: response.data.data.email,
          phone: response.data.data.phone,
          address: response.data.data.address,
          role: response.data.data.role,
        }
      }
    }
    return response.data
  }

  /**
   * Logout current user
   */
  async logout(): Promise<{ message: string }> {
    const response = await apiClient.post('/warehouse/auth/logout')
    return response.data
  }

  /**
   * Get Google OAuth URL
   */
  async getGoogleAuthUrl(): Promise<{ authUrl: string }> {
    const response = await apiClient.get('/warehouse/auth/google-url')
    return response.data.data
  }

  /**
   * Login with Google OAuth code
   */
  async loginWithGoogle(code: string): Promise<LoginResponse & { isNewUser: boolean }> {
    const response = await apiClient.post('/warehouse/auth/google-login', { code })
    return response.data.data
  }

  /**
   * Find user by property
   */
  async findUser(property: 'name' | 'email' | 'phone', value: string): Promise<User[]> {
    const response = await apiClient.get(`/warehouse/auth/find`, {
      params: { property, value }
    })
    return response.data
  }
}

export default new AuthService()
