import {Injectable} from '@angular/core';
import {CityScheduleModel} from '../models/city-schedule.model';
import {GroupScheduleModel} from '../models/group-schedule.model';
import {ScheduleEntryModel} from '../models/schedule-entry.model';

@Injectable({
  providedIn: 'root'
})
 export class ScheduleService {
  private schedule: CityScheduleModel[] = [];

  constructor() { }


  getSchedule(): CityScheduleModel[] {
    return this.schedule;
  }

  getCitySchedule(city: string): GroupScheduleModel[] {
    let result = this.schedule.find(elem => elem.city === city);
   if (result?.city === city) {
     return result.data;
   } else
     return [];
  }

  setSchedules(schedule: CityScheduleModel[]) {
    this.schedule = schedule;
  }

  getScheduleForCityAndData(city:string,data:string): GroupScheduleModel[] {
    return this.getCitySchedule(city);
  }

  checkLightStatus(group:string) {
    let dataCity = this.getCitySchedule("Київ");
    let dataGroup = dataCity.find(elem => elem.group === group);

    if (!dataGroup) {
     return "Групу не знайдено";
    }

    let arr: ScheduleEntryModel[] = dataGroup?.schedule || [];
    let now = new Date();

    const pad = (num:number) => num.toString().padStart(2, "0");
    let nowStr = `${pad(now.getHours())}:${pad(now.getMinutes())}`;

    let result = arr.find(elem => elem.timeFrom <= nowStr && elem.timeTo >= nowStr);
    if (result) {
      return `Світла немає з ${result.timeFrom} до ${result.timeTo}`;
    } else {
     return "Світло є";
    }
  }



  loadMockData() {
    const mock: CityScheduleModel[] = [
      {
        city: "Київ",
        data: [
          {
            group: "1",
            schedule: [
              { timeFrom: "10:00", timeTo: "12:00" },
              { timeFrom: "17:00", timeTo: "21:00" }
            ]
          },
          {
            group: "2",
            schedule: [
              { timeFrom: "12:00", timeTo: "14:00" }
            ]
          }
        ]
      }
    ];

    this.setSchedules(mock);
  }

}

