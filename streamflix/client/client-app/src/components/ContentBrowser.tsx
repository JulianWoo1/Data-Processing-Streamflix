import { useState, useEffect } from "react";
import {
  getMovies,
  getSeries,
  type MovieDto,
  type SeriesDto,
  getMoviesByGenre,
  getSeriesByGenre,
  getMovieByTitle,
  getSeriesByTitle,
} from "../services/contentService";
import { addContentToWatchlist } from "../services/watchlistService";
import { startViewing } from "../services/viewingHistoryService";

interface ContentBrowserProps {
  profileId: number;
  token: string;
  fetchWatchlist: () => void;
}

const ContentBrowser = ({
  profileId,
  token,
  fetchWatchlist,
}: ContentBrowserProps) => {
  const [movies, setMovies] = useState<MovieDto[]>([]);
  const [series, setSeries] = useState<SeriesDto[]>([]);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [contentType, setContentType] = useState<"movies" | "series">("movies");
  const [genre, setGenre] = useState("");
  const [title, setTitle] = useState("");

  useEffect(() => {
    const fetchData = async () => {
      try {
        setError("");
        if (contentType === "movies") {
          const fetchedMovies = await getMovies();
          setMovies(fetchedMovies);
        } else {
          const fetchedSeries = await getSeries();
          setSeries(fetchedSeries);
        }
      } catch (err) {
        setError("Failed to fetch content.");
        console.error(err);
      }
    };

    fetchData();
  }, [contentType]);

  const handleGenreSearch = async () => {
    try {
      setError("");
      if (contentType === "movies") {
        const fetchedMovies = await getMoviesByGenre(genre);
        setMovies(fetchedMovies);
      } else {
        const fetchedSeries = await getSeriesByGenre(genre);
        setSeries(fetchedSeries);
      }
    } catch (err) {
      setError("Failed to fetch content by genre.");
      console.error(err);
    }
  };

  const handleTitleSearch = async () => {
    try {
      setError("");
      if (contentType === "movies") {
        const fetchedMovie = await getMovieByTitle(title);
        setMovies(fetchedMovie ? [fetchedMovie] : []);
      } else {
        const fetchedSeries = await getSeriesByTitle(title);
        setSeries(fetchedSeries ? [fetchedSeries] : []);
      }
    } catch (err) {
      setError("Failed to fetch content by title.");
      console.error(err);
    }
  };

  const handleAddToWatchlist = async (contentId: number) => {
    try {
      setSuccess("");
      setError("");
      await addContentToWatchlist(profileId, contentId, token);
      setSuccess("Added to watchlist!");
      fetchWatchlist();
    } catch (err) {
      setError("Failed to add to watchlist.");
      console.error(err);
    }
  };

  const handleStartViewing = async (contentId: number) => {
    try {
      setSuccess("");
      setError("");
      await startViewing({ profileId, contentId }, token);
      setSuccess("Started viewing!");
    } catch (err) {
      setError("Failed to start viewing.");
      console.error(err);
    }
  };

  return (
    <div>
      <h2>Content Browser</h2>
      {error && <p style={{ color: "red" }}>{error}</p>}
      {success && <p style={{ color: "green" }}>{success}</p>}
      <div>
        <button onClick={() => setContentType("movies")}>Movies</button>
        <button onClick={() => setContentType("series")}>Series</button>
      </div>
      <div>
        <input
          type="text"
          placeholder="Genre"
          value={genre}
          onChange={(e) => setGenre(e.target.value)}
        />
        <button onClick={handleGenreSearch}>Search by Genre</button>
      </div>
      <div>
        <input
          type="text"
          placeholder="Title"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
        />
        <button onClick={handleTitleSearch}>Search by Title</button>
      </div>

      {contentType === "movies" ? (
        <ul>
          {movies.map((movie) => (
            <li key={movie.id}>
              {movie.title}
              <button onClick={() => handleAddToWatchlist(movie.id)}>
                Add to Watchlist
              </button>
              <button onClick={() => handleStartViewing(movie.id)}>
                Start Viewing
              </button>
            </li>
          ))}
        </ul>
      ) : (
        <ul>
          {series.map((s) => (
            <li key={s.id}>
              {s.title}
              <button onClick={() => handleAddToWatchlist(s.id)}>
                Add to Watchlist
              </button>
              <button onClick={() => handleStartViewing(s.id)}>
                Start Viewing
              </button>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
};

export default ContentBrowser;
