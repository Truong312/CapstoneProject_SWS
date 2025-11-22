import axios from 'axios';
export interface Inventory {
    inventoryId: number;
    productId: number;
    locationId: number;
    quantityAvailable: number;
    allocatedQuantity: number;
}
export interface InventoryAdjustmentDto {
    productId: number;
    locationId: number;
    newQuantity: number;
}
export interface InventoryTransferDto {
    productId: number;
    fromLocationId: number;
    toLocationId: number;
    quantity: number;
}
export interface InventoryItemDto {
    productId: number;
    productName: string;
    category: string;

    totalQuantity: number;
    available: number;
    allocated: number;
    damaged: number;
    inTransit: number;
}

export interface InventoryDashboardDto {
    totalStockValue: number;
    lowStockCount: number;
    outOfStockCount: number;
    turnoverRate: number;
}

export interface ProductInventoryDto {
    ProductId: number;      // ID của sản phẩm
    Name: string;           // Tên sản phẩm
    Total: number;          // Tổng số lượng hàng tồn kho
    Available: number;      // Số lượng hàng khả dụng
    Allocated: number;      // Số lượng hàng đã được phân bổ
    Damaged: number;        // Số lượng hàng hư hỏng
    InTransit: number;      // Số lượng hàng đang trong quá trình vận chuyển
}
const BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:8080/api';

export const getInventoryByProduct = async (productId: number): Promise<Inventory[]> => {
    const response = await axios.get(`${BASE_URL}/Inventory/product/${productId}`);
    return response.data;
};

export const adjustStock = async (dto: InventoryAdjustmentDto): Promise<void> => {
    await axios.post(`${BASE_URL}/Inventory/adjust`, dto);
};

export const transferStock = async (dto: InventoryTransferDto): Promise<void> => {
    await axios.post(`${BASE_URL}/Inventory/transfer`, dto);
};

export const getInventoryDashboard = async (): Promise<InventoryDashboardDto> => {
    const response = await axios.get(`${BASE_URL}/Inventory/dashboard`);
    return response.data;
};
export const getInventoryList = async (): Promise<ProductInventoryDto[]> => {
    const response = await axios.get(`${BASE_URL}/Inventory/product-inventory`);
    return response.data;
};