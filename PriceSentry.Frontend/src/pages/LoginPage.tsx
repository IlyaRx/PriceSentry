import { useState } from 'react';
import type { SyntheticEvent } from 'react';
import { useAuth } from '../context/AuthContext';

export default function LoginPage() {
  const { step, login, verify } = useAuth();
  const [email, setEmail] = useState<string>('');
  const [code, setCode] = useState<string>('');

  const handleLoginSubmit = (e: SyntheticEvent) => {
    e.preventDefault();
    login(email);
  };

  const handleVerifySubmit = (e: SyntheticEvent) => {
    e.preventDefault();
    verify(code);
  };

  return (
    <div style={{ 
      minHeight: '100vh', 
      display: 'flex', 
      alignItems: 'center', 
      justifyContent: 'center',
      backgroundColor: '#f3f4f6'
    }}>
      <div style={{ 
        backgroundColor: 'white', 
        padding: '32px', 
        borderRadius: '8px', 
        boxShadow: '0 1px 3px rgba(0,0,0,0.1)',
        width: '100%',
        maxWidth: '400px'
      }}>
        <h2 style={{ fontSize: '24px', fontWeight: 'bold', marginBottom: '24px', textAlign: 'center' }}>
          PriceSentry
        </h2>
        
        {step === 'login' ? (
          <form onSubmit={handleLoginSubmit}>
            <label style={{ display: 'block', marginBottom: '8px', fontSize: '14px', fontWeight: '500' }}>
              Email
            </label>
            <input
              type="email"
              style={{ 
                width: '100%', 
                padding: '8px 12px', 
                border: '1px solid #d1d5db', 
                borderRadius: '6px',
                marginBottom: '16px',
                boxSizing: 'border-box'
              }}
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
            />
            <button 
              type="submit"
              style={{ 
                width: '100%', 
                padding: '10px', 
                backgroundColor: '#2563eb', 
                color: 'white', 
                border: 'none',
                borderRadius: '6px',
                cursor: 'pointer'
              }}
            >
              Получить код
            </button>
          </form>
        ) : (
          <form onSubmit={handleVerifySubmit}>
            <p style={{ marginBottom: '16px', fontSize: '14px', color: '#6b7280' }}>
              Код отправлен на {email}
            </p>
            <label style={{ display: 'block', marginBottom: '8px', fontSize: '14px', fontWeight: '500' }}>
              Код подтверждения
            </label>
            <input
              type="text"
              style={{ 
                width: '100%', 
                padding: '8px 12px', 
                border: '1px solid #d1d5db', 
                borderRadius: '6px',
                marginBottom: '16px',
                boxSizing: 'border-box'
              }}
              value={code}
              onChange={(e) => setCode(e.target.value)}
              required
            />
            <button 
              type="submit"
              style={{ 
                width: '100%', 
                padding: '10px', 
                backgroundColor: '#16a34a', 
                color: 'white', 
                border: 'none',
                borderRadius: '6px',
                cursor: 'pointer'
              }}
            >
              Войти
            </button>
          </form>
        )}
      </div>
    </div>
  );
}