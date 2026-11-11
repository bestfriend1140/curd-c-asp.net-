
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CdkTrapFocus } from '@angular/cdk/a11y';
import { MatIconModule } from '@angular/material/icon';
import { TodoItem } from '../../../core/models/todo.model';

@Component({
  selector: 'app-delete-todo-modal',
  standalone: true,
  imports: [CdkTrapFocus, MatIconModule],
  templateUrl: './delete-todo-modal.html',
})
export class DeleteTodoModal {
  @Input({ required: true }) todo!: TodoItem;

  @Output() save = new EventEmitter<{
    title: string;
    description: string;
  }>();

  @Output() close = new EventEmitter<void>();

  protected titleError = false;

  protected submit(title: string, description: string): void {
    const trimmedTitle = title.trim();

    if (!trimmedTitle) {
      this.titleError = true;
      return;
    }

    this.save.emit({
      title: trimmedTitle,
      description: description.trim(),
    });
  }

  protected closeModal(): void {
    this.close.emit();
  }
}
