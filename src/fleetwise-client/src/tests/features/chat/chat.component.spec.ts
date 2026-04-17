import { TestBed, fakeAsync, tick } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA, ElementRef } from '@angular/core';
import { Subject } from 'rxjs';
import { ChatComponent } from '../../../app/features/chat/chat.component';
import { ChatService } from '../../../app/core/services/chat.service';
import { ChatMessage } from '../../../app/core/models/chat-message.model';

describe('ChatComponent', () => {
    let component: ChatComponent;
    let mockChatService: {
        messages: ChatMessage[];
        conversationId: string;
        streamMessage: jasmine.Spy;
        sendMessage: jasmine.Spy;
    };
    let streamSubject: Subject<string>;

    beforeEach(() => {
        streamSubject = new Subject<string>();
        mockChatService = {
            messages: [],
            conversationId: 'test-conversation-id',
            streamMessage: jasmine
                .createSpy('streamMessage')
                .and.returnValue(streamSubject.asObservable()),
            sendMessage: jasmine.createSpy('sendMessage'),
        };

        TestBed.configureTestingModule({
            imports: [ChatComponent],
            providers: [{ provide: ChatService, useValue: mockChatService }],
            schemas: [NO_ERRORS_SCHEMA],
        });

        const fixture = TestBed.createComponent(ChatComponent);
        component = fixture.componentInstance;
        const messageListStub = {
            nativeElement: { scrollTop: 0, scrollHeight: 500 },
        } as ElementRef<HTMLDivElement>;
        Object.defineProperty(component, 'messageList', {
            value: () => messageListStub,
            configurable: true,
        });
    });

    it('messages_WhenAccessed_ReturnsChatServiceMessages', () => {
        // Setup
        const testMessage: ChatMessage = { role: 'user', content: 'Hello', timestamp: new Date() };
        mockChatService.messages.push(testMessage);

        // Result
        expect(component.messages.length).toBe(1);
        expect(component.messages[0].content).toBe('Hello');
    });

    it('send_WhenMessageIsEmpty_DoesNotCallService', () => {
        // Setup
        component.userInput = '   ';

        // Act
        component.send();

        // Result
        expect(mockChatService.streamMessage).not.toHaveBeenCalled();
    });

    it('send_WhenStreaming_DoesNotCallService', () => {
        // Setup
        component.userInput = 'Hello';
        component.streaming.set(true);

        // Act
        component.send();

        // Result
        expect(mockChatService.streamMessage).not.toHaveBeenCalled();
    });

    it('send_WhenValidMessage_AddsUserMessageToMessages', () => {
        // Setup
        component.userInput = 'How many vehicles?';

        // Act
        component.send();

        // Result
        expect(mockChatService.messages[0].role).toBe('user');
        expect(mockChatService.messages[0].content).toBe('How many vehicles?');
    });

    it('send_WhenValidMessage_AddsEmptyAssistantMessage', () => {
        // Setup
        component.userInput = 'Hello';

        // Act
        component.send();

        // Result
        expect(mockChatService.messages[1].role).toBe('assistant');
        expect(mockChatService.messages[1].content).toBe('');
    });

    it('send_WhenValidMessage_ClearsUserInput', () => {
        // Setup
        component.userInput = 'Hello';

        // Act
        component.send();

        // Result
        expect(component.userInput).toBe('');
    });

    it('send_WhenValidMessage_SetsStreamingTrue', () => {
        // Setup
        component.userInput = 'Hello';

        // Act
        component.send();

        // Result
        expect(component.streaming()).toBeTrue();
    });

    it('send_WhenValidMessage_CallsStreamMessageWithCorrectRequest', () => {
        // Setup
        component.userInput = 'Count vehicles';

        // Act
        component.send();

        // Result
        expect(mockChatService.streamMessage).toHaveBeenCalledWith({
            message: 'Count vehicles',
            conversationId: 'test-conversation-id',
        });
    });

    it('send_WhenChunksReceived_AppendsChunksToAssistantMessage', fakeAsync(() => {
        // Setup
        component.userInput = 'Hello';
        component.send();

        // Act
        streamSubject.next('There are ');
        streamSubject.next('35 vehicles');
        tick();

        // Result
        const assistantMessageContent = mockChatService.messages[1].content;
        expect(assistantMessageContent).toBe('There are 35 vehicles');
    }));

    it('send_WhenStreamCompletes_SetsStreamingFalse', () => {
        // Setup
        component.userInput = 'Hello';
        component.send();

        // Act
        streamSubject.complete();

        // Result
        expect(component.streaming()).toBeFalse();
    });

    it('send_WhenStreamErrors_AppendsErrorMessageAndSetsStreamingFalse', () => {
        // Setup
        component.userInput = 'Hello';
        component.send();

        // Act
        streamSubject.error(new Error('Network failure'));

        // Result
        expect(mockChatService.messages[1].content).toContain('An error occurred');
        expect(component.streaming()).toBeFalse();
    });

    it('onKeydown_WhenEnterWithoutShift_CallsSend', () => {
        // Setup
        component.userInput = 'Hello';
        const enterEvent = new KeyboardEvent('keydown', { key: 'Enter', shiftKey: false });
        spyOn(component, 'send');
        spyOn(enterEvent, 'preventDefault');

        // Act
        component.onKeydown(enterEvent);

        // Result
        expect(enterEvent.preventDefault).toHaveBeenCalled();
        expect(component.send).toHaveBeenCalled();
    });

    it('onKeydown_WhenShiftEnter_DoesNotCallSend', () => {
        // Setup
        const shiftEnterEvent = new KeyboardEvent('keydown', { key: 'Enter', shiftKey: true });
        spyOn(component, 'send');

        // Act
        component.onKeydown(shiftEnterEvent);

        // Result
        expect(component.send).not.toHaveBeenCalled();
    });

    it('onKeydown_WhenOtherKey_DoesNotCallSend', () => {
        // Setup
        const otherKeyEvent = new KeyboardEvent('keydown', { key: 'a' });
        spyOn(component, 'send');

        // Act
        component.onKeydown(otherKeyEvent);

        // Result
        expect(component.send).not.toHaveBeenCalled();
    });

    it('ngAfterViewInit_WhenCalled_DoesNotThrow', () => {
        // Act & Result
        expect(() => component.ngAfterViewInit()).not.toThrow();
    });
});
