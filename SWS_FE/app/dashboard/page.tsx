'use client'

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { 
  Package, 
  ShoppingCart, 
  TrendingUp, 
  DollarSign,
  ArrowUpRight,
  ArrowDownRight,
  Users,
  Box,
  AlertCircle,
  CheckCircle,
  Clock
} from 'lucide-react'
import { useAuthStore } from '@/store/authStore'

const stats = [
  {
    title: 'Tổng sản phẩm',
    value: '2,543',
    change: '+12.5%',
    trend: 'up',
    icon: Package,
    color: 'text-purple-600',
    bgColor: 'bg-gradient-to-br from-purple-500 to-purple-600',
  },
  {
    title: 'Tổng đơn hàng',
    value: '1,234',
    change: '+8.2%',
    trend: 'up',
    icon: ShoppingCart,
    color: 'text-teal-600',
    bgColor: 'bg-gradient-to-br from-teal-500 to-teal-600',
  },
  {
    title: 'Doanh thu',
    value: '₫45,231,000',
    change: '+23.1%',
    trend: 'up',
    icon: DollarSign,
    color: 'text-blue-600',
    bgColor: 'bg-gradient-to-br from-blue-500 to-blue-600',
  },
  {
    title: 'Hàng sắp hết',
    value: '23',
    change: '-4.3%',
    trend: 'down',
    icon: AlertCircle,
    color: 'text-orange-600',
    bgColor: 'bg-gradient-to-br from-orange-500 to-orange-600',
  },
]

const recentOrders = [
  { id: '10001', customer: 'Nguyễn Văn A', amount: 4599000, status: 'completed', time: '5 phút trước' },
  { id: '10002', customer: 'Trần Thị B', amount: 3295000, status: 'processing', time: '12 phút trước' },
  { id: '10003', customer: 'Lê Văn C', amount: 7890000, status: 'pending', time: '25 phút trước' },
  { id: '10004', customer: 'Phạm Thị D', amount: 1999000, status: 'completed', time: '1 giờ trước' },
  { id: '10005', customer: 'Hoàng Văn E', amount: 5499000, status: 'processing', time: '2 giờ trước' },
]

const lowStockProducts = [
  { name: 'Laptop Stand Pro', sku: 'LSP-001', stock: 5, threshold: 10, status: 'critical' },
  { name: 'Wireless Mouse X', sku: 'WMX-234', stock: 12, threshold: 20, status: 'low' },
  { name: 'USB-C Hub Ultra', sku: 'UCH-567', stock: 8, threshold: 15, status: 'low' },
  { name: 'Mechanical Keyboard', sku: 'MKB-890', stock: 3, threshold: 10, status: 'critical' },
  { name: 'HD Webcam 4K', sku: 'HDW-123', stock: 15, threshold: 25, status: 'low' },
]

const getStatusColor = (status: string) => {
  switch (status) {
    case 'completed':
      return 'bg-green-100 text-green-700'
    case 'processing':
      return 'bg-blue-100 text-blue-700'
    case 'pending':
      return 'bg-yellow-100 text-yellow-700'
    case 'critical':
      return 'bg-red-100 text-red-700'
    case 'low':
      return 'bg-orange-100 text-orange-700'
    default:
      return 'bg-gray-100 text-gray-700'
  }
}

const getStatusText = (status: string) => {
  switch (status) {
    case 'completed':
      return 'Hoàn thành'
    case 'processing':
      return 'Đang xử lý'
    case 'pending':
      return 'Chờ xử lý'
    case 'critical':
      return 'Nguy hiểm'
    case 'low':
      return 'Thấp'
    default:
      return status
  }
}

const getStatusIcon = (status: string) => {
  switch (status) {
    case 'completed':
      return <CheckCircle className="w-4 h-4" />
    case 'processing':
      return <Clock className="w-4 h-4" />
    case 'pending':
      return <AlertCircle className="w-4 h-4" />
    default:
      return null
  }
}

