import { useState } from 'react'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import authService from '@/services/authService'
import { useToast } from '@/hooks/use-toast'

const Settings = () => {
  const { toast } = useToast()
  const [findProperty, setFindProperty] = useState<'email' | 'name' | 'phone'>('email')
  const [findValue, setFindValue] = useState('')
  const [findResult, setFindResult] = useState<any>(null)

  const handleFindUser = async () => {
    try {
      const result = await authService.findUser(findProperty, findValue)
      setFindResult(result)
      toast({
        title: 'User(s) found',
        description: `Found ${result.length} user(s)`,
      })
    } catch (error: any) {
      toast({
        variant: 'destructive',
        title: 'Error',
        description: error?.response?.data?.message || 'No matching users found',
      })
      setFindResult(null)
    }
  }

  const testExamples = [
    {
      title: 'Test User 1',
      data: {
        name: 'Test User',
        email: 'testuser@example.com',
        password: 'Test@123456',
        confirmPassword: 'Test@123456',
        phone: '0123456789',
        address: '123 Test Street',
        role: 'User (0)'
      }
    },
    {
      title: 'Admin User',
      data: {
        name: 'Admin User',
        email: 'admin@example.com',
        password: 'Admin@123456',
        confirmPassword: 'Admin@123456',
        phone: '0987654321',
        address: '456 Admin Avenue',
        role: 'Admin (1)'
      }
    }
  ]

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold tracking-tight">Settings & API Testing</h1>
        <p className="text-muted-foreground">
          Test API endpoints and configure system
        </p>
      </div>

      <div className="grid gap-4 md:grid-cols-2">
        <Card>
          <CardHeader>
            <CardTitle>Find User (Debug)</CardTitle>
            <CardDescription>
              Search for a user by username, email, or full name
            </CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="space-y-2">
              <Label>Search By</Label>
                            <select
                className="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm"
                value={findProperty}
                onChange={(e) => setFindProperty(e.target.value as any)}
              >
                <option value="email">Email</option>
                <option value="name">Name</option>
                <option value="phone">Phone</option>
              </select>
            </div>

            <div className="space-y-2">
              <Label>Search Value</Label>
              <Input
                placeholder="Enter value to search"
                value={findValue}
                onChange={(e) => setFindValue(e.target.value)}
              />
            </div>

            <Button onClick={handleFindUser} className="w-full">
              Find User
            </Button>

            {findResult && (
              <div className="mt-4 rounded-lg border p-4 bg-muted">
                <h4 className="font-semibold mb-2">Result ({Array.isArray(findResult) ? findResult.length : 1} user(s)):</h4>
                <pre className="text-xs overflow-auto max-h-96">
                  {JSON.stringify(findResult, null, 2)}
                </pre>
              </div>
            )}
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>API Examples</CardTitle>
            <CardDescription>
              Sample data for testing registration
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              {testExamples.map((example, index) => (
                <div key={index} className="rounded-lg border p-4 bg-muted">
                  <h4 className="font-semibold mb-2">{example.title}</h4>
                  <div className="text-xs space-y-1">
                    {Object.entries(example.data).map(([key, value]) => (
                      <div key={key}>
                        <strong>{key}:</strong> {value}
                      </div>
                    ))}
                  </div>
                </div>
              ))}

                            <div className="rounded-lg border p-4 bg-blue-50 dark:bg-blue-950">
                <h4 className="font-semibold mb-2">Password Requirements</h4>
                <ul className="text-xs space-y-1 list-disc list-inside">
                  <li>Minimum 8 characters</li>
                  <li>At least one letter and one number</li>
                  <li>Can include special characters (@, #, etc.)</li>
                  <li>Example: Test@123456</li>
                </ul>
              </div>

              <div className="rounded-lg border p-4 bg-green-50 dark:bg-green-950">
                <h4 className="font-semibold mb-2">Role Information</h4>
                <ul className="text-xs space-y-1 list-disc list-inside">
                  <li>0 = User (Default)</li>
                  <li>1 = Admin</li>
                  <li>Registration defaults to User role</li>
                </ul>
              </div>

              <div className="rounded-lg border p-4 bg-green-50 dark:bg-green-950">
                <h4 className="font-semibold mb-2">Required vs Optional Fields</h4>
                <div className="text-xs space-y-1">
                  <div><strong>Required:</strong> email, password, confirmPassword</div>
                  <div><strong>Optional:</strong> name, phone, address</div>
                  <div><strong>Role:</strong> 0 = User (default), 1 = Admin</div>
                </div>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>API Endpoints</CardTitle>
          <CardDescription>
            Available API endpoints for User management
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="space-y-2 text-sm">
            <div className="flex items-center gap-2 p-2 rounded bg-muted">
              <span className="font-mono bg-green-100 dark:bg-green-900 px-2 py-1 rounded text-xs">POST</span>
              <code>/api/User/Register</code>
            </div>
            <div className="flex items-center gap-2 p-2 rounded bg-muted">
              <span className="font-mono bg-blue-100 dark:bg-blue-900 px-2 py-1 rounded text-xs">POST</span>
              <code>/api/User/Login</code>
            </div>
            <div className="flex items-center gap-2 p-2 rounded bg-muted">
              <span className="font-mono bg-blue-100 dark:bg-blue-900 px-2 py-1 rounded text-xs">POST</span>
              <code>/api/User/Logout</code>
            </div>
            <div className="flex items-center gap-2 p-2 rounded bg-muted">
              <span className="font-mono bg-yellow-100 dark:bg-yellow-900 px-2 py-1 rounded text-xs">GET</span>
              <code>/api/User/find?property=email&value=testuser@example.com</code>
            </div>
            <div className="flex items-center gap-2 p-2 rounded bg-muted">
              <span className="font-mono bg-yellow-100 dark:bg-yellow-900 px-2 py-1 rounded text-xs">GET</span>
              <code>/api/User/find?property=email&value=testuser@example.com</code>
            </div>
            <div className="flex items-center gap-2 p-2 rounded bg-muted">
              <span className="font-mono bg-yellow-100 dark:bg-yellow-900 px-2 py-1 rounded text-xs">GET</span>
              <code>/api/User/find?property=name&value=Test User</code>
            </div>
            <div className="flex items-center gap-2 p-2 rounded bg-muted">
              <span className="font-mono bg-yellow-100 dark:bg-yellow-900 px-2 py-1 rounded text-xs">GET</span>
              <code>/api/User/find?property=phone&value=0123456789</code>
            </div>
            <div className="flex items-center gap-2 p-2 rounded bg-muted">
              <span className="font-mono bg-yellow-100 dark:bg-yellow-900 px-2 py-1 rounded text-xs">GET</span>
              <code>/api/User/find?property=phone&value=0123456789</code>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  )
}

export default Settings
