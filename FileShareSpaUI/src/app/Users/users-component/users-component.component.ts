import { Output } from '@angular/core';
import { Component, EventEmitter, Input, OnInit } from '@angular/core';
import { User } from '../../Models/User.model';
import { UserService } from '../../Services/UserService.service';

@Component({
  selector: 'app-users-component',
  templateUrl: './users-component.component.html',
  styleUrls: ['./users-component.component.css']
})
export class UsersComponentComponent implements OnInit {
  userList: User[] = [];
  constructor(private userService: UserService) { }

  ngOnInit(): void {
    this.userService.list(true).then(result => { this.userList = result });
  }

}
