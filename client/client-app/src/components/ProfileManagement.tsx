import { useState, useEffect } from "react";
import {
  getProfiles,
  createProfile,
  type ProfileDto,
  type CreateProfileDto,
} from "../services/profileService";
import { jwtDecode, type JwtPayload } from "jwt-decode";

interface ProfileManagementProps {
  token: string;
  setSelectedProfileId: (id: number) => void;
}

interface DecodedToken extends JwtPayload {
  sub: string;
}

const ProfileManagement = ({
  token,
  setSelectedProfileId,
}: ProfileManagementProps) => {
  const [profiles, setProfiles] = useState<ProfileDto[]>([]);
  const [error, setError] = useState("");
  const [showCreateForm, setShowCreateForm] = useState(false);
  const [newProfileName, setNewProfileName] = useState("");

  const getAccountIdFromToken = () => {
    try {
      const decoded = jwtDecode<DecodedToken>(token);
      return decoded.sub ? parseInt(decoded.sub, 10) : null;
    } catch (error) {
      console.error("Failed to decode token:", error);
      return null;
    }
  };

  useEffect(() => {
    const fetchProfiles = async () => {
      try {
        const fetchedProfiles = await getProfiles(token);
        setProfiles(fetchedProfiles);
      } catch (err) {
        setError("Failed to fetch profiles.");
        console.error(err);
      }
    };

    fetchProfiles();
  }, [token]);

  const handleCreateProfile = async (e: React.FormEvent) => {
    e.preventDefault();
    const accountId = getAccountIdFromToken();
    if (!accountId) {
      setError("Could not identify user account.");
      return;
    }

    const newProfile: CreateProfileDto = {
      accountId,
      name: newProfileName,
      ageCategory: "Adult", // Defaulting for simplicity
      imageUrl: "http://example.com/image.png", // Defaulting for simplicity
    };

    try {
      const createdProfile = await createProfile(newProfile, token);
      setProfiles([...profiles, createdProfile]);
      setNewProfileName("");
      setShowCreateForm(false);
    } catch (err) {
      setError("Failed to create profile.");
      console.error(err);
    }
  };

  return (
    <div>
      <h2>Profile Management</h2>
      {error && <p style={{ color: "red" }}>{error}</p>}
      <ul>
        {profiles.map((profile) => (
          <li key={profile.profileId}>
            {profile.name}
            <button onClick={() => setSelectedProfileId(profile.profileId)}>
              Select
            </button>
          </li>
        ))}
      </ul>
      <button onClick={() => setShowCreateForm(!showCreateForm)}>
        {showCreateForm ? "Cancel" : "Create New Profile"}
      </button>
      {showCreateForm && (
        <form onSubmit={handleCreateProfile}>
          <div>
            <label>Name:</label>
            <input
              type="text"
              value={newProfileName}
              onChange={(e) => setNewProfileName(e.target.value)}
              required
            />
          </div>
          <button type="submit">Create</button>
        </form>
      )}
    </div>
  );
};

export default ProfileManagement;
