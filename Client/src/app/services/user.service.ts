import { Injectable } from '@angular/core';
import { User } from '../models';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';

@Injectable()
export class UserService {
  public errors: any = {};
  public errorMsg: string = "אירעה שגיאה, נסה שוב מאוחר יותר";
  public user: User;
  httpOptions;
  url = "http://localhost:51396/";
  constructor(private http: HttpClient, private router: Router) { this.resetToken(); }

  changeToken(token: string) {
    this.httpOptions.headers =
      new HttpHeaders({ 'Content-Type': 'application/json', 'Authorization': "Bearer " + token });

  }

  resetToken() {
    this.httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json', 'Authorization': '' }) };
  }


  login(username, password) {
    return this.http.get(this.url + `login?username=${username}&password=${password}`)
      .subscribe((res: any) => { this.afterLogin(res); });
  }

  register(username, password) {
    this.http.post(`${this.url}/register`, { name: username, password: password })
      .subscribe((res: User) => { this.afterLogin(res); });
  }

  logout() {
    this.http.get(`${this.url}/logout`, this.httpOptions).subscribe((res: any) => {
      if (res.Success) {
        this.user = null;
        this.resetToken();
        this.router.navigate(["/account"]);
        alert(res.Message);
      }
    });
  }

  afterLogin(res) {
    if (res.Success) {
      var token = JSON.parse(res.Value.TokenJson)
      this.changeToken(token.access_token);
      this.user = res.Value.User;
      this.router.navigate([""]);
    }
    else alert(res.Message);
  }
}

