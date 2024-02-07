import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, catchError, tap, throwError } from 'rxjs';
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
import { CookieOptionsProvider, CookieService } from 'ngx-cookie';

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
  readonly #baseUrl = environment.baseApiIdentityUrl + '/user';
  constructor(
    private client: HttpClient,
    private router: Router,
    private cookieService: CookieService,
    private optionsProvider: CookieOptionsProvider
  ) {
    this._isAuthenticated$.next(!!this.token);
    if (this.token) {
      this.tokenUser = this.getUser(this.token);
    }
  }

  updateToken(): Observable<ITokenResponse> {
    const request = new RefreshTokenRequest(this.token, this.refreshToken);
    return this.client.post<ITokenResponse>(
      this.#baseUrl + '/refresh-token',
      request
    );
  }

  login(request: TokenRequest): Observable<ITokenResponse> {
    return this.client
      .post<ITokenResponse>(
        this.#baseUrl+'/token',
        request
      )
      .pipe(
        catchError(err => {
          console.log(err);
          return throwError('Invalid username or password');
        }),
        tap((response) => {
          this.updateAuthState(response);
        })
      );
  }
  updateAuthState(response: ITokenResponse) {
    this.saveTokens(response);
    this.tokenUser = this.getUser(response.accessToken);
    this._isAuthenticated$.next(true);
  }

  saveTokens(response: ITokenResponse) {
    this.optionsProvider.options.secure = true;
    this.cookieService.put(this.TOKEN_NAME, response.accessToken, this.optionsProvider.options);
    this.cookieService.put(this.REFRESH_TOKEN_NAME, response.refreshToken, this.optionsProvider.options);
    console.log(response);
    console.log(this.cookieService.get(this.TOKEN_NAME));
  }

  get token(): string {
    const result = this.cookieService.get(this.TOKEN_NAME) as string;
    return result;
  }
  get refreshToken(): string {
    const result = this.cookieService.get(this.REFRESH_TOKEN_NAME) as string;
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
    this.cookieService.remove(this.TOKEN_NAME);
    this.cookieService.remove(this.REFRESH_TOKEN_NAME);
    this._isAuthenticated$.next(false);
  }
  public logout(): void {
    this.clearTokens();
    this.router.navigate(['/login']);
  }
}
