import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { TokenRequest } from 'src/app/contracts/account.contracts';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
})
export class LoginComponent {
  stage: 'login' | 'regiser';
  tokenRequest = new TokenRequest('admin@thestarters.ru', 'qwert1234QW');
  constructor(private _auth: AuthService, private _router: Router) {
    this.stage = 'login';
  }

  public login() {
    this._auth.login(this.tokenRequest).subscribe((res) => {
      this._router.navigateByUrl('/');
    });
  }
}
