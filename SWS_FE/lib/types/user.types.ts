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

// Role enum
export enum UserRole {
  User = 0,
  Staff = 1,
}

// Role helper functions
export const isStaffOrAdmin = (role: UserRole): boolean => {
  return role === UserRole.Staff;
};

export const getRoleName = (role: UserRole): string => {
  switch (role) {
    case UserRole.Staff:
      return 'Staff/Admin';
    case UserRole.User:
      return 'User';
    default:
      return 'Unknown';
  }
};
