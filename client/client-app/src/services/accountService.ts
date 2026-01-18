import api from './api';

export interface CreateAccountDto {
  email: string;
  password: string;
}

export interface VerifyAccountDto {
  email: string;
  verificationToken: string;
}

export interface LoginDto {
  email: string;
  password: string;
}

export interface RequestPasswordResetDto {
  email: string;
}

export interface ResetPasswordDto {
  email: string;
  passwordResetToken: string;
  newPassword: string;
}

export interface LoginResult {
  token: string;
}

export const register = async (data: CreateAccountDto) => {
  const response = await api.post('/account/register', data);
  return response.data;
};

export const verify = async (data: VerifyAccountDto) => {
  const response = await api.post('/account/verify', data);
  return response.data;
};

export const login = async (data: LoginDto) => {
  const response = await api.post<LoginResult>('/account/login', data);
  return response.data;
};

export const requestPasswordReset = async (data: RequestPasswordResetDto) => {
  const response = await api.post('/account/requestPasswordReset', data);
  return response.data;
};

export const resetPassword = async (data: ResetPasswordDto) => {
  const response = await api.post('/account/resetPassword', data);
  return response.data;
};

export const getAccountInfo = async (email: string, token: string) => {
  const response = await api.get('/account/getAccountInfo', {
    params: { email },
    headers: { Authorization: `Bearer ${token}` },
  });
  return response.data;
};
