import {
  removeContentFromWatchlist,
  type WatchlistDto,
} from "../services/watchlistService";
import { useState } from "react";

interface WatchlistProps {
  profileId: number;
  token: string;
  watchlist: WatchlistDto | null;
  error: string;
  fetchWatchlist: () => void;
}

const Watchlist = ({
  profileId,
  token,
  watchlist,
  error,
  fetchWatchlist,
}: WatchlistProps) => {
  const [removeError, setRemoveError] = useState("");
  const handleRemove = async (contentId: number) => {
    try {
      await removeContentFromWatchlist(profileId, contentId, token);
      fetchWatchlist();
    } catch (err) {
      setRemoveError("Failed to remove from watchlist.");
      console.error(err);
    }
  };

  if (!profileId) {
    return <p>Select a profile to see the watchlist.</p>;
  }

  return (
    <div>
      <h3>Watchlist</h3>
      {error && <p style={{ color: "red" }}>{error}</p>}
      {removeError && <p style={{ color: "red" }}>{removeError}</p>}
      {watchlist && watchlist.items.length > 0 ? (
        <ul>
          {watchlist.items.map((item) => (
            <li key={item.contentId}>
              {item.title}{" "}
              <button onClick={() => handleRemove(item.contentId)}>
                Remove
              </button>
            </li>
          ))}
        </ul>
      ) : (
        <p>Watchlist is empty.</p>
      )}
    </div>
  );
};

export default Watchlist;
