export interface ChatMessage {
    role: 'user' | 'assistant';
    content: string;
    timestamp: Date;
    functionsUsed?: string[];
}

export interface ChatRequest {
    message: string;
    conversationId?: string;
}

export interface ChatResponse {
    response: string;
    conversationId: string;
    functionsUsed: string[];
}
