import axios from 'axios';
import type {
  LoginRequestDto,
  VerifyRequestDto,
  AuthResponse,
  ProductListVm,
  CreateProductDto,
  PriceListVm,
  ProductDitailsVm,
} from './types';

const API_URL = 'https://localhost:7004';

const api = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('auth_token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export const authApi = {
  login: (email: string) =>
    api.post<string>('/api/Auth/login', { email } as LoginRequestDto),
  
  verify: (email: string, code: string) =>
    api.post<AuthResponse>('/api/Auth/verify', { email, code } as VerifyRequestDto),

  getTelegramUrl: () =>
    api.get<string>(`/api/Auth/telegramUrl`),
};

export const productApi = {
  getAll: () => api.get<ProductListVm>('/api/PriceProduct'),
  getById: (id: string) =>
    api.get<ProductDitailsVm>(`/api/PriceProduct/product/${id}`),
  create: (data: CreateProductDto) =>
    api.post<string>('/api/PriceProduct', data),
  update: (data: { id: string; desiredPrice: number }) =>
    api.put<string>('/api/PriceProduct', data),
  delete: (id: string) =>
    api.delete(`/api/PriceProduct/${id}`),
  getPriceHistory: (id: string) =>
    api.get<PriceListVm>(`/api/PriceProduct/pricehistory/${id}`),
};