import { DatePipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-main',
  imports: [DatePipe, RouterLink, MatButtonModule, MatIconModule],
  templateUrl: './main.html',
  styleUrl: './main.css',
})
export class MainPage {
  protected readonly auth = inject(AuthService);
}
