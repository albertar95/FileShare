import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { CreateUserDTO } from '../../Models/CreateUserDTO.model';
import { UserService } from '../../Services/UserService.service';

@Component({
  selector: 'app-add-user-component',
  templateUrl: './add-user-component.component.html',
  styleUrls: ['./add-user-component.component.css']
})
export class AddUserComponentComponent implements OnInit {
  @ViewChild('Frm') addUserForm: NgForm;
  constructor(private userService: UserService, private router: Router) { }

  ngOnInit(): void {
  }
  FormSubmit()
  {
    this.userService.add(new CreateUserDTO(this.addUserForm.value.Username, this.addUserForm.value.Password, this.addUserForm.value.Fullname,
      this.addUserForm.value.IsDisabled, this.addUserForm.value.IsAdmin)).subscribe(
        x => {
          if (x) {
            alert("user added successfully");
            this.router.navigate(['users']);
          } else
          {
            alert("error occured");
          }
        });
  }
}
