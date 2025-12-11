import { useState, useEffect } from "react";
import {
  getHistory,
  type ViewingHistoryDto,
} from "../services/viewingHistoryService";

interface ViewingHistoryProps {
  profileId: number;
  token: string;
}

const ViewingHistory = ({ profileId, token }: ViewingHistoryProps) => {
  const [history, setHistory] = useState<ViewingHistoryDto[]>([]);
  const [error, setError] = useState("");

  useEffect(() => {
    const fetchHistory = async () => {
      try {
        const fetchedHistory = await getHistory(profileId, token);
        setHistory(fetchedHistory);
      } catch (err) {
        setError("Failed to fetch viewing history.");
        console.error(err);
      }
    };

    if (profileId) {
      fetchHistory();
    }
  }, [profileId, token]);

  if (!profileId) {
    return null;
  }

  return (
    <div>
      <h3>Viewing History</h3>
      {error && <p style={{ color: "red" }}>{error}</p>}
      {history.length > 0 ? (
        <ul>
          {history.map((item) => (
            <li key={item.viewingHistoryId}>
              Content ID: {item.contentId} - Progress: {item.lastPosition}s{" "}
              {item.isCompleted ? "(Completed)" : ""}
            </li>
          ))}
        </ul>
      ) : (
        <p>No viewing history.</p>
      )}
    </div>
  );
};

export default ViewingHistory;
