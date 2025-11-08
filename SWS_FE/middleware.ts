import { NextResponse } from 'next/server'
import type { NextRequest } from 'next/server'

// Routes công khai - không cần authentication
const PUBLIC_ROUTES = ['/login', '/register']

// Routes cần authentication  
const PROTECTED_ROUTES = [
  '/dashboard',
  '/dashboard/inventory', 
  '/dashboard/products',
  '/dashboard/orders',
  '/dashboard/reports',
  '/dashboard/settings'
]

// Routes chỉ dành cho admin (role = 1)
const ADMIN_ONLY_ROUTES = ['/dashboard/settings/users', '/dashboard/settings/roles']

export function middleware(request: NextRequest) {
  const { pathname } = request.nextUrl
  
  const token = request.cookies.get('token')?.value
  const userRole = request.cookies.get('userRole')?.value
  
  // Nếu đã login và truy cập public route -> redirect về dashboard
  if (PUBLIC_ROUTES.includes(pathname) && token) {
    return NextResponse.redirect(new URL('/dashboard', request.url))
  }
  
  // Nếu chưa login và truy cập protected route -> redirect về login
  const isProtectedRoute = PROTECTED_ROUTES.some(route => pathname.startsWith(route))
  if (isProtectedRoute && !token) {
    const loginUrl = new URL('/login', request.url)
    loginUrl.searchParams.set('redirect', pathname)
    return NextResponse.redirect(loginUrl)
  }
  
  // Nếu không phải admin và truy cập admin route -> redirect về dashboard
  const isAdminRoute = ADMIN_ONLY_ROUTES.some(route => pathname.startsWith(route))
  if (isAdminRoute && userRole !== '1') {
    return NextResponse.redirect(new URL('/dashboard', request.url))
  }
  
  // Thêm security headers
  const response = NextResponse.next()
  response.headers.set('X-Frame-Options', 'DENY')
  response.headers.set('X-Content-Type-Options', 'nosniff')
  response.headers.set('Referrer-Policy', 'strict-origin-when-cross-origin')
  
  return response
}

export const config = {
  matcher: [
    '/((?!api|_next/static|_next/image|favicon.ico|.*\\..*|public).*)',
  ],
}
