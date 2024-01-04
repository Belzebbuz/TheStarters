import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable,  tap } from 'rxjs';
import { environment } from 'src/environments/environment';
import { IDataResult } from '../contracts/result.contracts';
import {
  ITokenResponse,
  RefreshTokenRequest,
  TokenRequest,
} from '../contracts/account.contracts';
import { jwtDecode } from 'jwt-decode';
import { Router } from '@angular/router';
import { ITokenUser } from '../contracts/user.dto';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  public refreshing = new BehaviorSubject(false);
  public refreshing$ = this.refreshing.asObservable();
  private TOKEN_NAME: string = 'acess_token';
  private REFRESH_TOKEN_NAME: string = 'refresh_token';
  private _isAuthenticated$ = new BehaviorSubject<boolean>(false);
  public isAuthenticated = this._isAuthenticated$.asObservable();
  public tokenUser!: ITokenUser;
  readonly #baseUrl = environment.baseApiUrl + '/token';
  constructor(private client: HttpClient, private router: Router) {
    this._isAuthenticated$.next(!!this.token);
    if (this.token) {
      this.tokenUser = this.getUser(this.token);
    }
  }

  updateToken(): Observable<IDataResult<ITokenResponse>> {
    const request = new RefreshTokenRequest(this.token, this.refreshToken);
    return this.client.post<IDataResult<ITokenResponse>>(
      this.#baseUrl + '/refresh-token',
      request
    );
  }

  login(request: TokenRequest): Observable<IDataResult<ITokenResponse>> {
    return this.client
      .post<IDataResult<ITokenResponse>>(this.#baseUrl, request)
      .pipe(
        tap((response) => {
          this.updateAuthState(response);
        })
      );
  }
  updateAuthState(response: IDataResult<ITokenResponse>) {
    if (response.succeeded) {
      this.saveTokens(response);
      this.tokenUser = this.getUser(response.data.accessToken);
      this._isAuthenticated$.next(true);
    }
  }

  saveTokens(response: IDataResult<ITokenResponse>) {
    localStorage.setItem(this.TOKEN_NAME, response.data.accessToken);
    localStorage.setItem(this.REFRESH_TOKEN_NAME, response.data.refreshToken);
  }

  get token(): string {
    const result = localStorage.getItem(this.TOKEN_NAME) as string;
    return result;
  }
  get refreshToken(): string {
    const result = localStorage.getItem(this.REFRESH_TOKEN_NAME) as string;
    return result;
  }
  private getUser(token: string): ITokenUser {
    const data = jwtDecode<{ [key: string]: any }>(token);
    const user: ITokenUser = {
      id: data[
        'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'
      ],
      email:
        data[
          'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'
        ],
      roles:
        data['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'],
    };
    return user;
  }

  public hasRole(role: string): boolean {
    if (!this.tokenUser) {
      return false;
    }
    return this.tokenUser.roles.includes(role);
  }
  public clearTokens() {
    localStorage.removeItem(this.TOKEN_NAME);
    localStorage.removeItem(this.REFRESH_TOKEN_NAME);
    this._isAuthenticated$.next(false);
  }
  public logout(): void {
    this.clearTokens();
    this.router.navigate(['/login']);
  }
}
