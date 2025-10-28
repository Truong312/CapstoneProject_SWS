import { useState } from 'react';
import {
  LineChart,
  Line,
  AreaChart,
  Area,
  BarChart,
  Bar,
  PieChart,
  Pie,
  Cell,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer,
  RadarChart,
  PolarGrid,
  PolarAngleAxis,
  PolarRadiusAxis,
  Radar,
} from 'recharts';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Badge } from '@/components/ui/badge';
import { Checkbox } from '@/components/ui/checkbox';
import { Switch } from '@/components/ui/switch';
import { Avatar, AvatarFallback } from '@/components/ui/avatar';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Alert } from '@/components/ui/alert';
import { Progress } from '@/components/ui/progress';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import {
  Search,
  Filter,
  Download,
  Upload,
  Trash2,
  Edit,
  Eye,
  MoreVertical,
  Plus,
  ChevronLeft,
  ChevronRight,
  Star,
  Heart,
  Share2,
  Copy,
  AlertCircle,
  CheckCircle2,
  GripVertical,
  ArrowUpDown,
  Settings,
  TrendingUp,
  TrendingDown,
  Package,
  Users,
  ShoppingCart,
  DollarSign
} from 'lucide-react';

interface Product {
  id: string;
  name: string;
  category: string;
  price: number;
  stock: number;
  status: 'active' | 'inactive' | 'low-stock';
  rating: number;
  lastUpdated: string;
}

