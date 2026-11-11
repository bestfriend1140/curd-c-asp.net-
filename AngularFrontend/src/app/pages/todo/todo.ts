import { CdkTrapFocus } from '@angular/cdk/a11y';
import { DatePipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, inject, signal, computed } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

import { TodoItem } from '../../core/models/todo.model';
import { environment } from '../../../environments/environment';

import { AddTodoModal } from './todoDialog/add-todo-modal';
import { EditTodoModal } from './todoDialog/edit-todo-modal';
import { DeleteTodoModal } from './todoDialog/delete-todo-modal';

@Component({
  selector: 'app-todo',
  imports: [
    DatePipe,
    MatButtonModule,
    MatIconModule,
    CdkTrapFocus,
    AddTodoModal,
    EditTodoModal,
    DeleteTodoModal,
  ],
  templateUrl: './todo.html',
  styleUrl: './todo.css',
})
export class TodoPage {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiBaseUrl}/api/tasks`;

  private getTasksByUserIdUrl(userId: number, status?: string): string {
    const base_url = `${this.apiUrl}/byUserId/${userId}`;
    return status ? `${base_url}/${status}` : base_url;
  }
  protected readonly todos = signal<TodoItem[]>([]);
  protected readonly loading = signal(true);
  protected readonly error = signal<string | null>(null);

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

  protected readonly showAddModal = signal(false);
  protected readonly editingTodo = signal<TodoItem | null>(null);
  protected readonly deletingTodo = signal<TodoItem | null>(null);

  constructor() {
    this.load();
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
      createdAt: todo.createdAt ?? todo.CreatedAt ?? new Date().toISOString(),
      status: todo.status ?? todo.Status,
      userId: Number(todo.userId ?? todo.UserId ?? 0),
    };
  }

  protected load(): void {
    const userId = this.getCurrentUserId();

    if (!userId) {
      this.error.set('User ID not found. Please login again.');
      this.loading.set(false);
      return;
    }

    this.loading.set(true);
    this.error.set(null);

    this.http.get<any[]>(this.getTasksByUserIdUrl(userId,'A')).subscribe({
      next: (data) => {
        this.todos.set((data ?? []).map((item) => this.normalizeTodo(item)));
        this.loading.set(false);
      },
      error: (err) => {
        console.error(err);
        this.error.set('Failed to load tasks. Is the API running?');
        this.loading.set(false);
      },
    });
  }

  protected openAddModal(): void {
    this.showAddModal.set(true);
  }

  protected closeAddModal(): void {
    this.showAddModal.set(false);
  }

  protected openEditModal(todo: TodoItem): void {
    this.editingTodo.set(todo);
  }

  protected closeEditModal(): void {
    this.editingTodo.set(null);
  }

    protected openDeleteModal(todo: TodoItem): void {
    this.deletingTodo.set(todo);
  }

  protected closeDeleteModal(): void {
    this.deletingTodo.set(null);
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

  protected addTodo(title: string, description: string): void {
    const userId = this.getCurrentUserId();
    if (!userId) {
      console.error('Failed to create task: user not found');
      return;
    }

    const newTodo = {
      title,
      description: description || undefined,
      isCompleted: false,
      status: 'A',
      userId,
    };

    this.http.post<any>(this.apiUrl, newTodo).subscribe({
      next: (response) => {
        const createdTodo = this.normalizeTodo(response);

        this.todos.update((todos) => [createdTodo, ...todos]);
        this.closeAddModal();
      },
      error: (err) => {
        console.error('Failed to create task:', err);
      },
    });
  }

  protected saveEdit(title: string, description: string): void {
    const current = this.editingTodo();

    if (!current) return;

    const request = {
      title,
      description: description || undefined,
      isCompleted: current.isCompleted,
      status: current.status ?? 'A',
    };

    this.http.put<any>(
      `${this.apiUrl}/${current.id}`,
      request,
    ).subscribe({
      next: (response) => {
        const updatedTodo = this.normalizeTodo(response);

        this.todos.update((todos) =>
          todos.map((todo) =>
            String(todo.id) === String(current.id)
              ? updatedTodo
              : todo,
          ),
        );

        this.closeEditModal();
      },
      error: (err) => {
        console.error('Failed to update task:', err);
      },
    });
  }

protected saveDelete(): void {
  const current = this.deletingTodo();

  if (!current) return;

  const request = {
    title: current.title,
    description: current.description ?? undefined,
    isCompleted: current.isCompleted,
    status: 'I', // เปลี่ยนสถานะเป็น Inactive ใน DB
  };

  this.http.put<any>(
    `${this.apiUrl}/${current.id}`,
    request,
  ).subscribe({
    next: () => {
      // ตัดไอเท็มที่ถูกลบ (Inactive) ออกจากลิสต์ในหน้าจอทันที
      this.todos.update((todos) =>
        todos.filter((todo) => String(todo.id) !== String(current.id))
      );

      this.closeDeleteModal();
    },
    error: (err) => {
      console.error('Failed to update task status:', err);
    },
  });
}

  protected deactivateEditingTodo(): void {
    const current = this.editingTodo();

    if (!current) return;

    const request = {
      title: current.title,
      description: current.description ?? undefined,
      isCompleted: current.isCompleted,
      status: 'I',
    };

    this.http.put<any>(
      `${this.apiUrl}/${current.id}`,
      request,
    ).subscribe({
      next: (response) => {
        const updatedTodo = this.normalizeTodo(response);

        this.todos.update((todos) =>
          todos.map((todo) =>
            String(todo.id) === String(current.id)
              ? updatedTodo
              : todo,
          ),
        );

        this.closeEditModal();
      },
      error: (err) => {
        console.error('Failed to update task status:', err);
      },
    });
  }

  protected toggleComplete(todo: TodoItem): void {
    const newStatus = !todo.isCompleted;

    this.http.put<any>(
      `${this.apiUrl}/${todo.id}`,
      {
        title: todo.title,
        description: todo.description,
        isCompleted: newStatus,
        status: todo.status ?? 'A',
      },
    ).subscribe({
      next: (response) => {
        const updatedTodo = this.normalizeTodo(response);

        this.todos.update((todos) =>
          todos.map((item) =>
            String(item.id) === String(todo.id)
              ? updatedTodo
              : item,
          ),
        );
      },
      error: (err) => {
        console.error('Failed to toggle task:', err);
      },
    });
  }
}