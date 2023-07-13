export class Folder {
  Id: string;
  CreateDate: string;
  LastModified: string;
  Title: string;
  IsLocal: boolean;
  IsPublic: boolean;
  IsProtected: boolean;
  Path: string;
  UserId: string;
  constructor(id: string, createdate: string, lastmodified: string, title: string, islocal: boolean, ispublic: boolean, isprotected: boolean
    , path: string, userid: string)
  {
    this.Id = id;
    this.CreateDate = createdate;
    this.LastModified = lastmodified;
    this.Title = title;
    this.IsLocal = islocal;
    this.IsPublic = ispublic;
    this.IsProtected = isprotected;
    this.Path = path;
    this.UserId = userid;
  }
}
