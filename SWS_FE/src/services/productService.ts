import apiClient from '@/lib/api'

export interface Product {
  id: string
  name: string
  sku: string
  price: number
  category: string
  stock: number
  description?: string
  createdAt?: string
  updatedAt?: string
}

export interface CreateProductRequest {
  name: string
  sku: string
  price: number
  category: string
  stock: number
  description?: string
}

export interface UpdateProductRequest {
  name?: string
  sku?: string
  price?: number
  category?: string
  stock?: number
  description?: string
}

class ProductService {
  /**
   * Get all products
   */
  async getProducts(): Promise<Product[]> {
    const response = await apiClient.get('/Product')
    return response.data
  }

  /**
   * Get product by ID
   */
  async getProductById(id: string): Promise<Product> {
    const response = await apiClient.get(`/Product/${id}`)
    return response.data
  }

  /**
   * Create a new product
   */
  async createProduct(data: CreateProductRequest): Promise<Product> {
    const response = await apiClient.post('/Product', data)
    return response.data
  }

  /**
   * Update a product
   */
  async updateProduct(id: string, data: UpdateProductRequest): Promise<Product> {
    const response = await apiClient.put(`/Product/${id}`, data)
    return response.data
  }

  /**
   * Delete a product
   */
  async deleteProduct(id: string): Promise<void> {
    await apiClient.delete(`/Product/${id}`)
  }

  /**
   * Search products
   */
  async searchProducts(query: string): Promise<Product[]> {
    const response = await apiClient.get('/Product/search', {
      params: { q: query }
    })
    return response.data
  }
}

export default new ProductService()
