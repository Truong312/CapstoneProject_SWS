import { useState } from "react";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Badge } from "@/components/ui/badge";
import { Copy, Check } from "lucide-react";

const UIComponents = () => {
  const [copiedCode, setCopiedCode] = useState<string | null>(null);

  const copyToClipboard = (code: string, id: string) => {
    navigator.clipboard.writeText(code);
    setCopiedCode(id);
    setTimeout(() => setCopiedCode(null), 2000);
  };

  const codeExamples = {
    button: {
      basic: `<Button>Click me</Button>
<Button variant="secondary">Secondary</Button>
<Button variant="outline">Outline</Button>
<Button variant="destructive">Delete</Button>`,
      
      withIcon: `<Button>
  <Mail className="mr-2 h-4 w-4" />
  Email
</Button>

<Button size="icon">
  <Search className="h-4 w-4" />
</Button>`,
      
      loading: `const [isLoading, setIsLoading] = useState(false);

<Button onClick={() => setIsLoading(!isLoading)}>
  {isLoading && (
    <div className="mr-2 h-4 w-4 animate-spin rounded-full border-2 border-background border-t-transparent" />
  )}
  {isLoading ? "Loading..." : "Click to Load"}
</Button>`,
    },
    
    card: {
      basic: `<Card>
  <CardHeader>
    <CardTitle>Card Title</CardTitle>
    <CardDescription>Card Description</CardDescription>
  </CardHeader>
  <CardContent>
    <p>Card content goes here</p>
  </CardContent>
  <CardFooter>
    <Button>Action</Button>
  </CardFooter>
</Card>`,
      
      stats: `<Card>
  <CardHeader className="pb-3">
    <CardTitle className="text-sm">Total Revenue</CardTitle>
  </CardHeader>
  <CardContent>
    <div className="text-2xl font-bold">$45,231.89</div>
    <p className="text-xs text-muted-foreground">
      +20.1% from last month
    </p>
  </CardContent>
</Card>`,
      
      profile: `<Card className="overflow-hidden">
  <div className="h-32 bg-gradient-to-r from-blue-500 to-purple-600" />
  <CardContent className="pt-0">
    <div className="flex flex-col items-center -mt-16">
      <Avatar className="h-32 w-32 border-4 border-white">
        <AvatarImage src="https://github.com/shadcn.png" />
        <AvatarFallback>CN</AvatarFallback>
      </Avatar>
      <h3 className="mt-4 text-xl font-semibold">John Doe</h3>
      <p className="text-sm text-muted-foreground">Software Developer</p>
    </div>
  </CardContent>
</Card>`,
    },
    
    table: {
      basic: `<Table>
  <TableCaption>A list of your recent invoices.</TableCaption>
  <TableHeader>
    <TableRow>
      <TableHead>Invoice</TableHead>
      <TableHead>Status</TableHead>
      <TableHead className="text-right">Amount</TableHead>
    </TableRow>
  </TableHeader>
  <TableBody>
    <TableRow>
      <TableCell className="font-medium">INV001</TableCell>
      <TableCell>
        <Badge variant="success">Paid</Badge>
      </TableCell>
      <TableCell className="text-right">$250.00</TableCell>
    </TableRow>
  </TableBody>
</Table>`,
      
      withActions: `<Table>
  <TableHeader>
    <TableRow>
      <TableHead>Product</TableHead>
      <TableHead>Status</TableHead>
      <TableHead className="text-center">Actions</TableHead>
    </TableRow>
  </TableHeader>
  <TableBody>
    <TableRow>
      <TableCell>Product Name</TableCell>
      <TableCell>
        <Badge variant="success">Active</Badge>
      </TableCell>
      <TableCell className="text-center">
        <div className="flex justify-center gap-2">
          <Button size="icon" variant="ghost">
            <Eye className="h-4 w-4" />
          </Button>
          <Button size="icon" variant="ghost">
            <Edit className="h-4 w-4" />
          </Button>
          <Button size="icon" variant="ghost">
            <Trash2 className="h-4 w-4" />
          </Button>
        </div>
      </TableCell>
    </TableRow>
  </TableBody>
</Table>`,
    },
    
    form: {
      basic: `<div className="space-y-4">
  <div className="space-y-2">
    <Label htmlFor="email">Email</Label>
    <Input id="email" type="email" placeholder="email@example.com" />
  </div>
  
  <div className="space-y-2">
    <Label htmlFor="password">Password</Label>
    <Input id="password" type="password" />
  </div>
  
  <Button className="w-full">Submit</Button>
</div>`,
      
      withCheckbox: `<div className="flex items-center space-x-2">
  <Checkbox id="terms" />
  <label htmlFor="terms" className="text-sm">
    Accept terms and conditions
  </label>
</div>

<div className="flex items-center justify-between">
  <Label htmlFor="notifications">Enable notifications</Label>
  <Switch id="notifications" />
</div>`,
      
      complete: `<Card>
  <CardHeader>
    <CardTitle>Create Account</CardTitle>
    <CardDescription>Enter your details below</CardDescription>
  </CardHeader>
  <CardContent className="space-y-4">
    <div className="grid grid-cols-2 gap-4">
      <div className="space-y-2">
        <Label htmlFor="first-name">First name</Label>
        <Input id="first-name" placeholder="John" />
      </div>
      <div className="space-y-2">
        <Label htmlFor="last-name">Last name</Label>
        <Input id="last-name" placeholder="Doe" />
      </div>
    </div>
    <div className="space-y-2">
      <Label htmlFor="email">Email</Label>
      <Input id="email" type="email" />
    </div>
    <div className="space-y-2">
      <Label htmlFor="message">Message</Label>
      <Textarea id="message" placeholder="Type your message..." />
    </div>
    <div className="flex items-center space-x-2">
      <Checkbox id="terms" />
      <label htmlFor="terms" className="text-sm">
        I agree to the terms and conditions
      </label>
    </div>
  </CardContent>
  <CardFooter className="flex justify-between">
    <Button variant="outline">Cancel</Button>
    <Button>Create Account</Button>
  </CardFooter>
</Card>`,
    },
    
    tabs: {
      basic: `<Tabs defaultValue="account">
  <TabsList>
    <TabsTrigger value="account">Account</TabsTrigger>
    <TabsTrigger value="password">Password</TabsTrigger>
    <TabsTrigger value="settings">Settings</TabsTrigger>
  </TabsList>
  <TabsContent value="account">
    Account settings content
  </TabsContent>
  <TabsContent value="password">
    Password settings content
  </TabsContent>
  <TabsContent value="settings">
    General settings content
  </TabsContent>
</Tabs>`,
      
      withCards: `<Tabs defaultValue="overview" className="w-full">
  <TabsList className="grid w-full grid-cols-3">
    <TabsTrigger value="overview">Overview</TabsTrigger>
    <TabsTrigger value="analytics">Analytics</TabsTrigger>
    <TabsTrigger value="reports">Reports</TabsTrigger>
  </TabsList>
  <TabsContent value="overview">
    <Card>
      <CardHeader>
        <CardTitle>Overview</CardTitle>
        <CardDescription>View your overview here</CardDescription>
      </CardHeader>
      <CardContent>
        Content goes here...
      </CardContent>
    </Card>
  </TabsContent>
</Tabs>`,
    },
    
    badge: {
      basic: `<Badge>Default</Badge>
<Badge variant="secondary">Secondary</Badge>
<Badge variant="destructive">Destructive</Badge>
<Badge variant="outline">Outline</Badge>
<Badge variant="success">Success</Badge>
<Badge variant="warning">Warning</Badge>
<Badge variant="info">Info</Badge>`,
      
      withIcon: `<Badge variant="success">
  <CheckCircle2 className="mr-1 h-3 w-3" />
  Active
</Badge>

<Badge variant="warning">
  <AlertTriangle className="mr-1 h-3 w-3" />
  Pending
</Badge>

<Badge variant="info">
  <Bell className="mr-1 h-3 w-3" />
  New (5)
</Badge>`,
    },
    
    alert: {
      basic: `<Alert>
  <Info className="h-4 w-4" />
  <AlertTitle>Information</AlertTitle>
  <AlertDescription>
    This is an informational message.
  </AlertDescription>
</Alert>`,
      
      variants: `<Alert variant="success">
  <CheckCircle2 className="h-4 w-4" />
  <AlertTitle>Success</AlertTitle>
  <AlertDescription>
    Your changes have been saved successfully!
  </AlertDescription>
</Alert>

<Alert variant="warning">
  <AlertTriangle className="h-4 w-4" />
  <AlertTitle>Warning</AlertTitle>
  <AlertDescription>
    Please review your input before submitting.
  </AlertDescription>
</Alert>

<Alert variant="destructive">
  <AlertCircle className="h-4 w-4" />
  <AlertTitle>Error</AlertTitle>
  <AlertDescription>
    An error occurred. Please try again later.
  </AlertDescription>
</Alert>`,
    },
    
    avatar: {
      basic: `<Avatar>
  <AvatarImage src="https://github.com/shadcn.png" />
  <AvatarFallback>CN</AvatarFallback>
</Avatar>`,
      
      sizes: `<Avatar className="h-20 w-20">
  <AvatarImage src="https://github.com/shadcn.png" />
  <AvatarFallback>CN</AvatarFallback>
</Avatar>

<Avatar className="h-12 w-12">
  <AvatarFallback>AB</AvatarFallback>
</Avatar>

<Avatar className="h-8 w-8">
  <AvatarFallback className="bg-blue-500 text-white">JD</AvatarFallback>
</Avatar>`,
      
      group: `<div className="flex items-center gap-2">
  <Avatar className="h-10 w-10">
    <AvatarFallback className="bg-red-500 text-white">A</AvatarFallback>
  </Avatar>
  <Avatar className="h-10 w-10">
    <AvatarFallback className="bg-blue-500 text-white">B</AvatarFallback>
  </Avatar>
  <Avatar className="h-10 w-10">
    <AvatarFallback className="bg-green-500 text-white">C</AvatarFallback>
  </Avatar>
</div>`,
    },
    
    progress: {
      basic: `<Progress value={33} />`,
      
      withLabel: `<div className="space-y-2">
  <div className="flex items-center justify-between">
    <Label>Download Progress</Label>
    <span className="text-sm text-muted-foreground">33%</span>
  </div>
  <Progress value={33} />
</div>`,
      
      dynamic: `const [progress, setProgress] = useState(33);

<div className="space-y-2">
  <Progress value={progress} />
  <Button onClick={() => setProgress(Math.min(100, progress + 10))}>
    Increase Progress
  </Button>
</div>`,
    },
    
    skeleton: {
      basic: `<Skeleton className="h-4 w-full" />
<Skeleton className="h-4 w-3/4" />
<Skeleton className="h-4 w-1/2" />`,
      
      card: `<Card>
  <CardHeader>
    <Skeleton className="h-4 w-1/2" />
    <Skeleton className="h-3 w-3/4 mt-2" />
  </CardHeader>
  <CardContent className="space-y-2">
    <Skeleton className="h-20 w-full" />
    <Skeleton className="h-4 w-full" />
    <Skeleton className="h-4 w-2/3" />
  </CardContent>
</Card>`,
    },
  };

  const CodeBlock = ({ code, id }: { code: string; id: string }) => (
    <div className="relative">
      <pre className="bg-slate-950 text-slate-50 p-4 rounded-lg overflow-x-auto text-sm">
        <code>{code}</code>
      </pre>
      <Button
        size="icon"
        variant="ghost"
        className="absolute top-2 right-2 h-8 w-8 text-white hover:bg-slate-800"
        onClick={() => copyToClipboard(code, id)}
      >
        {copiedCode === id ? (
          <Check className="h-4 w-4 text-green-400" />
        ) : (
          <Copy className="h-4 w-4" />
        )}
      </Button>
    </div>
  );

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-50 to-slate-100 p-8">
      <div className="max-w-6xl mx-auto space-y-8">
        {/* Header */}
        <div className="text-center space-y-4">
          <h1 className="text-4xl font-bold bg-gradient-to-r from-blue-600 to-purple-600 bg-clip-text text-transparent">
            UI Components - Code Examples
          </h1>
          <p className="text-lg text-muted-foreground">
            Copy và paste code examples để sử dụng trong dự án của bạn
          </p>
          <Badge variant="info" className="text-sm">
            Click icon <Copy className="inline h-3 w-3 mx-1" /> để copy code
          </Badge>
        </div>

        {/* Buttons */}
        <Card>
          <CardHeader>
            <CardTitle>Buttons</CardTitle>
            <CardDescription>Các mẫu button phổ biến</CardDescription>
          </CardHeader>
          <CardContent>
            <Tabs defaultValue="basic">
              <TabsList>
                <TabsTrigger value="basic">Basic</TabsTrigger>
                <TabsTrigger value="icon">With Icon</TabsTrigger>
                <TabsTrigger value="loading">Loading State</TabsTrigger>
              </TabsList>
              <TabsContent value="basic" className="mt-4">
                <CodeBlock code={codeExamples.button.basic} id="button-basic" />
              </TabsContent>
              <TabsContent value="icon" className="mt-4">
                <CodeBlock code={codeExamples.button.withIcon} id="button-icon" />
              </TabsContent>
              <TabsContent value="loading" className="mt-4">
                <CodeBlock code={codeExamples.button.loading} id="button-loading" />
              </TabsContent>
            </Tabs>
          </CardContent>
        </Card>

        {/* Cards */}
        <Card>
          <CardHeader>
            <CardTitle>Cards</CardTitle>
            <CardDescription>Các mẫu card layout</CardDescription>
          </CardHeader>
          <CardContent>
            <Tabs defaultValue="basic">
              <TabsList>
                <TabsTrigger value="basic">Basic</TabsTrigger>
                <TabsTrigger value="stats">Stats Card</TabsTrigger>
                <TabsTrigger value="profile">Profile Card</TabsTrigger>
              </TabsList>
              <TabsContent value="basic" className="mt-4">
                <CodeBlock code={codeExamples.card.basic} id="card-basic" />
              </TabsContent>
              <TabsContent value="stats" className="mt-4">
                <CodeBlock code={codeExamples.card.stats} id="card-stats" />
              </TabsContent>
              <TabsContent value="profile" className="mt-4">
                <CodeBlock code={codeExamples.card.profile} id="card-profile" />
              </TabsContent>
            </Tabs>
          </CardContent>
        </Card>

        {/* Tables */}
        <Card>
          <CardHeader>
            <CardTitle>Tables</CardTitle>
            <CardDescription>Các mẫu table hiển thị dữ liệu</CardDescription>
          </CardHeader>
          <CardContent>
            <Tabs defaultValue="basic">
              <TabsList>
                <TabsTrigger value="basic">Basic Table</TabsTrigger>
                <TabsTrigger value="actions">With Actions</TabsTrigger>
              </TabsList>
              <TabsContent value="basic" className="mt-4">
                <CodeBlock code={codeExamples.table.basic} id="table-basic" />
              </TabsContent>
              <TabsContent value="actions" className="mt-4">
                <CodeBlock code={codeExamples.table.withActions} id="table-actions" />
              </TabsContent>
            </Tabs>
          </CardContent>
        </Card>

        {/* Forms */}
        <Card>
          <CardHeader>
            <CardTitle>Forms</CardTitle>
            <CardDescription>Các mẫu form input</CardDescription>
          </CardHeader>
          <CardContent>
            <Tabs defaultValue="basic">
              <TabsList>
                <TabsTrigger value="basic">Basic Form</TabsTrigger>
                <TabsTrigger value="checkbox">With Checkbox/Switch</TabsTrigger>
                <TabsTrigger value="complete">Complete Form</TabsTrigger>
              </TabsList>
              <TabsContent value="basic" className="mt-4">
                <CodeBlock code={codeExamples.form.basic} id="form-basic" />
              </TabsContent>
              <TabsContent value="checkbox" className="mt-4">
                <CodeBlock code={codeExamples.form.withCheckbox} id="form-checkbox" />
              </TabsContent>
              <TabsContent value="complete" className="mt-4">
                <CodeBlock code={codeExamples.form.complete} id="form-complete" />
              </TabsContent>
            </Tabs>
          </CardContent>
        </Card>

        {/* Tabs */}
        <Card>
          <CardHeader>
            <CardTitle>Tabs</CardTitle>
            <CardDescription>Tabs navigation examples</CardDescription>
          </CardHeader>
          <CardContent>
            <Tabs defaultValue="basic">
              <TabsList>
                <TabsTrigger value="basic">Basic Tabs</TabsTrigger>
                <TabsTrigger value="cards">With Cards</TabsTrigger>
              </TabsList>
              <TabsContent value="basic" className="mt-4">
                <CodeBlock code={codeExamples.tabs.basic} id="tabs-basic" />
              </TabsContent>
              <TabsContent value="cards" className="mt-4">
                <CodeBlock code={codeExamples.tabs.withCards} id="tabs-cards" />
              </TabsContent>
            </Tabs>
          </CardContent>
        </Card>

        {/* Badges */}
        <Card>
          <CardHeader>
            <CardTitle>Badges</CardTitle>
            <CardDescription>Badge variants và use cases</CardDescription>
          </CardHeader>
          <CardContent>
            <Tabs defaultValue="basic">
              <TabsList>
                <TabsTrigger value="basic">Basic</TabsTrigger>
                <TabsTrigger value="icon">With Icon</TabsTrigger>
              </TabsList>
              <TabsContent value="basic" className="mt-4">
                <CodeBlock code={codeExamples.badge.basic} id="badge-basic" />
              </TabsContent>
              <TabsContent value="icon" className="mt-4">
                <CodeBlock code={codeExamples.badge.withIcon} id="badge-icon" />
              </TabsContent>
            </Tabs>
          </CardContent>
        </Card>

        {/* Alerts */}
        <Card>
          <CardHeader>
            <CardTitle>Alerts</CardTitle>
            <CardDescription>Alert messages và notifications</CardDescription>
          </CardHeader>
          <CardContent>
            <Tabs defaultValue="basic">
              <TabsList>
                <TabsTrigger value="basic">Basic</TabsTrigger>
                <TabsTrigger value="variants">All Variants</TabsTrigger>
              </TabsList>
              <TabsContent value="basic" className="mt-4">
                <CodeBlock code={codeExamples.alert.basic} id="alert-basic" />
              </TabsContent>
              <TabsContent value="variants" className="mt-4">
                <CodeBlock code={codeExamples.alert.variants} id="alert-variants" />
              </TabsContent>
            </Tabs>
          </CardContent>
        </Card>

        {/* Avatars */}
        <Card>
          <CardHeader>
            <CardTitle>Avatars</CardTitle>
            <CardDescription>User avatar examples</CardDescription>
          </CardHeader>
          <CardContent>
            <Tabs defaultValue="basic">
              <TabsList>
                <TabsTrigger value="basic">Basic</TabsTrigger>
                <TabsTrigger value="sizes">Different Sizes</TabsTrigger>
                <TabsTrigger value="group">Avatar Group</TabsTrigger>
              </TabsList>
              <TabsContent value="basic" className="mt-4">
                <CodeBlock code={codeExamples.avatar.basic} id="avatar-basic" />
              </TabsContent>
              <TabsContent value="sizes" className="mt-4">
                <CodeBlock code={codeExamples.avatar.sizes} id="avatar-sizes" />
              </TabsContent>
              <TabsContent value="group" className="mt-4">
                <CodeBlock code={codeExamples.avatar.group} id="avatar-group" />
              </TabsContent>
            </Tabs>
          </CardContent>
        </Card>

        {/* Progress */}
        <Card>
          <CardHeader>
            <CardTitle>Progress</CardTitle>
            <CardDescription>Progress bars và loading states</CardDescription>
          </CardHeader>
          <CardContent>
            <Tabs defaultValue="basic">
              <TabsList>
                <TabsTrigger value="basic">Basic</TabsTrigger>
                <TabsTrigger value="label">With Label</TabsTrigger>
                <TabsTrigger value="dynamic">Dynamic</TabsTrigger>
              </TabsList>
              <TabsContent value="basic" className="mt-4">
                <CodeBlock code={codeExamples.progress.basic} id="progress-basic" />
              </TabsContent>
              <TabsContent value="label" className="mt-4">
                <CodeBlock code={codeExamples.progress.withLabel} id="progress-label" />
              </TabsContent>
              <TabsContent value="dynamic" className="mt-4">
                <CodeBlock code={codeExamples.progress.dynamic} id="progress-dynamic" />
              </TabsContent>
            </Tabs>
          </CardContent>
        </Card>

        {/* Skeleton */}
        <Card>
          <CardHeader>
            <CardTitle>Skeleton</CardTitle>
            <CardDescription>Loading skeletons</CardDescription>
          </CardHeader>
          <CardContent>
            <Tabs defaultValue="basic">
              <TabsList>
                <TabsTrigger value="basic">Basic</TabsTrigger>
                <TabsTrigger value="card">Card Skeleton</TabsTrigger>
              </TabsList>
              <TabsContent value="basic" className="mt-4">
                <CodeBlock code={codeExamples.skeleton.basic} id="skeleton-basic" />
              </TabsContent>
              <TabsContent value="card" className="mt-4">
                <CodeBlock code={codeExamples.skeleton.card} id="skeleton-card" />
              </TabsContent>
            </Tabs>
          </CardContent>
        </Card>

        {/* Import Guide */}
        <Card className="bg-gradient-to-r from-blue-500 to-purple-600 text-white border-0">
          <CardHeader>
            <CardTitle>Import Components</CardTitle>
            <CardDescription className="text-blue-100">
              Đừng quên import các components trước khi sử dụng
            </CardDescription>
          </CardHeader>
          <CardContent>
            <CodeBlock
              code={`import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Badge } from "@/components/ui/badge";
import { Table, TableBody, TableCaption, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Checkbox } from "@/components/ui/checkbox";
import { Switch } from "@/components/ui/switch";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Textarea } from "@/components/ui/textarea";
import { Progress } from "@/components/ui/progress";
import { Skeleton } from "@/components/ui/skeleton";
import { AlertCircle, Check, Info, Mail, Search } from "lucide-react";`}
              id="imports"
            />
          </CardContent>
        </Card>
      </div>
    </div>
  );
};

export default UIComponents;
