import { apiClient } from '@/lib/api';
import type {
  ExportOrder,
  ExportOrderDetail,
  ExportOrderListItem,
  CreateExportOrderRequest,
  CreateExportOrderResponse,
  AddExportDetailRequest,
  AddExportDetailResponse,
  UpdateExportOrderRequest,
  UpdateExportDetailRequest,
} from '@/lib/types/order.types';
import type { ApiResponse } from '@/lib/types/api.types';

export interface ExportOrderStatusStats {
  status: string;
  count: number;
}

/**
 * Get all export orders
 */
export async function getAllExportOrders(): Promise<ExportOrderListItem[]> {
  const response = await apiClient.get<ExportOrderListItem[]>('/ExportOrder/All');
  return response.data;
}

/**
 * Get export orders by status
 * Status can be: "Pending", "Shipped", "Completed", "Cancelled" or null
 */
export async function getExportOrdersByStatus(status: string): Promise<ExportOrderListItem[]> {
  const response = await apiClient.get<ExportOrderListItem[]>('/ExportOrder/by-status', {
    params: { status }
  });
  return response.data;
}

/**
 * Get export order detail by ID
 */
export async function getExportOrderDetail(exportOrderId: number): Promise<ExportOrderDetail[]> {
  const response = await apiClient.get<ExportOrderDetail[]>(`/ExportOrder/${exportOrderId}Details`);
  return response.data;
}

/**
 * Create new export order
 */
export async function createExportOrder(
  data: CreateExportOrderRequest
): Promise<ApiResponse<CreateExportOrderResponse>> {
  const response = await apiClient.post<ApiResponse<CreateExportOrderResponse>>(
    '/ExportOrder/ExportOder',
    data
  );
  return response.data;
}

/**
 * Update export order
 */
export async function updateExportOrder(
  exportOrderId: number,
  data: UpdateExportOrderRequest
): Promise<ApiResponse<ExportOrder>> {
  const response = await apiClient.put<ApiResponse<ExportOrder>>(
    '/ExportOrder/ExportOder',
    data,
    { params: { exportOrderId } }
  );
  return response.data;
}

/**
 * Delete export order
 */
export async function deleteExportOrder(exportOrderId: number): Promise<ApiResponse<void>> {
  const response = await apiClient.delete<ApiResponse<void>>(
    '/ExportOrder/ExportOder',
    { params: { exportOrderId } }
  );
  return response.data;
}

/**
 * Add detail to export order
 */
export async function addExportDetail(
  exportOrderId: number,
  data: AddExportDetailRequest
): Promise<ApiResponse<AddExportDetailResponse>> {
  const response = await apiClient.post<ApiResponse<AddExportDetailResponse>>(
    '/ExportOrder/ExportDetail',
    data,
    { params: { exportOrderId } }
  );
  return response.data;
}

/**
 * Update export detail
 */
export async function updateExportDetail(
  exportDetailId: number,
  data: UpdateExportDetailRequest
): Promise<ApiResponse<ExportOrderDetail>> {
  const response = await apiClient.put<ApiResponse<ExportOrderDetail>>(
    '/ExportOrder/ExportDetail',
    data,
    { params: { exportDetailId } }
  );
  return response.data;
}

/**
 * Delete export detail
 */
export async function deleteExportDetail(exportDetailId: number): Promise<ApiResponse<void>> {
  const response = await apiClient.delete<ApiResponse<void>>(
    '/ExportOrder/ExportDetail',
    { params: { exportDetailId } }
  );
  return response.data;
}
