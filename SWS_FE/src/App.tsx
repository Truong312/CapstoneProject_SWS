import { BrowserRouter as Router, Routes, Route } from 'react-router-dom'
import { Toaster } from '@/components/ui/toaster'
import Layout from '@/components/layout/Layout'
import Dashboard from '@/pages/Dashboard'
import Products from '@/pages/Products'
import Inventory from '@/pages/Inventory'
import Orders from '@/pages/Orders'
import Settings from '@/pages/Settings'
import Login from '@/pages/Login'
import UIShowcase from '@/pages/UIShowcase'
import UIComponents from '@/pages/UIComponents'
import UIResourcesLanding from '@/pages/UIResourcesLanding'

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/auth/callback" element={<Login />} />
        <Route path="/" element={<Layout />}>
          <Route index element={<Dashboard />} />
          <Route path="products" element={<Products />} />
          <Route path="inventory" element={<Inventory />} />
          <Route path="orders" element={<Orders />} />
          <Route path="settings" element={<Settings />} />
          <Route path="ui-resources" element={<UIResourcesLanding />} />
          <Route path="ui-showcase" element={<UIShowcase />} />
          <Route path="ui-components" element={<UIComponents />} />
        </Route>
      </Routes>
      <Toaster />
    </Router>
  )
}

export default App
