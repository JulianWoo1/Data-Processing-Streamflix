import { useState, useEffect } from "react";
import {
  getSubscription,
  createSubscription,
  cancelSubscription,
  renewSubscription,
  getPlans,
  type Subscription,
  type SubscriptionPlan,
  type CreateSubscriptionDto,
} from "../services/subscriptionService";
import { jwtDecode, type JwtPayload } from "jwt-decode";

interface SubscriptionManagementProps {
  token: string;
}

interface DecodedToken extends JwtPayload {
  sub: string;
}

const SubscriptionManagement = ({ token }: SubscriptionManagementProps) => {
  const [subscription, setSubscription] = useState<Subscription | null>(null);
  const [plans, setPlans] = useState<SubscriptionPlan[]>([]);
  const [error, setError] = useState("");
  const [selectedPlan, setSelectedPlan] = useState<SubscriptionPlan | null>(
    null,
  );

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

  useEffect(() => {
    const fetchSubAndPlans = async () => {
      if (!accountId) return;
      try {
        const [sub, pls] = await Promise.all([
          getSubscription(accountId, token).catch(() => null),
          getPlans(),
        ]);
        setSubscription(sub);
        setPlans(pls);
      } catch (err) {
        setError("Failed to fetch subscription details.");
        console.error(err);
      }
    };
    fetchSubAndPlans();
  }, [accountId, token]);

  const handleCreate = async () => {
    if (!accountId || !selectedPlan) return;
    const dto: CreateSubscriptionDto = {
      accountId,
      subscriptionType: selectedPlan.type,
      subscriptionDescription: `A ${selectedPlan.type} subscription.`,
      basePrice: selectedPlan.price,
    };
    try {
      const newSub = await createSubscription(dto, token);
      setSubscription(newSub);
    } catch {
      setError("Failed to create subscription.");
    }
  };

  const handleCancel = async () => {
    if (!subscription) return;
    try {
      await cancelSubscription(subscription.subscriptionId, token);
      setSubscription(null);
    } catch {
      setError("Failed to cancel subscription.");
    }
  };

  const handleRenew = async () => {
    if (!subscription) return;
    try {
      const renewedSub = await renewSubscription(
        subscription.subscriptionId,
        token,
      );
      setSubscription(renewedSub);
    } catch {
      setError("Failed to renew subscription.");
    }
  };

  return (
    <div>
      <h2>Subscription Management</h2>
      {error && <p style={{ color: "red" }}>{error}</p>}
      {subscription ? (
        <div>
          <p>Type: {subscription.subscriptionType}</p>
          <p>Status: {subscription.isActive ? "Active" : "Inactive"}</p>
          <button onClick={handleCancel}>Cancel Subscription</button>
          {!subscription.isActive && (
            <button onClick={handleRenew}>Renew Subscription</button>
          )}
        </div>
      ) : (
        <div>
          <p>No active subscription.</p>
          <select
            onChange={(e) =>
              setSelectedPlan(
                plans.find((p) => p.type === e.target.value) || null,
              )
            }
          >
            <option value="">Select a plan</option>
            {plans.map((p) => (
              <option key={p.type} value={p.type}>
                {p.type} - ${p.price}
              </option>
            ))}
          </select>
          <button onClick={handleCreate} disabled={!selectedPlan}>
            Subscribe
          </button>
        </div>
      )}
    </div>
  );
};

export default SubscriptionManagement;
