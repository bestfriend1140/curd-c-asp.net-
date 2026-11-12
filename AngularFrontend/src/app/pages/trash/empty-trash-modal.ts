import { CdkTrapFocus } from '@angular/cdk/a11y';
import { Component, EventEmitter, Output } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-empty-trash-modal',
  standalone: true,
  imports: [CdkTrapFocus, MatIconModule],
  templateUrl: './empty-trash-modal.html',
})
export class EmptyTrashModal {
  @Output() confirm = new EventEmitter<void>();
  @Output() close = new EventEmitter<void>();

  protected confirmEmpty(): void {
    this.confirm.emit();
  }

  protected closeModal(): void {
    this.close.emit();
  }
}
