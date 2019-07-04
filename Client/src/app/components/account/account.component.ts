import { Component, OnInit } from '@angular/core';
import { User } from '../../models';
import { UserService } from '../../services';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.scss']
})
export class AccountComponent implements OnInit {

  loading = false;
  submitted = false;
  user: User = new User();
  repassword: string = "";
  isRegular: boolean = false;
  tabNum: number = 1;
  passwordEye: boolean = false;
  repasswordEye: boolean = false;
  constructor(private http: HttpClient,private userServ:UserService) { }

  ngOnInit() {
    this.clearAll();
  }

  login() {
    this.userServ.login(this.user.name, this.user.password);
  }

  register() {
    this.userServ.register(this.user.name, this.user.password);
  }

  replaceTab(num) {
    if (this.tabNum != num) {
      this.clearAll();
    }
    this.tabNum = num;
  }

  clearAll() {

  }

 
}

