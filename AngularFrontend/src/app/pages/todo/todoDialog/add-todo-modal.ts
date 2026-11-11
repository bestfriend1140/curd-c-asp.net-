
import { Component, EventEmitter, Output } from '@angular/core';
import { CdkTrapFocus } from '@angular/cdk/a11y';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-add-todo-modal',
  standalone: true,
  imports: [CdkTrapFocus, MatIconModule],
  templateUrl: './add-todo-modal.html',
})
export class AddTodoModal {
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
