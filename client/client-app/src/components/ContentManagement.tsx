import React, { useState, useEffect } from "react";
import {
  getMovies,
  getSeries,
  createMovie,
  updateMovie,
  deleteMovie,
  createSeries,
  updateSeries,
  deleteSeries,
  type MovieDto,
  type SeriesDto,
  type MovieRequestDto,
  type SeriesRequestDto,
} from "../services/contentService";

const ContentManagement: React.FC = () => {
  const [movies, setMovies] = useState<MovieDto[]>([]);
  const [series, setSeries] = useState<SeriesDto[]>([]);
  const [isManaging, setIsManaging] = useState(false);
  const [editingMovie, setEditingMovie] = useState<MovieDto | null>(null);
  const [editingSeries, setEditingSeries] = useState<SeriesDto | null>(null);
  const [isAddingMovie, setIsAddingMovie] = useState(false);
  const [isAddingSeries, setIsAddingSeries] = useState(false);

  useEffect(() => {
    if (isManaging) {
      loadContent();
    }
  }, [isManaging]);

  const loadContent = async () => {
    const moviesData = await getMovies();
    const seriesData = await getSeries();
    setMovies(moviesData);
    setSeries(seriesData);
  };

  const handleToggleManage = () => {
    setIsManaging(!isManaging);
  };

  const handleDeleteMovie = async (id: number) => {
    await deleteMovie(id);
    loadContent();
  };

  const handleDeleteSeries = async (id: number) => {
    await deleteSeries(id);
    loadContent();
  };

  const handleSaveMovie = async (movieData: MovieRequestDto) => {
    if (editingMovie) {
      await updateMovie(editingMovie.id, movieData);
    } else {
      await createMovie(movieData);
    }
    setEditingMovie(null);
    setIsAddingMovie(false);
    loadContent();
  };

  const handleSaveSeries = async (seriesData: SeriesRequestDto) => {
    if (editingSeries) {
      await updateSeries(editingSeries.id, seriesData);
    } else {
      await createSeries(seriesData);
    }
    setEditingSeries(null);
    setIsAddingSeries(false);
    loadContent();
  };

  return (
    <div>
      <button onClick={handleToggleManage}>
        {isManaging ? "Hide Content Management" : "Manage Content"}
      </button>

      {isManaging && (
        <div>
          <h2>Movies</h2>
          <button
            onClick={() => {
              setEditingMovie(null);
              setIsAddingMovie(true);
            }}
          >
            Add Movie
          </button>
          {movies.map((movie) => (
            <div key={movie.id}>
              <span>{movie.title}</span>
              <button
                onClick={() => {
                  setEditingMovie(movie);
                  setIsAddingMovie(true);
                }}
              >
                Edit
              </button>
              <button onClick={() => handleDeleteMovie(movie.id)}>
                Delete
              </button>
            </div>
          ))}

          <h2>Series</h2>
          <button
            onClick={() => {
              setEditingSeries(null);
              setIsAddingSeries(true);
            }}
          >
            Add Series
          </button>
          {series.map((s) => (
            <div key={s.id}>
              <span>{s.title}</span>
              <button
                onClick={() => {
                  setEditingSeries(s);
                  setIsAddingSeries(true);
                }}
              >
                Edit
              </button>
              <button onClick={() => handleDeleteSeries(s.id)}>Delete</button>
            </div>
          ))}

          {(isAddingMovie || editingMovie) && (
            <MovieForm
              movie={editingMovie}
              onSave={handleSaveMovie}
              onCancel={() => {
                setEditingMovie(null);
                setIsAddingMovie(false);
              }}
            />
          )}

          {(isAddingSeries || editingSeries) && (
            <SeriesForm
              series={editingSeries}
              onSave={handleSaveSeries}
              onCancel={() => {
                setEditingSeries(null);
                setIsAddingSeries(false);
              }}
            />
          )}
        </div>
      )}
    </div>
  );
};

