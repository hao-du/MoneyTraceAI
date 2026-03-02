import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { RouterProvider } from 'react-router-dom';
import { ConfigProvider } from 'antd';
import { router } from './router';

// Create a client
const queryClient = new QueryClient();

// Ant Design Theme Customization matching Tailwind colors
const theme = {
    token: {
        colorPrimary: '#4f46e5', // indigo-600
        colorSuccess: '#10b981', // emerald-500
        fontFamily: 'Inter, sans-serif',
        borderRadius: 6,
    },
};

function App() {
    return (
        <QueryClientProvider client={queryClient}>
            <ConfigProvider theme={theme}>
                <RouterProvider router={router} />
            </ConfigProvider>
        </QueryClientProvider>
    );
}

export default App;
