import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { FuelRate } from '../domain/fuel-rate';
import { FuelService } from '../infra/fuel.service';

@Injectable({
    providedIn: 'root'
})
export class GetLatestService {
    private latestRate$ = new BehaviorSubject<FuelRate | null>(null);

    constructor(private fuelService: FuelService) {}

    refresh(area: string = 'NUS'): void {
        this.fuelService.getLatest().subscribe(rate => this.latestRate$.next(rate));
    }

    getLatestObservable() {
        return this.latestRate$.asObservable();
    }
}
