import { Component, OnInit } from '@angular/core';
import { GetLatestService } from '../../usecase/get-latest.service';
import { FuelRate } from '../../domain/fuel-rate';

@Component({
    selector: 'app-fuel-table',
    templateUrl: './fuel-table.component.html',
    styleUrls: ['./fuel-table.component.css']
})
export class FuelTableComponent implements OnInit {
    fuelRate: FuelRate | null = null;

    constructor(private getLatest: GetLatestService) {}

    ngOnInit(): void {
        this.getLatest.getLatestObservable().subscribe(rate => this.fuelRate = rate);
        this.getLatest.refresh();
    }

    refresh(): void {
        this.getLatest.refresh();
    }
}
