import React, { useState } from "react";

const RandomFact: React.FC = () => {
  const [fact, setFact] = useState<string>("");
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  const fetchFact = async () => {
    setLoading(true);
    setError(null);
    try {
      const response = await fetch(
        "https://uselessfacts.jsph.pl/api/v2/facts/random",
      );
      if (!response.ok) {
        throw new Error("Failed to fetch fact");
      }
      const data = await response.json();
      setFact(data.text);
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
