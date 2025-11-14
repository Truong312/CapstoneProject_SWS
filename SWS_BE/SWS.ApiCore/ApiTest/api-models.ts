// TypeScript interfaces for FE - derived from backend DTOs
// Location: SWS.ApiCore/ApiTest/api-models.ts

// Generic API wrapper used in many endpoints
export interface ApiResult<T = any> {
  isSuccess: boolean;
  message: string;
  data?: T;
  statusCode?: number;
  total?: number; // some endpoints return total
}

// --- Auth / User ---
export interface UserResponseDto {
  userId: number;
  fullName: string;
  email: string;
  phone?: string;
  address?: string;
  role: number;
}

export interface RegisterWarehouseRequest {
  fullName: string;
  email: string;
  password: string;
  phone?: string;
  address?: string;
  role?: number;
}

export interface LoginWarehouseRequest {
  email: string;
  password: string;
}

export interface GoogleLoginResponseDto {
  userId: number;
  fullName: string;
  email: string;
  phone?: string;
  address?: string;
  role: number;
  token: string;
  isNewUser: boolean;
}

// --- Import Orders ---
export interface ImportOrderListItemDto {
  importOrderId: number;
  invoiceNumber: string;
  orderDate: string; // YYYY-MM-DD
  providerName: string;
  status?: string;
  totalItems: number;
  createdByName?: string;
}

export interface ImportOrderListResult {
  total: number;
  page: number;
  pageSize: number;
  items: ImportOrderListItemDto[];
}

export interface ImportOrderDetailItemDto {
  importDetailId: number;
  productId: number;
  productName: string;
  quantity: number;
  importPrice?: number;
}

export interface ImportOrderDetailDto {
  importOrderId: number;
  invoiceNumber: string;
  orderDate: string;
  providerId: number;
  providerName: string;
  status?: string;
  createdDate?: string;
  createdBy?: number;
  createdByName?: string;
  items: ImportOrderDetailItemDto[];
}

export interface CreateImportOrderItem {
  productId: number;
  quantity: number;
  importPrice?: number;
}

export interface CreateImportOrderRequest {
  providerId: number;
  orderDate?: string;
  invoiceNumber?: string;
  items: CreateImportOrderItem[];
}

export interface CreateImportOrderResponse {
  importOrderId: number;
  invoiceNumber: string;
}

// --- Export Orders ---
export interface ExportOrderResponse {
  exportOrderId: number;
  invoiceNumber?: string;
  orderDate: string;
  customerId: number;
  currency?: string;
  createdDate?: string;
  shippedDate?: string;
  shippedAddress?: string;
  taxRate?: number;
  taxAmount?: number;
  totalPayment?: number;
  description?: string;
  status?: string;
  createdBy?: number;
}

export interface CreateExportOrderRequest {
  invoiceNumber?: string;
  orderDate: string;
  customerId: number;
  currency?: string;
  shippedDate?: string;
  shippedAddress?: string;
  taxRate?: number;
  taxAmount?: number;
  totalPayment?: number;
  description?: string;
  createdBy?: number;
}

export interface CreateExportDetail {
  productId: number;
  quantity: number;
}

// --- Returns / Lookup ---
export interface ReturnReasonDto {
  reasonId: number;
  reasonCode?: string;
  description?: string;
}

export interface ReturnStatusDto {
  status: string;
  count: number;
}

export interface ReturnOrderListItemDto {
  returnOrderId: number;
  exportOrderId?: number;
  checkInTime?: string;
  status?: string;
  note?: string;
  checkedByName?: string;
  reviewedByName?: string;
}

export interface ReturnOrderLineDto {
  returnDetailId: number;
  productId: number;
  productName: string;
  quantity: number;
  reasonId?: number;
  reasonCode?: string;
  note?: string;
  actionId?: number;
  locationId?: number;
}

export interface ReturnOrderDetailDto {
  header: ReturnOrderListItemDto;
  lines: ReturnOrderLineDto[];
}

// End of models

