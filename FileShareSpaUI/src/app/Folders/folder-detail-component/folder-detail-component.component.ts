import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Folder } from '../../Models/Folder.model';
import { FolderService } from '../../Services/FolderService.service';

@Component({
  selector: 'app-folder-detail-component',
  templateUrl: './folder-detail-component.component.html',
  styleUrls: ['./folder-detail-component.component.css']
})
export class FolderDetailComponentComponent implements OnInit {
  folder: Folder;
  constructor(private folderService: FolderService, private router: ActivatedRoute) { }

  ngOnInit(): void {
    this.folderService.get(this.router.snapshot.params["id"]).subscribe(x => { this.folder = x });
  }

}
