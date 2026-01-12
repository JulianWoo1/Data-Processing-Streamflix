import api from "./api";

export interface ViewingHistoryDto {
  viewingHistoryId: number;
  profileId: number;
  contentId: number;
  episodeId?: number;
  startTime: string;
  endTime?: string;
  lastPosition: number;
  isCompleted: boolean;
}

export interface UpdateViewingHistoryDto {
  lastPosition: number;
  isCompleted: boolean;
}

export interface StartViewingDto {
  profileId: number;
  contentId: number;
  episodeId?: number;
}

export interface ViewingHistoriesDto {
  viewingHistories: ViewingHistoryDto[];
}

const getAuthHeaders = (token: string) => ({
  headers: { Authorization: `Bearer ${token}` },
});

export const getHistory = async (profileId: number, token: string) => {
  const response = await api.get<ViewingHistoriesDto>(
    `/viewinghistory/${profileId}`,
    getAuthHeaders(token),
  );
  return response.data.viewingHistories;
};

export const startViewing = async (data: StartViewingDto, token: string) => {
  const response = await api.post<ViewingHistoryDto>(
    "/viewinghistory",
    data,
    getAuthHeaders(token),
  );
  return response.data;
};

export const updateProgress = async (
  viewingHistoryId: number,
  data: UpdateViewingHistoryDto,
  token: string,
) => {
  const response = await api.put(
    `/viewinghistory/${viewingHistoryId}`,
    data,
    getAuthHeaders(token),
  );
  return response.data;
};

export const markAsCompleted = async (
  viewingHistoryId: number,
  token: string,
) => {
  const response = await api.post(
    `/viewinghistory/${viewingHistoryId}/complete`,
    {},
    getAuthHeaders(token),
  );
  return response.data;
};

export const resumeContent = async (
  profileId: number,
  contentId: number,
  token: string,
) => {
  const response = await api.get<ViewingHistoryDto>(
    `/viewinghistory/resume/${profileId}/${contentId}`,
    getAuthHeaders(token),
  );
  return response.data;
};
