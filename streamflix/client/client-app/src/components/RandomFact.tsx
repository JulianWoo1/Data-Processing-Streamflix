import React, { useState } from "react";
import api from "../services/api";

const RandomFact: React.FC = () => {
  const [fact, setFact] = useState<string>("");
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  const fetchFact = async () => {
    setLoading(true);
    setError(null);
    try {
      const response = await api.get("/RandomFact");
      if (response.status !== 200) {
        throw new Error("Failed to fetch fact");
      }
      setFact(response.data.text);
    } catch (err) {
      setError((err as Error).message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="random-fact">
      <button onClick={fetchFact} disabled={loading}>
        {loading ? "Loading..." : "Get a random fact"}
      </button>
      {error && <p style={{ color: "red" }}>{error}</p>}
      {fact && <p>{fact}</p>}
    </div>
  );
};

export default RandomFact;
