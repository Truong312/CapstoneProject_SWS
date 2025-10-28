import { Link } from "react-router-dom";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { 
  Code, 
  Palette, 
  Rocket, 
  Zap, 
  Copy,
  Layers,
  Package,
  CheckCircle2,
  ArrowRight
} from "lucide-react";

const UIResourcesLanding = () => {
  const features = [
    {
      icon: Palette,
      title: "UI Showcase",
      description: "Xem preview trực tiếp các components với UI đẹp mắt",
      path: "/ui-showcase",
      color: "from-blue-500 to-cyan-500"
    },
    {
      icon: Code,
      title: "UI Components",
      description: "Copy code examples để sử dụng ngay trong dự án",
      path: "/ui-components",
      color: "from-purple-500 to-pink-500"
    }
  ];

  const stats = [
    { label: "Components", value: "10+", icon: Package },
    { label: "Examples", value: "50+", icon: Copy },
    { label: "Variants", value: "30+", icon: Layers },
  ];

  const componentsList = [
    "Button", "Card", "Table", "Form", "Badge", "Alert",
    "Avatar", "Progress", "Tabs", "Skeleton", "Input", "Checkbox"
  ];

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-50 via-blue-50 to-purple-50">
      {/* Hero Section */}
      <div className="relative overflow-hidden">
        <div className="absolute inset-0 bg-grid-slate-100 [mask-image:linear-gradient(0deg,white,rgba(255,255,255,0.6))] -z-10" />
        
        <div className="max-w-7xl mx-auto px-8 py-24">
          <div className="text-center space-y-8">
            <div className="inline-flex items-center gap-2 px-4 py-2 bg-white rounded-full shadow-sm">
              <Zap className="h-4 w-4 text-yellow-500" />
              <span className="text-sm font-medium">UI Components Library</span>
            </div>
            
            <h1 className="text-6xl font-bold tracking-tight">
              <span className="bg-gradient-to-r from-blue-600 via-purple-600 to-pink-600 bg-clip-text text-transparent">
                Beautiful UI Components
              </span>
              <br />
              <span className="text-slate-900">Ready to Use</span>
            </h1>
            
            <p className="text-xl text-slate-600 max-w-2xl mx-auto">
              Bộ sưu tập components UI đẹp, dễ sử dụng và có thể tùy chỉnh.
              Copy code và bắt đầu build ngay!
            </p>

            <div className="flex items-center justify-center gap-4">
              <Link to="/ui-showcase">
                <Button size="lg" className="gap-2">
                  <Palette className="h-5 w-5" />
                  View Showcase
                  <ArrowRight className="h-4 w-4" />
                </Button>
              </Link>
              <Link to="/ui-components">
                <Button size="lg" variant="outline" className="gap-2">
                  <Code className="h-5 w-5" />
                  Get Code
                </Button>
              </Link>
            </div>
          </div>

          {/* Stats */}
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mt-16">
            {stats.map((stat, index) => (
              <Card key={index} className="text-center border-2 hover:shadow-lg transition-shadow">
                <CardContent className="pt-6">
                  <stat.icon className="h-8 w-8 mx-auto mb-3 text-blue-600" />
                  <div className="text-3xl font-bold mb-1">{stat.value}</div>
                  <div className="text-sm text-muted-foreground">{stat.label}</div>
                </CardContent>
              </Card>
            ))}
          </div>
        </div>
      </div>

      {/* Features Section */}
      <div className="max-w-7xl mx-auto px-8 py-16">
        <div className="text-center mb-12">
          <h2 className="text-3xl font-bold mb-4">Choose Your Path</h2>
          <p className="text-lg text-muted-foreground">
            Chọn cách bạn muốn khám phá components
          </p>
        </div>

        <div className="grid md:grid-cols-2 gap-8">
          {features.map((feature, index) => (
            <Link key={index} to={feature.path} className="group">
              <Card className="h-full border-2 transition-all hover:shadow-2xl hover:scale-105">
                <CardHeader>
                  <div className={`w-16 h-16 rounded-2xl bg-gradient-to-r ${feature.color} p-4 mb-4 group-hover:scale-110 transition-transform`}>
                    <feature.icon className="h-full w-full text-white" />
                  </div>
                  <CardTitle className="text-2xl">{feature.title}</CardTitle>
                  <CardDescription className="text-base">
                    {feature.description}
                  </CardDescription>
                </CardHeader>
                <CardContent>
                  <Button variant="ghost" className="w-full justify-between group-hover:bg-accent">
                    Explore
                    <ArrowRight className="h-4 w-4 group-hover:translate-x-1 transition-transform" />
                  </Button>
                </CardContent>
              </Card>
            </Link>
          ))}
        </div>
      </div>

      {/* Components Grid */}
      <div className="max-w-7xl mx-auto px-8 py-16">
        <Card className="bg-white/80 backdrop-blur-sm">
          <CardHeader>
            <div className="flex items-center justify-between">
              <div>
                <CardTitle className="text-2xl mb-2">Available Components</CardTitle>
                <CardDescription>
                  Tất cả components đều được xây dựng với Radix UI và Tailwind CSS
                </CardDescription>
              </div>
              <Badge variant="info" className="text-lg px-4 py-2">
                {componentsList.length} Components
              </Badge>
            </div>
          </CardHeader>
          <CardContent>
            <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-6 gap-3">
              {componentsList.map((component, index) => (
                <div
                  key={index}
                  className="flex items-center gap-2 p-3 rounded-lg bg-gradient-to-r from-slate-50 to-slate-100 border hover:border-blue-300 hover:shadow-md transition-all"
                >
                  <CheckCircle2 className="h-4 w-4 text-green-500 flex-shrink-0" />
                  <span className="text-sm font-medium">{component}</span>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Features List */}
      <div className="max-w-7xl mx-auto px-8 py-16">
        <div className="grid md:grid-cols-3 gap-8">
          <Card>
            <CardHeader>
              <Rocket className="h-10 w-10 mb-3 text-blue-600" />
              <CardTitle>Fast Development</CardTitle>
              <CardDescription>
                Copy và paste code để bắt đầu ngay. Không cần setup phức tạp.
              </CardDescription>
            </CardHeader>
          </Card>

          <Card>
            <CardHeader>
              <Zap className="h-10 w-10 mb-3 text-yellow-600" />
              <CardTitle>Highly Customizable</CardTitle>
              <CardDescription>
                Dễ dàng tùy chỉnh với Tailwind CSS. Thay đổi colors, spacing, và nhiều hơn nữa.
              </CardDescription>
            </CardHeader>
          </Card>

          <Card>
            <CardHeader>
              <Package className="h-10 w-10 mb-3 text-purple-600" />
              <CardTitle>Production Ready</CardTitle>
              <CardDescription>
                Components được test kỹ và sẵn sàng sử dụng trong production.
              </CardDescription>
            </CardHeader>
          </Card>
        </div>
      </div>

      {/* CTA Section */}
      <div className="max-w-4xl mx-auto px-8 py-16">
        <Card className="bg-gradient-to-r from-blue-600 to-purple-600 text-white border-0">
          <CardContent className="p-12 text-center">
            <h2 className="text-3xl font-bold mb-4">
              Sẵn sàng bắt đầu?
            </h2>
            <p className="text-lg text-blue-100 mb-8">
              Khám phá bộ sưu tập components và tăng tốc độ phát triển dự án của bạn
            </p>
            <div className="flex items-center justify-center gap-4">
              <Link to="/ui-showcase">
                <Button size="lg" variant="secondary" className="gap-2">
                  <Palette className="h-5 w-5" />
                  View Showcase
                </Button>
              </Link>
              <Link to="/ui-components">
                <Button 
                  size="lg" 
                  variant="outline" 
                  className="gap-2 bg-white/10 border-white/20 text-white hover:bg-white/20"
                >
                  <Code className="h-5 w-5" />
                  Browse Components
                </Button>
              </Link>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Footer */}
      <div className="text-center py-12 text-muted-foreground">
        <p className="text-sm">
          Built with ❤️ using React, Tailwind CSS, and Radix UI
        </p>
      </div>
    </div>
  );
};

export default UIResourcesLanding;
