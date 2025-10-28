import apiClient from '@/lib/api'

export interface DashboardStats {
  totalProducts: number
  totalInventory: number
  totalOrders: number
  revenue: number
  lowStockProducts?: number
  pendingOrders?: number
}

export interface RecentActivity {
  id: string
  type: string
  description: string
  timestamp: string
  user?: string
}

class DashboardService {
  /**
   * Get dashboard statistics
   */
  async getStats(): Promise<DashboardStats> {
    const response = await apiClient.get('/Dashboard/stats')
    return response.data
  }

  /**
   * Get recent activities
   */
  async getRecentActivities(limit: number = 10): Promise<RecentActivity[]> {
    const response = await apiClient.get('/Dashboard/activities', {
      params: { limit }
    })
    return response.data
  }
}

export default new DashboardService()
