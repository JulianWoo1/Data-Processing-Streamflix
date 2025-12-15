import { useState } from "react";
import { Link } from "react-router-dom";
import { register, type CreateAccountDto } from "../services/accountService";
import { AxiosError } from "axios";

const Register = () => {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [success, setSuccess] = useState<string | React.ReactNode>("");

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    setSuccess("");
    try {
      const data: CreateAccountDto = { email, password };
      const response = await register(data);
      setSuccess(
        <div>
          <p>
            Registration successful! Your verification token is:{" "}
            {response.verificationToken}.
          </p>
          <p>
            Please go to the <Link to="/verify">verification page</Link> to
            activate your account.
          </p>
        </div>,
      );
    } catch (err) {
      if (err instanceof AxiosError && err.response?.data) {
        setError(err.response.data as string);
      } else {
        setError("Registration failed. Please try again.");
      }
      console.error(err);
    }
  };

  return (
    <div>
      <h2>Register</h2>
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
          <label>Password:</label>
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
        </div>
        <button type="submit">Register</button>
        {error && <p style={{ color: "red" }}>{error}</p>}
        {success && <div style={{ color: "green" }}>{success}</div>}
      </form>
    </div>
  );
};

export default Register;
