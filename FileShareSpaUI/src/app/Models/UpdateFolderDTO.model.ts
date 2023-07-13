export class UpdateFolderDTO {
  Id: string;
  Title: string;
  IsPublic: boolean;
  IsProtected: boolean;
  constructor(id: string, title: string, ispublic: boolean, isprotected: boolean)
  {
    this.Id = id;
    this.Title = title;
    this.IsPublic = ispublic;
    this.IsProtected = isprotected;
  }
}
