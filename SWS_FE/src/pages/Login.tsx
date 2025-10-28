import { useState, FormEvent } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuthStore } from '@/store/authStore'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { useToast } from '@/hooks/use-toast'

const Login = () => {
  const navigate = useNavigate()
  const { login, register } = useAuthStore()
  const { toast } = useToast()
  const [isLoginMode, setIsLoginMode] = useState(true)
  
  // Form fields
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const [name, setName] = useState('')
  const [phone, setPhone] = useState('')
  const [address, setAddress] = useState('')
  
  const [isLoading, setIsLoading] = useState(false)

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault()
    setIsLoading(true)

    try {
      if (isLoginMode) {
        // Login with email
        await login(email, password)
        toast({
          title: 'Login successful',
          description: 'Welcome back!',
        })
        navigate('/')
      } else {
        // Register - validate
        if (password.length < 8) {
          toast({
            variant: 'destructive',
            title: 'Validation error',
            description: 'Password must be at least 8 characters',
          })
          setIsLoading(false)
          return
        }

        if (password !== confirmPassword) {
          toast({
            variant: 'destructive',
            title: 'Validation error',
            description: 'Password and Confirm Password does not match',
          })
          setIsLoading(false)
          return
        }

        await register(name, email, password, confirmPassword, phone, address)
        toast({
          title: 'Registration successful',
          description: 'You can now login with your credentials',
        })
        setIsLoginMode(true)
        // Clear form
        setName('')
        setEmail('')
        setPassword('')
        setConfirmPassword('')
        setPhone('')
        setAddress('')
      }
    } catch (error: any) {
      console.error('Error:', error)
      
      // Extract error message from response
      let errorMessage = 'An error occurred'
      
      if (error?.response?.data) {
        const data = error.response.data
        if (typeof data === 'string') {
          errorMessage = data
        } else if (data.message) {
          errorMessage = data.message
        } else if (data.errors) {
          // Handle validation errors
          const validationErrors = Object.entries(data.errors)
            .map(([field, messages]: [string, any]) => {
              const msgs = Array.isArray(messages) ? messages : [messages]
              return `${field}: ${msgs.join(', ')}`
            })
          errorMessage = validationErrors.join('\n')
        } else if (data.title) {
          errorMessage = data.title
        }
      } else if (error?.message) {
        errorMessage = error.message
      }

      toast({
        variant: 'destructive',
        title: isLoginMode ? 'Login failed' : 'Registration failed',
        description: errorMessage,
      })
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <div className="flex min-h-screen items-center justify-center bg-background">
      <Card className="w-full max-w-md">
        <CardHeader className="space-y-1">
          <CardTitle className="text-2xl font-bold">
            {isLoginMode ? 'Login' : 'Register'}
          </CardTitle>
          <CardDescription>
            {isLoginMode 
              ? 'Enter your username/email and password to access your account'
              : 'Create a new account to get started'
            }
          </CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="email">Email *</Label>
              <Input
                id="email"
                type="email"
                placeholder="testuser@example.com"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
              />
            </div>

            {!isLoginMode && (
              <>
                <div className="space-y-2">
                  <Label htmlFor="name">Name (Optional)</Label>
                  <Input
                    id="name"
                    type="text"
                    placeholder="Test User"
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                  />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="phone">Phone (Optional)</Label>
                  <Input
                    id="phone"
                    type="tel"
                    placeholder="0123456789"
                    value={phone}
                    onChange={(e) => setPhone(e.target.value)}
                  />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="address">Address (Optional)</Label>
                  <Input
                    id="address"
                    type="text"
                    placeholder="123 Test Street"
                    value={address}
                    onChange={(e) => setAddress(e.target.value)}
                  />
                </div>
              </>
            )}

            <div className="space-y-2">
              <Label htmlFor="password">Password *</Label>
              <Input
                id="password"
                type="password"
                placeholder={isLoginMode ? "Enter password" : "Min 8 chars with letter & number"}
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
                minLength={isLoginMode ? undefined : 8}
              />
              {!isLoginMode && (
                <p className="text-xs text-muted-foreground">
                  Must have at least 8 characters with letter and number (e.g., Test@123456)
                </p>
              )}
            </div>

            {!isLoginMode && (
              <div className="space-y-2">
                <Label htmlFor="confirmPassword">Confirm Password *</Label>
                <Input
                  id="confirmPassword"
                  type="password"
                  placeholder="Re-enter password"
                  value={confirmPassword}
                  onChange={(e) => setConfirmPassword(e.target.value)}
                  required
                />
              </div>
            )}

            <Button type="submit" className="w-full" disabled={isLoading}>
              {isLoading 
                ? (isLoginMode ? 'Logging in...' : 'Registering...') 
                : (isLoginMode ? 'Login' : 'Register')
              }
            </Button>

            <div className="text-center text-sm">
              <button
                type="button"
                onClick={() => {
                  setIsLoginMode(!isLoginMode)
                  setEmail('')
                  setPassword('')
                  setConfirmPassword('')
                  setName('')
                  setPhone('')
                  setAddress('')
                }}
                className="text-primary hover:underline"
              >
                {isLoginMode 
                  ? "Don't have an account? Register" 
                  : 'Already have an account? Login'
                }
              </button>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  )
}

export default Login
