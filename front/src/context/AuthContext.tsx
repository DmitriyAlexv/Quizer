import {
  createContext,
  useCallback,
  useContext,
  useMemo,
  useState,
  type ReactNode,
} from 'react';
import { authorizeUser, registerUser } from '../api/auth';

const TOKEN_STORAGE_KEY = 'quizer.token';

interface AuthContextValue {
  /** JWT-токен текущего пользователя (null, если не авторизован). */
  token: string | null;
  /** Признак того, что пользователь авторизован. */
  isAuthenticated: boolean;
  /** Выполняет вход по email и паролю. */
  login: (email: string, password: string) => Promise<void>;
  /** Регистрирует нового пользователя и выполняет вход. */
  register: (email: string, password: string, name: string) => Promise<void>;
  /** Выходит из системы. */
  logout: () => void;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

function readStoredToken(): string | null {
  try {
    return localStorage.getItem(TOKEN_STORAGE_KEY);
  } catch {
    return null;
  }
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState<string | null>(readStoredToken);

  const persistToken = useCallback((value: string | null) => {
    setToken(value);
    try {
      if (value) {
        localStorage.setItem(TOKEN_STORAGE_KEY, value);
      } else {
        localStorage.removeItem(TOKEN_STORAGE_KEY);
      }
    } catch {
      // localStorage может быть недоступен (например, в приватном режиме)
    }
  }, []);

  const login = useCallback(
    async (email: string, password: string) => {
      const response = await authorizeUser({ email, password });
      persistToken(response.token);
    },
    [persistToken],
  );

  const register = useCallback(
    async (email: string, password: string, name: string) => {
      await registerUser({ email, password, name });
      const response = await authorizeUser({ email, password });
      persistToken(response.token);
    },
    [persistToken],
  );

  const logout = useCallback(() => {
    persistToken(null);
  }, [persistToken]);

  const value = useMemo<AuthContextValue>(
    () => ({
      token,
      isAuthenticated: Boolean(token),
      login,
      register,
      logout,
    }),
    [token, login, register, logout],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

// eslint-disable-next-line react-refresh/only-export-components
export function useAuth(): AuthContextValue {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth должен использоваться внутри <AuthProvider>.');
  }
  return context;
}
