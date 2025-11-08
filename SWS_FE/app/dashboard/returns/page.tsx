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
import { Search, Plus, RotateCcw, Eye, Loader2 } from 'lucide-react'
import { useToast } from '@/hooks/use-toast'
import { returnOrderApi, returnStatusApi } from '@/services/api/return.api'
import type { ReturnOrderListItem, ReturnStatus } from '@/lib/types'

export default function ReturnsPage() {
  const router = useRouter()
  const { toast } = useToast()
  
  const [orders, setOrders] = useState<ReturnOrderListItem[]>([])
  const [statuses, setStatuses] = useState<ReturnStatus[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [searchQuery, setSearchQuery] = useState('')
  const [statusFilter, setStatusFilter] = useState<string>('all')
  const [fromDate, setFromDate] = useState<string>('')
  const [toDate, setToDate] = useState<string>('')

  useEffect(() => {
    fetchStatuses()
    fetchOrders()
  }, [])

  useEffect(() => {
    fetchOrders()
  }, [searchQuery, statusFilter, fromDate, toDate])

  const fetchStatuses = async () => {
    try {
      const response = await returnStatusApi.list()
      if (response.isSuccess && response.data) {
        setStatuses(response.data)
      }
    } catch (error) {
      console.error('Failed to fetch return statuses:', error)
    }
  }

  const fetchOrders = async () => {
    try {
      setIsLoading(true)
      const response = await returnOrderApi.list({
        q: searchQuery || undefined,
        status: statusFilter !== 'all' ? statusFilter : undefined,
        from: fromDate || undefined,
        to: toDate || undefined,
      })

      if (response.isSuccess && response.data) {
        setOrders(Array.isArray(response.data) ? response.data : [])
      }
    } catch (error: any) {
      toast({
        variant: 'destructive',
        title: 'Lỗi',
        description: error.response?.data?.message || 'Không thể tải danh sách đơn trả hàng',
      })
    } finally {
      setIsLoading(false)
    }
  }

  const getStatusBadge = (statusCode: string) => {
    const variants: Record<string, 'default' | 'secondary' | 'destructive' | 'outline'> = {
      Pending: 'outline',
      Approved: 'secondary',
      Completed: 'default',
      Rejected: 'destructive',
    }

    return (
      <Badge variant={variants[statusCode] || 'outline'}>
        {statusCode}
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
            Đơn Trả Hàng
          </h1>
          <p className="text-gray-500 mt-1">
            Quản lý đơn trả hàng từ khách hàng
          </p>
        </div>
        <Button
          onClick={() => router.push('/dashboard/returns/new')}
          className="bg-gradient-to-r from-purple-600 to-teal-600 hover:from-purple-700 hover:to-teal-700"
        >
          <Plus className="mr-2 h-4 w-4" />
          Tạo Đơn Trả Hàng
        </Button>
      </div>

      <Card>
        <CardHeader>
          <CardTitle className="text-lg">Tìm kiếm & Lọc</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="grid gap-4 md:grid-cols-4">
            <div className="relative md:col-span-2">
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
              <Input
                placeholder="Tìm kiếm theo số đơn, khách hàng..."
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                className="pl-10"
              />
            </div>
            
            <Select value={statusFilter} onValueChange={setStatusFilter}>
              <SelectTrigger>
                <SelectValue placeholder="Trạng thái" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">Tất cả</SelectItem>
                {statuses.map((status) => (
                  <SelectItem key={status.statusId} value={status.statusCode}>
                    {status.description || status.statusCode}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>

            <div className="grid grid-cols-2 gap-2">
              <Input
                type="date"
                placeholder="Từ ngày"
                value={fromDate}
                onChange={(e) => setFromDate(e.target.value)}
              />
              <Input
                type="date"
                placeholder="Đến ngày"
                value={toDate}
                onChange={(e) => setToDate(e.target.value)}
              />
            </div>
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <div className="flex items-center justify-between">
            <CardTitle>
              Danh sách đơn trả hàng ({orders.length})
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
              <RotateCcw className="h-12 w-12 mx-auto text-gray-400 mb-4" />
              <p className="text-gray-500">Không có đơn trả hàng nào</p>
            </div>
          ) : (
            <div className="rounded-md border">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Số đơn trả</TableHead>
                    <TableHead>Ngày trả</TableHead>
                    <TableHead>Khách hàng</TableHead>
                    <TableHead className="text-center">Số SP</TableHead>
                    <TableHead>Trạng thái</TableHead>
                    <TableHead className="text-right">Thao tác</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {orders.map((order) => (
                    <TableRow key={order.returnOrderId}>
                      <TableCell className="font-medium">
                        {order.returnNumber}
                      </TableCell>
                      <TableCell>{formatDate(order.returnDate)}</TableCell>
                      <TableCell>{order.customerName || '-'}</TableCell>
                      <TableCell className="text-center">
                        {order.totalItems}
                      </TableCell>
                      <TableCell>{getStatusBadge(order.statusCode)}</TableCell>
                      <TableCell className="text-right">
                        <Button
                          variant="ghost"
                          size="sm"
                          onClick={() =>
                            router.push(`/dashboard/returns/${order.returnOrderId}`)
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
