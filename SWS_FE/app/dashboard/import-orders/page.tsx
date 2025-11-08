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
import { Search, Plus, FileText, Eye, Loader2 } from 'lucide-react'
import { useToast } from '@/hooks/use-toast'
import { importOrderApi } from '@/services/api/order.api'
import type { ImportOrderListItem, OrderStatus } from '@/lib/types'

export default function ImportOrdersPage() {
  const router = useRouter()
  const { toast } = useToast()
  
  const [orders, setOrders] = useState<ImportOrderListItem[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [searchQuery, setSearchQuery] = useState('')
  const [statusFilter, setStatusFilter] = useState<string>('all')
  const [currentPage, setCurrentPage] = useState(1)
  const [totalPages, setTotalPages] = useState(1)
  const [totalItems, setTotalItems] = useState(0)
  const pageSize = 10

  useEffect(() => {
    fetchOrders()
  }, [currentPage, searchQuery, statusFilter])

  const fetchOrders = async () => {
    try {
      setIsLoading(true)
      const response = await importOrderApi.list({
        q: searchQuery || undefined,
        status: statusFilter !== 'all' ? statusFilter : undefined,
        page: currentPage,
        pageSize,
      })

      if (response.isSuccess && response.data) {
        setOrders(response.data.items || [])
        setTotalItems(response.data.total || 0)
        setTotalPages(Math.ceil((response.data.total || 0) / pageSize))
      }
    } catch (error: any) {
      toast({
        variant: 'destructive',
        title: 'Lỗi',
        description: error.response?.data?.message || 'Không thể tải danh sách đơn nhập hàng',
      })
    } finally {
      setIsLoading(false)
    }
  }

  const getStatusBadge = (status: string) => {
    const variants: Record<string, 'default' | 'secondary' | 'destructive' | 'outline'> = {
      Pending: 'outline',
      Approved: 'secondary',
      Completed: 'default',
      Cancelled: 'destructive',
    }

    const labels: Record<string, string> = {
      Pending: 'Chờ duyệt',
      Approved: 'Đã duyệt',
      Completed: 'Hoàn thành',
      Cancelled: 'Đã hủy',
    }

    return (
      <Badge variant={variants[status] || 'outline'}>
        {labels[status] || status}
      </Badge>
    )
  }

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('vi-VN', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
    })
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold bg-gradient-to-r from-purple-600 to-teal-600 bg-clip-text text-transparent">
            Đơn Nhập Hàng
          </h1>
          <p className="text-gray-500 mt-1">
            Quản lý đơn nhập hàng từ nhà cung cấp
          </p>
        </div>
        <Button
          onClick={() => router.push('/dashboard/import-orders/new')}
          className="bg-gradient-to-r from-purple-600 to-teal-600 hover:from-purple-700 hover:to-teal-700"
        >
          <Plus className="mr-2 h-4 w-4" />
          Tạo Đơn Nhập
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
                placeholder="Tìm kiếm theo số hóa đơn, nhà cung cấp..."
                value={searchQuery}
                onChange={(e) => {
                  setSearchQuery(e.target.value)
                  setCurrentPage(1)
                }}
                className="pl-10"
              />
            </div>
            <Select
              value={statusFilter}
              onValueChange={(value) => {
                setStatusFilter(value)
                setCurrentPage(1)
              }}
            >
              <SelectTrigger className="w-full sm:w-[180px]">
                <SelectValue placeholder="Trạng thái" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">Tất cả</SelectItem>
                <SelectItem value="Pending">Chờ duyệt</SelectItem>
                <SelectItem value="Approved">Đã duyệt</SelectItem>
                <SelectItem value="Completed">Hoàn thành</SelectItem>
                <SelectItem value="Cancelled">Đã hủy</SelectItem>
              </SelectContent>
            </Select>
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <div className="flex items-center justify-between">
            <CardTitle>
              Danh sách đơn nhập ({totalItems})
            </CardTitle>
          </div>
        </CardHeader>
        <CardContent>
          {isLoading ? (
            <div className="flex items-center justify-center py-12">
              <Loader2 className="h-8 w-8 animate-spin text-purple-600" />
            </div>
          ) : orders.length === 0 ? (
            <div className="text-center py-12">
              <FileText className="h-12 w-12 mx-auto text-gray-400 mb-4" />
              <p className="text-gray-500">Không có đơn nhập hàng nào</p>
            </div>
          ) : (
            <>
              <div className="rounded-md border">
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>Số hóa đơn</TableHead>
                      <TableHead>Ngày đặt</TableHead>
                      <TableHead>Nhà cung cấp</TableHead>
                      <TableHead className="text-center">Số lượng SP</TableHead>
                      <TableHead>Trạng thái</TableHead>
                      <TableHead>Người tạo</TableHead>
                      <TableHead className="text-right">Thao tác</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {orders.map((order) => (
                      <TableRow key={order.importOrderId}>
                        <TableCell className="font-medium">
                          {order.invoiceNumber}
                        </TableCell>
                        <TableCell>{formatDate(order.orderDate)}</TableCell>
                        <TableCell>{order.providerName}</TableCell>
                        <TableCell className="text-center">
                          {order.totalItems}
                        </TableCell>
                        <TableCell>{getStatusBadge(order.status)}</TableCell>
                        <TableCell>{order.createdByName}</TableCell>
                        <TableCell className="text-right">
                          <Button
                            variant="ghost"
                            size="sm"
                            onClick={() =>
                              router.push(`/dashboard/import-orders/${order.importOrderId}`)
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

              {/* Pagination */}
              <div className="flex items-center justify-between mt-4">
                <p className="text-sm text-gray-500">
                  Hiển thị {(currentPage - 1) * pageSize + 1} -{' '}
                  {Math.min(currentPage * pageSize, totalItems)} trong số {totalItems}
                </p>
                <div className="flex gap-2">
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => setCurrentPage((prev) => Math.max(1, prev - 1))}
                    disabled={currentPage === 1}
                  >
                    Trước
                  </Button>
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => setCurrentPage((prev) => Math.min(totalPages, prev + 1))}
                    disabled={currentPage === totalPages}
                  >
                    Sau
                  </Button>
                </div>
              </div>
            </>
          )}
        </CardContent>
      </Card>
    </div>
  )
}
