export interface TodoItem {
  id: string;
  title: string;
  description?: string | null;
  isCompleted: boolean;
  createdAt: string;
  status?: string | null;
  userId: number;
}
