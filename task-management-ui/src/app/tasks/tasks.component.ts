import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Task, TaskPayload, TaskService, TaskUpdatePayload } from '../services/task.service';
import { AuthService } from '../services/auth.service';
import { getApiErrorMessage } from '../shared/error-message';

@Component({
  selector: 'app-tasks',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './tasks.component.html',
  styleUrl: './tasks.component.css'
})
export class TasksComponent implements OnInit {
  private readonly tasksService = inject(TaskService);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  tasks: Task[] = [];
  loading = false;
  saving = false;
  error = '';
  message = '';
  searchTitle = '';
  filterStatus = 0;
  showForm = false;
  editing = false;
  deletingId: number | null = null;

  readonly statuses = [
    { id: 2, name: 'Initiated' },
    { id: 3, name: 'In Progress' },
    { id: 4, name: 'Completed' },
    { id: 5, name: 'Cancelled' }
  ];

  form: TaskPayload & { id: number } = this.emptyForm();

  ngOnInit(): void { this.loadTasks(); }

  loadTasks(): void {
    this.loading = true;
    this.error = '';
    this.tasksService.getByCriteria(this.searchTitle, this.filterStatus || undefined).subscribe({
      next: (tasks) => { this.tasks = tasks; this.loading = false; },
      error: (error) => { this.loading = false; this.error = getApiErrorMessage(error, 'Unable to load your tasks.'); }
    });
  }

  openAdd(): void {
    this.editing = false;
    this.form = this.emptyForm();
    this.error = '';
    this.showForm = true;
  }

  openEdit(task: Task): void {
    this.editing = true;
    this.form = {
      id: task.id,
      title: task.title,
      description: task.description,
      fromDate: this.toLocalInput(task.fromDate),
      toDate: this.toLocalInput(task.toDate),
      statusId: task.statusId
    };
    this.error = '';
    this.showForm = true;
  }

  closeForm(): void {
    if (!this.saving) this.showForm = false;
  }

  saveTask(): void {
    this.error = '';
    this.message = '';
    if (!this.form.title.trim()) { this.error = 'Title is required.'; return; }
    if (!this.form.fromDate || !this.form.toDate) { this.error = 'Start and due dates are required.'; return; }
    if (new Date(this.form.fromDate) > new Date(this.form.toDate)) { this.error = 'From date must be less than or equal to To date.'; return; }

    this.saving = true;
    const payload = {
      title: this.form.title.trim(),
      description: this.form.description?.trim() || null,
      fromDate: this.form.fromDate,
      toDate: this.form.toDate,
      statusId: Number(this.form.statusId)
    };

    const request = this.editing
      ? this.tasksService.update({ ...payload, id: this.form.id } as TaskUpdatePayload)
      : this.tasksService.add(payload);

    request.subscribe({
      next: () => {
        this.saving = false;
        this.showForm = false;
        this.message = this.editing ? 'Task updated successfully.' : 'Task added successfully.';
        this.loadTasks();
      },
      error: (error) => { this.saving = false; this.error = getApiErrorMessage(error, 'Unable to save the task.'); }
    });
  }

  deleteTask(task: Task): void {
    if (!confirm(`Delete "${task.title}"?`)) return;
    this.deletingId = task.id;
    this.error = '';
    this.tasksService.delete(task.id).subscribe({
      next: () => {
        this.deletingId = null;
        this.message = 'Task deleted successfully.';
        this.loadTasks();
      },
      error: (error) => {
        this.deletingId = null;
        this.error = getApiErrorMessage(error, 'Unable to delete the task.');
      }
    });
  }

  logout(): void {
    this.auth.logout();
    this.router.navigate(['/login']);
  }

  statusClass(statusId: number): string {
    return `status-${statusId}`;
  }

  private emptyForm(): TaskPayload & { id: number } {
    return {
      id: 0,
      title: '',
      description: '',
      fromDate: this.localDateTime(new Date()),
      toDate: this.localDateTime(new Date(Date.now() + 86400000)),
      statusId: 2
    };
  }

  private localDateTime(date: Date): string {
    const pad = (n: number) => String(n).padStart(2, '0');
    return `${date.getFullYear()}-${pad(date.getMonth()+1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}`;
  }

  private toLocalInput(value: string): string {
    const date = new Date(value);
    return Number.isNaN(date.getTime()) ? value.slice(0, 16) : this.localDateTime(date);
  }
}
