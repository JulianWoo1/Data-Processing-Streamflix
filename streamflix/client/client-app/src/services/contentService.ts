import api from './api';

export interface MovieDto {
  id: number;
  title: string;
  description: string;
  ageRating: number;
  imageURL: string;
  duration: number;
  genre: string;
  contentWarnings: string[];
  availableQualities: string[];
}

export interface EpisodeDto {
  episodeId: number;
  episodeNumber: number;
  title: string;
  duration: number;
}

export interface SeasonDto {
  seasonId: number;
  seasonNumber: number;
  totalEpisodes: number;
  episodes: EpisodeDto[];
}

export interface SeriesDto {
  id: number;
  title: string;
  description: string;
  ageRating: number;
  imageURL: string;
  totalSeasons: number;
  genre: string;
  contentWarnings: string[];
  availableQualities: string[];
  seasons: SeasonDto[];
}

export const getMovies = async () => {
  const response = await api.get<MovieDto[]>('/content/movies');
  return response.data;
};

export const getSeries = async () => {
  const response = await api.get<SeriesDto[]>('/content/series');
  return response.data;
};

export const getMoviesByGenre = async (genre: string) => {
    const response = await api.get<MovieDto[]>(`/content/movies/genre/${genre}`);
    return response.data;
}

export const getSeriesByGenre = async (genre: string) => {
    const response = await api.get<SeriesDto[]>(`/content/series/genre/${genre}`);
    return response.data;
}

export const getMovieByTitle = async (title: string) => {
    const response = await api.get<MovieDto>(`/content/movies/title/${title}`);
    return response.data;
}

export const getSeriesByTitle = async (title: string) => {
    const response = await api.get<SeriesDto>(`/content/series/title/${title}`);
    return response.data;
}
