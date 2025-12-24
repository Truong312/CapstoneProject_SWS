/**
 * User Related Types
 */

// User Model
export interface User {
  userId: number;
  fullName: string;
  email: string;
  phone?: string;
  address?: string;
  role: UserRole;
}

// User Profile (from API)
export interface UserProfile {
  userId: number;
  username: string;
  email: string;
  fullName: string;
  phoneNumber?: string;
  address?: string;
  roleId: number;
  roleName: string;
  isActive: boolean;
  createdAt: string;
}

// Update Profile Request
export interface UpdateProfileRequest {
  username?: string;
  email: string;
  fullName: string;
  phoneNumber?: string;
  address?: string;
  roleId?: number;
}

// Change Password Request
export interface ChangePasswordRequest {
  oldPassword: string;
  newPassword: string;
}

// Role enum - matching database values
export enum UserRole {
  User = 0,
  Admin = 1,
  Manager = 2,
  Staff = 3,
}

// Role helper functions
export const isStaffOrAdmin = (role: UserRole): boolean => {
  return role >= UserRole.Admin;
};

export const isAdmin = (role: UserRole): boolean => {
  return role === UserRole.Admin;
};

export const isManager = (role: UserRole): boolean => {
  return role === UserRole.Manager;
};

export const isStaff = (role: UserRole): boolean => {
  return role === UserRole.Staff;
};

export const getRoleName = (role: UserRole): string => {
  switch (role) {
    case UserRole.Admin:
      return 'Admin';
    case UserRole.Manager:
      return 'Manager';
    case UserRole.Staff:
      return 'Staff';
    case UserRole.User:
      return 'User';
    default:
      return 'Unknown';
  }
};
