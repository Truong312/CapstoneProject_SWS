// Return Order Types

export interface ReturnReason {
  reasonId: number;
  reasonCode: string;
  description: string;
}

export interface ReturnStatus {
  statusId: number;
  statusCode: string;
  description: string;
}

export interface ReturnOrderItem {
  returnDetailId?: number;
  productId: number;
  productName?: string;
  quantity: number;
  reasonId: number;
  reasonCode?: string;
  reasonDescription?: string;
}

export interface ReturnOrder {
  returnOrderId: number;
  returnNumber: string;
  returnDate: string;
  customerId?: number;
  customerName?: string;
  exportOrderId?: number;
  exportOrderNumber?: string;
  statusId: number;
  statusCode?: string;
  statusDescription?: string;
  totalItems?: number;
  createdDate?: string;
  createdBy?: number;
  createdByName?: string;
  items?: ReturnOrderItem[];
}

export interface ReturnOrderListItem {
  returnOrderId: number;
  returnNumber: string;
  returnDate: string;
  customerName?: string;
  statusCode: string;
  totalItems: number;
}

export interface ReturnOrderDetail extends ReturnOrder {
  items: ReturnOrderItem[];
}

export interface CreateReturnOrderRequest {
  returnNumber: string;
  returnDate: string;
  customerId?: number;
  exportOrderId?: number;
  statusId: number;
  items: {
    productId: number;
    quantity: number;
    reasonId: number;
  }[];
}

export interface ReturnOrderQueryParams {
  q?: string;
  from?: string;
  to?: string;
  status?: string;
  page?: number;
  pageSize?: number;
}

export interface ReturnReasonQueryParams {
  q?: string;
}

export interface ReturnStatusQueryParams {
  q?: string;
}
