export interface ChatMessage {
  role: 'user' | 'assistant';
  content: string;
  timestamp: Date;
  sourcesUsed?: string[];
}

export interface ChatRequest {
  message: string;
  conversationId?: string;
}

export interface ChatResponse {
  response: string;
  conversationId: string;
  sourcesUsed: string[];
}
