import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HTTP_INTERCEPTORS,
  HttpErrorResponse,
} from '@angular/common/http';
import {
  BehaviorSubject,
  Observable,
  catchError,
  concatMap,
  filter,
  finalize,
  switchMap,
  take,
  throwError,
} from 'rxjs';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  isRefreshingToken = false;
  tokenRefreshed$ = new BehaviorSubject<boolean>(false);
  constructor(private auth: AuthService) {}

  intercept(
    request: HttpRequest<unknown>,
    next: HttpHandler
  ): Observable<HttpEvent<unknown>> {
    if (!request.url.includes('refresh-token')) {
      request = this.addTokenHeader(request, this.auth.token);
    }
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401) {
          return this.handleAuthError(request, next);
        }
        return throwError(() => error);
      })
    );
  }
  handleAuthError(request: HttpRequest<any>, next: HttpHandler) {
    if (this.isRefreshingToken) {
      return this.tokenRefreshed$.pipe(
        filter(Boolean),
        take(1),
        concatMap(() =>
          next.handle(this.addTokenHeader(request, this.auth.token))
        )
      );
    }
    this.isRefreshingToken = true;
    this.tokenRefreshed$.next(false);
    return this.auth.updateToken().pipe(
      switchMap((result) => {
        this.auth.saveTokens(result);
        this.tokenRefreshed$.next(true);
        return next.handle(this.addTokenHeader(request, this.auth.token));
      }),
      catchError((error) => {
        return throwError(() => {
          this.auth.logout();
        });
      }),
      finalize(() => (this.isRefreshingToken = false))
    );
  }
  private addTokenHeader(request: HttpRequest<any>, token: string) {
    request = request.clone({
      headers: request.headers.set('Authorization', 'Bearer ' + token),
    });
    return request;
  }
}

export const AuthInterceptorProviders = {
  provide: HTTP_INTERCEPTORS,
  useClass: AuthInterceptor,
  multi: true,
};
