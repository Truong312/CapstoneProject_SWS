import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Package, Warehouse, ShoppingCart, TrendingUp } from 'lucide-react'
import { useAuthStore } from '@/store/authStore'

const Dashboard = () => {
  const { user } = useAuthStore()

  // Mock data cho demo - sẽ thay bằng API thật khi backend có
  const stats = {
    totalProducts: 0,
    totalInventory: 0,
    totalOrders: 0,
    revenue: 0,
  }

  const statCards = [
    {
      title: 'Total Products',
      value: stats.totalProducts,
      icon: Package,
      color: 'text-blue-600',
    },
    {
      title: 'Inventory Items',
      value: stats.totalInventory,
      icon: Warehouse,
      color: 'text-green-600',
    },
    {
      title: 'Total Orders',
      value: stats.totalOrders,
      icon: ShoppingCart,
      color: 'text-orange-600',
    },
    {
      title: 'Revenue',
      value: `$${stats.revenue.toLocaleString()}`,
      icon: TrendingUp,
      color: 'text-purple-600',
    },
  ]

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold tracking-tight">Dashboard</h1>
        <p className="text-muted-foreground">
          Welcome back, {user?.name || user?.email || 'User'}!
        </p>
      </div>

      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        {statCards.map((stat) => (
          <Card key={stat.title}>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">
                {stat.title}
              </CardTitle>
              <stat.icon className={`h-4 w-4 ${stat.color}`} />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">{stat.value}</div>
            </CardContent>
          </Card>
        ))}
      </div>

      <div className="grid gap-4 md:grid-cols-2">
        <Card>
          <CardHeader>
            <CardTitle>User Information</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-2">
              <div className="flex justify-between">
                <span className="text-sm font-medium">Email:</span>
                <span className="text-sm text-muted-foreground">{user?.email}</span>
              </div>
              {user?.name && (
                <div className="flex justify-between">
                  <span className="text-sm font-medium">Name:</span>
                  <span className="text-sm text-muted-foreground">{user.name}</span>
                </div>
              )}
              {user?.phone && (
                <div className="flex justify-between">
                  <span className="text-sm font-medium">Phone:</span>
                  <span className="text-sm text-muted-foreground">{user.phone}</span>
                </div>
              )}
              {user?.address && (
                <div className="flex justify-between">
                  <span className="text-sm font-medium">Address:</span>
                  <span className="text-sm text-muted-foreground">{user.address}</span>
                </div>
              )}
              <div className="flex justify-between">
                <span className="text-sm font-medium">Role:</span>
                <span className="text-sm text-muted-foreground">
                  {user?.role === 1 ? 'Admin' : 'User'}
                </span>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Quick Actions</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-2">
              <p className="text-sm text-muted-foreground">
                Quick action buttons coming soon
              </p>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  )
}

export default Dashboard
