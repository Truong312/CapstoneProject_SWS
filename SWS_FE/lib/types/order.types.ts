// Order Types for Import and Export Orders

export enum OrderStatus {
  Pending = 'Pending',
  Approved = 'Approved',
  Completed = 'Completed',
  Cancelled = 'Cancelled',
}

// Import Order Types
export interface ImportOrderItem {
  importDetailId?: number;
  productId: number;
  productName?: string;
  quantity: number;
  importPrice: number;
}

export interface ImportOrder {
  importOrderId: number;
  invoiceNumber: string;
  orderDate: string;
  providerId: number;
  providerName?: string;
  status: OrderStatus | string;
  totalItems?: number;
  createdDate?: string;
  createdBy?: number;
  createdByName?: string;
  items?: ImportOrderItem[];
}

export interface ImportOrderListItem {
  importOrderId: number;
  invoiceNumber: string;
  orderDate: string;
  providerName: string;
  status: string;
  totalItems: number;
  createdByName: string;
}

export interface ImportOrderDetail extends ImportOrder {
  items: ImportOrderItem[];
}

export interface CreateImportOrderRequest {
  providerId: number;
  orderDate: string;
  invoiceNumber: string;
  items: {
    productId: number;
    quantity: number;
    importPrice: number;
  }[];
}

export interface CreateImportOrderResponse {
  importOrderId: number;
  invoiceNumber: string;
}

// Export Order Types
export interface ExportOrderItem {
  exportDetailId?: number;
  productId: number;
  productName?: string;
  quantity: number;
  exportPrice?: number;
}

export interface ExportOrder {
  exportOrderId: number;
  invoiceNumber: string;
  orderDate: string;
  customerId: number;
  customerName?: string;
  currency?: string;
  shippedDate?: string;
  shippedAddress?: string;
  taxRate?: number;
  taxAmount?: number;
  totalPayment?: number;
  description?: string;
  status: OrderStatus | string;
  createdDate?: string;
  createdBy?: number;
  createdByName?: string;
  items?: ExportOrderItem[];
}

export interface ExportOrderListItem {
  exportOrderId: number;
  invoiceNumber: string;
  orderDate: string;
  customerId: number;
  customerName?: string;
  status: string;
  totalPayment?: number;
}

export interface ExportOrderDetail extends ExportOrder {
  items: ExportOrderItem[];
}

export interface CreateExportOrderRequest {
  invoiceNumber: string;
  orderDate: string;
  customerId: number;
  currency: string;
  shippedDate?: string;
  shippedAddress?: string;
  taxRate: number;
  taxAmount: number;
  totalPayment: number;
  description?: string;
  createdBy: number;
}

export interface CreateExportOrderResponse {
  exportOrderId: number;
}

export interface AddExportDetailRequest {
  productId: number;
  quantity: number;
}

export interface AddExportDetailResponse {
  exportDetailId: number;
}

// Provider for Import Orders
export interface Provider {
  providerId: number;
  providerName: string;
  contactPerson?: string;
  phone?: string;
  email?: string;
  address?: string;
}

// Customer for Export Orders
export interface Customer {
  customerId: number;
  customerName: string;
  contactPerson?: string;
  phone?: string;
  email?: string;
  address?: string;
}

// List response with pagination
export interface OrderListResponse<T> {
  total: number;
  page: number;
  pageSize: number;
  items: T[];
}

// Query params for listing orders
export interface OrderQueryParams {
  q?: string;
  status?: string;
  from?: string;
  to?: string;
  page?: number;
  pageSize?: number;
}
