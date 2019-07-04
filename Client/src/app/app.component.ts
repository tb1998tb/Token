import { Component } from '@angular/core';
import { UserService } from './services';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'Client';
  constructor(public userServ: UserService) { }

  logout() {
    this.userServ.logout();
  }
}
