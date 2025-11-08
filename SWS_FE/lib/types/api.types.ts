// Base API Response Types
export interface ApiResponse<T = any> {
  isSuccess: boolean;
  message: string;
  data?: T;
  statusCode: number;
}

// User Model
export interface User {
  userId: number;
  fullName: string;
  email: string;
  phone?: string;
  address?: string;
  role: number;
}

// Auth Response Models
export interface AuthData {
  user: User;
  token: string;
}

export interface AuthResponse extends ApiResponse<AuthData> {}

export interface GoogleAuthData extends User {
  token: string;
  isNewUser: boolean;
}

export interface GoogleAuthResponse extends ApiResponse<GoogleAuthData> {}

export interface GoogleAuthUrlData {
  authUrl: string;
}

export interface GoogleAuthUrlResponse extends ApiResponse<GoogleAuthUrlData> {}

// Request Models
export interface RegisterRequest {
  fullName: string;
  email: string;
  password: string;
  phone?: string;
  address?: string;
  role: number;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface ChangePasswordRequest {
  oldPassword: string;
  newPassword: string;
}

export interface GoogleLoginRequest {
  code: string;
}

export interface UpdateUserRequest {
  fullName?: string;
  email?: string;
  phone?: string;
  address?: string;
  role?: number;
}

// Role enum
export enum UserRole {
  User = 0,
  Staff = 1,
  Admin = 1,
}
