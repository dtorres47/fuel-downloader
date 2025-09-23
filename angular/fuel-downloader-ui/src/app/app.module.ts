import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';

import { AppComponent } from './app.component';
import { FuelTableComponent } from './components/fuel-table/fuel-table.component';

@NgModule({
    declarations: [
        AppComponent,
        FuelTableComponent // register our new component
    ],
    imports: [
        BrowserModule,
        HttpClientModule // required for HttpClient in FuelService
    ],
    providers: [],
    bootstrap: [AppComponent]
})
export class AppModule { }
