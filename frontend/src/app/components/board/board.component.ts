import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { BoardStore } from '../../services/board.store';
import { CdkDragDrop, DragDropModule, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-board',
  standalone: true,
  imports: [CommonModule, DragDropModule, FormsModule],
  template: `
    <div class="board-container">
      <header class="board-header">
        <h1>{{ boardStore.currentBoard()?.title || 'Loading...' }}</h1>
        <p>{{ boardStore.currentBoard()?.description }}</p>
        <div class="connection-status" [class.connected]="boardStore.isConnected() === 'connected'">
          {{ boardStore.isConnected() }}
        </div>
      </header>

      @if (boardStore.loading()) {
        <div class="loading">Loading board...</div>
      }

      @if (boardStore.error()) {
        <div class="error">{{ boardStore.error() }}</div>
      }

      <div class="lists-container" cdkDropListGroup>
        @for (list of boardStore.currentBoardLists(); track list.id) {
          <div class="list">
            <div class="list-header">
              <h3>{{ list.title }}</h3>
              <button (click)="deleteList(list.id)" class="btn-delete">×</button>
            </div>

            <div
              class="cards-container"
              cdkDropList
              [cdkDropListData]="list.cards"
              [id]="list.id"
              (cdkDropListDropped)="onCardDrop($event)">
              @for (card of list.cards; track card.id) {
                <div class="card" cdkDrag>
                  <div class="card-header">
                    <h4>{{ card.title }}</h4>
                    <button (click)="deleteCard(card.id)" class="btn-delete-small">×</button>
                  </div>
                  @if (card.description) {
                    <p class="card-description">{{ card.description }}</p>
                  }
                  @if (card.priority) {
                    <span class="badge priority-{{ card.priority?.toLowerCase() }}">{{ card.priority }}</span>
                  }
                </div>
              }
            </div>

            <div class="add-card">
              <input
                type="text"
                placeholder="Add a card..."
                [(ngModel)]="newCardTitles[list.id]"
                (keyup.enter)="addCard(list.id)"
                class="card-input">
              <button (click)="addCard(list.id)" class="btn-add">Add</button>
            </div>
          </div>
        }

        <div class="list add-list-container">
          <input
            type="text"
            placeholder="Add a list..."
            [(ngModel)]="newListTitle"
            (keyup.enter)="addList()"
            class="list-input">
          <button (click)="addList()" class="btn-add">Add List</button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .board-container {
      padding: 20px;
      min-height: 100vh;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    }

    .board-header {
      color: white;
      margin-bottom: 30px;
      h1 { margin: 0; font-size: 2.5rem; }
      p { margin: 10px 0; opacity: 0.9; }
    }

    .connection-status {
      display: inline-block;
      padding: 4px 12px;
      border-radius: 12px;
      background: rgba(255,255,255,0.2);
      font-size: 0.85rem;
      &.connected { background: rgba(0,255,0,0.3); }
    }

    .lists-container {
      display: flex;
      gap: 20px;
      overflow-x: auto;
      padding-bottom: 20px;
    }

    .list {
      background: #ebecf0;
      border-radius: 8px;
      min-width: 300px;
      max-width: 300px;
      display: flex;
      flex-direction: column;
      max-height: 80vh;
    }

    .list-header {
      padding: 12px 16px;
      display: flex;
      justify-content: space-between;
      align-items: center;
      h3 { margin: 0; font-size: 1.1rem; }
    }

    .cards-container {
      flex: 1;
      overflow-y: auto;
      padding: 0 8px;
      min-height: 100px;
    }

    .card {
      background: white;
      border-radius: 6px;
      padding: 12px;
      margin-bottom: 8px;
      box-shadow: 0 1px 3px rgba(0,0,0,0.1);
      cursor: pointer;
      transition: box-shadow 0.2s;

      &:hover { box-shadow: 0 4px 8px rgba(0,0,0,0.15); }
    }

    .card-header {
      display: flex;
      justify-content: space-between;
      align-items: start;
      h4 { margin: 0; font-size: 0.95rem; }
    }

    .card-description {
      margin: 8px 0 0 0;
      font-size: 0.85rem;
      color: #666;
    }

    .badge {
      display: inline-block;
      padding: 2px 8px;
      border-radius: 4px;
      font-size: 0.75rem;
      margin-top: 8px;

      &.priority-low { background: #61bd4f; color: white; }
      &.priority-medium { background: #f2d600; color: #333; }
      &.priority-high { background: #eb5a46; color: white; }
    }

    .add-card, .add-list-container {
      padding: 12px;
      display: flex;
      gap: 8px;
    }

    .card-input, .list-input {
      flex: 1;
      padding: 8px;
      border: none;
      border-radius: 4px;
      &:focus { outline: 2px solid #0079bf; }
    }

    .btn-add {
      padding: 8px 16px;
      background: #0079bf;
      color: white;
      border: none;
      border-radius: 4px;
      cursor: pointer;
      &:hover { background: #026aa7; }
    }

    .btn-delete, .btn-delete-small {
      background: none;
      border: none;
      color: #999;
      font-size: 1.5rem;
      cursor: pointer;
      padding: 0;
      line-height: 1;
      &:hover { color: #eb5a46; }
    }

    .btn-delete-small {
      font-size: 1.2rem;
    }

    .loading, .error {
      color: white;
      padding: 20px;
      text-align: center;
    }

    .error {
      background: rgba(235, 90, 70, 0.9);
      border-radius: 8px;
    }

    .cdk-drag-preview {
      box-shadow: 0 5px 15px rgba(0,0,0,0.3);
      opacity: 0.8;
    }

    .cdk-drag-animating {
      transition: transform 250ms cubic-bezier(0, 0, 0.2, 1);
    }
  `]
})
export class BoardComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  boardStore = inject(BoardStore);

  newListTitle = '';
  newCardTitles: { [listId: string]: string } = {};

  async ngOnInit() {
    const boardId = this.route.snapshot.paramMap.get('id');
    if (boardId) {
      await this.boardStore.loadBoard(boardId);
    }
  }

  async addList() {
    if (this.newListTitle.trim()) {
      await this.boardStore.createList(this.newListTitle.trim());
      this.newListTitle = '';
    }
  }

  async addCard(listId: string) {
    const title = this.newCardTitles[listId];
    if (title?.trim()) {
      await this.boardStore.createCard(listId, title.trim());
      this.newCardTitles[listId] = '';
    }
  }

  async deleteCard(cardId: string) {
    if (confirm('Delete this card?')) {
      await this.boardStore.deleteCard(cardId);
    }
  }

  async deleteList(listId: string) {
    if (confirm('Delete this list and all its cards?')) {
      await this.boardStore.deleteList(listId);
    }
  }

  onCardDrop(event: CdkDragDrop<any[]>) {
    if (event.previousContainer === event.container) {
      // Same list - reorder
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    } else {
      // Different list - move
      const card = event.previousContainer.data[event.previousIndex];
      const targetListId = event.container.id;

      transferArrayItem(
        event.previousContainer.data,
        event.container.data,
        event.previousIndex,
        event.currentIndex
      );

      this.boardStore.moveCard(card.id, targetListId, event.currentIndex);
    }
  }
}
