# UI Components Library - Hướng Dẫn Sử Dụng

## 📚 Tổng Quan

Đây là bộ sưu tập UI components được xây dựng dựa trên **Radix UI** và **Tailwind CSS**, cung cấp các components đẹp, dễ sử dụng và có thể tùy chỉnh cho dự án.

## 🎨 Xem Demo

- **UI Showcase**: http://localhost:5173/ui-showcase - Xem tất cả components với UI đẹp
- **UI Components**: http://localhost:5173/ui-components - Copy code examples

## 📦 Components Có Sẵn

### 1. **Button** (`src/components/ui/button.tsx`)
Nút bấm với nhiều variants và sizes

**Variants:**
- `default` - Nút chính (primary)
- `secondary` - Nút phụ
- `destructive` - Nút xóa/hủy
- `outline` - Nút viền
- `ghost` - Nút trong suốt
- `link` - Nút dạng link

**Sizes:**
- `sm` - Nhỏ
- `default` - Mặc định
- `lg` - Lớn
- `icon` - Icon vuông

**Example:**
```tsx
import { Button } from "@/components/ui/button"

<Button variant="default">Click me</Button>
<Button variant="outline" size="lg">Large Button</Button>
<Button size="icon"><Search className="h-4 w-4" /></Button>
```

### 2. **Card** (`src/components/ui/card.tsx`)
Thẻ nội dung với header, content và footer

**Components:**
- `Card` - Container chính
- `CardHeader` - Phần header
- `CardTitle` - Tiêu đề
- `CardDescription` - Mô tả
- `CardContent` - Nội dung
- `CardFooter` - Footer với actions

**Example:**
```tsx
import { Card, CardContent, CardDescription, CardHeader, CardTitle, CardFooter } from "@/components/ui/card"

<Card>
  <CardHeader>
    <CardTitle>Card Title</CardTitle>
    <CardDescription>Description here</CardDescription>
  </CardHeader>
  <CardContent>
    <p>Your content</p>
  </CardContent>
  <CardFooter>
    <Button>Action</Button>
  </CardFooter>
</Card>
```

### 3. **Badge** (`src/components/ui/badge.tsx`)
Nhãn trạng thái, tag

**Variants:**
- `default` - Mặc định
- `secondary` - Phụ
- `destructive` - Nguy hiểm
- `outline` - Viền
- `success` - Thành công (màu xanh lá)
- `warning` - Cảnh báo (màu vàng)
- `info` - Thông tin (màu xanh dương)

**Example:**
```tsx
import { Badge } from "@/components/ui/badge"

<Badge variant="success">Active</Badge>
<Badge variant="warning">Pending</Badge>
<Badge variant="destructive">Inactive</Badge>
```

### 4. **Table** (`src/components/ui/table.tsx`)
Bảng dữ liệu

**Components:**
- `Table` - Container bảng
- `TableHeader` - Header
- `TableBody` - Body
- `TableRow` - Dòng
- `TableHead` - Cell header
- `TableCell` - Cell dữ liệu
- `TableCaption` - Caption

**Example:**
```tsx
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table"

<Table>
  <TableHeader>
    <TableRow>
      <TableHead>Name</TableHead>
      <TableHead>Status</TableHead>
    </TableRow>
  </TableHeader>
  <TableBody>
    <TableRow>
      <TableCell>Product 1</TableCell>
      <TableCell><Badge variant="success">Active</Badge></TableCell>
    </TableRow>
  </TableBody>
</Table>
```

### 5. **Form Components**

#### Input (`src/components/ui/input.tsx`)
```tsx
import { Input } from "@/components/ui/input"

<Input type="email" placeholder="Email" />
```

#### Label (`src/components/ui/label.tsx`)
```tsx
import { Label } from "@/components/ui/label"

<Label htmlFor="email">Email</Label>
<Input id="email" />
```

#### Textarea (`src/components/ui/textarea.tsx`)
```tsx
import { Textarea } from "@/components/ui/textarea"

<Textarea placeholder="Type your message..." />
```

