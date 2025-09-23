import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { FuelRate } from '../domain/fuel-rate';

@Injectable({
    providedIn: 'root'
})
export class FuelService {
    private apiUrl = 'http://localhost:8080/fuel/latest'; // Go/C# API placeholder

    constructor(private http: HttpClient) {}

    getLatest(): Observable<FuelRate> {
        // TODO: Replace with real backend API
        return this.http.get<FuelRate>(this.apiUrl);
    }
}
