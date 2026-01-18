import api from "./api";

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

export interface MoviesDto {
  movies: MovieDto[];
}

export interface SeriesListDto {
  series: SeriesDto[];
}

export interface MovieRequestDto {
  title: string;
  description: string;
  ageRating: number;
  imageURL: string;
  duration: number;
  genre: string;
  contentWarnings: string[];
  availableQualities: string[];
}

export interface SeriesRequestDto {
  title: string;
  description: string;
  ageRating: number;
  imageURL: string;
  genre: string;
  contentWarnings: string[];
  availableQualities: string[];
}

export const getMovies = async () => {
  const response = await api.get<MoviesDto>("/content/movies");
  return response.data.movies;
};

export const createMovie = async (movie: MovieRequestDto) => {
  const response = await api.post<MovieDto>("/content/movies", movie);
  return response.data;
};

export const updateMovie = async (id: number, movie: MovieRequestDto) => {
  await api.put(`/content/movies/${id}`, movie);
};

export const deleteMovie = async (id: number) => {
  await api.delete(`/content/movies/${id}`);
};

export const getSeries = async () => {
  const response = await api.get<SeriesListDto>("/content/series");
  return response.data.series;
};

export const createSeries = async (series: SeriesRequestDto) => {
  const response = await api.post<SeriesDto>("/content/series", series);
  return response.data;
};

export const updateSeries = async (id: number, series: SeriesRequestDto) => {
  await api.put(`/content/series/${id}`, series);
};

export const deleteSeries = async (id: number) => {
  await api.delete(`/content/series/${id}`);
};

export const getMoviesByGenre = async (genre: string) => {
  const response = await api.get<MoviesDto>(`/content/movies?genre=${genre}`);
  return response.data.movies;
};

export const getSeriesByGenre = async (genre: string) => {
  const response = await api.get<SeriesListDto>(
    `/content/series?genre=${genre}`,
  );
  return response.data.series;
};

export const searchMoviesByTitle = async (title: string) => {
  const response = await api.get<MoviesDto>(`/content/movies?title=${title}`);
  return response.data.movies;
};

export const searchSeriesByTitle = async (title: string) => {
  const response = await api.get<SeriesListDto>(
    `/content/series?title=${title}`,
  );
  return response.data.series;
};