const MovieForm: React.FC<{
  movie: MovieDto | null;
  onSave: (data: MovieRequestDto) => void;
  onCancel: () => void;
}> = ({ movie, onSave, onCancel }) => {
  const [formData, setFormData] = useState<MovieRequestDto>({
    title: movie?.title || "",
    description: movie?.description || "",
    ageRating: movie?.ageRating || 0,
    imageURL: movie?.imageURL || "",
    duration: movie?.duration || 0,
    genre: movie?.genre || "",
    contentWarnings: movie?.contentWarnings || [],
    availableQualities: movie?.availableQualities || [],
  });

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>,
  ) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleListChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value.split(",").map((item) => item.trim()),
    }));
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSave(formData);
  };

  return (
    <form onSubmit={handleSubmit}>
      <h3>{movie ? "Edit Movie" : "Add Movie"}</h3>
      <input
        name="title"
        value={formData.title}
        onChange={handleChange}
        placeholder="Title"
        required
      />
      <textarea
        name="description"
        value={formData.description}
        onChange={handleChange}
        placeholder="Description"
        required
      />
      <input
        name="ageRating"
        type="number"
        value={formData.ageRating}
        onChange={handleChange}
        placeholder="Age Rating"
        required
      />
      <input
        name="imageURL"
        value={formData.imageURL}
        onChange={handleChange}
        placeholder="Image URL"
        required
      />
      <input
        name="duration"
        type="number"
        value={formData.duration}
        onChange={handleChange}
        placeholder="Duration"
        required
      />
      <input
        name="genre"
        value={formData.genre}
        onChange={handleChange}
        placeholder="Genre"
        required
      />
      <input
        name="contentWarnings"
        value={formData.contentWarnings.join(", ")}
        onChange={handleListChange}
        placeholder="Content Warnings (comma-separated)"
      />
      <input
        name="availableQualities"
        value={formData.availableQualities.join(", ")}
        onChange={handleListChange}
        placeholder="Available Qualities (comma-separated)"
      />
      <button type="submit">Save</button>
      <button type="button" onClick={onCancel}>
        Cancel
      </button>
    </form>
  );
};

const SeriesForm: React.FC<{
  series: SeriesDto | null;
  onSave: (data: SeriesRequestDto) => void;
  onCancel: () => void;
}> = ({ series, onSave, onCancel }) => {
  const [formData, setFormData] = useState<SeriesRequestDto>({
    title: series?.title || "",
    description: series?.description || "",
    ageRating: series?.ageRating || 0,
    imageURL: series?.imageURL || "",
    genre: series?.genre || "",
    contentWarnings: series?.contentWarnings || [],
    availableQualities: series?.availableQualities || [],
  });

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>,
  ) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleListChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value.split(",").map((item) => item.trim()),
    }));
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSave(formData);
  };

  return (
    <form onSubmit={handleSubmit}>
      <h3>{series ? "Edit Series" : "Add Series"}</h3>
      <input
        name="title"
        value={formData.title}
        onChange={handleChange}
        placeholder="Title"
        required
      />
      <textarea
        name="description"
        value={formData.description}
        onChange={handleChange}
        placeholder="Description"
        required
      />
      <input
        name="ageRating"
        type="number"
        value={formData.ageRating}
        onChange={handleChange}
        placeholder="Age Rating"
        required
      />
      <input
        name="imageURL"
        value={formData.imageURL}
        onChange={handleChange}
        placeholder="Image URL"
        required
      />
      <input
        name="genre"
        value={formData.genre}
        onChange={handleChange}
        placeholder="Genre"
        required
      />
      <input
        name="contentWarnings"
        value={formData.contentWarnings.join(", ")}
        onChange={handleListChange}
        placeholder="Content Warnings (comma-separated)"
      />
      <input
        name="availableQualities"
        value={formData.availableQualities.join(", ")}
        onChange={handleListChange}
        placeholder="Available Qualities (comma-separated)"
      />
      <button type="submit">Save</button>
      <button type="button" onClick={onCancel}>
        Cancel
      </button>
    </form>
  );
};

export default ContentManagement;
