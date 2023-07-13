export class User {
  Id: string;
  CreateDate: string;
  Username: string;
  Password: string;
  Fullname: string;
  LastLoginDate: string;
  LastModified: string;
  IsDisabled: boolean;
  IsAdmin: boolean;
  constructor(id: string = '', createdate: string = '', username: string = '', password: string = '', fullname: string = ''
    , lastlogindate: string = '', lastmodified: string = '', isdisabled: boolean = false, isadmin: boolean = false)
  {
    this.Id = id;
    this.CreateDate = createdate;
    this.Username = username;
    this.Password = password;
    this.Fullname = fullname;
    this.LastLoginDate = lastlogindate;
    this.LastModified = lastmodified;
    this.IsDisabled = isdisabled;
    this.IsAdmin = isadmin;
  }
}
