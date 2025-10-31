import { Injectable, signal, computed, inject } from '@angular/core';
import { Board, BoardList, Card } from '../models/board.model';
import { ApiService } from './api.service';
import { SignalRService } from './signalr.service';

@Injectable({
  providedIn: 'root'
})
export class BoardStore {
  private apiService = inject(ApiService);
  private signalRService = inject(SignalRService);

  // Signals for state management
  boards = signal<Board[]>([]);
  currentBoard = signal<Board | null>(null);
  loading = signal(false);
  error = signal<string | null>(null);

  // Computed signals
  currentBoardLists = computed(() => this.currentBoard()?.lists ?? []);
  isConnected = this.signalRService.connectionState;

  constructor() {
    this.setupSignalRListeners();
    this.signalRService.startConnection();
  }

  private setupSignalRListeners(): void {
    this.signalRService.onBoardCreated((board) => {
      this.boards.update(boards => [...boards, board]);
    });

    this.signalRService.onBoardUpdated((board) => {
      this.boards.update(boards =>
        boards.map(b => b.id === board.id ? board : b)
      );
      if (this.currentBoard()?.id === board.id) {
        this.currentBoard.set(board);
      }
    });

    this.signalRService.onBoardDeleted((boardId) => {
      this.boards.update(boards => boards.filter(b => b.id !== boardId));
      if (this.currentBoard()?.id === boardId) {
        this.currentBoard.set(null);
      }
    });

    this.signalRService.onListCreated((list) => {
      const board = this.currentBoard();
      if (board && board.id === list.boardId) {
        this.currentBoard.update(b => b ? {
          ...b,
          lists: [...b.lists, list].sort((a, b) => a.position - b.position)
        } : null);
      }
    });

    this.signalRService.onListUpdated((list) => {
      const board = this.currentBoard();
      if (board && board.id === list.boardId) {
        this.currentBoard.update(b => b ? {
          ...b,
          lists: b.lists.map(l => l.id === list.id ? list : l).sort((a, b) => a.position - b.position)
        } : null);
      }
    });

    this.signalRService.onListDeleted((listId) => {
      this.currentBoard.update(b => b ? {
        ...b,
        lists: b.lists.filter(l => l.id !== listId)
      } : null);
    });

    this.signalRService.onCardCreated((card) => {
      this.currentBoard.update(b => b ? {
        ...b,
        lists: b.lists.map(l =>
          l.id === card.listId
            ? { ...l, cards: [...l.cards, card].sort((a, b) => a.position - b.position) }
            : l
        )
      } : null);
    });

    this.signalRService.onCardUpdated((card) => {
      this.currentBoard.update(b => b ? {
        ...b,
        lists: b.lists.map(l =>
          l.id === card.listId
            ? { ...l, cards: l.cards.map(c => c.id === card.id ? card : c).sort((a, b) => a.position - b.position) }
            : l
        )
      } : null);
    });

    this.signalRService.onCardDeleted((cardId) => {
      this.currentBoard.update(b => b ? {
        ...b,
        lists: b.lists.map(l => ({
          ...l,
          cards: l.cards.filter(c => c.id !== cardId)
        }))
      } : null);
    });

    this.signalRService.onCardMoved((data) => {
      this.currentBoard.update(b => {
        if (!b) return null;

        let movedCard: Card | null = null;

        // Remove card from current list
        const listsWithoutCard = b.lists.map(l => ({
          ...l,
          cards: l.cards.filter(c => {
            if (c.id === data.cardId) {
              movedCard = { ...c, listId: data.targetListId, position: data.newPosition };
              return false;
            }
            return true;
          })
        }));

        // Add card to target list
        if (movedCard) {
          return {
            ...b,
            lists: listsWithoutCard.map(l =>
              l.id === data.targetListId
                ? { ...l, cards: [...l.cards, movedCard].sort((a, b) => a.position - b.position) }
                : l
            )
          };
        }

        return { ...b, lists: listsWithoutCard };
      });
    });
  }

  async loadBoards(): Promise<void> {
    this.loading.set(true);
    this.error.set(null);
    try {
      const boards = await this.apiService.getBoards().toPromise();
      this.boards.set(boards ?? []);
    } catch (err: any) {
      this.error.set(err.message);
    } finally {
      this.loading.set(false);
    }
  }

  async loadBoard(id: string): Promise<void> {
    this.loading.set(true);
    this.error.set(null);
    try {
      const board = await this.apiService.getBoard(id).toPromise();
      this.currentBoard.set(board ?? null);
      await this.signalRService.joinBoard(id);
    } catch (err: any) {
      this.error.set(err.message);
    } finally {
      this.loading.set(false);
    }
  }

  async createBoard(title: string, description: string): Promise<void> {
    try {
      await this.apiService.createBoard({ title, description }).toPromise();
    } catch (err: any) {
      this.error.set(err.message);
    }
  }

  async createList(title: string): Promise<void> {
    const board = this.currentBoard();
    if (!board) return;

    try {
      const position = board.lists.length;
      await this.apiService.createList({ title, boardId: board.id, position }).toPromise();
    } catch (err: any) {
      this.error.set(err.message);
    }
  }

  async createCard(listId: string, title: string): Promise<void> {
    const board = this.currentBoard();
    if (!board) return;

    const list = board.lists.find(l => l.id === listId);
    if (!list) return;

    try {
      const position = list.cards.length;
      await this.apiService.createCard({ title, listId, position }).toPromise();
    } catch (err: any) {
      this.error.set(err.message);
    }
  }

  async deleteCard(cardId: string): Promise<void> {
    try {
      await this.apiService.deleteCard(cardId).toPromise();
    } catch (err: any) {
      this.error.set(err.message);
    }
  }

  async deleteList(listId: string): Promise<void> {
    try {
      await this.apiService.deleteList(listId).toPromise();
    } catch (err: any) {
      this.error.set(err.message);
    }
  }

  async moveCard(cardId: string, targetListId: string, newPosition: number): Promise<void> {
    try {
      await this.apiService.moveCard(cardId, { targetListId, newPosition }).toPromise();
    } catch (err: any) {
      this.error.set(err.message);
    }
  }
}
