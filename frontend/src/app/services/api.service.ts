import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Board, CreateBoardDto, CreateListDto, CreateCardDto, MoveCardDto, BoardList, Card } from '../models/board.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  // Board endpoints
  getBoards(): Observable<Board[]> {
    return this.http.get<Board[]>(`${this.apiUrl}/api/boards`);
  }

  getBoard(id: string): Observable<Board> {
    return this.http.get<Board>(`${this.apiUrl}/api/boards/${id}`);
  }

  createBoard(dto: CreateBoardDto): Observable<Board> {
    return this.http.post<Board>(`${this.apiUrl}/api/boards`, dto);
  }

  updateBoard(id: string, dto: Partial<CreateBoardDto>): Observable<Board> {
    return this.http.put<Board>(`${this.apiUrl}/api/boards/${id}`, dto);
  }

  deleteBoard(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/api/boards/${id}`);
  }

  // List endpoints
  createList(dto: CreateListDto): Observable<BoardList> {
    return this.http.post<BoardList>(`${this.apiUrl}/api/lists`, dto);
  }

  updateList(id: string, dto: Partial<CreateListDto>): Observable<BoardList> {
    return this.http.put<BoardList>(`${this.apiUrl}/api/lists/${id}`, dto);
  }

  deleteList(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/api/lists/${id}`);
  }

  moveList(id: string, newPosition: number): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/api/lists/${id}/move?newPosition=${newPosition}`, {});
  }

  // Card endpoints
  createCard(dto: CreateCardDto): Observable<Card> {
    return this.http.post<Card>(`${this.apiUrl}/api/cards`, dto);
  }

  updateCard(id: string, dto: Partial<CreateCardDto>): Observable<Card> {
    return this.http.put<Card>(`${this.apiUrl}/api/cards/${id}`, dto);
  }

  deleteCard(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/api/cards/${id}`);
  }

  moveCard(id: string, dto: MoveCardDto): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/api/cards/${id}/move`, dto);
  }
}
