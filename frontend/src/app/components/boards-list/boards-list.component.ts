import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { BoardStore } from '../../services/board.store';

@Component({
  selector: 'app-boards-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="boards-container">
      <header>
        <h1>📋 Task Boards</h1>
        <div class="connection-status" [class.connected]="boardStore.isConnected() === 'connected'">
          {{ boardStore.isConnected() }}
        </div>
      </header>

      <div class="create-board-section">
        <h2>Create New Board</h2>
        <div class="create-board-form">
          <input
            type="text"
            [(ngModel)]="newBoardTitle"
            placeholder="Board title"
            class="input"
            (keyup.enter)="newBoardDesc ? null : createBoard()">
          <input
            type="text"
            [(ngModel)]="newBoardDesc"
            placeholder="Description (optional)"
            class="input"
            (keyup.enter)="createBoard()">
          <button (click)="createBoard()" class="btn-primary" [disabled]="!newBoardTitle.trim()">
            ➕ Create Board
          </button>
        </div>
      </div>

      @if (boardStore.loading()) {
        <div class="loading">
          <div class="spinner"></div>
          <p>Loading boards...</p>
        </div>
      }

      @if (boardStore.error()) {
        <div class="error">
          ⚠️ {{ boardStore.error() }}
        </div>
      }

      @if (boardStore.boards().length === 0 && !boardStore.loading()) {
        <div class="empty-state">
          <h3>📝 No boards yet</h3>
          <p>Create your first board to get started!</p>
        </div>
      }

      <div class="boards-grid">
        @for (board of boardStore.boards(); track board.id) {
          <div class="board-card" (click)="openBoard(board.id)">
            <div class="board-card-header">
              <h3>{{ board.title }}</h3>
              <span class="board-meta">
                {{ board.lists.length }} lists •
                {{ getTotalCards(board) }} cards
              </span>
            </div>
            <p class="board-description">{{ board.description || 'No description' }}</p>
            <div class="board-footer">
              <small>Updated {{ formatDate(board.updatedAt) }}</small>
            </div>
          </div>
        }
      </div>
    </div>
  `,
  styles: [`
    .boards-container {
      padding: 40px;
      min-height: 100vh;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    }

    header {
      color: white;
      margin-bottom: 40px;
      display: flex;
      justify-content: space-between;
      align-items: center;

      h1 {
        margin: 0;
        font-size: 2.5rem;
        text-shadow: 2px 2px 4px rgba(0,0,0,0.2);
      }
    }

    .connection-status {
      display: inline-block;
      padding: 6px 16px;
      border-radius: 20px;
      background: rgba(255,255,255,0.2);
      font-size: 0.9rem;
      text-transform: capitalize;

      &.connected {
        background: rgba(0,255,0,0.3);
        animation: pulse 2s infinite;
      }
    }

    @keyframes pulse {
      0%, 100% { opacity: 1; }
      50% { opacity: 0.7; }
    }

    .create-board-section {
      background: white;
      border-radius: 12px;
      padding: 24px;
      margin-bottom: 40px;
      box-shadow: 0 4px 6px rgba(0,0,0,0.1);

      h2 {
        margin: 0 0 16px 0;
        color: #333;
        font-size: 1.3rem;
      }
    }

    .create-board-form {
      display: flex;
      gap: 12px;
      flex-wrap: wrap;
    }

    .input {
      flex: 1;
      min-width: 200px;
      padding: 12px 16px;
      border: 2px solid #e0e0e0;
      border-radius: 8px;
      font-size: 1rem;
      transition: border-color 0.2s;

      &:focus {
        outline: none;
        border-color: #667eea;
      }
    }

    .btn-primary {
      padding: 12px 28px;
      background: #667eea;
      color: white;
      border: none;
      border-radius: 8px;
      cursor: pointer;
      font-size: 1rem;
      font-weight: 600;
      transition: all 0.2s;

      &:hover:not(:disabled) {
        background: #5568d3;
        transform: translateY(-2px);
        box-shadow: 0 4px 8px rgba(102, 126, 234, 0.4);
      }

      &:disabled {
        opacity: 0.5;
        cursor: not-allowed;
      }
    }

    .loading {
      text-align: center;
      color: white;
      padding: 60px 20px;

      .spinner {
        width: 50px;
        height: 50px;
        border: 4px solid rgba(255,255,255,0.3);
        border-top-color: white;
        border-radius: 50%;
        animation: spin 1s linear infinite;
        margin: 0 auto 20px;
      }
    }

    @keyframes spin {
      to { transform: rotate(360deg); }
    }

    .error {
      background: rgba(235, 90, 70, 0.9);
      color: white;
      padding: 20px;
      border-radius: 8px;
      margin-bottom: 20px;
      text-align: center;
      font-weight: 500;
    }

    .empty-state {
      background: rgba(255,255,255,0.95);
      border-radius: 12px;
      padding: 60px 40px;
      text-align: center;
      margin: 40px auto;
      max-width: 500px;

      h3 {
        margin: 0 0 12px 0;
        color: #333;
        font-size: 1.8rem;
      }

      p {
        margin: 0;
        color: #666;
        font-size: 1.1rem;
      }
    }

    .boards-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
      gap: 24px;
      margin-top: 20px;
    }

    .board-card {
      background: white;
      border-radius: 12px;
      padding: 24px;
      cursor: pointer;
      transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);

      &:hover {
        transform: translateY(-8px);
        box-shadow: 0 12px 24px rgba(0,0,0,0.15);
      }

      &:active {
        transform: translateY(-4px);
      }
    }

    .board-card-header {
      margin-bottom: 12px;

      h3 {
        margin: 0 0 8px 0;
        color: #333;
        font-size: 1.4rem;
        font-weight: 700;
      }
    }

    .board-meta {
      display: inline-block;
      color: #667eea;
      font-size: 0.9rem;
      font-weight: 600;
    }

    .board-description {
      color: #666;
      margin: 0 0 16px 0;
      line-height: 1.5;
      min-height: 48px;
    }

    .board-footer {
      padding-top: 16px;
      border-top: 1px solid #f0f0f0;

      small {
        color: #999;
        font-size: 0.85rem;
      }
    }

    @media (max-width: 768px) {
      .boards-container {
        padding: 20px;
      }

      header h1 {
        font-size: 1.8rem;
      }

      .create-board-form {
        flex-direction: column;
      }

      .input, .btn-primary {
        width: 100%;
      }

      .boards-grid {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class BoardsListComponent implements OnInit {
  private router = inject(Router);
  boardStore = inject(BoardStore);

  newBoardTitle = '';
  newBoardDesc = '';

  async ngOnInit() {
    await this.boardStore.loadBoards();
  }

  async createBoard() {
    if (this.newBoardTitle.trim()) {
      await this.boardStore.createBoard(
        this.newBoardTitle.trim(),
        this.newBoardDesc.trim()
      );
      this.newBoardTitle = '';
      this.newBoardDesc = '';
    }
  }

  openBoard(boardId: string) {
    this.router.navigate(['/board', boardId]);
  }

  getTotalCards(board: any): number {
    return board.lists.reduce((total: number, list: any) => total + list.cards.length, 0);
  }

  formatDate(date: Date): string {
    const now = new Date();
    const updatedDate = new Date(date);
    const diffMs = now.getTime() - updatedDate.getTime();
    const diffMins = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMs / 3600000);
    const diffDays = Math.floor(diffMs / 86400000);

    if (diffMins < 1) return 'just now';
    if (diffMins < 60) return `${diffMins} min${diffMins > 1 ? 's' : ''} ago`;
    if (diffHours < 24) return `${diffHours} hour${diffHours > 1 ? 's' : ''} ago`;
    if (diffDays < 7) return `${diffDays} day${diffDays > 1 ? 's' : ''} ago`;

    return updatedDate.toLocaleDateString();
  }
}
