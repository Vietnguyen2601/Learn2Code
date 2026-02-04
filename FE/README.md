# React + Tailwind Frontend

A modern React frontend application with TypeScript, Tailwind CSS, and API integration using Axios.

## Features

- ⚛️ **React 18** - Latest React with hooks support
- 📘 **TypeScript** - Type-safe development
- 🎨 **Tailwind CSS** - Utility-first CSS framework
- 🔗 **Axios** - HTTP client for API calls
- 📦 **Vite** - Lightning-fast build tool
- 🧭 **React Router** - Client-side routing
- 🎯 **ESLint** - Code quality and consistency

## Project Structure

```
src/
├── components/      # Reusable UI components
├── pages/           # Page components
├── api/             # API client and services
├── hooks/           # Custom React hooks
├── types/           # TypeScript type definitions
├── utils/           # Utility functions
├── styles/          # CSS and Tailwind styles
├── context/         # React context providers
├── App.tsx          # Main app component
└── main.tsx         # Entry point
```

## Installation

1. Clone the repository
```bash
git clone <your-repo-url>
cd FE
```

2. Install dependencies
```bash
npm install
```

3. Create `.env` file
```bash
cp .env.example .env
```

4. Update environment variables in `.env`
```
VITE_API_BASE_URL=http://localhost:3000/api
```

## Development

Start the development server:
```bash
npm run dev
```

The app will open at `http://localhost:5173`

## Build

Build for production:
```bash
npm run build
```

Preview production build:
```bash
npm run preview
```

## Linting

Check code quality:
```bash
npm run lint
```

## API Integration

The project includes a pre-configured Axios client with interceptors for:
- Automatic token injection in Authorization headers
- Handling 401 responses (redirect to login)
- Base URL configuration via environment variables

### Usage Example

```typescript
import { userService, postService } from '@api/services'

// Get user profile
const profile = await userService.getProfile()

// Get all posts
const posts = await postService.getPosts({ page: 1 })

// Create a new post
const newPost = await postService.createPost({
  title: 'My Post',
  content: 'Post content...'
})
```

## Environment Variables

Create a `.env` file based on `.env.example`:

```
VITE_API_BASE_URL=http://localhost:3000/api
```

## Available Scripts

- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run preview` - Preview production build
- `npm run lint` - Run ESLint

## Technologies Used

- **React** - UI library
- **TypeScript** - Type safety
- **Tailwind CSS** - Styling
- **Vite** - Build tool
- **Axios** - HTTP client
- **React Router** - Routing
- **ESLint** - Linting

## License

MIT
