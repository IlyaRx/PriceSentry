// Типы для авторизации
export interface LoginRequestDto {
  email: string;
}

export interface VerifyRequestDto {
  email: string;
  code: string;
}

export interface AuthResponse {
  token: string | null;
  expiresAt: string;
  message: string | null;
}

// Типы для товаров
export interface ProductLookupVm {
  id: string;
  title: string | null;
  productUrl: string | null;
  actualPrice: number;
  desiredPrice: number;
}

export interface ProductListVm {
  productList: ProductLookupVm[] | null;
}

export interface CreateProductDto {
  productUrl: string | null;
  desiredPrice?: number;
}

export interface PriceLookupDTO {
  price: number;
  addDate: string;
}

export interface PriceListVm {
  prices: PriceLookupDTO[] | null;
}

export interface ProductDitailsVm {
  id: string;
  title: string | null;
  productUrl: string | null;
  actualPrice: number;
  desiredPrice: number;
  lastTracking: string | null;
}