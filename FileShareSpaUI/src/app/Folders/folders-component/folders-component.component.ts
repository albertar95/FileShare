import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Folder } from '../../Models/Folder.model';
import { FolderService } from '../../Services/FolderService.service';

@Component({
  selector: 'app-folders-component',
  templateUrl: './folders-component.component.html',
  styleUrls: ['./folders-component.component.css']
})
export class FoldersComponentComponent implements OnInit {
  @ViewChild('deleteModal') deleteModal: ElementRef;
  folderList: Folder[] = [];
  userIdTest: string = '5da96548-7b4b-460d-b8ad-b2a4f666f13c';
  private CurrentFolderId: string = '';
  CurrentFolderName: string = '';
  showSuccess = false;
  showError = false;
  txtSuccess: string = '';
  txtError: string = '';
  constructor(private folderService: FolderService) { }

  ngOnInit(): void {
    this.folderService.list(this.userIdTest).then(result => { this.folderList = result });
  }
  DeleteFolderEvent(id: string, title: string)
  {
    this.CurrentFolderId = id;
    this.CurrentFolderName = title;
    this.submitDeleteFolder();
  }
  submitDeleteFolder()
  {
    let submitResult: boolean = false;
    this.folderService.delete(this.CurrentFolderId).subscribe(result => { submitResult = result });
    if (submitResult) {
      this.txtSuccess = 'folder deleted successfully';
      this.showSuccess = true;
      setTimeout(() => { this.showSuccess = false; }, 10000);
      this.folderService.list(this.userIdTest).then(result => { this.folderList = result });
    } else
    {
      this.txtError = 'error occured in deleting folder';
      this.showError = true;
      setTimeout(() => { this.showError = false; }, 10000);
    }
  }
}
