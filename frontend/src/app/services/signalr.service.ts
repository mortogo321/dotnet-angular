import { Injectable, signal } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Board, BoardList, Card } from '../models/board.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private hubConnection?: signalR.HubConnection;

  // Signals for real-time state management
  connectionState = signal<'connected' | 'disconnected' | 'connecting'>('disconnected');

  // Event callbacks
  private boardCreatedCallbacks: ((board: Board) => void)[] = [];
  private boardUpdatedCallbacks: ((board: Board) => void)[] = [];
  private boardDeletedCallbacks: ((boardId: string) => void)[] = [];
  private listCreatedCallbacks: ((list: BoardList) => void)[] = [];
  private listUpdatedCallbacks: ((list: BoardList) => void)[] = [];
  private listDeletedCallbacks: ((listId: string) => void)[] = [];
  private listMovedCallbacks: ((data: { listId: string; newPosition: number }) => void)[] = [];
  private cardCreatedCallbacks: ((card: Card) => void)[] = [];
  private cardUpdatedCallbacks: ((card: Card) => void)[] = [];
  private cardDeletedCallbacks: ((cardId: string) => void)[] = [];
  private cardMovedCallbacks: ((data: { cardId: string; targetListId: string; newPosition: number }) => void)[] = [];

  async startConnection(): Promise<void> {
    this.connectionState.set('connecting');

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.apiUrl}/hubs/taskboard`)
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build();

    this.setupEventHandlers();

    try {
      await this.hubConnection.start();
      this.connectionState.set('connected');
      console.log('SignalR Connected');
    } catch (err) {
      this.connectionState.set('disconnected');
      console.error('Error while starting SignalR connection: ', err);
      setTimeout(() => this.startConnection(), 5000);
    }
  }

  private setupEventHandlers(): void {
    if (!this.hubConnection) return;

    this.hubConnection.on('BoardCreated', (board: Board) => {
      this.boardCreatedCallbacks.forEach(cb => cb(board));
    });

    this.hubConnection.on('BoardUpdated', (board: Board) => {
      this.boardUpdatedCallbacks.forEach(cb => cb(board));
    });

    this.hubConnection.on('BoardDeleted', (boardId: string) => {
      this.boardDeletedCallbacks.forEach(cb => cb(boardId));
    });

    this.hubConnection.on('ListCreated', (list: BoardList) => {
      this.listCreatedCallbacks.forEach(cb => cb(list));
    });

    this.hubConnection.on('ListUpdated', (list: BoardList) => {
      this.listUpdatedCallbacks.forEach(cb => cb(list));
    });

    this.hubConnection.on('ListDeleted', (listId: string) => {
      this.listDeletedCallbacks.forEach(cb => cb(listId));
    });

    this.hubConnection.on('ListMoved', (data: { listId: string; newPosition: number }) => {
      this.listMovedCallbacks.forEach(cb => cb(data));
    });

    this.hubConnection.on('CardCreated', (card: Card) => {
      this.cardCreatedCallbacks.forEach(cb => cb(card));
    });

    this.hubConnection.on('CardUpdated', (card: Card) => {
      this.cardUpdatedCallbacks.forEach(cb => cb(card));
    });

    this.hubConnection.on('CardDeleted', (cardId: string) => {
      this.cardDeletedCallbacks.forEach(cb => cb(cardId));
    });

    this.hubConnection.on('CardMoved', (data: { cardId: string; targetListId: string; newPosition: number }) => {
      this.cardMovedCallbacks.forEach(cb => cb(data));
    });
  }

  async joinBoard(boardId: string): Promise<void> {
    if (this.hubConnection?.state === signalR.HubConnectionState.Connected) {
      await this.hubConnection.invoke('JoinBoard', boardId);
    }
  }

  async leaveBoard(boardId: string): Promise<void> {
    if (this.hubConnection?.state === signalR.HubConnectionState.Connected) {
      await this.hubConnection.invoke('LeaveBoard', boardId);
    }
  }

  // Event subscription methods
  onBoardCreated(callback: (board: Board) => void): void {
    this.boardCreatedCallbacks.push(callback);
  }

  onBoardUpdated(callback: (board: Board) => void): void {
    this.boardUpdatedCallbacks.push(callback);
  }

  onBoardDeleted(callback: (boardId: string) => void): void {
    this.boardDeletedCallbacks.push(callback);
  }

  onListCreated(callback: (list: BoardList) => void): void {
    this.listCreatedCallbacks.push(callback);
  }

  onListUpdated(callback: (list: BoardList) => void): void {
    this.listUpdatedCallbacks.push(callback);
  }

  onListDeleted(callback: (listId: string) => void): void {
    this.listDeletedCallbacks.push(callback);
  }

  onListMoved(callback: (data: { listId: string; newPosition: number }) => void): void {
    this.listMovedCallbacks.push(callback);
  }

  onCardCreated(callback: (card: Card) => void): void {
    this.cardCreatedCallbacks.push(callback);
  }

  onCardUpdated(callback: (card: Card) => void): void {
    this.cardUpdatedCallbacks.push(callback);
  }

  onCardDeleted(callback: (cardId: string) => void): void {
    this.cardDeletedCallbacks.push(callback);
  }

  onCardMoved(callback: (data: { cardId: string; targetListId: string; newPosition: number }) => void): void {
    this.cardMovedCallbacks.push(callback);
  }

  async stopConnection(): Promise<void> {
    if (this.hubConnection) {
      await this.hubConnection.stop();
      this.connectionState.set('disconnected');
    }
  }
}
