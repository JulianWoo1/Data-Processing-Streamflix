import api from "./api";

export interface CreateInvitationDto {
  referrerAccountId: number;
}

export interface AcceptInvitationDto {
  invitationCode: string;
  referredAccountId: number;
}

export interface ReferralStatus {
  invitationCode: string;
  status: string;
}

export interface Discount {
  discountStartDate: string;
  discountEndDate: string;
  discountAmount: number;
}

const getAuthHeaders = (token: string) => ({
  headers: { Authorization: `Bearer ${token}` },
});

export const createInvitation = async (
  dto: CreateInvitationDto,
  token: string,
) => {
  const response = await api.post(
    "/referral/createInvitation",
    dto,
    getAuthHeaders(token),
  );
  return response.data;
};

export const acceptInvitation = async (
  dto: AcceptInvitationDto,
  token: string,
) => {
  const response = await api.post(
    "/referral/acceptInvitation",
    dto,
    getAuthHeaders(token),
  );
  return response.data;
};

export const getReferralStatus = async (
  invitationCode: string,
  token: string,
) => {
  const response = await api.get<ReferralStatus>(
    `/referral/getReferralStatus/${invitationCode}`,
    getAuthHeaders(token),
  );
  return response.data;
};

export const getDiscount = async (accountId: number, token: string) => {
  const response = await api.get<Discount>(
    `/referral/getDiscount/${accountId}`,
    getAuthHeaders(token),
  );
  return response.data;
};
