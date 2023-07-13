import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';

@Component({
  selector: 'app-add-user-component',
  templateUrl: './add-user-component.component.html',
  styleUrls: ['./add-user-component.component.css']
})
export class AddUserComponentComponent implements OnInit {
  @ViewChild('ngForm') addUserForm: ElementRef;
  constructor() { }

  ngOnInit(): void {
  }
  FormSubmit() { }
}
