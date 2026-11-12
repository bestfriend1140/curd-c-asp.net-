import { DatePipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, computed, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { TodoItem } from '../../core/models/todo.model';
import { environment } from '../../../environments/environment';

import { EmptyTrashModal } from './empty-trash-modal';
import { DeleteTrashItemModal } from './delete-trash-item-modal';

@Component({
  selector: 'app-bin',
  standalone: true,
  imports: [
    DatePipe,
    MatButtonModule,
    MatIconModule,
    EmptyTrashModal,
    DeleteTrashItemModal,
  ],
  templateUrl: './bin.html',
  styleUrl: './bin.css',
})
export class BinPage {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiBaseUrl}/api/tasks`;

  protected readonly todos = signal<TodoItem[]>([]);
  protected readonly loading = signal(false);
  protected readonly error = signal<string | null>(null);

  protected readonly showEmptyTrashModal = signal(false);
  protected readonly deletingTodo = signal<TodoItem | null>(null);

  protected readonly completedCount = computed(
    () => this.todos().filter((todo) => todo.isCompleted).length,
  );

  protected readonly pendingCount = computed(
    () => this.todos().length - this.completedCount(),
  );

  protected readonly progress = computed(() => {
    const total = this.todos().length;
    return total === 0
      ? 0
      : Math.round((this.completedCount() / total) * 100);
  });

  constructor() {
    this.load();
  }

  private getCurrentUserId(): number | null {
    const rawUser = localStorage.getItem('auth_user') ?? localStorage.getItem('user');
    try {
      const user = rawUser ? JSON.parse(rawUser) : {};
      const userId = Number(user?.id ?? user?.userId);
      return userId ? userId : null;
    } catch {
      return null;
    }
  }

  private normalizeTodo(todo: any): TodoItem {
    const completed = todo.isCompleted ?? todo.IsCompleted;

    return {
      id: String(todo.id ?? todo.Id ?? ''),
      title: todo.title ?? todo.Title ?? '',
      description: todo.description ?? todo.Description,
      isCompleted:
        completed === true ||
        completed === 1 ||
        completed === '1' ||
        completed === 'true',
      createdAt:
        todo.createdAt ?? todo.CreatedAt ?? new Date().toISOString(),
      status: todo.status ?? todo.Status,
      userId: Number(todo.userId ?? todo.UserId ?? 0),
    };
  }

  // Load deleted/inactive tasks for current user
  protected load(): void {
    const userId = this.getCurrentUserId();
    if (!userId) {
      this.error.set('User ID not found. Please login again.');
      this.loading.set(false);
      return;
    }

    this.loading.set(true);
    this.error.set(null);

    this.http.get<any[]>(`${this.apiUrl}/byUserId/${userId}/I`).subscribe({
      next: (data) => {
        this.todos.set((data ?? []).map((item) => this.normalizeTodo(item)));
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Failed to load trash:', err);
        const message =
          err?.status === 0
            ? 'Failed to load trash. Is the API running?'
            : `Failed to load trash (${err?.status ?? 'unknown error'}).`;
        this.error.set(message);
        this.loading.set(false);
      },
    });
  }

  // Restore task (set status back to Active)
  protected restoreTodo(todo: TodoItem): void {
    this.http
      .put<any>(`${this.apiUrl}/${todo.id}`, {
        title: todo.title,
        description: todo.description ?? undefined,
        isCompleted: todo.isCompleted,
        status: 'A',
      })
      .subscribe({
        next: () => {
          this.todos.update((items) => items.filter((item) => item.id !== todo.id));
        },
        error: (err) => {
          console.error('Failed to restore task:', err);
        },
      });
  }

  // Open delete confirmation modal
  protected openDeleteTodoModal(todo: TodoItem): void {
    this.deletingTodo.set(todo);
  }

  // Close delete confirmation modal
  protected closeDeleteTodoModal(): void {
    this.deletingTodo.set(null);
  }

  // Confirm permanent delete
  protected confirmDeleteTodo(): void {
    const todo = this.deletingTodo();
    if (!todo) return;

    this.http.delete(`${this.apiUrl}/${todo.id}`).subscribe({
      next: () => {
        this.todos.update((items) => items.filter((item) => item.id !== todo.id));
        this.closeDeleteTodoModal();
      },
      error: (err) => {
        console.error('Failed to delete task permanently:', err);
      },
    });
  }

  // Open empty trash modal
  protected openEmptyTrashModal(): void {
    if (this.todos().length === 0) return;
    this.showEmptyTrashModal.set(true);
  }

  // Close empty trash modal
  protected closeEmptyTrashModal(): void {
    this.showEmptyTrashModal.set(false);
  }

  // Confirm empty trash
  protected emptyTrash(): void {
    const trashTodos = this.todos();
    if (trashTodos.length === 0) {
      this.closeEmptyTrashModal();
      return;
    }

    forkJoin(
      trashTodos.map((todo) =>
        this.http.delete(`${this.apiUrl}/${todo.id}`).pipe(
          catchError((err) => {
            console.error('Failed to delete task from trash:', err);
            return of(null);
          }),
        ),
      ),
    ).subscribe(() => {
      this.todos.set([]);
      this.closeEmptyTrashModal();
    });
  }
}