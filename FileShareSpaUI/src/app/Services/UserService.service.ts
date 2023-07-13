import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { map, catchError, tap } from 'rxjs/operators';
import { Observable } from "rxjs";
import { CreateUserDTO } from "../Models/CreateUserDTO.model";
import { UpdateUserDTO } from "../Models/UpdateUserDTO.model";
import { User } from "../Models/User.model";
import { LoginCredential } from "../Models/LoginCredential.model";
import { LoginResult } from "../Models/LoginResult.model";
@Injectable({ providedIn: 'root' })
export class UserService {
  private baseApiUrl: string = environment.baseApiUrl;
  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
    }),
  };
  constructor(private http: HttpClient) { }
  add(user: CreateUserDTO): Observable<boolean> {
    return this.http.post<boolean>(this.baseApiUrl + '/User/Post', JSON.stringify(user), this.httpOptions);
  }
  update(user: UpdateUserDTO): Observable<boolean> {
    return this.http.post<boolean>(this.baseApiUrl + '/User/Patch', JSON.stringify(user), this.httpOptions);
  }
  delete(userId: string): Observable<boolean> {
    return this.http.delete<boolean>(this.baseApiUrl + '/User/Delete/' + userId, this.httpOptions);
  }
  list(includeDisabled: boolean): Promise<User[]> {
    return this.http.get<User[]>(this.baseApiUrl + '/User/Get?IncludeDisabled=' + includeDisabled, this.httpOptions).toPromise()
      .then(response => response as User[]);
  }
  get(userId: string): Promise<User> {
    return this.http.get<User>(this.baseApiUrl + '/User/GetUserById/' + userId, this.httpOptions).toPromise();
  }
  login(credentials: LoginCredential): Promise<LoginResult>
  {
    return this.http.post<LoginResult>(this.baseApiUrl + '/User/LoginUser', JSON.stringify(credentials), this.httpOptions).toPromise()
      .then(response => response as LoginResult);
  }
}
