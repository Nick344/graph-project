import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SchedulePage } from './pages/schedule-page/schedule-page';
import { GroupPage } from './pages/group-page/group-page';

const routes: Routes = [
  {path: 'schedule', component: SchedulePage},
  {path:'group',component: GroupPage},
]

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
