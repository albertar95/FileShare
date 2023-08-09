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
    if (isdisabled)
      this.IsDisabled = isdisabled;
    else
      this.IsDisabled = false;
    if (isadmin)
      this.IsAdmin = isadmin;
    else
      this.IsAdmin = false;
  }
}
