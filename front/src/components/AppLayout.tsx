import type { ReactNode } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Logo } from './common/Logo';
import { Button } from './common/Button';
import { useAuth } from '../context/AuthContext';
import './AppLayout.css';

interface AppLayoutProps {
  children: ReactNode;
}

/**
 * Общий каркас для авторизованных страниц приложения:
 * шапка с логотипом, навигацией и кнопкой выхода, а также контентная область.
 */
export function AppLayout({ children }: AppLayoutProps) {
  const { logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <div className="app">
      <header className="app__header">
        <div className="app__header-inner">
          <Link to="/quizzes" className="app__logo-link" aria-label="Quizer — на главную">
            <Logo />
          </Link>
          <nav className="app__nav">
            <Link to="/quizzes" className="app__nav-link">
              Квизы
            </Link>
          </nav>
          <Button variant="secondary" onClick={handleLogout}>
            Выйти
          </Button>
        </div>
      </header>

      <main className="app__main">{children}</main>
    </div>
  );
}
