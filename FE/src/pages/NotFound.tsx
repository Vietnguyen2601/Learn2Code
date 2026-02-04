import { Container } from '@components'

export default function NotFound() {
  return (
    <Container className="py-12 text-center">
      <h1 className="text-4xl font-bold text-gray-900 mb-4">404</h1>
      <p className="text-lg text-gray-600">Page not found</p>
    </Container>
  )
}
