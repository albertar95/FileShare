export class CreateFolderDTO {
  Title: string;
  IsLocal: boolean;
  IsPublic: boolean;
  IsProtected: boolean;
  Path: string;
  UserId: string;
  constructor(title: string, islocal: boolean, ispublic: boolean, isprotected: boolean
    , path: string, userid: string)
  {
    this.Title = title;
    this.IsLocal = islocal;
    this.IsPublic = ispublic;
    this.IsProtected = isprotected;
    this.Path = path;
    this.UserId = userid;
  }
}
