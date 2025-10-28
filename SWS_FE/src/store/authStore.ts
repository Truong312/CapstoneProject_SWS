import { create } from 'zustand'
import authService, { type User } from '@/services/authService'

interface AuthState {
  user: User | null
  token: string | null
  isAuthenticated: boolean
  login: (email: string, password: string) => Promise<void>
  logout: () => Promise<void>
  setUser: (user: User) => void
  setToken: (token: string) => void
  register: (name: string, email: string, password: string, confirmPassword: string, phone?: string, address?: string) => Promise<void>
}

export const useAuthStore = create<AuthState>((set) => ({
  user: null,
  token: localStorage.getItem('token'),
  isAuthenticated: !!localStorage.getItem('token'),
  
  login: async (email: string, password: string) => {
    try {
      const data = await authService.login({ email, password })
      
      if (data.token) {
        localStorage.setItem('token', data.token)
        set({ 
          token: data.token, 
          user: data.user || null, 
          isAuthenticated: true 
        })
      }
    } catch (error) {
      console.error('Login failed:', error)
      throw error
    }
  },
  
  logout: async () => {
    try {
      await authService.logout()
    } catch (error) {
      console.error('Logout failed:', error)
    } finally {
      localStorage.removeItem('token')
      set({ token: null, user: null, isAuthenticated: false })
    }
  },
  
  register: async (name: string, email: string, password: string, confirmPassword: string, phone?: string, address?: string) => {
    try {
      await authService.register({ 
        name, 
        email, 
        password, 
        confirmPassword,
        phone,
        address,
        role: 0  // Default to User role
      })
    } catch (error) {
      console.error('Registration failed:', error)
      throw error
    }
  },
  
  setUser: (user: User) => set({ user }),
  
  setToken: (token: string) => {
    localStorage.setItem('token', token)
    set({ token, isAuthenticated: true })
  },
}))