#### Checkbox (`src/components/ui/checkbox.tsx`)
```tsx
import { Checkbox } from "@/components/ui/checkbox"

<Checkbox id="terms" />
<label htmlFor="terms">Accept terms</label>
```

#### Switch (`src/components/ui/switch.tsx`)
```tsx
import { Switch } from "@/components/ui/switch"

<Switch id="notifications" />
```

### 6. **Tabs** (`src/components/ui/tabs.tsx`)
Tabs điều hướng

**Example:**
```tsx
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"

<Tabs defaultValue="tab1">
  <TabsList>
    <TabsTrigger value="tab1">Tab 1</TabsTrigger>
    <TabsTrigger value="tab2">Tab 2</TabsTrigger>
  </TabsList>
  <TabsContent value="tab1">Content 1</TabsContent>
  <TabsContent value="tab2">Content 2</TabsContent>
</Tabs>
```

### 7. **Alert** (`src/components/ui/alert.tsx`)
Thông báo, cảnh báo

**Variants:**
- `default` - Mặc định
- `destructive` - Lỗi
- `success` - Thành công
- `warning` - Cảnh báo
- `info` - Thông tin

**Example:**
```tsx
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert"

<Alert variant="success">
  <CheckCircle2 className="h-4 w-4" />
  <AlertTitle>Success</AlertTitle>
  <AlertDescription>Your changes have been saved!</AlertDescription>
</Alert>
```

### 8. **Avatar** (`src/components/ui/avatar.tsx`)
Ảnh đại diện

**Example:**
```tsx
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar"

<Avatar>
  <AvatarImage src="https://github.com/shadcn.png" />
  <AvatarFallback>CN</AvatarFallback>
</Avatar>
```

### 9. **Progress** (`src/components/ui/progress.tsx`)
Thanh tiến trình

**Example:**
```tsx
import { Progress } from "@/components/ui/progress"

<Progress value={60} />
```

### 10. **Skeleton** (`src/components/ui/skeleton.tsx`)
Loading skeleton

**Example:**
```tsx
import { Skeleton } from "@/components/ui/skeleton"

<Skeleton className="h-4 w-full" />
<Skeleton className="h-4 w-3/4" />
```

## 🎯 Cách Sử Dụng

### 1. Copy từ UI Showcase
- Truy cập http://localhost:5173/ui-showcase
- Xem preview các components
- Nhấn vào icon Copy để copy code

### 2. Copy từ UI Components
- Truy cập http://localhost:5173/ui-components
- Xem code examples chi tiết
- Click icon Copy để copy code
- Paste vào file của bạn

### 3. Import Components
```tsx
// Import components bạn cần
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"

// Sử dụng trong component
function MyComponent() {
  return (
    <Card>
      <CardHeader>
        <CardTitle>Hello</CardTitle>
      </CardHeader>
      <CardContent>
        <Badge variant="success">Active</Badge>
        <Button>Click me</Button>
      </CardContent>
    </Card>
  )
}
```

## 🎨 Tùy Chỉnh Styles

Các components sử dụng Tailwind CSS, bạn có thể:

1. **Thêm className:**
```tsx
<Button className="mt-4 bg-blue-600">Custom Button</Button>
```

2. **Override styles:**
```tsx
<Card className="shadow-2xl border-blue-500">
  {/* content */}
</Card>
```

3. **Sử dụng Tailwind utilities:**
```tsx
<div className="grid grid-cols-3 gap-4">
  <Card>Card 1</Card>
  <Card>Card 2</Card>
  <Card>Card 3</Card>
</div>
```

## 📝 Best Practices

1. **Sử dụng đúng variant cho đúng mục đích:**
   - `destructive` cho actions nguy hiểm (delete, cancel)
   - `success` cho trạng thái thành công
   - `warning` cho cảnh báo
   - `info` cho thông tin

