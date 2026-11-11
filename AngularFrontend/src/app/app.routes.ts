import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'main' },
  {
    path: 'main',
    loadComponent: () => import('./pages/main/main').then((m) => m.MainPage),
  },
  {
    path: 'login',
    loadComponent: () => import('./pages/login/login').then((m) => m.LoginPage),
  },
  {
    path: 'register',
    loadComponent: () => import('./pages/register/register').then((m) => m.RegisterPage),
  },
  {
    path: 'todo',
    canActivate: [authGuard],
    loadComponent: () => import('./pages/todo/todo').then((m) => m.TodoPage),
  },
    {
    path: 'bin',
    loadComponent: () => import('./pages/bin/bin').then((m) => m.BinPage),
  },
  { path: '**', redirectTo: 'main' },
];
