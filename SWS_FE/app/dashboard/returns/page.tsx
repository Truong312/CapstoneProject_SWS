'use client'

import { useState, useEffect } from 'react'
import { useRouter } from 'next/navigation'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'
import { Plus, RotateCcw, Eye } from 'lucide-react'
import { useToast } from '@/hooks/use-toast'
import { returnOrderApi, returnStatusApi } from '@/services/api/return.api'
import type { ReturnOrderListItem, ReturnStatus } from '@/lib/types'
import { DataTable } from '@/components/data-table'
import type { DataTableColumn } from '@/components/data-table'

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
  const [currentPage, setCurrentPage] = useState(1)
  const [pageSize, setPageSize] = useState(10)

  useEffect(() => {
    fetchStatuses()
  }, [])

  useEffect(() => {
    fetchOrders()
  }, [currentPage, pageSize])

  useEffect(() => {
    const timer = setTimeout(() => {
      if (currentPage === 1) {
        fetchOrders()
      } else {
        setCurrentPage(1)
      }
    }, 500)
    return () => clearTimeout(timer)
  }, [searchQuery, statusFilter, fromDate, toDate])

  const fetchStatuses = async () => {
    try {
      const response = await returnStatusApi.list()
      if (response.isSuccess && response.data) {
        setStatuses(response.data)
      }
    } catch (error) {
      console.error('Failed to fetch return statuses:', error)
      // Set default statuses if API fails
      setStatuses([
        { status: 'Pending', count: 0 },
        { status: 'Processed', count: 0 },
      ])
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

  const getStatusBadge = (status: string) => {
    const variants: Record<string, 'default' | 'secondary' | 'destructive' | 'outline'> = {
      Pending: 'outline',
      Processed: 'default',
      Approved: 'secondary',
      Completed: 'default',
      Rejected: 'destructive',
    }

    const labels: Record<string, string> = {
      Pending: 'Chờ xử lý',
      Processed: 'Đã xử lý',
      Approved: 'Đã duyệt',
      Completed: 'Hoàn thành',
      Rejected: 'Từ chối',
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

  // Define columns for DataTable
  const columns: DataTableColumn<ReturnOrderListItem>[] = [
    {
      key: 'returnOrderId',
      header: 'Mã đơn trả',
      cell: (order: ReturnOrderListItem) => (
        <span className="font-medium">#{order.returnOrderId}</span>
      ),
      sortable: true,
    },
    {
      key: 'exportOrderId',
      header: 'Mã đơn xuất',
      cell: (order: ReturnOrderListItem) => `#${order.exportOrderId}`,
      sortable: true,
    },
    {
      key: 'checkInTime',
      header: 'Thời gian nhận',
      cell: (order: ReturnOrderListItem) => formatDate(order.checkInTime),
      sortable: true,
    },
    {
      key: 'status',
      header: 'Trạng thái',
      cell: (order: ReturnOrderListItem) => getStatusBadge(order.status),
    },
    {
      key: 'checkedByName',
      header: 'Người kiểm tra',
      cell: (order: ReturnOrderListItem) => order.checkedByName || '-',
    },
    {
      key: 'note',
      header: 'Ghi chú',
      cell: (order: ReturnOrderListItem) => (
        <span className="max-w-xs truncate block">{order.note || '-'}</span>
      ),
    },
  ]

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

      {/* Filter Section */}
      <Card>
        <CardContent className="pt-6">
          <div className="grid gap-4 md:grid-cols-3">
            <Select value={statusFilter} onValueChange={setStatusFilter}>
              <SelectTrigger>
                <SelectValue placeholder="Lọc theo trạng thái" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">Tất cả trạng thái</SelectItem>
                {statuses.map((item, index) => (
                  <SelectItem key={index} value={item.status}>
                    {item.status === 'Pending' ? 'Chờ xử lý' : item.status === 'Processed' ? 'Đã xử lý' : item.status} ({item.count})
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>

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
        </CardContent>
      </Card>

      {/* DataTable */}
      <DataTable
        data={orders}
        columns={columns}
        keyField="returnOrderId"
        isLoading={isLoading}
        emptyMessage="Không có đơn trả hàng nào"
        emptyIcon={<RotateCcw className="h-12 w-12 mx-auto text-gray-400 mb-4" />}
        searchable
        searchPlaceholder="Tìm kiếm theo số đơn, khách hàng..."
        searchValue={searchQuery}
        onSearchChange={setSearchQuery}
        sortable
        pagination={{
          currentPage,
          pageSize,
          totalItems: orders.length,
          totalPages: Math.ceil(orders.length / pageSize),
        }}
        onPageChange={setCurrentPage}
        onPageSizeChange={(size) => {
          setPageSize(size)
          setCurrentPage(1)
        }}
        pageSizeOptions={[5, 10, 20, 50, 100]}
        actions={[
          {
            label: 'Xem',
            icon: <Eye className="h-4 w-4 mr-1" />,
            onClick: (order: ReturnOrderListItem) =>
              router.push(`/dashboard/returns/${order.returnOrderId}`),
            variant: 'ghost',
          },
        ]}
        onRowClick={(order: ReturnOrderListItem) =>
          router.push(`/dashboard/returns/${order.returnOrderId}`)
        }
        hoverable
        striped
      />
    </div>
  )
}
