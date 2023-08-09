import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { DashboardComponentComponent } from "./Dashboard/dashboard-component/dashboard-component.component";
import { AddFolderComponentComponent } from "./Folders/add-folder-component/add-folder-component.component";
import { EditFolderComponentComponent } from "./Folders/edit-folder-component/edit-folder-component.component";
import { FolderDetailComponentComponent } from "./Folders/folder-detail-component/folder-detail-component.component";
import { FoldersComponentComponent } from "./Folders/folders-component/folders-component.component";
import { LoginComponent } from "./login/login.component";
import { LoginResult } from "./Models/LoginResult.model";
import { MyProfileComponentComponent } from "./UserProfile/my-profile-component/my-profile-component.component";
import { AddUserComponentComponent } from "./Users/add-user-component/add-user-component.component";
import { EditUserComponentComponent } from "./Users/edit-user-component/edit-user-component.component";
import { UserDetailComponentComponent } from "./Users/user-detail-component/user-detail-component.component";
import { UsersComponentComponent } from "./Users/users-component/users-component.component";

const appRoutes: Routes = [
  { path: '', component: DashboardComponentComponent },
  { path: 'users', component: UsersComponentComponent },
  { path: 'users/add', component: AddUserComponentComponent },
  { path: 'folders', component: FoldersComponentComponent },
  { path: 'folders/add', component: AddFolderComponentComponent },
  { path: 'folders/:id/edit', component: EditFolderComponentComponent },
  { path: 'folders/:id/detail', component: FolderDetailComponentComponent },
  { path: 'users/:id/edit', component: EditUserComponentComponent },
  { path: 'users/:id/detail', component: UserDetailComponentComponent },
  { path: 'userprofile', component: MyProfileComponentComponent },
  { path: 'login', component: LoginComponent },
  /*,{ path: '**', redirectTo: '/not-found' }*/
];

@NgModule({
  imports: [
    // RouterModule.forRoot(appRoutes, {useHash: true})
    RouterModule.forRoot(appRoutes)
  ],
  exports: [RouterModule]
})
export class RouteConfig {

}
