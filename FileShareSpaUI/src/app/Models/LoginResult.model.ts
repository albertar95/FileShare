import { User } from "./User.model";

export class LoginResult
{
  successLogin: string;
  message: string;
  User: User;
  constructor(successlogin: string, message2: string, user: User)
  {
    this.successLogin = successlogin;
    this.message = message2;
    this.User = user;
  }
}
