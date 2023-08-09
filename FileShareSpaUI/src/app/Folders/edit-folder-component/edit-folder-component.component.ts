import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Folder } from '../../Models/Folder.model';
import { UpdateFolderDTO } from '../../Models/UpdateFolderDTO.model';
import { FolderService } from '../../Services/FolderService.service';

@Component({
  selector: 'app-edit-folder-component',
  templateUrl: './edit-folder-component.component.html',
  styleUrls: ['./edit-folder-component.component.css']
})
export class EditFolderComponentComponent implements OnInit {
  folder: UpdateFolderDTO;
  constructor(private folderService: FolderService, private router: Router, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.folderService.get(this.route.snapshot.params["id"]).subscribe(x => { this.folder = x });
  }
  FormSubmit()
  {
    this.folderService.update(this.folder).subscribe(x => {
      if (x)
      {
        alert("edited successfully");
        this.router.navigate(['folders']);
      }
      });
  }
}
