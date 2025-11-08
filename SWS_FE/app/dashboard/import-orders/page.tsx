'use client'

import { useState, useEffect } from 'react'
import { useRouter } from 'next/navigation'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'
import { Plus, FileText, Eye } from 'lucide-react'
import { useToast } from '@/hooks/use-toast'
import { importOrderApi } from '@/services/api/order.api'
import type { ImportOrderListItem } from '@/lib/types'
import { DataTable } from '@/components/data-table'
import type { DataTableColumn } from '@/components/data-table'

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
  const [pageSize, setPageSize] = useState(10)

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
  }, [searchQuery, statusFilter])

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

  // Define columns for DataTable
  const columns: DataTableColumn<ImportOrderListItem>[] = [
    {
      key: 'invoiceNumber',
      header: 'Số hóa đơn',
      cell: (order: ImportOrderListItem) => (
        <span className="font-medium">{order.invoiceNumber}</span>
      ),
      sortable: true,
    },
    {
      key: 'orderDate',
      header: 'Ngày đặt',
      cell: (order: ImportOrderListItem) => formatDate(order.orderDate),
      sortable: true,
    },
    {
      key: 'providerName',
      header: 'Nhà cung cấp',
      cell: (order: ImportOrderListItem) => order.providerName,
      sortable: true,
    },
    {
      key: 'totalItems',
      header: 'Số lượng SP',
      cell: (order: ImportOrderListItem) => order.totalItems,
      className: 'text-center',
      headerClassName: 'text-center',
    },
    {
      key: 'status',
      header: 'Trạng thái',
      cell: (order: ImportOrderListItem) => getStatusBadge(order.status),
    },
    {
      key: 'createdByName',
      header: 'Người tạo',
      cell: (order: ImportOrderListItem) => order.createdByName,
    },
  ]

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

      {/* Filter Section */}
      <Card>
        <CardContent className="pt-6">
          <div className="flex flex-col sm:flex-row gap-4">
            <Select
              value={statusFilter}
              onValueChange={(value) => {
                setStatusFilter(value)
                setCurrentPage(1)
              }}
            >
              <SelectTrigger className="w-full sm:w-[200px]">
                <SelectValue placeholder="Lọc theo trạng thái" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">Tất cả trạng thái</SelectItem>
                <SelectItem value="Pending">Chờ duyệt</SelectItem>
                <SelectItem value="Approved">Đã duyệt</SelectItem>
                <SelectItem value="Completed">Hoàn thành</SelectItem>
                <SelectItem value="Cancelled">Đã hủy</SelectItem>
              </SelectContent>
            </Select>
          </div>
        </CardContent>
      </Card>

      {/* DataTable */}
      <DataTable
        data={orders}
        columns={columns}
        keyField="importOrderId"
        isLoading={isLoading}
        emptyMessage="Không có đơn nhập hàng nào"
        emptyIcon={<FileText className="h-12 w-12 mx-auto text-gray-400 mb-4" />}
        searchable
        searchPlaceholder="Tìm kiếm theo số hóa đơn, nhà cung cấp..."
        searchValue={searchQuery}
        onSearchChange={setSearchQuery}
        sortable
        pagination={{
          currentPage,
          pageSize,
          totalItems,
          totalPages,
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
            onClick: (order: ImportOrderListItem) =>
              router.push(`/dashboard/import-orders/${order.importOrderId}`),
            variant: 'ghost',
          },
        ]}
        onRowClick={(order: ImportOrderListItem) =>
          router.push(`/dashboard/import-orders/${order.importOrderId}`)
        }
        hoverable
        striped
      />
    </div>
  )
}
