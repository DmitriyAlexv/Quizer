import { useState, type FormEvent } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { AuthLayout } from '../components/AuthLayout';
import { Alert } from '../components/common/Alert';
import { Button } from '../components/common/Button';
import { Input } from '../components/common/Input';
import { useAuth } from '../context/AuthContext';
import { ApiError } from '../api/client';
import './auth.css';

interface FormErrors {
  name?: string;
  email?: string;
  password?: string;
  confirmPassword?: string;
}

export function RegisterPage() {
  const { register } = useAuth();
  const navigate = useNavigate();

  const [name, setName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [errors, setErrors] = useState<FormErrors>({});
  const [serverError, setServerError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const validate = (): boolean => {
    const next: FormErrors = {};

    if (!name.trim()) {
      next.name = 'Введите имя';
    } else if (name.trim().length < 2) {
      next.name = 'Имя должно содержать минимум 2 символа';
    }

    if (!email.trim()) {
      next.email = 'Введите email';
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.trim())) {
      next.email = 'Некорректный email';
    }

    if (!password) {
      next.password = 'Введите пароль';
    } else if (password.length < 8) {
      next.password = 'Пароль должен содержать минимум 8 символов';
    } else if (!/[0-9]/.test(password)) {
      next.password = 'Пароль должен содержать минимум 1 цифру';
    } else if (!/[A-Z]/.test(password)) {
      next.password = 'Пароль должен содержать минимум 1 заглавную букву';
    } else if (!/[a-z]/.test(password)) {
      next.password = 'Пароль должен содержать минимум 1 строчную букву';
    } else if (!/[^\p{L}\p{N}\s]/u.test(password)) {
      next.password = 'Пароль должен содержать минимум 1 специальный символ';
    }

    if (!confirmPassword) {
      next.confirmPassword = 'Повторите пароль';
    } else if (confirmPassword !== password) {
      next.confirmPassword = 'Пароли не совпадают';
    }

    setErrors(next);
    return Object.keys(next).length === 0;
  };

  const handleSubmit = async (event: FormEvent) => {
    event.preventDefault();
    setServerError(null);

    if (!validate()) {
      return;
    }

    setLoading(true);
    try {
      await register(email.trim(), password, name.trim());
      navigate('/');
    } catch (error) {
      if (error instanceof ApiError) {
        setServerError(error.message);
      } else {
        setServerError('Произошла непредвиденная ошибка. Попробуйте ещё раз.');
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <AuthLayout title="Создать аккаунт" subtitle="Зарегистрируйтесь, чтобы начать создавать квизы">
      <form className="auth-form" onSubmit={handleSubmit} noValidate>
        {serverError && <Alert variant="error">{serverError}</Alert>}

        <Input
          label="Имя"
          placeholder="Как вас зовут?"
          autoComplete="name"
          value={name}
          onChange={(e) => setName(e.target.value)}
          error={errors.name}
          icon={<UserIcon />}
        />

        <Input
          label="Email"
          type="email"
          placeholder="you@example.com"
          autoComplete="email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          error={errors.email}
          icon={<MailIcon />}
        />

        <Input
          label="Пароль"
          password
          placeholder="Минимум 8 символов"
          autoComplete="new-password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          error={errors.password}
          icon={<LockIcon />}
        />

        <Input
          label="Повторите пароль"
          password
          placeholder="Ещё раз"
          autoComplete="new-password"
          value={confirmPassword}
          onChange={(e) => setConfirmPassword(e.target.value)}
          error={errors.confirmPassword}
          icon={<LockIcon />}
        />

        <Button type="submit" fullWidth loading={loading}>
          Зарегистрироваться
        </Button>

        <p className="auth-form__switch">
          Уже есть аккаунт? <Link to="/login">Войти</Link>
        </p>
      </form>
    </AuthLayout>
  );
}

function UserIcon() {
  return (
    <svg width="18" height="18" viewBox="0 0 24 24" fill="none" aria-hidden="true">
      <circle cx="12" cy="8" r="4" stroke="currentColor" strokeWidth="1.6" />
      <path d="M4 20c0-3.3 3.6-6 8-6s8 2.7 8 6" stroke="currentColor" strokeWidth="1.6" strokeLinecap="round" />
    </svg>
  );
}

function MailIcon() {
  return (
    <svg width="18" height="18" viewBox="0 0 24 24" fill="none" aria-hidden="true">
      <rect x="3" y="5" width="18" height="14" rx="2" stroke="currentColor" strokeWidth="1.6" />
      <path d="m3 7 9 6 9-6" stroke="currentColor" strokeWidth="1.6" strokeLinecap="round" strokeLinejoin="round" />
    </svg>
  );
}

function LockIcon() {
  return (
    <svg width="18" height="18" viewBox="0 0 24 24" fill="none" aria-hidden="true">
      <rect x="4" y="10" width="16" height="10" rx="2" stroke="currentColor" strokeWidth="1.6" />
      <path d="M8 10V7a4 4 0 0 1 8 0v3" stroke="currentColor" strokeWidth="1.6" strokeLinecap="round" />
    </svg>
  );
}
