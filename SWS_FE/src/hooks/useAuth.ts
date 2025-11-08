import { useEffect, useState } from "react"
import { useNavigate } from "react-router-dom"

export default function useAuth(redirectToLogin: boolean = true) {
  const [isAuthenticated, setIsAuthenticated] = useState(false)
  const navigate = useNavigate()

  useEffect(() => {
    const token = localStorage.getItem("token")

    if (!token) {
      setIsAuthenticated(false)
      if (redirectToLogin) navigate("/login")
    } else {
      setIsAuthenticated(true)
    }
  }, [redirectToLogin, navigate])

  return { isAuthenticated }
}
