import { useState } from "react";
import {
  createInvitation,
  acceptInvitation,
  getDiscount,
  type Discount,
  type CreateInvitationDto,
  type AcceptInvitationDto,
} from "../services/referralService";
import { jwtDecode, type JwtPayload } from "jwt-decode";

interface ReferralManagementProps {
  token: string;
}

interface DecodedToken extends JwtPayload {
  sub: string;
}

const ReferralManagement = ({ token }: ReferralManagementProps) => {
  const [invitationCode, setInvitationCode] = useState("");
  const [generatedCode, setGeneratedCode] = useState("");
  const [discount, setDiscount] = useState<Discount | null>(null);
  const [error, setError] = useState("");

  const getAccountIdFromToken = () => {
    try {
      const decoded = jwtDecode<DecodedToken>(token);
      return decoded.sub ? parseInt(decoded.sub, 10) : null;
    } catch (error) {
      console.error("Failed to decode token:", error);
      return null;
    }
  };

  const accountId = getAccountIdFromToken();

  const handleGenerateCode = async () => {
    if (!accountId) return;
    try {
      const dto: CreateInvitationDto = { referrerAccountId: accountId };
      const response = await createInvitation(dto, token);
      setGeneratedCode(response.invitationCode);
    } catch {
      setError("Failed to generate invitation code.");
    }
  };

  const handleAcceptCode = async () => {
    if (!accountId) return;
    try {
      const dto: AcceptInvitationDto = {
        invitationCode,
        referredAccountId: accountId,
      };
      await acceptInvitation(dto, token);
      // Maybe show a success message
    } catch {
      setError("Failed to accept invitation code.");
    }
  };

  const handleGetDiscount = async () => {
    if (!accountId) return;
    try {
      const response = await getDiscount(accountId, token);
      setDiscount(response);
    } catch {
      setError("Failed to get discount.");
    }
  };

  return (
    <div>
      <h2>Referral Management</h2>
      {error && <p style={{ color: "red" }}>{error}</p>}

      <div>
        <button onClick={handleGenerateCode}>Generate Invitation Code</button>
        {generatedCode && <p>Your code: {generatedCode}</p>}
      </div>

      <div>
        <input
          type="text"
          value={invitationCode}
          onChange={(e) => setInvitationCode(e.target.value)}
          placeholder="Enter invitation code"
        />
        <button onClick={handleAcceptCode}>Accept Code</button>
      </div>

      <div>
        <button onClick={handleGetDiscount}>Check Discount</button>
        {discount && (
          <p>
            Your discount: ${discount.discountAmount.toFixed(2)} until{" "}
            {new Date(discount.discountEndDate).toLocaleDateString()}
          </p>
        )}
      </div>
    </div>
  );
};

export default ReferralManagement;
