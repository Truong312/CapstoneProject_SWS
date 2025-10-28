import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'

const Inventory = () => {
  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold tracking-tight">Inventory</h1>
        <p className="text-muted-foreground">
          Track and manage your warehouse inventory
        </p>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Inventory Overview</CardTitle>
        </CardHeader>
        <CardContent>
          <p className="text-sm text-muted-foreground">
            Inventory management features coming soon...
          </p>
        </CardContent>
      </Card>
    </div>
  )
}

export default Inventory
