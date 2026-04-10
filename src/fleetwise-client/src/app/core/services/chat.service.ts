import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { ChatRequest, ChatResponse } from '../models/chat-message.model';

@Injectable({ providedIn: 'root' })
export class ChatService {
  constructor(private api: ApiService) {}

  sendMessage(request: ChatRequest): Observable<ChatResponse> {
    return this.api.post<ChatResponse>('/chat', request);
  }
}
