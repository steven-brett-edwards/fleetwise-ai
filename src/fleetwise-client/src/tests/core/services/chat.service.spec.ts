import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { ChatService } from '../../../app/core/services/chat.service';
import { ApiService } from '../../../app/core/services/api.service';
import { createMockSSEResponse } from '../../helpers/mock-data.factory';
import { ChatResponse } from '../../../app/core/models/chat-message.model';
import { environment } from '../../../environments/environment';

describe('ChatService', () => {
  let service: ChatService;
  let mockApiService: jasmine.SpyObj<ApiService>;
  let originalFetch: typeof globalThis.fetch;

  beforeEach(() => {
    mockApiService = jasmine.createSpyObj('ApiService', ['get', 'post']);
    originalFetch = globalThis.fetch;

    TestBed.configureTestingModule({
      providers: [
        ChatService,
        { provide: ApiService, useValue: mockApiService },
      ],
    });

    service = TestBed.inject(ChatService);
  });

  afterEach(() => {
    globalThis.fetch = originalFetch;
  });

  it('constructor_WhenInstantiated_InitializesEmptyMessagesArray', () => {
    // Result
    expect(service.messages).toEqual([]);
  });

  it('constructor_WhenInstantiated_GeneratesConversationId', () => {
    // Result
    expect(service.conversationId).toBeTruthy();
    expect(service.conversationId.length).toBeGreaterThan(0);
  });

  it('sendMessage_WithRequest_DelegatesToApiPost', () => {
    // Setup
    const expectedRequest = { message: 'Hello', conversationId: 'test-id' };
    mockApiService.post.and.returnValue(of({ response: 'Hi', conversationId: 'test-id', functionsUsed: [] }));

    // Act
    service.sendMessage(expectedRequest).subscribe();

    // Result
    expect(mockApiService.post).toHaveBeenCalledWith('/chat', expectedRequest);
  });

  it('sendMessage_WhenCalled_ReturnsChatResponse', () => {
    // Setup
    const expectedResponseText = 'There are 35 vehicles';
    mockApiService.post.and.returnValue(of({ response: expectedResponseText, conversationId: 'id', functionsUsed: [] }));
    let actualResponse!: ChatResponse;

    // Act
    service.sendMessage({ message: 'count' }).subscribe(r => actualResponse = r);

    // Result
    expect(actualResponse.response).toBe(expectedResponseText);
  });

  it('streamMessage_WhenCalled_SendsFetchPostToStreamEndpoint', (done: DoneFn) => {
    // Setup
    globalThis.fetch = jasmine.createSpy('fetch').and.returnValue(
      Promise.resolve(createMockSSEResponse(['data: hello\n\ndata: [DONE]\n\n']))
    );

    // Act
    service.streamMessage({ message: 'test' }).subscribe({
      complete: () => {
        // Result
        const fetchCall = (globalThis.fetch as jasmine.Spy).calls.mostRecent();
        expect(fetchCall.args[0]).toBe(`${environment.apiUrl}/chat/stream`);
        expect(fetchCall.args[1].method).toBe('POST');
        expect(fetchCall.args[1].headers['Content-Type']).toBe('application/json');
        done();
      },
    });
  });

  it('streamMessage_WhenDataLinesReceived_EmitsEachChunk', (done: DoneFn) => {
    // Setup
    globalThis.fetch = jasmine.createSpy('fetch').and.returnValue(
      Promise.resolve(createMockSSEResponse(['data: hello\n\ndata: world\n\ndata: [DONE]\n\n']))
    );
    const receivedChunks: string[] = [];

    // Act
    service.streamMessage({ message: 'test' }).subscribe({
      next: chunk => receivedChunks.push(chunk),
      complete: () => {
        // Result
        expect(receivedChunks).toEqual(['hello', 'world']);
        done();
      },
    });
  });

  it('streamMessage_WhenDoneReceived_CompletesObservable', (done: DoneFn) => {
    // Setup
    globalThis.fetch = jasmine.createSpy('fetch').and.returnValue(
      Promise.resolve(createMockSSEResponse(['data: chunk\n\ndata: [DONE]\n\n']))
    );

    // Act & Result
    service.streamMessage({ message: 'test' }).subscribe({
      complete: () => {
        expect(true).toBeTrue(); // Reached completion
        done();
      },
    });
  });

  it('streamMessage_WhenStreamEndsNaturally_CompletesObservable', (done: DoneFn) => {
    // Setup
    globalThis.fetch = jasmine.createSpy('fetch').and.returnValue(
      Promise.resolve(createMockSSEResponse(['data: chunk\n\n']))
    );

    // Act & Result
    service.streamMessage({ message: 'test' }).subscribe({
      complete: () => {
        expect(true).toBeTrue();
        done();
      },
    });
  });

  it('streamMessage_WhenNoResponseBody_EmitsError', (done: DoneFn) => {
    // Setup
    const responseWithNoBody = new Response(null);
    globalThis.fetch = jasmine.createSpy('fetch').and.returnValue(Promise.resolve(responseWithNoBody));

    // Act & Result
    service.streamMessage({ message: 'test' }).subscribe({
      error: (err: Error) => {
        expect(err.message).toBe('No response body');
        done();
      },
    });
  });

  it('streamMessage_WhenFetchFails_EmitsError', (done: DoneFn) => {
    // Setup
    const expectedNetworkError = new Error('Network failure');
    globalThis.fetch = jasmine.createSpy('fetch').and.returnValue(Promise.reject(expectedNetworkError));

    // Act & Result
    service.streamMessage({ message: 'test' }).subscribe({
      error: (err: Error) => {
        expect(err.message).toBe('Network failure');
        done();
      },
    });
  });

  it('streamMessage_WhenAbortErrorOccurs_DoesNotEmitError', (done: DoneFn) => {
    // Setup
    const abortError = new DOMException('The operation was aborted', 'AbortError');
    globalThis.fetch = jasmine.createSpy('fetch').and.returnValue(Promise.reject(abortError));
    let errorEmitted = false;

    // Act
    service.streamMessage({ message: 'test' }).subscribe({
      error: () => { errorEmitted = true; },
    });

    // Result
    setTimeout(() => {
      expect(errorEmitted).toBeFalse();
      done();
    }, 50);
  });

  it('streamMessage_WhenUnsubscribed_AbortsFetch', () => {
    // Setup
    let capturedSignal: AbortSignal | undefined;
    globalThis.fetch = jasmine.createSpy('fetch').and.callFake((_url: string, init: RequestInit) => {
      capturedSignal = init.signal as AbortSignal;
      return new Promise(() => { /* noop */ }); // Never resolves
    });

    // Act
    const subscription = service.streamMessage({ message: 'test' }).subscribe();
    subscription.unsubscribe();

    // Result
    expect(capturedSignal!.aborted).toBeTrue();
  });
});
