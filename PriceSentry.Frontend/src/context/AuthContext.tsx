import { createContext, useState, useContext } from 'react';
import type { ReactNode } from 'react';
import { authApi } from '../api';

interface AuthContextType {
  token: string | null;
  userId: string | null; // 👈 Добавляем ID пользователя
  step: 'login' | 'verify' | 'dashboard';
  login: (email: string) => Promise<void>;
  verify: (code: string) => Promise<void>;
  logout: () => void;
  bindTelegram: () => Promise<string | null>; // 👈 Метод для привязки
}

const AuthContext = createContext<AuthContextType | null>(null);

interface AuthProviderProps {
  children: ReactNode;
}

export function AuthProvider({ children }: AuthProviderProps) {
  const [token, setToken] = useState<string | null>(localStorage.getItem('auth_token'));
  const [userId, setUserId] = useState<string | null>(localStorage.getItem('user_id'));
  const [email, setEmail] = useState<string>('');
  const [step, setStep] = useState<'login' | 'verify' | 'dashboard'>(
    token ? 'dashboard' : 'login'
  );

  const login = async (userEmail: string) => {
    setEmail(userEmail);
    const res = await authApi.login(userEmail);

    setUserId(res.data);
    localStorage.setItem('user_id', res.data);
    setStep('verify');
  };

  const verify = async (code: string) => {
    const res = await authApi.verify(email, code);
    const newToken = res.data.token;
    if (newToken) {
      localStorage.setItem('auth_token', newToken);
      setToken(newToken);
      setStep('dashboard');
    }
  };

  const bindTelegram = async (): Promise<string | null> => {
    if (!userId) {
      alert('Пользователь не авторизован');
      return null;
    }
    try {
      const res = await authApi.getTelegramUrl();
      const telegramUrl = res.data;
      

      window.open(telegramUrl, '_blank');
      
      return telegramUrl;
    } catch (error) {
      console.error('Ошибка получения ссылки Telegram:', error);
      alert('Не удалось получить ссылку для привязки Telegram');
      return null;
    }
  };

  const logout = () => {
    localStorage.removeItem('auth_token');
    localStorage.removeItem('user_id');
    setToken(null);
    setUserId(null);
    setStep('login');
  };

  return (
    <AuthContext.Provider value={{ token, userId, step, login, verify, logout, bindTelegram }}>
      {children}
    </AuthContext.Provider>
  );
}

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within AuthProvider');
  }
  return context;
};