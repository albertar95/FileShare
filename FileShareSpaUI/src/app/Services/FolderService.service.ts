import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { CreateFolderDTO } from "../Models/CreateFolderDTO.model";
import { Folder } from "../Models/Folder.model";
import { UpdateFolderDTO } from "../Models/UpdateFolderDTO.model";
import { map, catchError, tap } from 'rxjs/operators';
import { Observable } from "rxjs";
@Injectable({ providedIn: 'root' })
export class FolderService {
  private baseApiUrl: string = environment.baseApiUrl;
  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
    }),
  };
  constructor(private http: HttpClient) { }
  add(folder: CreateFolderDTO): Observable<boolean> {
    return this.http.post<boolean>(this.baseApiUrl + '/Folder/Post', JSON.stringify(folder), this.httpOptions);
  }
  update(folder: UpdateFolderDTO): Observable<boolean> {
    return this.http.post<boolean>(this.baseApiUrl + '/Folder/Patch', JSON.stringify(folder), this.httpOptions);
  }
  delete(folderId: string): Observable<boolean> {
    return this.http.delete<boolean>(this.baseApiUrl + '/Folder/Delete/' + folderId, this.httpOptions);
  }
  list(userId: string): Promise<Folder[]> {
    userId = '5da96548-7b4b-460d-b8ad-b2a4f666f13c';//for test
    return this.http.get<Folder[]>(this.baseApiUrl + '/Folder/GetFoldersByUserId/' + userId, this.httpOptions).toPromise()
      .then(response => response as Folder[]);
  }
  get(folderId: string): Observable<Folder> {
    return this.http.get<Folder>(this.baseApiUrl + '/Folder/GetFolderById/' + folderId, this.httpOptions);
  }
}
