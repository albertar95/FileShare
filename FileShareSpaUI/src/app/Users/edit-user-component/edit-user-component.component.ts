import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UpdateUserDTO } from '../../Models/UpdateUserDTO.model';
import { User } from '../../Models/User.model';
import { UserService } from '../../Services/UserService.service';

@Component({
  selector: 'app-edit-user-component',
  templateUrl: './edit-user-component.component.html',
  styleUrls: ['./edit-user-component.component.css']
})
export class EditUserComponentComponent implements OnInit {
  currentUser: UpdateUserDTO;
  constructor(private userService: UserService, private route: ActivatedRoute, private router: Router) { }

  ngOnInit(): void {
    const userId = this.route.snapshot.params["id"];
    this.userService.get(userId).then(r => { this.currentUser = r });
  }
  FormSubmit()
  {
    this.userService.update(this.currentUser).subscribe(x => {
      if (x) {
        alert("user edited successfully")
        this.router.navigate(['users']);
      }
    });
  }
}
