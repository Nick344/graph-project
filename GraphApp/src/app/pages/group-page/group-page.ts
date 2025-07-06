import { Component } from '@angular/core';
import {OnInit} from '@angular/core';
import {ScheduleService} from '../../core/services/schedule.service';
import {SchedulePage} from '../schedule-page/schedule-page';
import {ScheduleEntryModel} from '../../core/models/schedule-entry.model';

@Component({
  selector: 'app-group-page',
  standalone: false,
  templateUrl: './group-page.html',
  styleUrl: './group-page.css'
})
export class GroupPage implements OnInit{
constructor(private scheduleService: ScheduleService) {}

  ngOnInit() {
    this.scheduleService.loadMockData();
  }

statusMessage: string = '';
selectedGroup: string = '';
groupSchedule: ScheduleEntryModel[] = [];


search() {
  let scheduleCity = this.scheduleService.getCitySchedule("Київ");
  let dataGroup = scheduleCity.find(elem => elem.group === this.selectedGroup);

  if (!dataGroup) {
    this.statusMessage = 'Групу не знайдено';
    return;
  } else
this.groupSchedule = dataGroup.schedule;
  this.statusMessage = this.scheduleService.checkLightStatus(this.selectedGroup);

}
}
