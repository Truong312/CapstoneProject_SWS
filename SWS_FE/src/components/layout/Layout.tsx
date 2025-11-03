import { Outlet, useNavigate, useLocation } from 'react-router-dom';
import { useState } from 'react';
import { 
  LayoutDashboard, 
  Package, 
  ShoppingCart, 
  Users,
  FileText,
  Settings,
  Search,
  Bell,
  User,
  ChevronDown,
  ChevronLeft,
  ChevronRight,
  Sun,
  Moon,
  Monitor,
  LogOut,
  Code,
  TrendingUp,
  BarChart3,
  ShoppingBag,
  Mic
} from 'lucide-react';
import { useTheme } from '../theme-provider';
import { Logo } from '../Logo';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
  DropdownMenuSeparator,
  DropdownMenuLabel
} from '../ui/dropdown-menu';
import { Avatar, AvatarFallback } from '../ui/avatar';
import { Input } from '../ui/input';
import { Badge } from '../ui/badge';

interface NavItem {
  name: string;
  icon: any;
  path: string;
  badge?: number;
  subItems?: { name: string; path: string }[];
}

const Layout = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { theme, setTheme } = useTheme();
  const [sidebarCollapsed, setSidebarCollapsed] = useState(false);
  const [expandedMenus, setExpandedMenus] = useState<string[]>([]);

  const navItems: NavItem[] = [
    { name: 'Dashboard', icon: LayoutDashboard, path: '/dashboard' },
    { 
      name: 'Products', 
      icon: Package, 
      path: '/products',
      subItems: [
        { name: 'All Products', path: '/products' },
        { name: 'Categories', path: '/products/categories' },
        { name: 'Suppliers', path: '/products/suppliers' },
      ]
    },
    { 
      name: 'Inventory', 
      icon: ShoppingCart, 
      path: '/inventory',
      subItems: [
        { name: 'Stock Overview', path: '/inventory' },
        { name: 'Warehouses', path: '/inventory/warehouses' },
        { name: 'Transfers', path: '/inventory/transfers' },
      ]
    },
    { 
      name: 'Orders', 
      icon: ShoppingBag, 
      path: '/orders',
      badge: 5,
      subItems: [
        { name: 'All Orders', path: '/orders' },
        { name: 'Pending', path: '/orders/pending' },
        { name: 'Completed', path: '/orders/completed' },
      ]
    },
    { name: 'Customers', icon: Users, path: '/customers' },
    { name: 'Voice Query', icon: Mic, path: '/voice-query', badge: 0 },
    { 
      name: 'Reports', 
      icon: BarChart3, 
      path: '/reports',
      subItems: [
        { name: 'Analytics', path: '/reports/analytics' },
        { name: 'Sales Report', path: '/reports/sales' },
        { name: 'Inventory Report', path: '/reports/inventory' },
      ]
    },
    { name: 'Settings', icon: Settings, path: '/settings' },
  ];

  const isActive = (path: string) => location.pathname === path;

  const handleNavigation = (path: string) => {
    navigate(path);
  };

  const toggleMenu = (menuName: string) => {
    setExpandedMenus(prev => 
      prev.includes(menuName) 
        ? prev.filter(m => m !== menuName)
        : [...prev, menuName]
    );
  };

  return (
    <div className="flex h-screen bg-gradient-to-br from-slate-50 via-violet-50/20 to-purple-50/20 dark:from-slate-950 dark:via-violet-950/20 dark:to-purple-950/20">
      {/* Sidebar */}
      <aside 
        className={`fixed left-0 top-0 h-full bg-white/80 dark:bg-slate-900/80 backdrop-blur-xl border-r border-slate-200/50 dark:border-slate-800/50 transition-all duration-300 z-40 shadow-2xl shadow-violet-500/5 ${
          sidebarCollapsed ? 'w-20' : 'w-72'
        }`}
      >
        {/* Logo Section */}
        <div className="h-16 flex items-center justify-between px-4 border-b border-slate-200/50 dark:border-slate-800/50 bg-gradient-to-r from-violet-600 via-violet-500 to-purple-600 relative overflow-hidden">
          {/* Animated background pattern */}
          <div className="absolute inset-0 opacity-10">
            <div className="absolute inset-0 bg-gradient-to-r from-transparent via-white to-transparent animate-shimmer"></div>
          </div>
          
          <div className="flex items-center gap-3 relative z-10">
            <div className="transform hover:scale-110 transition-transform duration-300">
              <Logo variant="white" size="sm" />
            </div>
            {!sidebarCollapsed && (
              <div className="flex flex-col">
                <span className="text-xl font-bold text-white tracking-tight">WMS Pro</span>
                <span className="text-xs text-violet-100">Warehouse System</span>
              </div>
            )}
          </div>
          <button
            onClick={() => setSidebarCollapsed(!sidebarCollapsed)}
            className="p-1.5 hover:bg-white/20 rounded-lg transition-all duration-200 text-white relative z-10 group"
          >
            <div className="transform group-hover:scale-110 transition-transform">
              {sidebarCollapsed ? <ChevronRight className="h-5 w-5" /> : <ChevronLeft className="h-5 w-5" />}
            </div>
          </button>
        </div>

        {/* Navigation */}
        <nav className="p-4 space-y-1 overflow-y-auto h-[calc(100vh-8rem)] custom-scrollbar">
          {navItems.map((item) => {
            const Icon = item.icon;
            const active = isActive(item.path);
            const hasSubItems = item.subItems && item.subItems.length > 0;
            const isExpanded = expandedMenus.includes(item.name);

            return (
              <div key={item.name} className="group/item">
                <button
                  onClick={() => {
                    if (hasSubItems) {
                      toggleMenu(item.name);
                    } else {
                      handleNavigation(item.path);
                    }
                  }}
                  className={`w-full flex items-center gap-3 px-4 py-3 rounded-xl font-medium transition-all duration-200 group relative overflow-hidden ${
                    active 
                      ? 'bg-gradient-to-r from-violet-500 to-purple-600 text-white shadow-lg shadow-violet-500/30 scale-[1.02]' 
                      : 'text-slate-700 dark:text-slate-300 hover:bg-gradient-to-r hover:from-violet-50 hover:to-purple-50 dark:hover:from-violet-900/20 dark:hover:to-purple-900/20'
                  }`}
                >
                  {/* Hover effect background */}
                  {!active && (
                    <div className="absolute inset-0 bg-gradient-to-r from-violet-500/0 via-violet-500/5 to-purple-500/0 opacity-0 group-hover:opacity-100 transition-opacity duration-300"></div>
                  )}
                  
                  {/* Active indicator bar */}
                  {active && (
                    <div className="absolute left-0 top-1/2 -translate-y-1/2 w-1 h-8 bg-white rounded-r-full"></div>
                  )}
                  
                  <Icon className={`h-5 w-5 flex-shrink-0 relative z-10 transition-all duration-200 ${
                    active 
                      ? 'text-white transform scale-110' 
                      : 'text-slate-500 dark:text-slate-400 group-hover:text-violet-600 dark:group-hover:text-violet-400 group-hover:scale-110'
                  }`} />
                  {!sidebarCollapsed && (
                    <>
                      <span className="flex-1 text-left text-sm relative z-10">{item.name}</span>
                      {item.badge && (
                        <Badge className="bg-red-500 text-white text-xs px-2 py-0.5 relative z-10 animate-pulse">
                          {item.badge}
                        </Badge>
                      )}
                      {hasSubItems && (
                        <ChevronDown className={`h-4 w-4 transition-transform duration-200 relative z-10 ${isExpanded ? 'rotate-180' : ''}`} />
                      )}
                    </>
                  )}
                  {sidebarCollapsed && item.badge && (
                    <span className="absolute -top-1 -right-1 h-5 w-5 bg-red-500 text-white text-xs rounded-full flex items-center justify-center font-bold shadow-lg animate-pulse">
                      {item.badge}
                    </span>
                  )}
                </button>

                {/* Submenu */}
                {hasSubItems && isExpanded && !sidebarCollapsed && (
                  <div className="ml-12 mt-1 space-y-1 animate-in slide-in-from-top-2 duration-200">
                    {item.subItems?.map((subItem) => (
                      <button
                        key={subItem.path}
                        onClick={() => handleNavigation(subItem.path)}
                        className={`w-full text-left px-3 py-2 rounded-lg text-sm transition-all duration-200 relative group/sub ${
                          isActive(subItem.path)
                            ? 'bg-gradient-to-r from-violet-100 to-purple-100 dark:from-violet-900/30 dark:to-purple-900/30 text-violet-700 dark:text-violet-300 font-medium shadow-sm'
                            : 'text-slate-600 dark:text-slate-400 hover:bg-slate-50 dark:hover:bg-slate-800/30 hover:text-violet-600 dark:hover:text-violet-400'
                        }`}
                      >
                        {isActive(subItem.path) && (
                          <div className="absolute left-0 top-1/2 -translate-y-1/2 w-0.5 h-4 bg-violet-500 rounded-full"></div>
                        )}
                        <span className="block pl-2">{subItem.name}</span>
                      </button>
                    ))}
                  </div>
                )}
              </div>
            );
          })}

          {/* UI Resources */}
          <div className="pt-4 border-t border-slate-200 dark:border-slate-800">
            <button
              onClick={() => handleNavigation('/ui-showcase')}
              className={`w-full flex items-center gap-3 px-4 py-3 rounded-xl font-medium transition-all duration-200 group ${
                isActive('/ui-showcase')
                  ? 'bg-gradient-to-r from-violet-500 to-purple-600 text-white shadow-lg shadow-violet-500/30' 
                  : 'text-slate-700 dark:text-slate-300 hover:bg-slate-100 dark:hover:bg-slate-800/50'
              }`}
            >
              <Code className={`h-5 w-5 flex-shrink-0 ${isActive('/ui-showcase') ? 'text-white' : 'text-slate-500 dark:text-slate-400 group-hover:text-violet-600'}`} />
              {!sidebarCollapsed && <span className="text-sm">UI Showcase</span>}
            </button>
          </div>
        </nav>

        {/* User Section */}
        <div className="absolute bottom-0 left-0 right-0 p-4 border-t border-slate-200 dark:border-slate-800 bg-slate-50 dark:bg-slate-900/50">
          <DropdownMenu>
            <DropdownMenuTrigger asChild>
              <button className={`w-full flex items-center gap-3 px-3 py-2 rounded-xl hover:bg-slate-200 dark:hover:bg-slate-800 transition-colors ${
                sidebarCollapsed ? 'justify-center' : ''
              }`}>
                <Avatar className="h-9 w-9 border-2 border-violet-500">
                  <AvatarFallback className="bg-gradient-to-br from-violet-500 to-purple-600 text-white font-bold">AD</AvatarFallback>
                </Avatar>
                {!sidebarCollapsed && (
                  <>
                    <div className="flex-1 text-left">
                      <p className="text-sm font-semibold text-slate-900 dark:text-white">Admin User</p>
                      <p className="text-xs text-slate-500 dark:text-slate-400">admin@wms.com</p>
                    </div>
                    <ChevronDown className="h-4 w-4 text-slate-400" />
                  </>
                )}
              </button>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end" className="w-56">
              <DropdownMenuLabel>My Account</DropdownMenuLabel>
              <DropdownMenuSeparator />
              <DropdownMenuItem onClick={() => handleNavigation('/profile')}>
                <User className="h-4 w-4 mr-2" />
                Profile
              </DropdownMenuItem>
              <DropdownMenuItem onClick={() => handleNavigation('/settings')}>
                <Settings className="h-4 w-4 mr-2" />
                Settings
              </DropdownMenuItem>
              <DropdownMenuSeparator />
              <DropdownMenuItem
  onClick={() => {
    localStorage.removeItem("token"); // 🧹 Xóa token khỏi localStorage
    navigate("/login");               // 🔄 Quay về trang đăng nhập
  }}
  className="text-red-600 dark:text-red-400 cursor-pointer"
>
  <LogOut className="h-4 w-4 mr-2" />
  Logout
</DropdownMenuItem>

            </DropdownMenuContent>
          </DropdownMenu>
        </div>
      </aside>

      {/* Main Content */}
      <div className={`flex-1 flex flex-col transition-all duration-300 ${
        sidebarCollapsed ? 'ml-20' : 'ml-72'
      }`}>
        {/* Top Header */}
        <header className="h-16 bg-white dark:bg-slate-900 border-b border-slate-200 dark:border-slate-800 flex items-center justify-between px-6 shadow-sm">
          <div className="flex items-center gap-4 flex-1">
            {/* Search */}
            <div className="relative max-w-md w-full">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-slate-400" />
              <Input
                placeholder="Search anything..."
                className="pl-10 bg-slate-50 dark:bg-slate-800 border-slate-200 dark:border-slate-700 focus:border-violet-500 dark:focus:border-violet-500"
              />
            </div>
          </div>

          <div className="flex items-center gap-3">
            {/* Theme Switcher */}
            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <button className="p-2 hover:bg-slate-100 dark:hover:bg-slate-800 rounded-lg transition-colors">
                  {theme === 'light' && <Sun className="h-5 w-5 text-slate-600 dark:text-slate-400" />}
                  {theme === 'dark' && <Moon className="h-5 w-5 text-slate-600 dark:text-slate-400" />}
                  {theme === 'system' && <Monitor className="h-5 w-5 text-slate-600 dark:text-slate-400" />}
                </button>
              </DropdownMenuTrigger>
              <DropdownMenuContent align="end">
                <DropdownMenuItem onClick={() => setTheme('light')}>
                  <Sun className="h-4 w-4 mr-2" />
                  Light
                </DropdownMenuItem>
                <DropdownMenuItem onClick={() => setTheme('dark')}>
                  <Moon className="h-4 w-4 mr-2" />
                  Dark
                </DropdownMenuItem>
                <DropdownMenuItem onClick={() => setTheme('system')}>
                  <Monitor className="h-4 w-4 mr-2" />
                  System
                </DropdownMenuItem>
              </DropdownMenuContent>
            </DropdownMenu>

            {/* Notifications */}
            <button className="relative p-2 hover:bg-slate-100 dark:hover:bg-slate-800 rounded-lg transition-colors">
              <Bell className="h-5 w-5 text-slate-600 dark:text-slate-400" />
              <span className="absolute top-1 right-1 h-2 w-2 bg-red-500 rounded-full ring-2 ring-white dark:ring-slate-900"></span>
            </button>
          </div>
        </header>

        {/* Page Content */}
        <main className="flex-1 overflow-auto p-6">
          <div className="max-w-[1920px] mx-auto">
            <Outlet />
          </div>
        </main>
      </div>
    </div>
  );
};

export default Layout;
