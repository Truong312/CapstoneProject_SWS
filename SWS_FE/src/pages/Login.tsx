import { useState, FormEvent, useEffect } from 'react'
import { useNavigate, useSearchParams } from 'react-router-dom'
import { useAuthStore } from '@/store/authStore'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { useToast } from '@/hooks/use-toast'
import { Logo } from '@/components/Logo'
import { Eye, EyeOff, Mail, Lock, User, Phone, MapPin } from 'lucide-react'
import authService from '@/services/authService'

const Login = () => {
  const navigate = useNavigate()
  const [searchParams] = useSearchParams()
  const { login, register, setToken } = useAuthStore()
  const { toast } = useToast()
  const [isLoginMode, setIsLoginMode] = useState(true)
  const [showPassword, setShowPassword] = useState(false)
  
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const [name, setName] = useState('')
  const [phone, setPhone] = useState('')
  const [address, setAddress] = useState('')
  
  const [isLoading, setIsLoading] = useState(false)
  const [googleAuthUrl, setGoogleAuthUrl] = useState('')

  // Handle OAuth callback
  useEffect(() => {
    const token = searchParams.get('token')
    const isNewUser = searchParams.get('isNewUser')
    const error = searchParams.get('error')

    if (error) {
      toast({
        variant: 'destructive',
        title: 'Authentication Failed',
        description: error === 'access_denied' ? 'Access was denied' : 'An error occurred',
        duration: 5000,
      })
      return
    }

    if (token) {
      setToken(token)
      toast({
        title: isNewUser === 'true' ? 'Welcome!' : 'Welcome back!',
        description: isNewUser === 'true' ? 'Your account has been created' : 'You are now logged in',
        duration: 3000,
      })
      navigate('/')
    }
  }, [searchParams, setToken, navigate, toast])

  // Fetch Google Auth URL
  useEffect(() => {
    const fetchGoogleUrl = async () => {
      try {
        const data = await authService.getGoogleAuthUrl()
        if (data?.authUrl) {
          setGoogleAuthUrl(data.authUrl)
        }
      } catch (error) {
        console.error('Failed to fetch Google auth URL:', error)
      }
    }
    fetchGoogleUrl()
  }, [])

  const handleGoogleLogin = () => {
    if (googleAuthUrl) {
      window.location.href = googleAuthUrl
    } else {
      toast({
        variant: 'destructive',
        title: 'Error',
        description: 'Google login is currently unavailable',
      })
    }
  }

  const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault()
    e.stopPropagation()
    
    if (isLoading) return // Prevent double submission
    
    setIsLoading(true)

    try {
      if (isLoginMode) {
        // Login flow
        await login(email, password)
        
        toast({
          title: 'Login successful',
          description: 'Welcome back!',
          duration: 3000,
        })
        
        // Navigate after short delay to show toast
        setTimeout(() => {
          navigate('/', { replace: true })
        }, 100)
      } else {
        // Register flow - validate first
        if (password.length < 8) {
          toast({
            variant: 'destructive',
            title: 'Invalid password',
            description: 'Password must be at least 8 characters',
            duration: 4000,
          })
          return
        }

        if (password !== confirmPassword) {
          toast({
            variant: 'destructive',
            title: 'Password mismatch',
            description: 'Passwords do not match',
            duration: 4000,
          })
          return
        }

        await register(name, email, password, confirmPassword, phone, address)
        
        toast({
          title: 'Registration successful',
          description: 'You can now login',
          duration: 3000,
        })
        
        // Switch to login mode and clear form
        setIsLoginMode(true)
        setName('')
        setEmail('')
        setPassword('')
        setConfirmPassword('')
        setPhone('')
        setAddress('')
      }
    } catch (error: any) {
      console.error('Auth error:', error)
      
      let errorMessage = 'An error occurred'
      
      if (error?.response?.data) {
        const data = error.response.data
        
        if (typeof data === 'string') {
          errorMessage = data
        } else if (data.message) {
          errorMessage = data.message
        } else if (data.errors) {
          const validationErrors = Object.entries(data.errors)
            .map(([field, messages]: [string, any]) => {
              const msgs = Array.isArray(messages) ? messages : [messages]
              return `${field}: ${msgs.join(', ')}`
            })
          errorMessage = validationErrors.join('\n')
        }
      } else if (error?.message) {
        errorMessage = error.message
      }

      toast({
        variant: 'destructive',
        title: isLoginMode ? 'Login failed' : 'Registration failed',
        description: errorMessage,
        duration: 5000,
      })
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <div className="min-h-screen w-full flex items-center justify-center relative overflow-hidden bg-gradient-to-br from-violet-50 via-purple-50 to-fuchsia-50 dark:from-gray-950 dark:via-violet-950/20 dark:to-purple-950/20">
      {/* Animated Background */}
      <div className="absolute inset-0 overflow-hidden pointer-events-none">
        {/* Gradient Orbs */}
        <div className="absolute top-0 -left-4 w-72 h-72 bg-violet-300/30 dark:bg-violet-500/20 rounded-full mix-blend-multiply dark:mix-blend-normal filter blur-xl animate-blob"></div>
        <div className="absolute top-0 -right-4 w-72 h-72 bg-purple-300/30 dark:bg-purple-500/20 rounded-full mix-blend-multiply dark:mix-blend-normal filter blur-xl animate-blob animation-delay-2000"></div>
        <div className="absolute -bottom-8 left-20 w-72 h-72 bg-fuchsia-300/30 dark:bg-fuchsia-500/20 rounded-full mix-blend-multiply dark:mix-blend-normal filter blur-xl animate-blob animation-delay-4000"></div>
        
        {/* Grid Pattern */}
        <div className="absolute inset-0 bg-grid-pattern opacity-[0.02] dark:opacity-[0.05]"></div>
      </div>

      {/* Login Card */}
      <div className="relative z-10 w-full max-w-md mx-4">
        <div className="backdrop-blur-xl bg-white/70 dark:bg-gray-900/70 rounded-2xl shadow-2xl border border-violet-200/50 dark:border-violet-500/20 p-8">
          {/* Logo & Header */}
          <div className="flex flex-col items-center mb-8">
            <div className="mb-6 transform transition-transform hover:scale-110 duration-300">
              <Logo size="lg" />
            </div>
            <h1 className="text-3xl font-bold bg-gradient-to-r from-violet-600 to-purple-600 dark:from-violet-400 dark:to-purple-400 bg-clip-text text-transparent">
              {isLoginMode ? 'Welcome Back' : 'Create Account'}
            </h1>
            <p className="text-sm text-gray-600 dark:text-gray-400 mt-2">
              {isLoginMode ? 'Sign in to continue' : 'Join us today'}
            </p>
          </div>

          {/* Google Login Button */}
          <Button
            type="button"
            variant="outline"
            className="w-full mb-6 h-12 border-2 border-gray-200 dark:border-gray-700 hover:border-violet-300 dark:hover:border-violet-500 hover:bg-violet-50 dark:hover:bg-violet-950/30 transition-all duration-300 group"
            onClick={handleGoogleLogin}
            disabled={!googleAuthUrl}
          >
            <svg className="w-5 h-5 mr-3" viewBox="0 0 24 24">
              <path
                fill="currentColor"
                d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z"
                className="text-[#4285F4]"
              />
              <path
                fill="currentColor"
                d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z"
                className="text-[#34A853]"
              />
              <path
                fill="currentColor"
                d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z"
                className="text-[#FBBC05]"
              />
              <path
                fill="currentColor"
                d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z"
                className="text-[#EA4335]"
              />
            </svg>
            <span className="font-medium">Continue with Google</span>
          </Button>

          {/* Divider */}
          <div className="relative mb-6">
            <div className="absolute inset-0 flex items-center">
              <div className="w-full border-t border-gray-200 dark:border-gray-700"></div>
            </div>
            <div className="relative flex justify-center text-sm">
              <span className="px-4 bg-white/70 dark:bg-gray-900/70 text-gray-500 dark:text-gray-400">
                Or continue with email
              </span>
            </div>
          </div>

          {/* Form */}
          <form onSubmit={handleSubmit} className="space-y-4">
            {!isLoginMode && (
              <div className="space-y-2">
                <Label htmlFor="name" className="text-gray-700 dark:text-gray-300">Full Name</Label>
                <div className="relative">
                  <User className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                  <Input
                    id="name"
                    type="text"
                    placeholder="John Doe"
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                    className="pl-10 h-12 border-gray-200 dark:border-gray-700 focus:border-violet-400 dark:focus:border-violet-500 bg-white/50 dark:bg-gray-800/50"
                    required={!isLoginMode}
                  />
                </div>
              </div>
            )}

            <div className="space-y-2">
              <Label htmlFor="email" className="text-gray-700 dark:text-gray-300">Email</Label>
              <div className="relative">
                <Mail className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                <Input
                  id="email"
                  type="email"
                  placeholder="name@example.com"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  className="pl-10 h-12 border-gray-200 dark:border-gray-700 focus:border-violet-400 dark:focus:border-violet-500 bg-white/50 dark:bg-gray-800/50"
                  required
                />
              </div>
            </div>

            <div className="space-y-2">
              <Label htmlFor="password" className="text-gray-700 dark:text-gray-300">Password</Label>
              <div className="relative">
                <Lock className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                <Input
                  id="password"
                  type={showPassword ? 'text' : 'password'}
                  placeholder="••••••••"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  className="pl-10 pr-10 h-12 border-gray-200 dark:border-gray-700 focus:border-violet-400 dark:focus:border-violet-500 bg-white/50 dark:bg-gray-800/50"
                  required
                />
                <button
                  type="button"
                  onClick={() => setShowPassword(!showPassword)}
                  className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600 dark:hover:text-gray-300 transition-colors"
                >
                  {showPassword ? <EyeOff className="h-5 w-5" /> : <Eye className="h-5 w-5" />}
                </button>
              </div>
            </div>

            {!isLoginMode && (
              <>
                <div className="space-y-2">
                  <Label htmlFor="confirmPassword" className="text-gray-700 dark:text-gray-300">Confirm Password</Label>
                  <div className="relative">
                    <Lock className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                    <Input
                      id="confirmPassword"
                      type="password"
                      placeholder="••••••••"
                      value={confirmPassword}
                      onChange={(e) => setConfirmPassword(e.target.value)}
                      className="pl-10 h-12 border-gray-200 dark:border-gray-700 focus:border-violet-400 dark:focus:border-violet-500 bg-white/50 dark:bg-gray-800/50"
                      required={!isLoginMode}
                    />
                  </div>
                </div>

                <div className="space-y-2">
                  <Label htmlFor="phone" className="text-gray-700 dark:text-gray-300">Phone Number</Label>
                  <div className="relative">
                    <Phone className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                    <Input
                      id="phone"
                      type="tel"
                      placeholder="+84 123 456 789"
                      value={phone}
                      onChange={(e) => setPhone(e.target.value)}
                      className="pl-10 h-12 border-gray-200 dark:border-gray-700 focus:border-violet-400 dark:focus:border-violet-500 bg-white/50 dark:bg-gray-800/50"
                    />
                  </div>
                </div>

                <div className="space-y-2">
                  <Label htmlFor="address" className="text-gray-700 dark:text-gray-300">Address</Label>
                  <div className="relative">
                    <MapPin className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                    <Input
                      id="address"
                      type="text"
                      placeholder="123 Street, City"
                      value={address}
                      onChange={(e) => setAddress(e.target.value)}
                      className="pl-10 h-12 border-gray-200 dark:border-gray-700 focus:border-violet-400 dark:focus:border-violet-500 bg-white/50 dark:bg-gray-800/50"
                    />
                  </div>
                </div>
              </>
            )}

            <Button
              type="submit"
              className="w-full h-12 bg-gradient-to-r from-violet-600 to-purple-600 hover:from-violet-700 hover:to-purple-700 text-white font-medium shadow-lg shadow-violet-500/50 dark:shadow-violet-900/50 transition-all duration-300 transform hover:scale-[1.02]"
              disabled={isLoading}
            >
              {isLoading ? 'Please wait...' : isLoginMode ? 'Sign In' : 'Create Account'}
            </Button>
          </form>

          {/* Toggle Mode */}
          <div className="mt-6 text-center">
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
              className="text-sm text-gray-600 dark:text-gray-400 hover:text-violet-600 dark:hover:text-violet-400 transition-colors"
            >
              {isLoginMode ? (
                <>
                  Don't have an account? <span className="font-semibold">Sign up</span>
                </>
              ) : (
                <>
                  Already have an account? <span className="font-semibold">Sign in</span>
                </>
              )}
            </button>
          </div>
        </div>

        {/* Footer */}
        <p className="mt-6 text-center text-sm text-gray-600 dark:text-gray-400">
          Warehouse Management System
        </p>
      </div>

      {/* Custom Animations in style tag */}
      <style>{`
        @keyframes blob {
          0%, 100% {
            transform: translate(0, 0) scale(1);
          }
          33% {
            transform: translate(30px, -50px) scale(1.1);
          }
          66% {
            transform: translate(-20px, 20px) scale(0.9);
          }
        }
        
        .animate-blob {
          animation: blob 7s infinite;
        }
        
        .animation-delay-2000 {
          animation-delay: 2s;
        }
        
        .animation-delay-4000 {
          animation-delay: 4s;
        }
        
        .bg-grid-pattern {
          background-image: 
            linear-gradient(to right, currentColor 1px, transparent 1px),
            linear-gradient(to bottom, currentColor 1px, transparent 1px);
          background-size: 40px 40px;
        }
      `}</style>
    </div>
  )
}

export default Login
