import type { ReactNode } from 'react';
import { Logo } from './common/Logo';
import './AuthLayout.css';

interface AuthLayoutProps {
  children: ReactNode;
  /** Заголовок формы (например, «Вход»). */
  title: string;
  /** Подзаголовок формы. */
  subtitle?: string;
}

/**
 * Общий каркас для страниц авторизации и регистрации:
 * слева — брендинговая панель, справа — форма.
 */
export function AuthLayout({ children, title, subtitle }: AuthLayoutProps) {
  return (
    <div className="auth">
      <aside className="auth__brand">
        <div className="auth__brand-inner">
          <Logo size="lg" />
          <div className="auth__brand-copy">
            <h2 className="auth__brand-title">Создавайте и проходите квизы</h2>
            <p className="auth__brand-text">
              Удобный сервис для создания небольших квизов, прохождения
              и соревнования с друзьями в таблице лидеров.
            </p>
          </div>
          <ul className="auth__features">
            <li>
              <span className="auth__feature-dot" />
              Быстрое создание квизов
            </li>
            <li>
              <span className="auth__feature-dot" />
              Мгновенная проверка ответов
            </li>
            <li>
              <span className="auth__feature-dot" />
              Рейтинг и лидерборд
            </li>
          </ul>
        </div>
      </aside>

      <main className="auth__main">
        <div className="auth__card">
          <div className="auth__mobile-logo">
            <Logo />
          </div>
          <header className="auth__header">
            <h1 className="auth__title">{title}</h1>
            {subtitle && <p className="auth__subtitle">{subtitle}</p>}
          </header>
          {children}
        </div>
      </main>
    </div>
  );
}
