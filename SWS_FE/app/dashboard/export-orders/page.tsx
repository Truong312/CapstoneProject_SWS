'use client'

import { useState, useEffect } from 'react'
import { useRouter } from 'next/navigation'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table'
import { Badge } from '@/components/ui/badge'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'
import { Search, Plus, FileText, Eye, Loader2, Package } from 'lucide-react'
import { useToast } from '@/hooks/use-toast'
import { 
  getAllExportOrders, 
  getExportOrdersByStatus,
  getExportOrderStatuses,
  ExportOrderStatusStats 
} from '@/services/api/export-orders.api'
import type { ExportOrderListItem } from '@/lib/types'

export default function ExportOrdersPage() {
  const router = useRouter()
  const { toast } = useToast()
  
  const [orders, setOrders] = useState<ExportOrderListItem[]>([])
  const [filteredOrders, setFilteredOrders] = useState<ExportOrderListItem[]>([])
  const [statuses, setStatuses] = useState<ExportOrderStatusStats[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [searchQuery, setSearchQuery] = useState('')
  const [statusFilter, setStatusFilter] = useState<string>('all')

  // Status mapping: API uses numbers
  const exportStatuses = [
    { value: 0, label: 'Chờ duyệt', apiName: 'Pending' },
    { value: 1, label: 'Đã duyệt', apiName: 'Approved' },
    { value: 2, label: 'Hoàn thành', apiName: 'Completed' },
  ]

  useEffect(() => {
    fetchOrders()
  }, [statusFilter])

  useEffect(() => {
    filterOrders()
  }, [orders, searchQuery])

  const fetchOrders = async () => {
    try {
      setIsLoading(true)
      
      let data
      if (statusFilter === 'all') {
        data = await getAllExportOrders()
      } else {
        data = await getExportOrdersByStatus(Number(statusFilter))
      }

      setOrders(Array.isArray(data) ? data : [])
    } catch (error: any) {
      toast({
        variant: 'destructive',
        title: 'Lỗi',
        description: error.response?.data?.message || 'Không thể tải danh sách đơn xuất hàng',
      })
      setOrders([])
    } finally {
      setIsLoading(false)
    }
  }

  const filterOrders = () => {
    if (!searchQuery) {
      setFilteredOrders(orders)
      return
    }

    const query = searchQuery.toLowerCase()
    const filtered = orders.filter(
      (order) =>
        order.invoiceNumber.toLowerCase().includes(query) ||
        order.customerName?.toLowerCase().includes(query)
    )
    setFilteredOrders(filtered)
  }

  const getStatusBadge = (status: number | null) => {
    // Handle null or undefined status
    if (status === null || status === undefined) {
      return <Badge variant="outline">Chưa xác định</Badge>
    }

    const statusConfig: Record<number, { variant: 'default' | 'secondary' | 'destructive' | 'outline', label: string }> = {
      0: { variant: 'outline', label: 'Chờ duyệt' },
      1: { variant: 'secondary', label: 'Đã duyệt' },
      2: { variant: 'default', label: 'Hoàn thành' },
    }

    const config = statusConfig[status]
    if (!config) {
      return <Badge variant="outline">Không xác định</Badge>
    }

    return <Badge variant={config.variant}>{config.label}</Badge>
  }

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('vi-VN', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
    })
  }

  const formatCurrency = (amount?: number) => {
    if (!amount) return '-'
    return new Intl.NumberFormat('vi-VN', {
      style: 'currency',
      currency: 'VND',
    }).format(amount)
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold bg-gradient-to-r from-purple-600 to-teal-600 bg-clip-text text-transparent">
            Đơn Xuất Hàng
          </h1>
          <p className="text-gray-500 mt-1">
            Quản lý đơn xuất hàng cho khách hàng
          </p>
        </div>
        <Button
          onClick={() => router.push('/dashboard/export-orders/new')}
          className="bg-gradient-to-r from-purple-600 to-teal-600 hover:from-purple-700 hover:to-teal-700"
        >
          <Plus className="mr-2 h-4 w-4" />
          Tạo Đơn Xuất
        </Button>
      </div>

      <Card>
        <CardHeader>
          <CardTitle className="text-lg">Tìm kiếm & Lọc</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="flex flex-col sm:flex-row gap-4">
            <div className="relative flex-1">
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
              <Input
                placeholder="Tìm kiếm theo số hóa đơn, khách hàng..."
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                className="pl-10"
              />
            </div>
            <Select
              value={statusFilter}
              onValueChange={(value) => setStatusFilter(value)}
            >
              <SelectTrigger className="w-full sm:w-[200px]">
                <SelectValue placeholder="Tất cả trạng thái" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">Tất cả trạng thái</SelectItem>
                {exportStatuses.map((item) => (
                  <SelectItem key={item.status} value={item.status}>
                    {item.label}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <div className="flex items-center justify-between">
            <CardTitle>
              Danh sách đơn xuất ({filteredOrders.length})
            </CardTitle>
          </div>
        </CardHeader>
        <CardContent>
          {isLoading ? (
            <div className="flex items-center justify-center py-12">
              <Loader2 className="h-8 w-8 animate-spin text-purple-600" />
            </div>
          ) : filteredOrders.length === 0 ? (
            <div className="text-center py-12">
              <Package className="h-12 w-12 mx-auto text-gray-400 mb-4" />
              <p className="text-gray-500">Không có đơn xuất hàng nào</p>
            </div>
          ) : (
            <div className="rounded-md border">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Số hóa đơn</TableHead>
                    <TableHead>Ngày đặt</TableHead>
                    <TableHead>Khách hàng</TableHead>
                    <TableHead>Trạng thái</TableHead>
                    <TableHead className="text-right">Tổng tiền</TableHead>
                    <TableHead className="text-right">Thao tác</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {filteredOrders.map((order) => (
                    <TableRow key={order.exportOrderId}>
                      <TableCell className="font-medium">
                        {order.invoiceNumber}
                      </TableCell>
                      <TableCell>{formatDate(order.orderDate)}</TableCell>
                      <TableCell>{order.customerName || `KH-${order.customerId}`}</TableCell>
                      <TableCell>{getStatusBadge(order.status)}</TableCell>
                      <TableCell className="text-right font-medium">
                        {formatCurrency(order.totalPayment)}
                      </TableCell>
                      <TableCell className="text-right">
                        <Button
                          variant="ghost"
                          size="sm"
                          onClick={() =>
                            router.push(`/dashboard/export-orders/${order.exportOrderId}`)
                          }
                        >
                          <Eye className="h-4 w-4 mr-1" />
                          Xem
                        </Button>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  )
}
