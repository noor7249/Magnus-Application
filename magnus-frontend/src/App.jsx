import { BrowserRouter } from 'react-router-dom'
import { GlobalError } from './components/GlobalError'
import { AppRoutes } from './routes/AppRoutes'
import './App.css'

function App() {
  return (
    <BrowserRouter>
      <GlobalError />
      <AppRoutes />
    </BrowserRouter>
  )
}

export default App
