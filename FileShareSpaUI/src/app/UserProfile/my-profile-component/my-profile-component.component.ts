import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { User } from '../../Models/User.model';
import { UserService } from '../../Services/UserService.service';

@Component({
  selector: 'app-my-profile-component',
  templateUrl: './my-profile-component.component.html',
  styleUrls: ['./my-profile-component.component.css']
})
export class MyProfileComponentComponent implements OnInit {
  user: User = new User();
  constructor(private userService: UserService,
    private router: Router,
    private route: ActivatedRoute) { }

  ngOnInit(): void {
    const id = '5da96548-7b4b-460d-b8ad-b2a4f666f13c';
    this.userService.get(id).then(x => { this.user = x; });
  }

}
