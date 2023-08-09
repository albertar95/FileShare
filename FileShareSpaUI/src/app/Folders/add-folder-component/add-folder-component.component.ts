import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CreateFolderDTO } from '../../Models/CreateFolderDTO.model';
import { Folder } from '../../Models/Folder.model';
import { FolderService } from '../../Services/FolderService.service';

@Component({
  selector: 'app-add-folder-component',
  templateUrl: './add-folder-component.component.html',
  styleUrls: ['./add-folder-component.component.css']
})
export class AddFolderComponentComponent implements OnInit {

  constructor(private folderService: FolderService, private router: Router) { }
  @ViewChild('Frm') addForm: NgForm;
  userIdTest: string = '5da96548-7b4b-460d-b8ad-b2a4f666f13c';
  IsLocalFolder: boolean = true;
  ngOnInit(): void {
  }
  FormSubmit()
  {
    console.log(new CreateFolderDTO(this.addForm.value.Title, this.addForm.value.IsLocal,
      this.addForm.value.IsPublic, this.addForm.value.IsProtected, this.addForm.value.Path, this.userIdTest))
    this.folderService.add(new CreateFolderDTO(this.addForm.value.Title, this.addForm.value.IsLocal,
      this.addForm.value.IsPublic, this.addForm.value.IsProtected, this.addForm.value.Path, this.userIdTest)).subscribe(x => {
        if (x) {
          alert("successfully added");
          this.router.navigate(['folders']);
        } else {
          alert("error");
        }
        })
  }
  IsLocalChanged()
  {
    if (this.addForm.value.IsLocal) {
      this.IsLocalFolder = false;
    } else
    {
      this.IsLocalFolder = true;
    }
  }
}
