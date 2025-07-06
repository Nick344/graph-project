import { Component } from '@angular/core';
import {ScheduleService} from '../../core/services/schedule.service';
import {OnInit} from '@angular/core';
import {GroupScheduleModel} from '../../core/models/group-schedule.model';
import {ScheduleEntryModel} from '../../core/models/schedule-entry.model';

@Component({
  selector: 'app-schedule-page',
  standalone: false,
  templateUrl: './schedule-page.html',
  styleUrl: './schedule-page.css'
})
export class SchedulePage implements  OnInit {
  groupSchedules: GroupScheduleModel[] = [];

  constructor(private scheduleService: ScheduleService) {}


  ngOnInit(): void {
    this.scheduleService.loadMockData();
  }


  cities = ["Київ","Житомир","Львів"];
selectedCity:string = '';
selectedDate: string = '';
selectedGroup: string = '';
statusMessage: string = '';


/*
checkLightStatus(){
  let dataCity = this.scheduleService.getCitySchedule(this.selectedCity);
  let dataGroup = dataCity.find(elem => elem.group === this.selectedGroup);

  if (!dataGroup) {
    this.statusMessage = "Групу не знайдено";
    return;
  }

   let arr: ScheduleEntryModel[] = dataGroup?.schedule || [];
   let now = new Date();

   const pad = (num:number) => num.toString().padStart(2, "0");
   let nowStr = `${pad(now.getHours())}:${pad(now.getMinutes())}`;

   let result = arr.find(elem => elem.timeFrom <= nowStr && elem.timeTo >= nowStr);
   if (result) {
this.statusMessage = `Світла немає з ${result.timeFrom} до ${result.timeTo}`;
   } else {
     this.statusMessage = "Світло є";
   }
}
*/





search() {
  this.groupSchedules = this.scheduleService.getScheduleForCityAndData(this.selectedCity, this.selectedDate);
}

}

