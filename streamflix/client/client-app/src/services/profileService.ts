import api from './api';

export interface ProfileDto {
  profileId: number;
  accountId: number;
  name: string;
  ageCategory: string;
  imageUrl: string;
  // Add other fields from ProfileDto if needed
}

export interface CreateProfileDto {
  accountId: number; // This might need to be handled carefully
  name: string;
  ageCategory: string;
  imageUrl: string;
}

export interface UpdateProfileDto {
  name: string;
  ageCategory: string;
  imageUrl: string;
}

const getAuthHeaders = (token: string) => ({
  headers: { Authorization: `Bearer ${token}` },
});

export const getProfiles = async (token: string) => {
  const response = await api.get<ProfileDto[]>('/profile', getAuthHeaders(token));
  return response.data;
};

export const createProfile = async (profile: CreateProfileDto, token: string) => {
  const response = await api.post<ProfileDto>('/profile', profile, getAuthHeaders(token));
  return response.data;
};

export const updateProfile = async (id: number, profile: UpdateProfileDto, token: string) => {
  const response = await api.put(`/profile/${id}`, profile, getAuthHeaders(token));
  return response.data;
};

export const deleteProfile = async (id: number, token: string) => {
  const response = await api.delete(`/profile/${id}`, getAuthHeaders(token));
  return response.data;
};