const UIShowcase = () => {
  const [selectedRows, setSelectedRows] = useState<string[]>([]);
  const [searchQuery, setSearchQuery] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const [sortColumn, setSortColumn] = useState<string | null>(null);
  const [sortDirection, setSortDirection] = useState<'asc' | 'desc'>('asc');

  const products: Product[] = [
    { id: '1', name: 'Laptop Pro X1', category: 'Electronics', price: 1299.99, stock: 45, status: 'active', rating: 4.8, lastUpdated: '2024-01-15' },
    { id: '2', name: 'Wireless Mouse', category: 'Accessories', price: 29.99, stock: 120, status: 'active', rating: 4.5, lastUpdated: '2024-01-14' },
    { id: '3', name: 'USB-C Cable', category: 'Accessories', price: 15.99, stock: 8, status: 'low-stock', rating: 4.2, lastUpdated: '2024-01-13' },
    { id: '4', name: 'Monitor 27"', category: 'Electronics', price: 349.99, stock: 0, status: 'inactive', rating: 4.7, lastUpdated: '2024-01-12' },
    { id: '5', name: 'Keyboard Mechanical', category: 'Accessories', price: 129.99, stock: 67, status: 'active', rating: 4.9, lastUpdated: '2024-01-11' },
    { id: '6', name: 'Webcam HD', category: 'Electronics', price: 79.99, stock: 34, status: 'active', rating: 4.3, lastUpdated: '2024-01-10' },
    { id: '7', name: 'Desk Lamp LED', category: 'Furniture', price: 45.99, stock: 5, status: 'low-stock', rating: 4.6, lastUpdated: '2024-01-09' },
    { id: '8', name: 'Office Chair', category: 'Furniture', price: 299.99, stock: 18, status: 'active', rating: 4.4, lastUpdated: '2024-01-08' },
  ];

  const statsCards = [
    { title: 'Total Revenue', value: '$45,231', change: '+12.5%', trend: 'up', icon: DollarSign, color: 'violet' },
    { title: 'Total Orders', value: '1,234', change: '+8.2%', trend: 'up', icon: ShoppingCart, color: 'blue' },
    { title: 'Total Products', value: '856', change: '-2.3%', trend: 'down', icon: Package, color: 'purple' },
    { title: 'Total Customers', value: '3,456', change: '+15.3%', trend: 'up', icon: Users, color: 'pink' },
  ];

  // Chart data
  const revenueData = [
    { month: 'Jan', revenue: 4000, orders: 240, customers: 1200 },
    { month: 'Feb', revenue: 3000, orders: 198, customers: 980 },
    { month: 'Mar', revenue: 5000, orders: 320, customers: 1500 },
    { month: 'Apr', revenue: 4500, orders: 280, customers: 1350 },
    { month: 'May', revenue: 6000, orders: 390, customers: 1800 },
    { month: 'Jun', revenue: 5500, orders: 350, customers: 1650 },
    { month: 'Jul', revenue: 7000, orders: 450, customers: 2100 },
  ];

  const categoryData = [
    { name: 'Electronics', value: 4500, color: '#8B5CF6' },
    { name: 'Furniture', value: 3200, color: '#A78BFA' },
    { name: 'Accessories', value: 2800, color: '#C4B5FD' },
    { name: 'Others', value: 1500, color: '#DDD6FE' },
  ];

  const performanceData = [
    { subject: 'Sales', A: 120, B: 110, fullMark: 150 },
    { subject: 'Delivery', A: 98, B: 130, fullMark: 150 },
    { subject: 'Quality', A: 86, B: 130, fullMark: 150 },
    { subject: 'Support', A: 99, B: 100, fullMark: 150 },
    { subject: 'Returns', A: 85, B: 90, fullMark: 150 },
  ];

  const handleSelectAll = () => {
    if (selectedRows.length === products.length) {
      setSelectedRows([]);
    } else {
      setSelectedRows(products.map(p => p.id));
    }
  };

  const handleSelectRow = (id: string) => {
    setSelectedRows(prev =>
      prev.includes(id) ? prev.filter(rowId => rowId !== id) : [...prev, id]
    );
  };

  const handleBulkDelete = () => {
    console.log('Deleting:', selectedRows);
    setSelectedRows([]);
  };

  const handleSort = (column: string) => {
    if (sortColumn === column) {
      setSortDirection(sortDirection === 'asc' ? 'desc' : 'asc');
    } else {
      setSortColumn(column);
      setSortDirection('asc');
    }
  };

  const getStatusBadge = (status: string) => {
    const variants: Record<string, { className: string; label: string }> = {
      active: { className: 'bg-emerald-100 text-emerald-700 dark:bg-emerald-900/30 dark:text-emerald-400', label: 'Active' },
      inactive: { className: 'bg-slate-100 text-slate-700 dark:bg-slate-800 dark:text-slate-400', label: 'Inactive' },
      'low-stock': { className: 'bg-amber-100 text-amber-700 dark:bg-amber-900/30 dark:text-amber-400', label: 'Low Stock' },
    };
    const variant = variants[status] || variants.active;
    return <Badge className={variant.className}>{variant.label}</Badge>;
  };

  return (
    <div className="space-y-6">
      {/* Page Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-slate-900 dark:text-white bg-gradient-to-r from-violet-600 to-purple-600 bg-clip-text text-transparent">
            UI Components Showcase
          </h1>
          <p className="text-slate-600 dark:text-slate-400 mt-1">
            Comprehensive collection of interactive UI elements and components
          </p>
        </div>
        <Button className="bg-gradient-to-r from-violet-600 to-purple-600 hover:from-violet-700 hover:to-purple-700 text-white">
          <Plus className="h-4 w-4 mr-2" />
          Add New
        </Button>
      </div>

      {/* Stats Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        {statsCards.map((stat, index) => {
          const Icon = stat.icon;
          return (
            <Card key={index} className="p-6 hover:shadow-lg transition-shadow border-slate-200 dark:border-slate-800">
              <div className="flex items-center justify-between">
                <div className="flex-1">
                  <p className="text-sm font-medium text-slate-600 dark:text-slate-400">{stat.title}</p>
                  <p className="text-2xl font-bold text-slate-900 dark:text-white mt-2">{stat.value}</p>
                  <div className="flex items-center gap-1 mt-2">
                    {stat.trend === 'up' ? (
                      <TrendingUp className="h-4 w-4 text-emerald-500" />
                    ) : (
                      <TrendingDown className="h-4 w-4 text-red-500" />
                    )}
                    <span className={`text-sm font-medium ${stat.trend === 'up' ? 'text-emerald-600' : 'text-red-600'}`}>
                      {stat.change}
                    </span>
                  </div>
                </div>
                <div className={`p-3 rounded-xl bg-gradient-to-br ${
                  stat.color === 'violet' ? 'from-violet-500 to-purple-600' :
                  stat.color === 'blue' ? 'from-blue-500 to-indigo-600' :
                  stat.color === 'purple' ? 'from-purple-500 to-pink-600' :
                  'from-pink-500 to-rose-600'
                }`}>
                  <Icon className="h-6 w-6 text-white" />
                </div>
              </div>
            </Card>
          );
        })}
      </div>

      {/* Alerts Section */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <Alert className="border-emerald-200 bg-emerald-50 dark:bg-emerald-900/20 dark:border-emerald-800">
          <CheckCircle2 className="h-4 w-4 text-emerald-600" />
          <div className="ml-2">
            <h4 className="font-semibold text-emerald-900 dark:text-emerald-400">Success!</h4>
            <p className="text-sm text-emerald-700 dark:text-emerald-500">Your changes have been saved successfully.</p>
          </div>
        </Alert>
        <Alert className="border-amber-200 bg-amber-50 dark:bg-amber-900/20 dark:border-amber-800">
          <AlertCircle className="h-4 w-4 text-amber-600" />
          <div className="ml-2">
            <h4 className="font-semibold text-amber-900 dark:text-amber-400">Warning!</h4>
            <p className="text-sm text-amber-700 dark:text-amber-500">Some items are running low on stock.</p>
          </div>
        </Alert>
      </div>

      {/* Charts Section */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Revenue Line Chart */}
        <Card className="p-6 border-slate-200 dark:border-slate-800">
          <h3 className="text-lg font-semibold mb-4 text-slate-900 dark:text-white flex items-center gap-2">
            <TrendingUp className="h-5 w-5 text-violet-600" />
            Revenue Trend
          </h3>
          <ResponsiveContainer width="100%" height={300}>
            <LineChart data={revenueData}>
              <CartesianGrid strokeDasharray="3 3" stroke="#e2e8f0" />
              <XAxis dataKey="month" stroke="#64748b" />
              <YAxis stroke="#64748b" />
              <Tooltip 
                contentStyle={{ 
                  backgroundColor: '#fff', 
                  border: '1px solid #e2e8f0',
                  borderRadius: '8px'
                }}
              />
              <Legend />
              <Line 
                type="monotone" 
                dataKey="revenue" 
                stroke="#8B5CF6" 
                strokeWidth={3}
                dot={{ fill: '#8B5CF6', r: 5 }}
                activeDot={{ r: 7 }}
              />
            </LineChart>
          </ResponsiveContainer>
        </Card>

        {/* Orders Area Chart */}
        <Card className="p-6 border-slate-200 dark:border-slate-800">
          <h3 className="text-lg font-semibold mb-4 text-slate-900 dark:text-white flex items-center gap-2">
            <ShoppingCart className="h-5 w-5 text-blue-600" />
            Orders & Customers
          </h3>
          <ResponsiveContainer width="100%" height={300}>
            <AreaChart data={revenueData}>
              <CartesianGrid strokeDasharray="3 3" stroke="#e2e8f0" />
              <XAxis dataKey="month" stroke="#64748b" />
              <YAxis stroke="#64748b" />
              <Tooltip 
                contentStyle={{ 
                  backgroundColor: '#fff', 
                  border: '1px solid #e2e8f0',
                  borderRadius: '8px'
                }}
              />
              <Legend />
              <Area 
                type="monotone" 
                dataKey="orders" 
                stackId="1"
                stroke="#3b82f6" 
                fill="#3b82f6"
                fillOpacity={0.6}
              />
              <Area 
                type="monotone" 
                dataKey="customers" 
                stackId="2"
                stroke="#8B5CF6" 
                fill="#8B5CF6"
                fillOpacity={0.6}
              />
            </AreaChart>
          </ResponsiveContainer>
        </Card>

        {/* Category Pie Chart */}
        <Card className="p-6 border-slate-200 dark:border-slate-800">
          <h3 className="text-lg font-semibold mb-4 text-slate-900 dark:text-white flex items-center gap-2">
            <Package className="h-5 w-5 text-purple-600" />
            Sales by Category
          </h3>
          <ResponsiveContainer width="100%" height={300}>
            <PieChart>
              <Pie
                data={categoryData}
                cx="50%"
                cy="50%"
                labelLine={false}
                label={({ name, percent }) => `${name} ${(percent * 100).toFixed(0)}%`}
                outerRadius={100}
                fill="#8884d8"
                dataKey="value"
              >
                {categoryData.map((entry, index) => (
                  <Cell key={`cell-${index}`} fill={entry.color} />
                ))}
              </Pie>
              <Tooltip />
            </PieChart>
          </ResponsiveContainer>
        </Card>

        {/* Performance Radar Chart */}
        <Card className="p-6 border-slate-200 dark:border-slate-800">
          <h3 className="text-lg font-semibold mb-4 text-slate-900 dark:text-white flex items-center gap-2">
            <TrendingUp className="h-5 w-5 text-pink-600" />
            Performance Metrics
          </h3>
          <ResponsiveContainer width="100%" height={300}>
            <RadarChart data={performanceData}>
              <PolarGrid stroke="#e2e8f0" />
              <PolarAngleAxis dataKey="subject" stroke="#64748b" />
              <PolarRadiusAxis stroke="#64748b" />
              <Radar 
                name="This Month" 
                dataKey="A" 
                stroke="#8B5CF6" 
                fill="#8B5CF6" 
                fillOpacity={0.6} 
              />
              <Radar 
                name="Last Month" 
                dataKey="B" 
                stroke="#3b82f6" 
                fill="#3b82f6" 
                fillOpacity={0.6} 
              />
              <Legend />
              <Tooltip />
            </RadarChart>
          </ResponsiveContainer>
        </Card>

        {/* Bar Chart */}
        <Card className="p-6 border-slate-200 dark:border-slate-800 lg:col-span-2">
          <h3 className="text-lg font-semibold mb-4 text-slate-900 dark:text-white flex items-center gap-2">
            <DollarSign className="h-5 w-5 text-emerald-600" />
            Monthly Comparison
          </h3>
          <ResponsiveContainer width="100%" height={300}>
            <BarChart data={revenueData}>
              <CartesianGrid strokeDasharray="3 3" stroke="#e2e8f0" />
              <XAxis dataKey="month" stroke="#64748b" />
              <YAxis stroke="#64748b" />
              <Tooltip 
                contentStyle={{ 
                  backgroundColor: '#fff', 
                  border: '1px solid #e2e8f0',
                  borderRadius: '8px'
                }}
              />
              <Legend />
              <Bar dataKey="revenue" fill="#8B5CF6" radius={[8, 8, 0, 0]} />
              <Bar dataKey="orders" fill="#3b82f6" radius={[8, 8, 0, 0]} />
            </BarChart>
          </ResponsiveContainer>
        </Card>
      </div>

      {/* Tabs Component */}
      <Card className="border-slate-200 dark:border-slate-800">
        <Tabs defaultValue="table" className="w-full">
          <div className="border-b border-slate-200 dark:border-slate-800 px-6 pt-6">
            <TabsList className="bg-slate-100 dark:bg-slate-800">
              <TabsTrigger value="table" className="data-[state=active]:bg-white dark:data-[state=active]:bg-slate-700">
                Advanced Table
              </TabsTrigger>
              <TabsTrigger value="charts" className="data-[state=active]:bg-white dark:data-[state=active]:bg-slate-700">
                Charts & Analytics
              </TabsTrigger>
              <TabsTrigger value="buttons" className="data-[state=active]:bg-white dark:data-[state=active]:bg-slate-700">
                Buttons & Actions
              </TabsTrigger>
              <TabsTrigger value="forms" className="data-[state=active]:bg-white dark:data-[state=active]:bg-slate-700">
                Forms & Inputs
              </TabsTrigger>
              <TabsTrigger value="misc" className="data-[state=active]:bg-white dark:data-[state=active]:bg-slate-700">
                Misc Components
              </TabsTrigger>
            </TabsList>
          </div>

          {/* Advanced Table Tab */}
          <TabsContent value="table" className="p-6">
            <div className="space-y-4">
              {/* Table Actions */}
              <div className="flex flex-col sm:flex-row gap-4 items-start sm:items-center justify-between">
                <div className="relative flex-1 max-w-sm">
                  <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-slate-400" />
                  <Input
                    placeholder="Search products..."
                    value={searchQuery}
                    onChange={(e) => setSearchQuery(e.target.value)}
                    className="pl-10"
                  />
                </div>
                <div className="flex gap-2 flex-wrap">
                  {selectedRows.length > 0 && (
                    <>
                      <Button variant="outline" size="sm" onClick={handleBulkDelete} className="text-red-600 hover:text-red-700">
                        <Trash2 className="h-4 w-4 mr-2" />
                        Delete ({selectedRows.length})
                      </Button>
                      <Button variant="outline" size="sm">
                        <Download className="h-4 w-4 mr-2" />
                        Export
                      </Button>
                    </>
                  )}
                  <Button variant="outline" size="sm">
                    <Filter className="h-4 w-4 mr-2" />
                    Filter
                  </Button>
                  <Button variant="outline" size="sm">
                    <Upload className="h-4 w-4 mr-2" />
                    Import
                  </Button>
                </div>
              </div>

              {/* Table */}
              <div className="border border-slate-200 dark:border-slate-800 rounded-lg overflow-hidden">
                <Table>
                  <TableHeader>
                    <TableRow className="bg-slate-50 dark:bg-slate-800/50 hover:bg-slate-50 dark:hover:bg-slate-800/50">
                      <TableHead className="w-12">
                        <Checkbox
                          checked={selectedRows.length === products.length}
                          onCheckedChange={handleSelectAll}
                        />
                      </TableHead>
                      <TableHead className="w-12"></TableHead>
                      <TableHead>
                        <button
                          onClick={() => handleSort('name')}
                          className="flex items-center gap-2 font-semibold hover:text-violet-600"
                        >
                          Product Name
                          <ArrowUpDown className="h-4 w-4" />
                        </button>
                      </TableHead>
                      <TableHead>Category</TableHead>
                      <TableHead>
                        <button
                          onClick={() => handleSort('price')}
                          className="flex items-center gap-2 font-semibold hover:text-violet-600"
                        >
                          Price
                          <ArrowUpDown className="h-4 w-4" />
                        </button>
                      </TableHead>
                      <TableHead>
                        <button
                          onClick={() => handleSort('stock')}
                          className="flex items-center gap-2 font-semibold hover:text-violet-600"
                        >
                          Stock
                          <ArrowUpDown className="h-4 w-4" />
                        </button>
                      </TableHead>
                      <TableHead>Status</TableHead>
                      <TableHead>Rating</TableHead>
                      <TableHead>Actions</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {products.map((product) => (
                      <TableRow
                        key={product.id}
                        className={`${
                          selectedRows.includes(product.id) ? 'bg-violet-50 dark:bg-violet-900/10' : ''
                        } hover:bg-slate-50 dark:hover:bg-slate-800/50 transition-colors`}
                      >
                        <TableCell>
                          <Checkbox
                            checked={selectedRows.includes(product.id)}
                            onCheckedChange={() => handleSelectRow(product.id)}
                          />
                        </TableCell>
                        <TableCell>
                          <button className="cursor-grab hover:cursor-grabbing">
                            <GripVertical className="h-4 w-4 text-slate-400" />
                          </button>
                        </TableCell>
                        <TableCell className="font-medium text-slate-900 dark:text-white">
                          {product.name}
                        </TableCell>
                        <TableCell>
                          <Badge variant="outline" className="text-xs">
                            {product.category}
                          </Badge>
                        </TableCell>
                        <TableCell className="font-semibold text-slate-900 dark:text-white">
                          ${product.price.toFixed(2)}
                        </TableCell>
                        <TableCell>
                          <div className="flex items-center gap-2">
                            <Progress value={(product.stock / 120) * 100} className="w-16 h-2" />
                            <span className="text-sm text-slate-600 dark:text-slate-400">{product.stock}</span>
                          </div>
                        </TableCell>
                        <TableCell>{getStatusBadge(product.status)}</TableCell>
                        <TableCell>
                          <div className="flex items-center gap-1">
                            <Star className="h-4 w-4 fill-amber-400 text-amber-400" />
                            <span className="text-sm font-medium">{product.rating}</span>
                          </div>
                        </TableCell>
                        <TableCell>
                          <div className="flex items-center gap-1">
                            <Button variant="ghost" size="sm" className="h-8 w-8 p-0">
                              <Eye className="h-4 w-4" />
                            </Button>
                            <Button variant="ghost" size="sm" className="h-8 w-8 p-0">
                              <Edit className="h-4 w-4" />
                            </Button>
                            <Button variant="ghost" size="sm" className="h-8 w-8 p-0">
                              <Heart className="h-4 w-4" />
                            </Button>
                            <DropdownMenu>
                              <DropdownMenuTrigger asChild>
                                <Button variant="ghost" size="sm" className="h-8 w-8 p-0">
                                  <MoreVertical className="h-4 w-4" />
                                </Button>
                              </DropdownMenuTrigger>
                              <DropdownMenuContent align="end">
                                <DropdownMenuLabel>Actions</DropdownMenuLabel>
                                <DropdownMenuSeparator />
                                <DropdownMenuItem>
                                  <Copy className="h-4 w-4 mr-2" />
                                  Duplicate
                                </DropdownMenuItem>
                                <DropdownMenuItem>
                                  <Share2 className="h-4 w-4 mr-2" />
                                  Share
                                </DropdownMenuItem>
                                <DropdownMenuItem>
                                  <Settings className="h-4 w-4 mr-2" />
                                  Settings
                                </DropdownMenuItem>
                                <DropdownMenuSeparator />
                                <DropdownMenuItem className="text-red-600">
                                  <Trash2 className="h-4 w-4 mr-2" />
                                  Delete
                                </DropdownMenuItem>
                              </DropdownMenuContent>
                            </DropdownMenu>
                          </div>
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </div>

              {/* Pagination */}
              <div className="flex items-center justify-between">
                <p className="text-sm text-slate-600 dark:text-slate-400">
                  Showing <span className="font-medium">1-8</span> of <span className="font-medium">8</span> results
                </p>
                <div className="flex gap-2">
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => setCurrentPage(Math.max(1, currentPage - 1))}
                    disabled={currentPage === 1}
                  >
                    <ChevronLeft className="h-4 w-4" />
                  </Button>
                  <Button variant="outline" size="sm" className="bg-violet-50 dark:bg-violet-900/20 border-violet-200 dark:border-violet-800">
                    1
                  </Button>
                  <Button variant="outline" size="sm">2</Button>
                  <Button variant="outline" size="sm">3</Button>
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => setCurrentPage(currentPage + 1)}
                  >
                    <ChevronRight className="h-4 w-4" />
                  </Button>
                </div>
              </div>
            </div>
          </TabsContent>

          {/* Charts Tab */}
          <TabsContent value="charts" className="p-6">
            <div className="space-y-6">
              <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                {/* Line Chart */}
                <div>
                  <h3 className="text-lg font-semibold mb-4 text-slate-900 dark:text-white">Line Chart</h3>
                  <ResponsiveContainer width="100%" height={250}>
                    <LineChart data={revenueData}>
                      <CartesianGrid strokeDasharray="3 3" stroke="#e2e8f0" />
                      <XAxis dataKey="month" stroke="#64748b" />
                      <YAxis stroke="#64748b" />
                      <Tooltip />
                      <Legend />
                      <Line type="monotone" dataKey="revenue" stroke="#8B5CF6" strokeWidth={2} />
                    </LineChart>
                  </ResponsiveContainer>
                </div>

                {/* Area Chart */}
                <div>
                  <h3 className="text-lg font-semibold mb-4 text-slate-900 dark:text-white">Area Chart</h3>
                  <ResponsiveContainer width="100%" height={250}>
                    <AreaChart data={revenueData}>
                      <CartesianGrid strokeDasharray="3 3" stroke="#e2e8f0" />
                      <XAxis dataKey="month" stroke="#64748b" />
                      <YAxis stroke="#64748b" />
                      <Tooltip />
                      <Legend />
                      <Area type="monotone" dataKey="orders" stroke="#3b82f6" fill="#3b82f6" fillOpacity={0.6} />
                    </AreaChart>
                  </ResponsiveContainer>
                </div>

                {/* Bar Chart */}
                <div>
                  <h3 className="text-lg font-semibold mb-4 text-slate-900 dark:text-white">Bar Chart</h3>
                  <ResponsiveContainer width="100%" height={250}>
                    <BarChart data={revenueData}>
                      <CartesianGrid strokeDasharray="3 3" stroke="#e2e8f0" />
                      <XAxis dataKey="month" stroke="#64748b" />
                      <YAxis stroke="#64748b" />
                      <Tooltip />
                      <Legend />
                      <Bar dataKey="customers" fill="#A78BFA" radius={[8, 8, 0, 0]} />
                    </BarChart>
                  </ResponsiveContainer>
                </div>

                {/* Pie Chart */}
                <div>
                  <h3 className="text-lg font-semibold mb-4 text-slate-900 dark:text-white">Pie Chart</h3>
                  <ResponsiveContainer width="100%" height={250}>
                    <PieChart>
                      <Pie
                        data={categoryData}
                        cx="50%"
                        cy="50%"
                        outerRadius={80}
                        fill="#8884d8"
                        dataKey="value"
                        label
                      >
                        {categoryData.map((entry, index) => (
                          <Cell key={`cell-${index}`} fill={entry.color} />
                        ))}
                      </Pie>
                      <Tooltip />
                    </PieChart>
                  </ResponsiveContainer>
                </div>
              </div>
            </div>
          </TabsContent>

          {/* Buttons Tab */}
          <TabsContent value="buttons" className="p-6">
            <div className="space-y-6">
              <div>
                <h3 className="text-lg font-semibold mb-4 text-slate-900 dark:text-white">Button Variants</h3>
                <div className="flex flex-wrap gap-3">
                  <Button className="bg-gradient-to-r from-violet-600 to-purple-600 hover:from-violet-700 hover:to-purple-700">
                    Primary Button
                  </Button>
                  <Button variant="outline">Outline Button</Button>
                  <Button variant="ghost">Ghost Button</Button>
                  <Button variant="destructive">Destructive</Button>
                  <Button disabled>Disabled</Button>
                </div>
              </div>

              <div>
                <h3 className="text-lg font-semibold mb-4 text-slate-900 dark:text-white">Button Sizes</h3>
                <div className="flex flex-wrap items-center gap-3">
                  <Button size="sm">Small</Button>
                  <Button size="default">Default</Button>
                  <Button size="lg">Large</Button>
                </div>
              </div>

              <div>
                <h3 className="text-lg font-semibold mb-4 text-slate-900 dark:text-white">Icon Buttons</h3>
                <div className="flex flex-wrap gap-3">
                  <Button className="bg-gradient-to-r from-violet-600 to-purple-600">
                    <Plus className="h-4 w-4 mr-2" />
                    Add New
                  </Button>
                  <Button variant="outline">
                    <Download className="h-4 w-4 mr-2" />
                    Download
                  </Button>
                  <Button variant="outline">
                    <Upload className="h-4 w-4 mr-2" />
                    Upload
                  </Button>
                  <Button variant="outline" size="sm" className="h-8 w-8 p-0">
                    <Settings className="h-4 w-4" />
                  </Button>
                </div>
              </div>
            </div>
          </TabsContent>

          {/* Forms Tab */}
          <TabsContent value="forms" className="p-6">
            <div className="space-y-6 max-w-2xl">
              <div>
                <h3 className="text-lg font-semibold mb-4 text-slate-900 dark:text-white">Form Controls</h3>
                <div className="space-y-4">
                  <div>
                    <label className="text-sm font-medium mb-2 block">Text Input</label>
                    <Input placeholder="Enter your name..." />
                  </div>
                  <div>
                    <label className="text-sm font-medium mb-2 block">Search Input</label>
                    <div className="relative">
                      <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-slate-400" />
                      <Input placeholder="Search..." className="pl-10" />
                    </div>
                  </div>
                  <div className="flex items-center space-x-2">
                    <Checkbox id="terms" />
                    <label htmlFor="terms" className="text-sm font-medium">
                      Accept terms and conditions
                    </label>
                  </div>
                  <div className="flex items-center space-x-2">
                    <Switch id="notifications" />
                    <label htmlFor="notifications" className="text-sm font-medium">
                      Enable notifications
                    </label>
                  </div>
                </div>
              </div>
            </div>
          </TabsContent>

          {/* Misc Tab */}
          <TabsContent value="misc" className="p-6">
            <div className="space-y-6">
              <div>
                <h3 className="text-lg font-semibold mb-4 text-slate-900 dark:text-white">Badges</h3>
                <div className="flex flex-wrap gap-2">
                  <Badge className="bg-violet-100 text-violet-700 dark:bg-violet-900/30 dark:text-violet-400">Violet</Badge>
                  <Badge className="bg-blue-100 text-blue-700 dark:bg-blue-900/30 dark:text-blue-400">Blue</Badge>
                  <Badge className="bg-emerald-100 text-emerald-700 dark:bg-emerald-900/30 dark:text-emerald-400">Success</Badge>
                  <Badge className="bg-amber-100 text-amber-700 dark:bg-amber-900/30 dark:text-amber-400">Warning</Badge>
                  <Badge className="bg-red-100 text-red-700 dark:bg-red-900/30 dark:text-red-400">Error</Badge>
                </div>
              </div>

              <div>
                <h3 className="text-lg font-semibold mb-4 text-slate-900 dark:text-white">Avatars</h3>
                <div className="flex gap-3">
                  <Avatar className="h-12 w-12 border-2 border-violet-500">
                    <AvatarFallback className="bg-gradient-to-br from-violet-500 to-purple-600 text-white font-bold">
                      JD
                    </AvatarFallback>
                  </Avatar>
                  <Avatar className="h-12 w-12 border-2 border-blue-500">
                    <AvatarFallback className="bg-gradient-to-br from-blue-500 to-indigo-600 text-white font-bold">
                      AB
                    </AvatarFallback>
                  </Avatar>
                  <Avatar className="h-12 w-12 border-2 border-pink-500">
                    <AvatarFallback className="bg-gradient-to-br from-pink-500 to-rose-600 text-white font-bold">
                      CD
                    </AvatarFallback>
                  </Avatar>
                </div>
              </div>

              <div>
                <h3 className="text-lg font-semibold mb-4 text-slate-900 dark:text-white">Progress Bars</h3>
                <div className="space-y-3">
                  <div>
                    <div className="flex justify-between text-sm mb-2">
                      <span>Progress 75%</span>
                      <span className="font-medium">75/100</span>
                    </div>
                    <Progress value={75} className="h-2" />
                  </div>
                  <div>
                    <div className="flex justify-between text-sm mb-2">
                      <span>Progress 45%</span>
                      <span className="font-medium">45/100</span>
                    </div>
                    <Progress value={45} className="h-2" />
                  </div>
                  <div>
                    <div className="flex justify-between text-sm mb-2">
                      <span>Progress 90%</span>
                      <span className="font-medium">90/100</span>
                    </div>
                    <Progress value={90} className="h-2" />
                  </div>
                </div>
              </div>
            </div>
          </TabsContent>
        </Tabs>
      </Card>
    </div>
  );
};

export default UIShowcase;
