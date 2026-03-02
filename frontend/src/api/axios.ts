import axios, { AxiosError } from 'axios';
import { notification } from 'antd';

const api = axios.create({
    baseURL: import.meta.env.VITE_API_URL || 'http://localhost:5044/api', // Default local backend URL
    headers: {
        'Content-Type': 'application/json',
    },
});

// Request Interceptor
api.interceptors.request.use(
    (config) => {
        // We will store the token in localStorage
        const token = localStorage.getItem('token');
        if (token && config.headers) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => Promise.reject(error)
);

// Response Interceptor
api.interceptors.response.use(
    (response) => response,
    async (error: AxiosError) => {

        if (error.response) {
            const { status, data } = error.response;

            if (status === 401) {
                // Token expired or invalid
                // Save current location so we can redirect back after login
                localStorage.setItem('redirectUrl', window.location.pathname);
                localStorage.removeItem('token');

                // Let the app know we are unauthorized so the router can redirect
                window.dispatchEvent(new Event('auth-unauthorized'));
            }
            else if (status === 400 || status === 500) {
                // Global error notification
                const errorData = data as any;
                const message = errorData?.detail || errorData?.message || error.message;

                notification.error({
                    message: status === 500 ? 'Server Error' : 'Request Failed',
                    description: message,
                    placement: 'topRight',
                });
            }
        } else if (error.request) {
            notification.error({
                message: 'Network Error',
                description: 'Unable to connect to the server. Please check your internet connection.',
                placement: 'topRight',
            });
        }

        return Promise.reject(error);
    }
);

export default api;
