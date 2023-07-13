import { getNgModuleById, NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { AppComponent } from './app.component';
import { LayoutComponentComponent } from './layout-component/layout-component.component';
import { AddUserComponentComponent } from './Users/add-user-component/add-user-component.component';
import { UsersComponentComponent } from './Users/users-component/users-component.component';
import { EditUserComponentComponent } from './Users/edit-user-component/edit-user-component.component';
import { FoldersComponentComponent } from './Folders/folders-component/folders-component.component';
import { AddFolderComponentComponent } from './Folders/add-folder-component/add-folder-component.component';
import { EditFolderComponentComponent } from './Folders/edit-folder-component/edit-folder-component.component';
import { DashboardComponentComponent } from './Dashboard/dashboard-component/dashboard-component.component';
import { MyProfileComponentComponent } from './UserProfile/my-profile-component/my-profile-component.component';
import { RouteConfig } from './RouteConfig.Route';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { FolderDetailComponentComponent } from './Folders/folder-detail-component/folder-detail-component.component';
import { UserDetailComponentComponent } from './Users/user-detail-component/user-detail-component.component';
@NgModule({
  declarations: [
    AppComponent,
    LayoutComponentComponent,
    AddUserComponentComponent,
    UsersComponentComponent,
    EditUserComponentComponent,
    FoldersComponentComponent,
    AddFolderComponentComponent,
    EditFolderComponentComponent,
    DashboardComponentComponent,
    MyProfileComponentComponent,
    FolderDetailComponentComponent,
    UserDetailComponentComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    RouteConfig,
    HttpClientModule
    
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
