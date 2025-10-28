# Warehouse Management System - Frontend

## Tech Stack

- **React 18** - UI library
- **TypeScript** - Type safety
- **Vite** - Build tool
- **React Router** - Routing
- **Zustand** - State management
- **shadcn/ui** - UI components (built on Radix UI)
- **Tailwind CSS** - Styling
- **Axios** - HTTP client

## Getting Started

### Prerequisites

- Node.js 18+ and npm

### Installation

1. Install dependencies:
```bash
npm install
```

2. Create environment file:
```bash
cp .env.example .env
```

3. Update the `.env` file with your API URL:
```
VITE_API_URL=http://localhost:4000/api
```

### Development

Start the development server:
```bash
npm run dev
```

The app will be available at `http://localhost:3000`

### Build

Build for production:
```bash
npm run build
```

Preview production build:
```bash
npm run preview
```

## Project Structure

```
src/
├── components/          # Reusable components
│   ├── layout/         # Layout components
│   └── ui/             # shadcn/ui components
├── hooks/              # Custom React hooks
├── lib/                # Utility functions and configs
├── pages/              # Page components
├── store/              # Zustand stores
├── App.tsx             # Main app component
├── main.tsx            # Entry point
└── index.css           # Global styles
```

## Features

- ✅ Modern React with TypeScript
- ✅ State management with Zustand
- ✅ Beautiful UI with shadcn/ui components
- ✅ Responsive design
- ✅ Authentication flow
- ✅ API integration ready
- ✅ Toast notifications
- ✅ Protected routes

## Available Scripts

- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run preview` - Preview production build
- `npm run lint` - Run ESLint

## Customization

### Adding shadcn/ui Components

This project uses shadcn/ui. To add more components, you can manually copy them from [shadcn/ui](https://ui.shadcn.com/) or use their CLI (after installing it separately).

### Styling

The project uses Tailwind CSS. You can customize the theme in `tailwind.config.js` and global styles in `src/index.css`.

## License

MIT
