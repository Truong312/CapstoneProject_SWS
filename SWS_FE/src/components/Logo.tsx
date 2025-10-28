interface LogoProps {
  className?: string
  variant?: 'default' | 'white' | 'dark'
  size?: 'sm' | 'md' | 'lg'
}

export const Logo = ({ className = '', variant = 'default', size = 'md' }: LogoProps) => {
  const sizeClasses = {
    sm: 'w-10 h-10',
    md: 'w-12 h-12',
    lg: 'w-16 h-16'
  }

  const colors = {
    default: {
      primary: '#8B5CF6', // violet-500
      secondary: '#A78BFA', // violet-400
      accent: '#C4B5FD', // violet-300
      text: '#7C3AED', // violet-600
      glow: '#DDD6FE' // violet-200
    },
    white: {
      primary: '#ffffff',
      secondary: '#f8fafc',
      accent: '#f1f5f9',
      text: '#ffffff',
      glow: '#e2e8f0'
    },
    dark: {
      primary: '#1e293b',
      secondary: '#334155',
      accent: '#475569',
      text: '#1e293b',
      glow: '#64748b'
    }
  }

  const currentColors = colors[variant]

  return (
    <svg 
      className={`${sizeClasses[size]} ${className}`}
      viewBox="0 0 120 120" 
      fill="none" 
      xmlns="http://www.w3.org/2000/svg"
    >
      {/* Gradient Definitions */}
      <defs>
        <linearGradient id={`grad-main-${variant}`} x1="0%" y1="0%" x2="100%" y2="100%">
          <stop offset="0%" style={{ stopColor: currentColors.primary, stopOpacity: 1 }} />
          <stop offset="50%" style={{ stopColor: currentColors.secondary, stopOpacity: 1 }} />
          <stop offset="100%" style={{ stopColor: currentColors.accent, stopOpacity: 1 }} />
        </linearGradient>
        
        <linearGradient id={`grad-accent-${variant}`} x1="0%" y1="0%" x2="100%" y2="100%">
          <stop offset="0%" style={{ stopColor: currentColors.secondary, stopOpacity: 0.8 }} />
          <stop offset="100%" style={{ stopColor: currentColors.accent, stopOpacity: 0.6 }} />
        </linearGradient>

        <radialGradient id={`glow-${variant}`}>
          <stop offset="0%" style={{ stopColor: currentColors.glow, stopOpacity: 0.6 }} />
          <stop offset="100%" style={{ stopColor: currentColors.glow, stopOpacity: 0 }} />
        </radialGradient>
        
        <filter id="shadow-soft" x="-50%" y="-50%" width="200%" height="200%">
          <feGaussianBlur in="SourceAlpha" stdDeviation="3"/>
          <feOffset dx="0" dy="2" result="offsetblur"/>
          <feComponentTransfer>
            <feFuncA type="linear" slope="0.3"/>
          </feComponentTransfer>
          <feMerge>
            <feMergeNode/>
            <feMergeNode in="SourceGraphic"/>
          </feMerge>
        </filter>

        <filter id="inner-shadow">
          <feGaussianBlur in="SourceGraphic" stdDeviation="2" result="blur"/>
          <feOffset in="blur" dx="0" dy="1" result="offsetBlur"/>
          <feFlood floodColor="#000000" floodOpacity="0.2" result="offsetColor"/>
          <feComposite in="offsetColor" in2="offsetBlur" operator="in" result="offsetBlur"/>
          <feBlend in="SourceGraphic" in2="offsetBlur" mode="multiply"/>
        </filter>
      </defs>
      
      {/* Background Glow */}
      <circle cx="60" cy="60" r="50" fill={`url(#glow-${variant})`} opacity="0.4"/>
      
      {/* Main Shield Shape with Gradient */}
      <path 
        d="M60 15 L95 30 L95 65 Q95 85 60 100 Q25 85 25 65 L25 30 Z" 
        fill={`url(#grad-main-${variant})`}
        filter="url(#shadow-soft)"
        opacity="0.95"
      />
      
      {/* Inner Shield Border */}
      <path 
        d="M60 20 L90 33 L90 65 Q90 82 60 95 Q30 82 30 65 L30 33 Z" 
        fill="none"
        stroke={currentColors.accent}
        strokeWidth="1.5"
        opacity="0.4"
      />
      
      {/* 3D Cube/Box - representing warehouse */}
      <g filter="url(#inner-shadow)">
        {/* Front face */}
        <path 
          d="M45 50 L60 42 L75 50 L75 70 L60 78 L45 70 Z" 
          fill={variant === 'white' ? 'rgba(255,255,255,0.35)' : 'rgba(255,255,255,0.25)'}
          stroke={variant === 'white' ? 'white' : currentColors.accent}
          strokeWidth="1.5"
          strokeLinejoin="round"
        />
        
        {/* Top face */}
        <path 
          d="M45 50 L60 42 L75 50 L60 58 Z" 
          fill={variant === 'white' ? 'rgba(255,255,255,0.5)' : 'rgba(255,255,255,0.35)'}
          stroke={variant === 'white' ? 'white' : currentColors.accent}
          strokeWidth="1.5"
          strokeLinejoin="round"
        />
        
        {/* Right face */}
        <path 
          d="M60 58 L75 50 L75 70 L60 78 Z" 
          fill={variant === 'white' ? 'rgba(255,255,255,0.25)' : 'rgba(255,255,255,0.15)'}
          stroke={variant === 'white' ? 'white' : currentColors.accent}
          strokeWidth="1.5"
          strokeLinejoin="round"
        />
      </g>
      
      {/* Letter W - Modern Bold */}
      <text 
        x="60" 
        y="72" 
        fontFamily="system-ui, -apple-system, 'SF Pro Display', sans-serif" 
        fontSize="22" 
        fontWeight="900" 
        fill={currentColors.text}
        textAnchor="middle"
        style={{ 
          letterSpacing: '-1.5px',
          paintOrder: 'stroke fill'
        }}
        stroke={variant === 'white' ? 'rgba(255,255,255,0.3)' : 'rgba(0,0,0,0.1)'}
        strokeWidth="0.5"
      >
        W
      </text>
      
      {/* Decorative Elements */}
      <g opacity="0.6">
        {/* Corner accents */}
        <circle cx="35" cy="40" r="2" fill={currentColors.accent} />
        <circle cx="85" cy="40" r="2" fill={currentColors.accent} />
        
        {/* Bottom dots */}
        <circle cx="60" cy="88" r="2.5" fill={currentColors.accent} opacity="0.8"/>
        <circle cx="52" cy="88" r="1.5" fill={currentColors.accent} opacity="0.6"/>
        <circle cx="68" cy="88" r="1.5" fill={currentColors.accent} opacity="0.6"/>
      </g>
      
      {/* Shine effect */}
      <path 
        d="M50 25 Q55 35, 50 45" 
        stroke={variant === 'white' ? 'rgba(255,255,255,0.6)' : 'rgba(255,255,255,0.3)'}
        strokeWidth="2"
        strokeLinecap="round"
        fill="none"
        opacity="0.5"
      />
    </svg>
  )
}

export const LogoFull = ({ className = '' }: { className?: string }) => {
  return (
    <div className={`flex items-center gap-2 ${className}`}>
      <Logo size="md" variant="white" />
      <div className="flex flex-col">
        <span className="text-lg font-bold leading-tight">Warehouse</span>
        <span className="text-xs text-blue-200 leading-tight">Management System</span>
      </div>
    </div>
  )
}
