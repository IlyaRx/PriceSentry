import { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import { productApi } from '../api';
import type { ProductLookupVm } from '../types';

export default function Dashboard() {
  const { logout, bindTelegram } = useAuth();
  const [products, setProducts] = useState<ProductLookupVm[]>([]);
  const [newUrl, setNewUrl] = useState<string>('');
  const [newPrice, setNewPrice] = useState<string>('');

  const fetchProducts = async () => {
    try {
      const res = await productApi.getAll();
      setProducts(res.data.productList || []);
    } catch (e) {
      console.error('Ошибка загрузки товаров:', e);
    }
  };

  useEffect(() => {
    fetchProducts();
  }, []);

  const handleAdd = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    try {
      await productApi.create({ 
        productUrl: newUrl, 
        desiredPrice: parseFloat(newPrice) 
      });
      setNewUrl('');
      setNewPrice('');
      fetchProducts();
    } catch (err) {
      alert('Ошибка добавления товара');
    }
  };

  const handleDelete = async (id: string) => {
    if (confirm('Удалить этот товар?')) {
      await productApi.delete(id);
      fetchProducts();
    }
  };

  const handleBindTelegram = async () => {
    const url = await bindTelegram();
    if (url) {
      alert('Перейдите в Telegram для завершения привязки аккаунта');
    }
  };

  return (
    <div style={{ minHeight: '100vh', backgroundColor: '#f9fafb', padding: '32px' }}>
      <div style={{ maxWidth: '1200px', margin: '0 auto' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '32px' }}>
          <h1 style={{ fontSize: '32px', fontWeight: 'bold' }}>Мои товары</h1>
          <div style={{ display: 'flex', gap: '16px' }}>
            <button 
              onClick={handleBindTelegram}
              style={{ 
                padding: '10px 20px', 
                backgroundColor: '#229ED9', 
                color: 'white', 
                border: 'none',
                borderRadius: '6px',
                cursor: 'pointer',
                fontWeight: 'bold'
              }}
            >
              📱 Привязать Telegram
            </button>
            <button 
              onClick={logout}
              style={{ 
                padding: '10px 20px', 
                backgroundColor: '#dc2626', 
                color: 'white', 
                border: 'none',
                borderRadius: '6px',
                cursor: 'pointer'
              }}
            >
              Выйти
            </button>
          </div>
        </div>

        <form 
          onSubmit={handleAdd} 
          style={{ 
            backgroundColor: 'white', 
            padding: '16px', 
            borderRadius: '8px', 
            boxShadow: '0 1px 3px rgba(0,0,0,0.1)',
            marginBottom: '32px',
            display: 'flex',
            gap: '16px'
          }}
        >
          <input 
            placeholder="Ссылка на товар" 
            style={{ flex: 1, padding: '8px 12px', border: '1px solid #d1d5db', borderRadius: '6px' }}
            value={newUrl}
            onChange={(e) => setNewUrl(e.target.value)}
            required
          />
          <input 
            type="number" 
            placeholder="Желаемая цена" 
            style={{ width: '150px', padding: '8px 12px', border: '1px solid #d1d5db', borderRadius: '6px' }}
            value={newPrice}
            onChange={(e) => setNewPrice(e.target.value)}
            required
          />
          <button 
            type="submit"
            style={{ 
              padding: '8px 24px', 
              backgroundColor: '#2563eb', 
              color: 'white', 
              border: 'none',
              borderRadius: '6px',
              cursor: 'pointer'
            }}
          >
            Добавить
          </button>
        </form>

        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(350px, 1fr))', gap: '24px' }}>
          {products.map((p) => (
            <div 
              key={p.id} 
              style={{ 
                backgroundColor: 'white', 
                padding: '24px', 
                borderRadius: '8px', 
                boxShadow: '0 1px 3px rgba(0,0,0,0.1)'
              }}
            >
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start', marginBottom: '16px' }}>
                <h3 style={{ fontSize: '18px', fontWeight: '600', flex: 1 }}>
                  {p.title || 'Без названия'}
                </h3>
                <button 
                  onClick={() => handleDelete(p.id)}
                  style={{ color: '#9ca3af', background: 'none', border: 'none', cursor: 'pointer' }}
                >
                  🗑️
                </button>
              </div>
              
              <div style={{ marginBottom: '16px' }}>
                <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '8px' }}>
                  <span style={{ color: '#6b7280' }}>Текущая:</span>
                  <span style={{ fontWeight: 'bold' }}>{p.actualPrice} ₽</span>
                </div>
                <div style={{ display: 'flex', justifyContent: 'space-between' }}>
                  <span style={{ color: '#6b7280' }}>Целевая:</span>
                  <span style={{ color: p.actualPrice <= p.desiredPrice ? '#16a34a' : 'inherit', fontWeight: 'bold' }}>
                    {p.desiredPrice} ₽
                  </span>
                </div>
              </div>

              <a 
                href={p.productUrl || '#'} 
                target="_blank" 
                rel="noopener noreferrer"
                style={{ 
                  display: 'block', 
                  textAlign: 'center', 
                  padding: '8px', 
                  border: '1px solid #e5e7eb', 
                  borderRadius: '6px',
                  color: '#2563eb',
                  textDecoration: 'none'
                }}
              >
                Открыть товар →
              </a>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}