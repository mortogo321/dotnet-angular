import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'boards'
  },
  {
    path: 'boards',
    loadComponent: () => import('./components/boards-list/boards-list.component').then(m => m.BoardsListComponent)
  },
  {
    path: 'board/:id',
    loadComponent: () => import('./components/board/board.component').then(m => m.BoardComponent)
  }
];
