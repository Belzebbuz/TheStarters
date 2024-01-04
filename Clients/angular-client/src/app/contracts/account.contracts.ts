export class SelfRegisterRequest {
  constructor(public email: string, public password: string) {}
}

export class TokenRequest {
  constructor(public email: string, public password: string) {}
}
export class RefreshTokenRequest {
  constructor(public accessToken: string, public refreshToken: string) {}
}
export class ChangePasswordRequest {
  constructor(public newPassword: string, public oldPassword: string) {}
}
export class ConfirmAccountRequest {
  constructor(public confirmToken: number, public email: string) {}
}
export class ResetPasswordCallbackRequest {
  constructor(public email: string) {}
}
export class ResetPasswordRequest {
  constructor(
    public resetToken: string,
    public userId: string,
    public newPassword: string
  ) {}
}


export interface ITokenResponse {
  accessToken: string;
  refreshToken: string;
  refreshTokenExpTime: string;
}
