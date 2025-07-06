import {ScheduleEntryModel} from './schedule-entry.model';

export interface GroupScheduleModel {
  group: string;
  schedule: ScheduleEntryModel[];
}
