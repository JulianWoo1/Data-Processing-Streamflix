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
import {
  startViewing,
  getHistory,
  updateProgress,
  markAsCompleted,
  type ViewingHistoryDto,
} from "../services/viewingHistoryService";

interface ContentBrowserProps {
  profileId: number;
  token: string;
  fetchWatchlist: () => void;
}

const modalOverlayStyle: React.CSSProperties = {
  position: "fixed",
  top: 0,
  left: 0,
  right: 0,
  bottom: 0,
  backgroundColor: "rgba(0, 0, 0, 0.7)",
  display: "flex",
  justifyContent: "center",
  alignItems: "center",
  zIndex: 1000,
};

const modalContentStyle: React.CSSProperties = {
  backgroundColor: "#fff",
  padding: "20px",
  borderRadius: "5px",
  maxWidth: "500px",
  width: "100%",
  color: "#000",
  maxHeight: "80vh",
  overflowY: "auto",
};

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
  const [viewingHistory, setViewingHistory] = useState<ViewingHistoryDto[]>([]);
  const [nowWatchingInfo, setNowWatchingInfo] = useState<{
    historyId: number;
    contentId: number;
    startTime: number;
  } | null>(null);
  const [selectedContentInfo, setSelectedContentInfo] = useState<
    MovieDto | SeriesDto | null
  >(null);

  useEffect(() => {
    const fetchViewingHistory = async () => {
      try {
        if (profileId && token) {
          const history = await getHistory(profileId, token);
          setViewingHistory(history);
        }
      } catch (err) {
        setError("Failed to fetch viewing history.");
      }
    };
    fetchViewingHistory();
  }, [profileId, token]);

  useEffect(() => {
    let progressInterval: NodeJS.Timeout | null = null;

    if (nowWatchingInfo) {
      progressInterval = setInterval(async () => {
        const lastPosition = Math.floor(
          (new Date().getTime() - nowWatchingInfo.startTime) / 1000,
        );
        try {
          await updateProgress(
            nowWatchingInfo.historyId,
            { lastPosition, isCompleted: false },
            token,
          );
        } catch (err) {
          console.error("Failed to update progress", err);
        }
      }, 5000); // Update every 5 seconds
    }

    return () => {
      if (progressInterval) {
        clearInterval(progressInterval);
      }
    };
  }, [nowWatchingInfo, token]);

  const handleStartViewing = async (contentId: number) => {
    try {
      setSuccess("");
      setError("");
      const newHistory = await startViewing({ profileId, contentId }, token);
      setSuccess("Started viewing!");
      setViewingHistory((prev) => [...prev, newHistory]);
      setNowWatchingInfo({
        historyId: newHistory.viewingHistoryId,
        contentId: newHistory.contentId,
        startTime: new Date().getTime(),
      });
    } catch (err) {
      setError("Failed to start viewing.");
      console.error(err);
    }
  };

  const handleStopViewing = async () => {
    if (!nowWatchingInfo) return;

    try {
      setSuccess("");
      setError("");
      await markAsCompleted(nowWatchingInfo.historyId, token);
      setSuccess("Stopped viewing.");
      setNowWatchingInfo(null);
    } catch (err) {
      setError("Failed to stop viewing.");
      console.error(err);
    }
  };

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

  const renderContent = (content: (MovieDto | SeriesDto)[]) => (
    <ul>
      {content.map((item) => (
        <li key={item.id}>
          {item.title}
          <button onClick={() => setSelectedContentInfo(item)}>Info</button>
          <button onClick={() => handleAddToWatchlist(item.id)}>
            Add to Watchlist
          </button>
          {nowWatchingInfo?.contentId === item.id ? (
            <button onClick={handleStopViewing}>Stop Viewing</button>
          ) : (
            <button onClick={() => handleStartViewing(item.id)}>
              Start Viewing
            </button>
          )}
        </li>
      ))}
    </ul>
  );

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

      {contentType === "movies" ? renderContent(movies) : renderContent(series)}

      {selectedContentInfo && (
        <div style={modalOverlayStyle}>
          <div style={modalContentStyle}>
            <h3>{selectedContentInfo.title}</h3>
            <img
              src={selectedContentInfo.imageURL}
              alt={selectedContentInfo.title}
              style={{ maxWidth: "150px", float: "left", marginRight: "20px" }}
            />
            <p>
              <strong>Description:</strong> {selectedContentInfo.description}
            </p>
            <p>
              <strong>Genre:</strong> {selectedContentInfo.genre}
            </p>
            <p>
              <strong>Age Rating:</strong> {selectedContentInfo.ageRating}+
            </p>
            <p>
              <strong>Available Qualities:</strong>{" "}
              {selectedContentInfo.availableQualities.join(", ")}
            </p>
            <p>
              <strong>Content Warnings:</strong>{" "}
              {selectedContentInfo.contentWarnings.join(", ")}
            </p>

            {"duration" in selectedContentInfo && (
              <p>
                <strong>Duration:</strong> {selectedContentInfo.duration}{" "}
                minutes
              </p>
            )}

            {"seasons" in selectedContentInfo && (
              <div>
                <h4>Seasons ({selectedContentInfo.totalSeasons}):</h4>
                {selectedContentInfo.seasons.map((season) => (
                  <div key={season.seasonId}>
                    <h5>
                      Season {season.seasonNumber} ({season.totalEpisodes}{" "}
                      episodes)
                    </h5>
                    <ul>
                      {season.episodes.map((episode) => (
                        <li key={episode.episodeId}>
                          Ep {episode.episodeNumber}: {episode.title} (
                          {episode.duration} mins)
                        </li>
                      ))}
                    </ul>
                  </div>
                ))}
              </div>
            )}
            <div style={{ clear: "both" }}></div>
            <button
              onClick={() => setSelectedContentInfo(null)}
              style={{ marginTop: "20px" }}
            >
              Close
            </button>
          </div>
        </div>
      )}
    </div>
  );
};

export default ContentBrowser;
