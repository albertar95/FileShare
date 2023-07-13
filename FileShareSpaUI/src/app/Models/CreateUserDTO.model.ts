export class CreateUserDTO {
  Username: string;
  Password: string;
  Fullname: string;
  IsDisabled: boolean;
  IsAdmin: boolean;
  constructor(username: string, password: string, fullname: string,isdisabled: boolean, isadmin: boolean)
  {
    this.Username = username;
    this.Password = password;
    this.Fullname = fullname;
    this.IsDisabled = isdisabled;
    this.IsAdmin = isadmin;
  }
}
