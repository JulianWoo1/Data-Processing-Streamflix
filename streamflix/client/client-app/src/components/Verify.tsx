import { useState } from "react";
import { verify } from "../services/accountService";

const Verify = () => {
  const [email, setEmail] = useState("");
  const [verificationToken, setVerificationToken] = useState("");
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    setSuccess("");
    try {
      await verify({ email, verificationToken });
      setSuccess("Account verified successfully! You can now log in.");
    } catch (err) {
      setError("Failed to verify account.");
      console.error(err);
    }
  };

  return (
    <div className="section">
      <h2>Verify Account</h2>
      <form onSubmit={handleSubmit}>
        <div>
          <label>Email:</label>
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
          />
        </div>
        <div>
          <label>Verification Token:</label>
          <input
            type="text"
            value={verificationToken}
            onChange={(e) => setVerificationToken(e.target.value)}
            required
          />
        </div>
        <button type="submit">Verify</button>
        {error && <p style={{ color: "red" }}>{error}</p>}
        {success && <p style={{ color: "green" }}>{success}</p>}
      </form>
    </div>
  );
};

export default Verify;
