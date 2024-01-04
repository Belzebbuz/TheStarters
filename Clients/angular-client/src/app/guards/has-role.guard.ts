import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const hasRoleGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const isAuthorized = authService.tokenUser.roles.includes(route.data['role']);
  if (!isAuthorized) {
    window.alert('You are not authorized to view this page');
    return false;
  }
  return true;
};