export default function DashboardPage() {
  const { user } = useAuthStore()

  return (
    <div className="space-y-6">
      {/* Welcome Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold bg-gradient-to-r from-purple-600 to-teal-500 bg-clip-text text-transparent">
            Chào mừng trở lại, {user?.fullName || 'User'}!
          </h1>
          <p className="text-gray-500 mt-1">
            Tổng quan hoạt động kho hàng của bạn hôm nay
          </p>
        </div>
        <div className="flex gap-3">
          <Button variant="outline" className="hidden md:flex">
            <TrendingUp className="w-4 h-4 mr-2" />
            Báo cáo
          </Button>
          <Button className="bg-gradient-to-r from-purple-600 to-teal-500 hover:from-purple-700 hover:to-teal-600">
            <Package className="w-4 h-4 mr-2" />
            Đơn hàng mới
          </Button>
        </div>
      </div>

      {/* Stats Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        {stats.map((stat) => (
          <Card key={stat.title} className="border-0 shadow-lg hover:shadow-xl transition-shadow duration-300">
            <CardContent className="pt-6">
              <div className="flex items-start justify-between">
                <div className="flex-1">
                  <p className="text-sm font-medium text-gray-500">
                    {stat.title}
                  </p>
                  <h3 className="text-3xl font-bold text-gray-900 mt-2">
                    {stat.value}
                  </h3>
                  <div className="flex items-center gap-1 mt-3">
                    {stat.trend === 'up' ? (
                      <ArrowUpRight className="w-4 h-4 text-green-600" />
                    ) : (
                      <ArrowDownRight className="w-4 h-4 text-red-600" />
                    )}
                    <span
                      className={`text-sm font-semibold ${
                        stat.trend === 'up'
                          ? 'text-green-600'
                          : 'text-red-600'
                      }`}
                    >
                      {stat.change}
                    </span>
                    <span className="text-xs text-gray-500 ml-1">so với tháng trước</span>
                  </div>
                </div>
                <div className={`p-4 rounded-2xl ${stat.bgColor} shadow-md`}>
                  <stat.icon className="w-7 h-7 text-white" />
                </div>
              </div>
            </CardContent>
          </Card>
        ))}
      </div>

      {/* Recent Activity */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Recent Orders */}
        <Card className="border-0 shadow-lg">
          <CardHeader className="border-b border-gray-100">
            <div className="flex items-center justify-between">
              <div>
                <CardTitle className="text-lg">Đơn hàng gần đây</CardTitle>
                <CardDescription>Các đơn hàng mới nhất từ kho của bạn</CardDescription>
              </div>
              <Button variant="ghost" size="sm" className="text-purple-600 hover:text-purple-700 hover:bg-purple-50">
                Xem tất cả
              </Button>
            </div>
          </CardHeader>
          <CardContent className="pt-6">
            <div className="space-y-4">
              {recentOrders.map((order) => (
                <div key={order.id} className="flex items-center justify-between py-3 border-b border-gray-50 last:border-0 hover:bg-gray-50 px-3 rounded-lg transition-colors">
                  <div className="flex items-center gap-4">
                    <div className="w-12 h-12 bg-gradient-to-br from-purple-500 to-teal-500 rounded-xl flex items-center justify-center shadow-md">
                      <ShoppingCart className="w-6 h-6 text-white" />
                    </div>
                    <div>
                      <p className="text-sm font-semibold text-gray-900">
                        Đơn hàng #{order.id}
                      </p>
                      <p className="text-xs text-gray-500">
                        {order.customer}
                      </p>
                      <p className="text-xs text-gray-400 mt-0.5">
                        {order.time}
                      </p>
                    </div>
                  </div>
                  <div className="text-right">
                    <p className="text-sm font-bold text-gray-900">
                      ₫{order.amount.toLocaleString('vi-VN')}
                    </p>
                    <span className={`inline-flex items-center gap-1 text-xs font-medium px-2 py-1 rounded-full mt-1 ${getStatusColor(order.status)}`}>
                      {getStatusIcon(order.status)}
                      {getStatusText(order.status)}
                    </span>
                  </div>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>

        {/* Low Stock Alerts */}
        <Card className="border-0 shadow-lg">
          <CardHeader className="border-b border-gray-100">
            <div className="flex items-center justify-between">
              <div>
                <CardTitle className="text-lg">Cảnh báo tồn kho</CardTitle>
                <CardDescription>Sản phẩm cần bổ sung hàng</CardDescription>
              </div>
              <Button variant="ghost" size="sm" className="text-purple-600 hover:text-purple-700 hover:bg-purple-50">
                Xem tất cả
              </Button>
            </div>
          </CardHeader>
          <CardContent className="pt-6">
            <div className="space-y-4">
              {lowStockProducts.map((product, i) => (
                <div key={i} className="flex items-center justify-between py-3 border-b border-gray-50 last:border-0 hover:bg-gray-50 px-3 rounded-lg transition-colors">
                  <div className="flex items-center gap-4">
                    <div className={`w-12 h-12 rounded-xl flex items-center justify-center shadow-md ${
                      product.status === 'critical' 
                        ? 'bg-gradient-to-br from-red-500 to-red-600' 
                        : 'bg-gradient-to-br from-orange-500 to-orange-600'
                    }`}>
                      <Package className="w-6 h-6 text-white" />
                    </div>
                    <div>
                      <p className="text-sm font-semibold text-gray-900">
                        {product.name}
                      </p>
                      <p className="text-xs text-gray-500">
                        SKU: {product.sku}
                      </p>
                      <p className="text-xs text-gray-400 mt-0.5">
                        Ngưỡng: {product.threshold} đơn vị
                      </p>
                    </div>
                  </div>
                  <div className="text-right">
                    <p className="text-sm font-bold text-gray-900">
                      {product.stock} đơn vị
                    </p>
                    <span className={`inline-flex items-center gap-1 text-xs font-medium px-2 py-1 rounded-full mt-1 ${getStatusColor(product.status)}`}>
                      <AlertCircle className="w-3 h-3" />
                      {getStatusText(product.status)}
                    </span>
                  </div>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  )
}
