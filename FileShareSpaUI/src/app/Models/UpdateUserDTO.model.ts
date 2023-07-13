export class UpdateUserDTO {
  Id: string;
  Username: string;
  Password: string;
  Fullname: string;
  IsDisabled: boolean;
  IsAdmin: boolean;
  constructor(id: string, username: string, password: string, fullname: string, isdisabled: boolean, isadmin: boolean)
  {
    this.Id = id;
    this.Username = username;
    this.Password = password;
    this.Fullname = fullname;
    this.IsDisabled = isdisabled;
    this.IsAdmin = isadmin;
  }
}