2. **Kết hợp components:**
```tsx
<Card>
  <CardHeader>
    <div className="flex items-center justify-between">
      <CardTitle>Product Name</CardTitle>
      <Badge variant="success">In Stock</Badge>
    </div>
  </CardHeader>
  <CardContent>
    <Table>
      {/* table content */}
    </Table>
  </CardContent>
  <CardFooter>
    <Button variant="outline">Cancel</Button>
    <Button>Save</Button>
  </CardFooter>
</Card>
```

3. **Sử dụng icons từ lucide-react:**
```tsx
import { Mail, Search, Trash2, Edit } from "lucide-react"

<Button>
  <Mail className="mr-2 h-4 w-4" />
  Send Email
</Button>
```

## 🚀 Mẫu Layouts Phổ Biến

### Dashboard Stats Cards
```tsx
<div className="grid grid-cols-1 md:grid-cols-3 gap-4">
  <Card>
    <CardHeader className="pb-3">
      <CardTitle className="text-sm">Total Revenue</CardTitle>
    </CardHeader>
    <CardContent>
      <div className="text-2xl font-bold">$45,231.89</div>
      <p className="text-xs text-muted-foreground">+20.1% from last month</p>
    </CardContent>
  </Card>
  {/* More cards */}
</div>
```

### Data Table with Actions
```tsx
<Table>
  <TableHeader>
    <TableRow>
      <TableHead>Name</TableHead>
      <TableHead>Status</TableHead>
      <TableHead className="text-center">Actions</TableHead>
    </TableRow>
  </TableHeader>
  <TableBody>
    <TableRow>
      <TableCell>Product Name</TableCell>
      <TableCell><Badge variant="success">Active</Badge></TableCell>
      <TableCell className="text-center">
        <div className="flex justify-center gap-2">
          <Button size="icon" variant="ghost"><Eye className="h-4 w-4" /></Button>
          <Button size="icon" variant="ghost"><Edit className="h-4 w-4" /></Button>
          <Button size="icon" variant="ghost"><Trash2 className="h-4 w-4" /></Button>
        </div>
      </TableCell>
    </TableRow>
  </TableBody>
</Table>
```

### Form Layout
```tsx
<Card>
  <CardHeader>
    <CardTitle>Create Account</CardTitle>
    <CardDescription>Enter your details below</CardDescription>
  </CardHeader>
  <CardContent className="space-y-4">
    <div className="grid grid-cols-2 gap-4">
      <div className="space-y-2">
        <Label htmlFor="firstName">First name</Label>
        <Input id="firstName" />
      </div>
      <div className="space-y-2">
        <Label htmlFor="lastName">Last name</Label>
        <Input id="lastName" />
      </div>
    </div>
    <div className="space-y-2">
      <Label htmlFor="email">Email</Label>
      <Input id="email" type="email" />
    </div>
  </CardContent>
  <CardFooter className="flex justify-between">
    <Button variant="outline">Cancel</Button>
    <Button>Create Account</Button>
  </CardFooter>
</Card>
```

## 📚 Resources

- [Radix UI Documentation](https://www.radix-ui.com/)
- [Tailwind CSS Documentation](https://tailwindcss.com/)
- [Lucide Icons](https://lucide.dev/)
- UI Showcase: http://localhost:5173/ui-showcase
- UI Components: http://localhost:5173/ui-components

## 💡 Tips

1. Luôn xem UI Showcase trước khi code để chọn component phù hợp
2. Copy code từ UI Components để tiết kiệm thời gian
3. Sử dụng Tailwind classes để tùy chỉnh nhanh
4. Kết hợp nhiều components để tạo layouts phức tạp
5. Sử dụng icons từ lucide-react để UI đẹp hơn

## 🤝 Đóng Góp

Nếu bạn muốn thêm components mới hoặc cải thiện components hiện tại:

1. Tạo component trong `src/components/ui/`
2. Thêm examples vào `UIShowcase.tsx` và `UIComponents.tsx`
3. Update README này với hướng dẫn sử dụng
4. Test kỹ trước khi commit

---

**Happy Coding! 🎉**
