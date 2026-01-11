import { useState, useEffect } from "react";
import { Routes, Route, Link, Navigate } from "react-router-dom";
import Login from "./components/Login";
import Register from "./components/Register";
import Verify from "./components/Verify";
import ProfileManagement from "./components/ProfileManagement";
import ContentBrowser from "./components/ContentBrowser";
import Watchlist from "./components/Watchlist";
import ViewingHistory from "./components/ViewingHistory";
import SubscriptionManagement from "./components/SubscriptionManagement";
import ReferralManagement from "./components/ReferralManagement";
import "./App.css";
import {
  getWatchlistByProfileId,
  type WatchlistDto,
} from "./services/watchlistService";
import { AxiosError } from "axios";

function App() {
  const [token, setToken] = useState<string | null>(
    localStorage.getItem("token"),
  );

  const setAndStoreToken = (token: string | null) => {
    setToken(token);
    if (token) {
      localStorage.setItem("token", token);
    } else {
      localStorage.removeItem("token");
    }
  };

  return (
    <div className="container">
      <h1>Streamflix Client</h1>
      <nav>
        {!token && (
          <>
            <Link to="/login">Login</Link> |{" "}
            <Link to="/register">Register</Link> |{" "}
            <Link to="/verify">Verify</Link>
          </>
        )}
      </nav>
      <Routes>
        <Route
          path="/login"
          element={
            !token ? <Login setToken={setAndStoreToken} /> : <Navigate to="/" />
          }
        />
        <Route
          path="/register"
          element={!token ? <Register /> : <Navigate to="/" />}
        />
        <Route
          path="/verify"
          element={!token ? <Verify /> : <Navigate to="/" />}
        />
        <Route
          path="/"
          element={
            token ? (
              <Home token={token} setToken={setAndStoreToken} />
            ) : (
              <Navigate to="/login" />
            )
          }
        />
      </Routes>
    </div>
  );
}

import RandomFact from "./components/RandomFact";

const Home = ({
  token,
  setToken,
}: {
  token: string;
  setToken: (token: string | null) => void;
}) => {
  const [selectedProfileId, setSelectedProfileId] = useState<number | null>(
    null,
  );
  const [watchlist, setWatchlist] = useState<WatchlistDto | null>(null);
  const [watchlistError, setWatchlistError] = useState("");

  // Memoize fetchWatchlist to avoid missing dependency warning
  const fetchWatchlist = async (profileId: number) => {
    try {
      const fetchedWatchlist = await getWatchlistByProfileId(profileId, token);
      setWatchlist(fetchedWatchlist);
    } catch (err) {
      if (err instanceof AxiosError && err.response?.status === 404) {
        setWatchlist({
          watchlistId: 0,
          profileId: profileId,
          items: [],
          createdAt: new Date().toISOString(),
        });
      } else {
        setWatchlistError("Failed to fetch watchlist.");
        console.error(err);
      }
    }
  };

  useEffect(() => {
    if (selectedProfileId) {
      Promise.resolve().then(() => fetchWatchlist(selectedProfileId));
    }
  }, [selectedProfileId, token]);

  return (
    <div>
      <RandomFact />
      <h2>Welcome!</h2>
      <button
        onClick={() => {
          setToken(null);
          setSelectedProfileId(null);
        }}
      >
        Logout
      </button>
      <div className="section">
        <SubscriptionManagement token={token} />
      </div>
      <div className="section">
        <ReferralManagement token={token} />
      </div>
      <div className="section">
        <ProfileManagement
          token={token}
          setSelectedProfileId={setSelectedProfileId}
        />
      </div>
      {selectedProfileId && (
        <div className="section">
          <Watchlist
            profileId={selectedProfileId}
            token={token}
            watchlist={watchlist}
            error={watchlistError}
            fetchWatchlist={() => fetchWatchlist(selectedProfileId)}
          />
          <ViewingHistory profileId={selectedProfileId} token={token} />
          <ContentBrowser
            profileId={selectedProfileId}
            token={token}
            fetchWatchlist={() => fetchWatchlist(selectedProfileId)}
          />
        </div>
      )}
    </div>
  );
};

export default App;
