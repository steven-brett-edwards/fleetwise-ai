import { Component, ElementRef, AfterViewInit, inject, signal, viewChild } from '@angular/core';

import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { ChatService } from '../../core/services/chat.service';
import { ChatMessage } from '../../core/models/chat-message.model';

@Component({
    selector: 'app-chat',
    standalone: true,
    imports: [
        FormsModule,
        MatFormFieldModule,
        MatInputModule,
        MatButtonModule,
        MatIconModule,
        MatProgressBarModule,
    ],
    templateUrl: './chat.component.html',
    styleUrl: './chat.component.scss',
})
export class ChatComponent implements AfterViewInit {
    private chatService = inject(ChatService);

    readonly messageList = viewChild<ElementRef<HTMLDivElement>>('messageList');

    get messages(): ChatMessage[] {
        return this.chatService.messages;
    }

    userInput = '';
    readonly streaming = signal(false);

    ngAfterViewInit(): void {
        this.scrollToBottom();
    }

    send(): void {
        const message = this.userInput.trim();
        if (!message || this.streaming()) return;

        this.messages.push({
            role: 'user',
            content: message,
            timestamp: new Date(),
        });

        this.userInput = '';
        this.streaming.set(true);

        const assistantMessage: ChatMessage = {
            role: 'assistant',
            content: '',
            timestamp: new Date(),
        };
        this.messages.push(assistantMessage);
        this.scrollToBottom();

        this.chatService
            .streamMessage({ message, conversationId: this.chatService.conversationId })
            .subscribe({
                next: (chunk) => {
                    assistantMessage.content += chunk;
                    this.scrollToBottom();
                },
                error: () => {
                    assistantMessage.content += '\n\n*An error occurred. Please try again.*';
                    this.streaming.set(false);
                },
                complete: () => {
                    this.streaming.set(false);
                },
            });
    }

    onKeydown(event: KeyboardEvent): void {
        if (event.key === 'Enter' && !event.shiftKey) {
            event.preventDefault();
            this.send();
        }
    }

    private scrollToBottom(): void {
        setTimeout(() => {
            const el = this.messageList()?.nativeElement;
            if (el) el.scrollTop = el.scrollHeight;
        });
    }
}
