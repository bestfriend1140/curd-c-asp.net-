import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { TodoItem } from '../../core/models/todo.model';

@Component({
  selector: 'app-delete-trash-item-modal',
  standalone: true,
  imports: [MatIconModule],
  templateUrl: './delete-trash-item-modal.html',
})
export class DeleteTrashItemModal {
  @Input({ required: true }) todo!: TodoItem;

  @Output() confirm = new EventEmitter<void>();
  @Output() close = new EventEmitter<void>();

  protected confirmDelete(): void {
    this.confirm.emit();
  }

  protected closeModal(): void {
    this.close.emit();
  }
}