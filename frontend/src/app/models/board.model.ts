export interface Board {
  id: string;
  title: string;
  description: string;
  createdAt: Date;
  updatedAt: Date;
  lists: BoardList[];
}

export interface BoardList {
  id: string;
  title: string;
  position: number;
  boardId: string;
  createdAt: Date;
  updatedAt: Date;
  cards: Card[];
}

export interface Card {
  id: string;
  title: string;
  description?: string;
  position: number;
  listId: string;
  createdAt: Date;
  updatedAt: Date;
  dueDate?: Date;
  priority?: 'Low' | 'Medium' | 'High';
  status?: 'Todo' | 'InProgress' | 'Done';
}

export interface CreateBoardDto {
  title: string;
  description: string;
}

export interface CreateListDto {
  title: string;
  boardId: string;
  position: number;
}

export interface CreateCardDto {
  title: string;
  description?: string;
  listId: string;
  position: number;
  dueDate?: Date;
  priority?: string;
  status?: string;
}

export interface MoveCardDto {
  targetListId: string;
  newPosition: number;
}
