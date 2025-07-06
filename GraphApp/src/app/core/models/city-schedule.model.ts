import {GroupScheduleModel} from './group-schedule.model';

export interface CityScheduleModel {
  city: string;
  data: GroupScheduleModel[];
}
