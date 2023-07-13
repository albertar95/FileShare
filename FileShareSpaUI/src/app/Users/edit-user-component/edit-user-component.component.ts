import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { User } from '../../Models/User.model';
import { UserService } from '../../Services/UserService.service';

@Component({
  selector: 'app-edit-user-component',
  templateUrl: './edit-user-component.component.html',
  styleUrls: ['./edit-user-component.component.css']
})
export class EditUserComponentComponent implements OnInit {
  currentUser: User = new User();
  constructor(private userService: UserService, private route: ActivatedRoute) { }

  ngOnInit(): void {
    const userId = this.route.snapshot.params["id"];
    this.userService.get(userId).then(r => { this.currentUser = r });
  }
  FormSubmit() {}
}
