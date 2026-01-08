import api from "./api";

export interface SubscriptionPlan {
  type: string;
  price: number;
}

export interface Subscription {
  subscriptionId: number;
  accountId: number;
  subscriptionType: string;
  subscriptionDescription: string;
  basePrice: number;
  startDate: string;
  endDate: string;
  isActive: boolean;
}

export interface CreateSubscriptionDto {
  subscriptionType: string;
  subscriptionDescription: string;
}

const getAuthHeaders = (token: string) => ({
  headers: { Authorization: `Bearer ${token}` },
});

export const getSubscription = async (token: string) => {
  const response = await api.get<Subscription>(
    "/subscription",
    getAuthHeaders(token),
  );
  return response.data;
};

export const createSubscription = async (
  dto: CreateSubscriptionDto,
  token: string,
) => {
  const response = await api.post<Subscription>(
    "/subscription",
    dto,
    getAuthHeaders(token),
  );
  return response.data;
};

export const changeSubscription = async (
  dto: ChangeSubscriptionDto,
  token: string,
) => {
  const response = await api.put<Subscription>(
    `/subscription/${dto.subscriptionId}`,
    dto,
    getAuthHeaders(token),
  );
  return response.data;
};

export const cancelSubscription = async (
  subscriptionId: number,
  token: string,
) => {
  const response = await api.delete(
    `/subscription/${subscriptionId}`,
    getAuthHeaders(token),
  );
  return response.data;
};

export const renewSubscription = async (
  subscriptionId: number,
  token: string,
) => {
  const response = await api.post<Subscription>(
    `/subscription/renew/${subscriptionId}`,
    {},
    getAuthHeaders(token),
  );
  return response.data;
};

export const getPlans = async () => {
  const response = await api.get<SubscriptionPlan[]>("/subscription/plans");
  return response.data;
};
