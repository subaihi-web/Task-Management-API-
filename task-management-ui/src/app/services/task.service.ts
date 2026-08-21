import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from './api.config';

export interface Task {
  id: number;
  title: string;
  description: string | null;
  fromDate: string;
  toDate: string;
  statusId: number;
  status: string;
  userId: number;
}

export interface TaskPayload {
  title: string;
  description?: string | null;
  fromDate: string;
  toDate: string;
  statusId: number;
}

export interface TaskUpdatePayload extends TaskPayload {
  id: number;
}

@Injectable({ providedIn: 'root' })
export class TaskService {
  private readonly http = inject(HttpClient);
  private readonly url = `${API_BASE_URL}/tasks`;

  getByCriteria(title?: string, statusId?: number): Observable<Task[]> {
    let params = new HttpParams();
    if (title?.trim()) params = params.set('Title', title.trim());
    if (statusId) params = params.set('StatusId', statusId);
    return this.http.get<Task[]>(`${this.url}/GetByCriteria`, { params });
  }

  getById(id: number): Observable<Task> {
    return this.http.get<Task>(`${this.url}/GetById/${id}`);
  }

  add(payload: TaskPayload): Observable<Task> {
    return this.http.post<Task>(`${this.url}/Add`, payload);
  }

  update(payload: TaskUpdatePayload): Observable<Task> {
    return this.http.put<Task>(`${this.url}/Update`, payload);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/Delete/${id}`);
  }
}
