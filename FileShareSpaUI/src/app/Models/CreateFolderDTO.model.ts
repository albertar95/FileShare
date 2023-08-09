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
    if (islocal)
      this.IsLocal = islocal;
    else
      this.IsLocal = false;
    if (ispublic)
      this.IsPublic = ispublic;
    else
      this.IsPublic = false;
    if (isprotected)
      this.IsProtected = isprotected;
    else
      this.IsProtected = false;
    this.Path = path;
    this.UserId = userid;
  }
}
