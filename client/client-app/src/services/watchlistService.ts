import api from './api';

export interface WatchlistContentDto {
  watchlistContentId: number;
  contentId: number;
  title: string;
  description: string;
  ageRating: number;
  imageURL: string;
  genre: string;
  contentWarnings: string[];
  availableQualities: string[];
  dateAdded: string;
}

export interface WatchlistDto {
  watchlistId: number;
  profileId: number;
  createdAt: string;
  items: WatchlistContentDto[];
}

const getAuthHeaders = (token: string) => ({
  headers: { Authorization: `Bearer ${token}` },
});

export const getWatchlistByProfileId = async (profileId: number, token: string) => {
  const response = await api.get<WatchlistDto>(`/watchlist/profile/${profileId}`, getAuthHeaders(token));
  return response.data;
};

export const addContentToWatchlist = async (profileId: number, contentId: number, token: string) => {
  const response = await api.post<WatchlistDto>(`/watchlist/profile/${profileId}/add/${contentId}`, {}, getAuthHeaders(token));
  return response.data;
};

export const removeContentFromWatchlist = async (profileId: number, contentId: number, token: string) => {
  const response = await api.delete(`/watchlist/profile/${profileId}/remove/${contentId}`, getAuthHeaders(token));
  return response.data;
};
