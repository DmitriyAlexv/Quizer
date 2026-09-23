import { useNavigate } from 'react-router-dom';
import { Button } from '../components/common/Button';
import { Logo } from '../components/common/Logo';
import { useAuth } from '../context/AuthContext';
import './home.css';

export function HomePage() {
  const { logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <div className="home">
      <header className="home__header">
        <Logo />
        <Button variant="secondary" onClick={handleLogout}>
          Выйти
        </Button>
      </header>

      <main className="home__main">
        <h1 className="home__title">Добро пожаловать в Quizer!</h1>
        <p className="home__text">
          Вы успешно авторизовались. Здесь будет располагаться список квизов
          и функциональность сервиса.
        </p>
      </main>
    </div>
  );
}
