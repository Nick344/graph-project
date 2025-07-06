import { NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import {FormsModule} from "@angular/forms";
import { AppRoutingModule } from './app-routing-module';
import { App } from './app';
import { SchedulePage } from './pages/schedule-page/schedule-page';
import {ReactiveFormsModule} from "@angular/forms";
import { GroupPage } from './pages/group-page/group-page';

@NgModule({
  declarations: [
    App,
    SchedulePage,
    GroupPage
  ],
    imports: [
        BrowserModule,
        AppRoutingModule,
        ReactiveFormsModule,
        FormsModule
    ],
  providers: [
    provideBrowserGlobalErrorListeners()
  ],
  bootstrap: [App]
})
export class AppModule { }
