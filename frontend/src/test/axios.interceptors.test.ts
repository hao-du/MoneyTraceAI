import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import MockAdapter from 'axios-mock-adapter';

// We test the interceptor logic in isolation to keep tests pure.
// The real api instance is imported to use the same interceptors we added.
import api from '../api/axios';

const mockAdapter = new MockAdapter(api);

describe('Axios Interceptors', () => {
    const originalConsoleError = console.error;

    beforeEach(() => {
        localStorage.clear();
        console.error = vi.fn(); // suppress antd notification errors in jsdom
        mockAdapter.reset();
    });

    afterEach(() => {
        console.error = originalConsoleError;
    });

    it('attaches Authorization header when token exists in localStorage', async () => {
        localStorage.setItem('token', 'my-jwt-token');
        mockAdapter.onGet('/test').reply((config) => {
            expect(config.headers?.Authorization).toBe('Bearer my-jwt-token');
            return [200, {}];
        });

        await api.get('/test');
    });

    it('does NOT attach Authorization header when no token in localStorage', async () => {
        mockAdapter.onGet('/test').reply((config) => {
            expect(config.headers?.Authorization).toBeUndefined();
            return [200, {}];
        });

        await api.get('/test');
    });

    it('on 401 response: removes token from localStorage', async () => {
        localStorage.setItem('token', 'expired-token');
        mockAdapter.onGet('/protected').reply(401);

        try {
            await api.get('/protected');
        } catch {
            // expected rejection
        }

        expect(localStorage.getItem('token')).toBeNull();
    });

    it('on 401 response: saves current path to localStorage.redirectUrl', async () => {
        // jsdom sets window.location.pathname to '/' by default
        mockAdapter.onGet('/protected').reply(401);

        try {
            await api.get('/protected');
        } catch {
            // expected rejection
        }

        expect(localStorage.getItem('redirectUrl')).toBe('/');
    });

    it('on 401 response: dispatches auth-unauthorized event', async () => {
        return new Promise<void>((resolve) => {
            localStorage.setItem('token', 'expired-token');
            mockAdapter.onGet('/protected').reply(401);

            window.addEventListener('auth-unauthorized', () => resolve(), { once: true });

            api.get('/protected').catch(() => { });
        });
    });
});
