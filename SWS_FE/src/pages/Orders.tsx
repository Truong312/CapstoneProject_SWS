import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'

const Orders = () => {
  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold tracking-tight">Orders</h1>
        <p className="text-muted-foreground">
          Manage customer orders and fulfillment
        </p>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Order Management</CardTitle>
        </CardHeader>
        <CardContent>
          <p className="text-sm text-muted-foreground">
            Order management features coming soon...
          </p>
        </CardContent>
      </Card>
    </div>
  )
}

export default Orders
